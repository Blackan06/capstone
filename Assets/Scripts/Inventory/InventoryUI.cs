using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

/* This object manages the inventory UI. */

public class InventoryUI : MonoBehaviour
{
    #region Singlton:InventoryUI

    public static InventoryUI Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        // Get the hash ID for the "IsRotating" animation parameter to improve performance
        IS_ROTATING_ANIM_PARAM = Animator.StringToHash("IsRotating");
        SetLoadingCircleAnimation(true);
    }

    #endregion

    [SerializeField] private Transform itemsParent;   // The parent object of all the items
    [SerializeField] private Image uiLoadingCircleImage;
    [SerializeField] private Animator loadingCircleAnimator;
    [SerializeField] private GameObject buttonBag;
    [SerializeField] private GameObject inventoryUI;  // The entire UI
    Inventory inventory;    // Our current inventory
    [SerializeField] private GameObject slotPrefab; // Tham chi?u t?i Prefab Slot
    private GetItemInventoryByPlayer getItemInventoryByPlayer1;
    List<Item> items;
    private List<Sprite> loadedSprites = new List<Sprite>();

    private bool isDataLoaded = false; // Bi?n ki?m tra xem d? li?u ?ã ???c t?i xong hay ch?a

    private int IS_ROTATING_ANIM_PARAM;

    private ItemData ItemData;

    void Start()
    {
        // Start the process of updating inventory from the API

       
    }
    public void SetActive()
    {
        // Show the bag button and hide the inventory UI
        buttonBag.SetActive(false);
        inventoryUI.SetActive(true);

        // Call UpdateInventoryFromAPI to update the inventory data
        StartCoroutine(UpdateInventoryFromAPI());

        // Get the Inventory instance
        inventory = Inventory.instance;

        // Add a callback to UpdateUI whenever the inventory is changed
        inventory.onItemChangedCallback += UpdateUI;
    }
    public void Recover()
    {
        // Show the bag button and hide the inventory UI
        buttonBag.SetActive(true);
        inventoryUI.SetActive(false);
    }

    private void SetLoadingCircleAnimation(bool animate)
    {
        // Set the "IsRotating" parameter of the loading circle animator to animate it
        loadingCircleAnimator.SetBool(IS_ROTATING_ANIM_PARAM, animate);
    }

    public IEnumerator UpdateInventoryFromAPI()
    {
        // Get the item data from the API
        var itemsApi = NamePrefab.GetInstance().itemInventoryDatas;

        if (itemsApi != null)
        {
            // If the item data is not null, start loading sprites from URLs
            foreach (var itemInventory in itemsApi)
            {
                Debug.Log("itemInventory.itemId" + itemInventory.itemId);
                yield return StartCoroutine(ItemApi.Instance.CheckItemById(itemInventory.itemId, OnGetItem));
                Debug.Log(ItemData.imageUrl);
                yield return StartCoroutine(LoadSpriteFromURL(ItemData.imageUrl, (sprite) =>
                {
                    // Add the loaded sprite to the temporary list of loadedSprites
                    loadedSprites.Add(sprite);

                    // Check if all sprites have been loaded from the API
                    if (loadedSprites.Count == itemsApi.Count)
                    {
                        // If all sprites are loaded, add the items to the inventory and update the UI
                        for (int i = 0; i < itemsApi.Count; i++)
                        {
                            Item newItem = new Item
                            {
                                name = ItemData.name,
                                icon = loadedSprites[i],
                                quantity = itemInventory.quantity,
                                // Add other item data that you want to copy from the ItemInventory
                            };
                            Inventory.instance.Add(newItem);
                        }

                        // Call UpdateUI after the current frame to avoid lag
                        StartCoroutine(UpdateUIAfterFrame());
                    }
                }));
            }
        }
        else
        {
            Debug.Log("items is null");
        }
    }
    private void OnGetItem(ItemData itemData)
    {
        ItemData = itemData;
    }
    private IEnumerator UpdateUIAfterFrame()
    {
        // Wait for the end of the current frame before updating the UI
        yield return new WaitForEndOfFrame();

        // Update the UI after adding items to the inventory
        UpdateUI();

        // Mark data as loaded
        isDataLoaded = true;

        // Update the animation of the loading circle
        SetLoadingCircleAnimation(!isDataLoaded);
    }

    private IEnumerator LoadSpriteFromURL(string url, System.Action<Sprite> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // Call the callback function with the loaded sprite
                callback?.Invoke(sprite);
            }
            else
            {
                Debug.LogError("Error loading image from URL: " + webRequest.error);
                // Call the callback function with null if the image cannot be loaded
                callback?.Invoke(null);
            }
        }
    }

    public void UpdateUI()
    {
        // Get all the inventory slots in the UI
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();

        // Update the UI for each slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.items.Count)
            {
                // If there is an item in the inventory at this slot index, add it to the UI
                slots[i].AddItem(Inventory.instance.items[i]);
            }
            else
            {
                // If there is no item in the inventory at this slot index, clear the slot
                slots[i].ClearSlot();
            }
        }

        // Data is loaded, update the animation of the loading circle
        SetLoadingCircleAnimation(!isDataLoaded);
    }
}