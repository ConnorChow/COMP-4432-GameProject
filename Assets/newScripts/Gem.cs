using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, ICollectible
{
    // Start is called before the first frame update
    public static event HandleGemCollected OnGemCollected;
    public delegate void HandleGemCollected(ItemData itemData);
    public ItemData gemData;
    
    public void Collect()
    {
        Destroy(gameObject);
        OnGemCollected?.Invoke(gemData);
    }


}
