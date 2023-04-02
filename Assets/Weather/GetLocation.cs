using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Mirror;

public class GetLocation : MonoBehaviour
{
    public LocationInfo Info;
	public float latitude;
	public float longitude;
	public WeatherData weatherData;
	private string IPAddress;
	void Start() 
	{
		weatherData = FindObjectOfType<WeatherData>();
		StartCoroutine (GetIP());
		//weatherData = GameObject.Find("WeatherDataObject").GetComponent<WeatherData>();
	}
	private IEnumerator GetIP()
	{
		/*
		var www = new UnityWebRequest("https://ipapi.co/json/")
		{
			downloadHandler = new DownloadHandlerBuffer()
		};

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			//error
			yield break;
		}

		IPAddress = www.downloadHandler.text;
		StartCoroutine (GetCoordinates());*/
		var www = UnityWebRequest.Get("https://api.ipify.org?format=json&ip=216.211.17.25");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        IPAddress = www.downloadHandler.text;
		IPAddress = IPAddress.Trim();
        StartCoroutine(GetCoordinates());
	}

	private IEnumerator GetCoordinates()
	{
		/*
		//string encodedIPAddress = Uri.EscapeDataString(IPAddress);
		var www = new UnityWebRequest("https://ipapi.co/json/" + IPAddress)
		{
			downloadHandler = new DownloadHandlerBuffer()
		};

		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			//error
			yield break;
		}

		Info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
		latitude = Info.lat;
		longitude = Info.lon;
		IPAddress = Info.ip;
		weatherData.Begin ();
	}*/
		//var www = UnityWebRequest.Get("https://ipapi.co/" + IPAddress.Trim() + "/json/");
		var www = UnityWebRequest.Get("https://geo.ipify.org/api/v2/country,city?apiKey=at_epWvkge9QQ6gRvowZ2cVZwhkd5u6K&ipAddress=216.211.17.25");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }
		Debug.Log(www.downloadHandler.text);
        
		Info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
        latitude = Info.lat;
        longitude = Info.lng;
        IPAddress = Info.ip;

		if (weatherData == null)
        {
            weatherData = FindObjectOfType<WeatherData>();
            if (weatherData == null)
            {
                Debug.LogError("WeatherData not found in scene!");
                yield break;
            }
        }
        weatherData.Begin();
    }
}

[Serializable]
public class LocationInfo
{
	public string ip;
	public string location;
	public string country;
	public string region;
	public string city;
	public float lat;
	public float lng;
	public string postalCode;
	public string timezone;
	public int geonameId;
	public string asType;
	public int asn;
	public string name;
	public string route;
	public string domain;
	public string type;
	public string isp;
}
