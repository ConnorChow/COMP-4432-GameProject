/*app.get('/api / Players ', function(req, res) {
const fs = require('fs');
const path = require('path');
const charactersDir = 'Assets/Characters/Players';
const characterFiles = fs.readdirSync(charactersDir);
const characterNames = characterFiles.filter((file) => {
    return path.extname(file).toLowerCase() === '.png';
}).map((file) => {
    return path.basename(file, path.extname(file));
});
res.send(JSON.stringify(characterNames));
});*/
/*using UnityEngine;
using System.Collections;
using System.IO;

public class RESTAPIEndpoint: MonoBehaviour {
    const fs = require('fs');
    const path = require('path');
    const charactersDir = 'Assets/Characters/Players';
    const characterFiles = fs.readdirSync(charactersDir);
    const characterNames = characterFiles.filter((file) => {
        return path.extname(file).toLowerCase() === '.png';
    }).map((file) => {
        return path.basename(file, path.extname(file));
    });
    res.send(JSON.stringify(characterNames));
}*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class RESTAPIEndpoint : MonoBehaviour
{
    [SerializeField] private string charactersDir = "Assets/Characters/Players";

    private void Start()
    {
        List<string> characterNames = new List<string>();
        foreach (string file in Directory.GetFiles(charactersDir))
        {
            if (Path.GetExtension(file).ToLower() == ".png")
            {
                characterNames.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        string json = JsonUtility.ToJson(characterNames);
        Debug.Log(json);
        // You can send the JSON response to the client here
    }
}