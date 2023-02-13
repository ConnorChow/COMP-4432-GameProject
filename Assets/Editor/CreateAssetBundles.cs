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
        string assetBundleDirectory = "Assets/Characters";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, 
                                                             BuildTarget.StandaloneWindows);
    }

    public class LoadFromFileExample : MonoBehaviour {
    IEnumerator InstantiateObject()
{
    string url = "file:///" + Application.dataPath + "/Characters/" + "Boss" + "Player" + "Goblin" + "Wizard";        
    var request 
        = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
    yield return request.Send();

    AssetBundle bundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
    GameObject Hero = bundle.LoadAsset<GameObject>("Hero");
    GameObject HeroWalk = bundle.LoadAsset<GameObject>("Hero walk");
    GameObject HeroUseItem = bundle.LoadAsset<GameObject>("Hero use item");
    GameObject HeroPlayer2 = bundle.LoadAsset<GameObject>("Hero Player 2");
    GameObject HeroPlayer2UseItem = bundle.LoadAsset<GameObject>("Hero Player 2 use item");
    GameObject HeroPlayer2walk = bundle.LoadAsset<GameObject>("Hero Player 2 walk");
    GameObject demonwalk = bundle.LoadAsset<GameObject>("Demon topdown walk");
    GameObject demonattack = bundle.LoadAsset<GameObject>("Demon topdown attack");
    GameObject demonview = bundle.LoadAsset<GameObject>("Demon topdown view");
    Instantiate(Hero);
    Instantiate(HeroWalk);
    Instantiate(HeroUseItem);
    Instantiate(HeroPlayer2);
    Instantiate(HeroPlayer2UseItem);
    Instantiate(HeroPlayer2walk);
    Instantiate(demonwalk);
    Instantiate(demonattack);
    Instantiate(demonview);
        }   
    }
}
