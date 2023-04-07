using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour {

	private string currentWeather;
	//private bool rain;
	//private bool snow;
	//private bool cloudy;
	//private bool sunny;
	//public GameObject rainObject;
	//public GameObject snowObject;
	//public GameObject cloudyObject;
	//public GameObject sunnyObject;

	public WeatherItem itemPrefab;

	public List<WeatherItem> weatherItems = new List<WeatherItem>();

	void Start()
	{
		for(int i = -120; i < 120;i += 30)
        {
			for (int j = -120; j < 120; j += 30)
			{
				var item = GameObject.Instantiate<WeatherItem>(itemPrefab);
				item.transform.position = new Vector3(i, j, 0);
				item.gameObject.SetActive(true);
				weatherItems.Add(item);
				item.transform.SetParent(transform);
			}
		}
	
		currentWeather = WeatherManager.instance.Info.weather[0].main.ToLower();
		UpdateWeather();
	}

	private void UpdateWeather() 
	{
		Debug.Log(currentWeather);
		foreach(var item in weatherItems)
        {
			item.UpdateWeather(currentWeather);
		}
	}
}