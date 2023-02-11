using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/assetBundles";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
                                        BuildAssetBundleOptions.None, 
                                        BuildTarget.StandaloneWindows);
    }

    public class LoadFromFileExample : MonoBehaviour {
    IEnumerator InstantiateObject()
{
    string url = "file:///" + Application.dataPath + "/AssetBundles/" + assetBundleName;        
    var request 
        = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
    yield return request.Send();
    AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
    GameObject cube = bundle.LoadAsset<GameObject>("Cube");
    GameObject sprite = bundle.LoadAsset<GameObject>("Sprite");
    Instantiate(cube);
    Instantiate(sprite);
        }   
    }
}
