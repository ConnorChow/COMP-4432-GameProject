using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Mirror;
using Newtonsoft.Json;

public class GetLocation : MonoBehaviour
{
	public LocationInfo location;

	void Start()
	{
		StartCoroutine(GetCoordinates());
	}


	private IEnumerator GetCoordinates()
	{
		var www = UnityWebRequest.Get("https://freeipapi.com/api/json/216.211.111.51");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError(www.error);
			yield break;
		}
		Debug.Log(www.downloadHandler.text);
		location = JsonConvert.DeserializeObject<LocationInfo>(www.downloadHandler.text);
		Debug.Log(location.cityName);
	}
}


[Serializable]
public class LocationInfo
{
	public string ipAddress;
	public double latitude;
	public double longitude;
	public string countryName;
	public string countryCode;
	public string timeZone;
	public string zipCode;
	public string cityName;
	public string regionName;

}
