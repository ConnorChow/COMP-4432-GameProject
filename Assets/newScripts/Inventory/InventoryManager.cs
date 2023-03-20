using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InventoryManager : NetworkBehaviour
{
    public GameObject slotPrefab;
    public List<InventorySlot> InventorySlots = new List<InventorySlot>(14);

    private void OnEnable()
    {
        Inventory.OnInventoryChange += DrawInventory;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChange -= DrawInventory;
    }

    void ResetInventory()
    {
        if (InventorySlots != null)
        {
        foreach(Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
        InventorySlots = new List<InventorySlot>(14);
        }
    }

    void DrawInventory(List<InventoryItem> inventory)
    {
        ResetInventory();

        for (int i = 0; i < InventorySlots.Capacity; i++)
        {
            CreateInventory();
        }

    if (InventorySlots != null && InventorySlots.Count > 0)
    {
        for (int i=0; i< inventory.Count;i++)
        {
            InventorySlots[i].DrawSlot(inventory[i]);
        }
    }
    }

    void CreateInventory()
    {
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(transform, false);

        InventorySlot newSlotComponent = newSlot.GetComponent<InventorySlot>();
        newSlotComponent.ClearSlot();

        InventorySlots.Add(newSlotComponent);
    }
}
