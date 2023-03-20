using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetPlayerData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IEnumerator GetPlayerData()
    {
    using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:8080/api/Players"))
    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Process the response data here
            string json = www.downloadHandler.text;
            Debug.Log(json);
        }
    }
}

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
