using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherItem : MonoBehaviour
{
	public List<GameObject> weatherObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void UpdateWeather(string currentWeather)
    {
		foreach(var obj in weatherObj)
        {
			obj.gameObject.SetActive(false);
        }

		if (currentWeather.Contains("rain"))
		{
			weatherObj[0].gameObject.SetActive(true);
		}
		else if (currentWeather.Contains("snow") || currentWeather.Contains("sleet"))
		{
			weatherObj[1].gameObject.SetActive(true);
		}
		else if (currentWeather.Contains("cloud") || currentWeather.Contains("fog"))
		{
			weatherObj[2].gameObject.SetActive(true);
		}
		else if (currentWeather.Contains("sunny") || currentWeather.Contains("clear")|| currentWeather.Contains("wind"))
		{
			weatherObj[3].gameObject.SetActive(true);
		}
	}
}

