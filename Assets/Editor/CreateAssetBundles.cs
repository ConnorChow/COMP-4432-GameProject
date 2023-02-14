using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build Characters")]
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

    AssetBundle Player = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
    AssetBundle Boss = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
    GameObject Hero = Player.LoadAsset<GameObject>("Hero");
    GameObject HeroWalk = Player.LoadAsset<GameObject>("Hero walk");
    GameObject HeroUseItem = Player.LoadAsset<GameObject>("Hero use item");
    GameObject HeroPlayer2 = Player.LoadAsset<GameObject>("Hero Player 2");
    GameObject HeroPlayer2UseItem = Player.LoadAsset<GameObject>("Hero Player 2 use item");
    GameObject HeroPlayer2walk = Player.LoadAsset<GameObject>("Hero Player 2 walk");
    GameObject demonwalk = Boss.LoadAsset<GameObject>("Demon topdown walk");
    GameObject demonattack = Boss.LoadAsset<GameObject>("Demon topdown attack");
    GameObject demonview = Boss.LoadAsset<GameObject>("Demon topdown view");

    Instantiate(Hero);
    Instantiate(HeroWalk);
    Instantiate(HeroUseItem);
    Instantiate(HeroPlayer2);
    Instantiate(HeroPlayer2UseItem);
    Instantiate(HeroPlayer2walk);
    Instantiate(demonwalk);
    Instantiate(demonattack);
    Instantiate(demonview);

    AssetBundle Characters = AssetBundle.LoadFromFile("Assets/Manifest");
    
    AssetBundleManifest CharactersManifest = Characters.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

    string[] dependencies = CharactersManifest.GetAllDependencies("Characters"); //Pass the name of the bundle you want the dependencies for.

    foreach(string dependency in dependencies)
    {
            AssetBundle.LoadFromFile(Path.Combine("Assets/Characters", dependency));
            }
        } 
    }
}
