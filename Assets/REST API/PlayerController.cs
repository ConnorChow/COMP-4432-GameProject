/*using System.Collections;
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

    
}*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public string apiUrl = "http://localhost/api/characters/players";
    public string characterImageDirectory = "Characters/Players";

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get a list of available character images from the REST API
        StartCoroutine(GetCharacterImages());
    }

    private IEnumerator GetCharacterImages()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Failed to get character images: " + www.error);
            }
            else
            {
                // Parse the JSON response to get the list of character image filenames
                string json = www.downloadHandler.text;
                CharacterList characterList = JsonUtility.FromJson<CharacterList>(json);

                if (characterList.characters != null && characterList.characters.Length > 0)
                {
                    // Pick a random character image from the list
                    string characterImageFilename = characterList.characters[Random.Range(0, characterList.characters.Length)];

                    // Load the character image from the Resources folder
                    string characterImagePath = characterImageDirectory + "/" + characterImageFilename;
                    Sprite characterSprite = Resources.Load<Sprite>(characterImagePath);

                    if (characterSprite != null)
                    {
                        // Set the character image sprite
                        spriteRenderer.sprite = characterSprite;
                    }
                    else
                    {
                        Debug.Log("Failed to load character image: " + characterImagePath);
                    }
                }
                else
                {
                    Debug.Log("No character images found");
                }
            }
        }
    }
}

[System.Serializable]
public class CharacterList
{
    public string[] characters;
}