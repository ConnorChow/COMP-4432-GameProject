using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Mirror;
using SimpleJSON;

public class WeatherData : MonoBehaviour {
	private float timer;
	public float minutesBetweenUpdate;
	public MyWeatherInfo Info;
	public string API_key;
	private float latitude;
	private float longitude;
	private bool locationInitialized;
	public Text currentWeatherText;
	public GetLocation getLocation;

	public void Begin() {
		latitude = getLocation.latitude;
		longitude = getLocation.longitude;
		locationInitialized = true;
	}
	void Update() {
		if (locationInitialized) {
			if (timer <= 0) {
				StartCoroutine (GetWeatherInfo());
				timer = minutesBetweenUpdate * 60;
			} else {
				timer -= Time.deltaTime;
			}
		}
	}
	private IEnumerator GetWeatherInfo()
	{
		//var www = new UnityWebRequest("http://api.openweathermap.org/geo/1.0/zip?zip=P7B,CA&appid=9a450fda46a74dc536c74286cbca0fbe" + API_key + "/" + latitude + "," + longitude)
		//var www = new UnityWebRequest("https://api.openweathermap.org/data/2.5/weather?lat=48.38202&lon=-89.25018&appid=9a450fda46a74dc536c74286cbca0fbe")
		var www = new UnityWebRequest("https://api.open-meteo.com/v1/forecast?latitude=48.38202&longitude=-89.25018&current_weather=true&hourly=temperature_2m,relativehumidity_2m,windspeed_10m")
		{
			downloadHandler = new DownloadHandlerBuffer()
		};

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			//error
			yield break;
		}
		
		// Info = JsonUtility.FromJson<MyWeatherInfo>(www.downloadHandler.text);
		// currentWeatherText.text = "Current weather: " + Info.currently.summary;
		string jsonResponse = Encoding.UTF8.GetString(www.downloadHandler.data);
    	Info = JsonUtility.FromJson<MyWeatherInfo>(jsonResponse);
    	currentWeatherText.text = "Current weather: " + Info.currently.current_weather;
	}
}   

    [Serializable]
    public class MyWeatherInfo
{
	public float latitude;
	public float longitude;
	public string timezone;
	public Currently currently;
	public int offset;
}

[Serializable]
public class Currently
{
	/*
	public int time;
	public string summary;
	public string icon;
	public int nearestStormDistance;
	public int nearestStormBearing;
	public int precipIntensity;
	public int precipProbability;
	public double temperature;
	public double apparentTemperature;
	public double dewPoint;
	public double humidity;
	public double pressure;
	public double windSpeed;
	public double windGust;
	public int windBearing;
	public int cloudCover;
	public int uvIndex;
	public double visibility;
	public double ozone;
	*/
	public float generationtime_ms;
	public int utc_offset_seconds;
	public string timezone;
	public string timezone_abbreviation;
	public float elevation;
	public string current_weather;
	public float temperature;
	public float windspeed;
	public float winddirection;
	public int weathercode;
	public string time;
	public string hourly_units;
	public string temperature_2m;
	public string relativehumidity_2m;
	public string windspeed_10m;
	public string hourly;
}
