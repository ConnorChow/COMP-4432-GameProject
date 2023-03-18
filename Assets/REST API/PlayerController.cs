using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    IEnumerator Start() {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/Character/Player")) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                string playerName = www.downloadHandler.text;
                Debug.Log("Got character: " + playerName);

                // Load the sprite for the character
                Sprite characterSprite = Resources.Load<Sprite>("Character/Player" + playerName);
                GetComponent<SpriteRenderer>().sprite = characterSprite;
            } else {
                Debug.Log("Error getting player: " + www.error);
            }
        }
    }

    
}
