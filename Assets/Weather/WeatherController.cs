using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;	
//using Asset;


[Serializable]
public class Weather
{
    public int id;
    public string main;
}

[Serializable]
public class WeatherInfo
{
    public int id;
    public string name;
    public List<Weather> weather;
}

public class WeatherController : MonoBehaviour
{
    private const string API_KEY = "88169749096b61e3b85398905927f53c";
    private const float API_CHECK_MAXTIME = 10 * 60.0f; //10 minutes
    public GameObject SnowSystem;
    public string CityId;
    private float apiCheckCountdown = API_CHECK_MAXTIME;

    void Start()
    {
        StartCoroutine(GetWeather(CheckSnowStatus));
    }

    void Update()
    {
        apiCheckCountdown -= Time.deltaTime;
        if (apiCheckCountdown <= 0)
        {
            apiCheckCountdown = API_CHECK_MAXTIME;
            StartCoroutine(GetWeather(CheckSnowStatus));
        }
    }

    public void CheckSnowStatus(WeatherInfo weatherObj)
    {
        if (weatherObj.weather.Count > 0)
        {
        bool snowing = weatherObj.weather[0].main.Equals("Snow");
        if (snowing)
            SnowSystem.SetActive(true);
        else
            SnowSystem.SetActive(false);
        }
    }

    IEnumerator GetWeather(Action<WeatherInfo> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(String.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&APPID={1}", CityId, API_KEY)))
        {
            yield return req.Send();
            while (!req.isDone)
            yield return null;
            byte[] result = req.downloadHandler.data;
            string weatherJSON = System.Text.Encoding.Default.GetString(result);
            WeatherInfo info = JsonUtility.FromJson<WeatherInfo>(weatherJSON);
            onSuccess(info);
        }
    }
}
