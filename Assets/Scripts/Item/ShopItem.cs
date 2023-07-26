using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    #region Singlton:Shop

    public static ShopItem Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        ShopItemsList = new List<Shop>(); // Kh?i t?o danh sách ShopItemsList

    }

    #endregion

   
    private Player player1;
    private List<Shop> ShopItemsList;
    [SerializeField] Animator NoCoinsAnim;


    [SerializeField] GameObject ItemTemplate;
    GameObject g;
    [SerializeField] Transform ShopScrollView;
    [SerializeField] GameObject ShopPanel;
    Button buyBtn;
    
    private GetItemInventoryByPlayer getItemInventoryByPlayer1;
    void Start()
    {
        StartCoroutine(FetchDataAndShowItems());
    }
    //get Item Inventory 
    private IEnumerator getlistItem()
    {
        yield return StartCoroutine(NamePrefab.GetInstance().GetlistItemInventoryByPlayerName(OnGetListItem));
    }
    private void OnGetListItem(GetItemInventoryByPlayer getItemInventoryByPlayer)
    {
        getItemInventoryByPlayer1 = getItemInventoryByPlayer;
    }
    private IEnumerator FetchDataAndShowItems()
    {
         
        // ??i cho d? li?u t? API ???c l?y
        yield return StartCoroutine(GetDataFromAPI());
        yield return StartCoroutine(getlistItem());

        // Sau khi có d? li?u, ??t bi?n flag là true và hi?n th? các m?c
        yield return StartCoroutine(GetItemFromApi());
    }
    private IEnumerator GetDataFromAPI()
    {
        // G?i hàm ?? th?c hi?n yêu c?u API và l?y danh sách các m?c
        yield return StartCoroutine(ItemApi.Instance.GetItemsFromAPI());
    }
    private IEnumerator GetItemFromApi()
    {
        List<ItemData> itemdatas = ItemApi.Instance.items;
        Debug.Log("Count" +itemdatas.Count);
        if (ShopItemsList.Count != itemdatas.Count)
        {
            ShopItemsList.Clear();
            for (int i = 0; i < itemdatas.Count; i++)
            {
                ShopItemsList.Add(new Shop());
            }
        }
        for (int i = 0; i < itemdatas.Count; i++)
        {
            ShopItemsList[i].Id = itemdatas[i].id;
            ShopItemsList[i].Price = itemdatas[i].price;
            ShopItemsList[i].Name = itemdatas[i].name;
            ShopItemsList[i].Quanity = itemdatas[i].quantity;
            yield return StartCoroutine(LoadImageFromURL(ShopItemsList[i], itemdatas[i].imageUrl));
        }
        ShowAllItems();

    }
    private void ShowAllItems()
    {
        int count = ItemApi.Instance.items.Count;
        Debug.Log("Count" + count);

        for (int i = 0; i < count; i++)
        {
            g = Instantiate(ItemTemplate, ShopScrollView);
            g.transform.GetChild(0).GetComponent<Image>().sprite = ShopItemsList[i].Image;
            g.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ShopItemsList[i].Price.ToString();
            buyBtn = g.transform.GetChild(2).GetComponent<Button>();
            g.transform.GetChild(3).GetComponent<Text>().text = ShopItemsList[i].Name;
            g.transform.GetChild(4).GetComponent<Text>().text = ShopItemsList[i].Quanity.ToString();
            if (ShopItemsList[i].Quanity == 0)
            {
                DisableBuyButton();
            }
            buyBtn.AddEventListener(i, OnShopItemBtnClicked);
        }
    }
    private IEnumerator LoadImageFromURL(Shop shopItem, string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                shopItem.Image = sprite; // C?p nh?t hình ?nh cho m?c
            }
            else
            {
                Debug.LogError("L?i khi t?i hình ?nh t? URL: " + webRequest.error);
            }
        }
    }

    void OnShopItemBtnClicked(int itemIndex)
    {
        if (GameCoins.Instance.HasEnoughCoins(ShopItemsList[itemIndex].Price))
        {
            
            GameCoins.Instance.UseCoins(ShopItemsList[itemIndex].Price);
            //purchase Item
            ShopItemsList[itemIndex].IsPurchased = true;

            //disable the button
            buyBtn = ShopScrollView.GetChild(itemIndex).GetChild(2).GetComponent<Button>();

            DisableBuyButton();
            //change UI text: coins
            GameCoins.Instance.UpdateAllCoinsUIText();
            //Update Point
            //Handle Item
            var inventoryId = getItemInventoryByPlayer1.inventoryId;
            var listItems = getItemInventoryByPlayer1.listItem;

            if (listItems.Count == 0)
            {
                
                var item = new Item()
                {
                    icon = ShopItemsList[itemIndex].Image,
                    name = ShopItemsList[itemIndex].Name,
                    quantity = 1,
                    showInInventory = true
                };
                Inventory.instance.Add(item);
                var itemInventory = new ItemInventoryData()
                {
                    inventoryId = inventoryId,
                    itemId = ShopItemsList[itemIndex].Id,
                    quantity = 1,
                };
                Debug.Log("itemInventory" + itemInventory);
                StartCoroutine(ItemInventoryDataApi.GetInstance().CreateItemInventory(itemInventory));
            }
            else
            {
                var itemName = listItems.FirstOrDefault(x => x.name == ShopItemsList[itemIndex].Name);
                if(itemName == null)
                {
                    var item = new Item()
                    {
                        icon = ShopItemsList[itemIndex].Image,
                        name = ShopItemsList[itemIndex].Name,
                        quantity = 1,
                        showInInventory = true
                    };
                    Inventory.instance.Add(item);
                    var itemInventory = new ItemInventoryData()
                    {
                        inventoryId = inventoryId,
                        itemId = ShopItemsList[itemIndex].Id,
                        quantity = 1,
                    };
                    StartCoroutine(ItemInventoryDataApi.GetInstance().CreateItemInventory(itemInventory));
                }
                else
                {
                    StartCoroutine(ItemInventoryDataApi.GetInstance().GetItemInventoryByItemName(itemName.name, (itemData) =>
                    {
                        itemData.quantity += 1;
                        itemData.itemId = itemName.id;
                        StartCoroutine(ItemInventoryDataApi.GetInstance().UpdateItemInventory(itemData));
                       
                    }));
                    
                }
            }
         

            //add avatar
        }
        else
        {
            NoCoinsAnim.SetTrigger("NoCoins");
            Debug.Log("You don't have enough coins!!");
        }
    }
    
   
    private void OnPlayer(Player player)
    {
        player1 = player;
    }

    void DisableBuyButton()
    {
        buyBtn.interactable = false;
        buyBtn.transform.GetChild(0).GetComponent<Text>().text = "PURCHASED";
    }
    /*---------------------Open & Close shop--------------------------*/
    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }

}
