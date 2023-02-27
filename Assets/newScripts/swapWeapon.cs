using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapWeapon : MonoBehaviour
{
    int totalWeapons = 1; //change this depending on how many items are being added
    public int currentWeaponIndex;

    public GameObject[] items;
    public GameObject weaponHolder;
    public GameObject currentItem;

    // Start is called before the first frame update
    void Start()
    {
        totalWeapons = weaponHolder.transform.childCount;
        items = new GameObject[totalWeapons];

        for(int i = 0; i< totalWeapons; i++)
        {
            items[i] = weaponHolder.transform.GetChild(i).gameObject;
            items[i].SetActive(false);
        }

        items[0].SetActive(true);
        currentItem = items[0];
        currentWeaponIndex = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //next Weapon
            if(currentWeaponIndex < totalWeapons - 1)
            {
                items[currentWeaponIndex].SetActive(false);
                currentWeaponIndex += 1;
                items[currentWeaponIndex].SetActive(true);
                currentItem = items[currentWeaponIndex];
            }
        }

        //previous Weapon
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(currentWeaponIndex > 0)
            {
                items[currentWeaponIndex].SetActive(false);
                currentWeaponIndex -= 1;
                items[currentWeaponIndex].SetActive(true);
                currentItem = items[currentWeaponIndex];
            }
        }
    }
}
