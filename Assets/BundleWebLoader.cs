using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleWebLoader : MonoBehaviour
{
    public string bundleUrl = "http://localhost/Assets/Characters";
    public string assetName = "BundledObject";
    
    IEnumerator InstantiateObject()
{
    string url = "file:///" + Application.dataPath + "/Characters/" + "Player" + "Boss";        
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
