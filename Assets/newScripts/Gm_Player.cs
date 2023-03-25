using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gm_Player : MonoBehaviour
{

    public Sprite[] sprites;
    public int index;
    private void OnEnable()
    {
        index = Random.Range(0, sprites.Length);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[index];
    }
}
