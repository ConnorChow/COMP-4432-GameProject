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
		StartCoroutine (GetIP());
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
		var www = UnityWebRequest.Get("https://ipapi.co/json/");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        IPAddress = www.downloadHandler.text;
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
		var www = UnityWebRequest.Get("https://ipapi.co/" + IPAddress + "/json/");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        Info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
        latitude = Info.latitude;
        longitude = Info.longitude;
        IPAddress = Info.ip;
        weatherData.Begin();
    }
}

[Serializable]
public class LocationInfo
{
	/*public string status;
	public string country;
	public string countryCode;
	public string region;
	public string regionName;
	public string city;
	public string zip;
	public float lat;
	public float lon;
	public string timezone;
	public string isp;
	public string org;
	public string @as;
	public string query;*/
	public string ip;
	public string network;
	public string version;
	public string city;
	public string region;
	public string region_code;
	public string country;
	public string country_name;
	public string country_code;
	public string country_code_iso3;
	public string country_capital;
	public string country_tld;
	public string continent_code;
	public string in_eu;
	public string postal;
	public float latitude;
    public float longitude;
	public string timezone;
	public string utc_offset;
	public string country_calling_code;
	public string currency;
	public string currency_name;
	public string languages;
	public string country_area;
	public string country_population;
	public string asn;
	public string org;
}
