using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetItemInventoryByPlayer
{
    public string playerId;
    public List<ItemData> listItem;
    public List<ItemInventoryData> listItemInventory;
    public string inventoryId;
}
