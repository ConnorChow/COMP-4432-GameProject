using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

public class WeatherManager : MonoBehaviour
{
    public MyWeatherInfo Info;
    public GetLocation myLocation;
    public static WeatherManager instance;
    private void Start()
    {
        instance = this;
        StartCoroutine(GetWeatherInfo());
        myLocation = GetComponent<GetLocation>();
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator GetWeatherInfo()
    {
        //var www = new UnityWebRequest("http://api.openweathermap.org/geo/1.0/zip?zip=P7B,CA&appid=9a450fda46a74dc536c74286cbca0fbe" + API_key + "/" + latitude + "," + longitude)
        var www = new UnityWebRequest("https://api.openweathermap.org/data/2.5/weather?lat=48.38202&lon=-89.25018&appid=9a450fda46a74dc536c74286cbca0fbe")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        //var www = new UnityWebRequest("https://api.open-meteo.com/v1/forecast?latitude=48.38202&longitude=-89.25018&current_weather=true&hourly=temperature_2m,relativehumidity_2m,windspeed_10m")
        //{
        //	downloadHandler = new DownloadHandlerBuffer()
        //};

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            //error
            yield break;
        }

        string jsonResponse = Encoding.UTF8.GetString(www.downloadHandler.data);
        Debug.Log(jsonResponse);
        Info = JsonConvert.DeserializeObject<MyWeatherInfo>(jsonResponse);
        Debug.Log(Info.weather[0].main);
    }
}

[Serializable]
public class MyWeatherInfo
{
    public List<WeatherModel> weather;
}
public class WeatherModel
{
    public string main;
}