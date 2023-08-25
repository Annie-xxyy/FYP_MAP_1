using System;
using System.Collections;
using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
            }

            return _instance;
        }
    }
    
    private List<Item> itemList;

    private ToolTip _toolTip;
    private bool isToolTipShow = false;
    private Vector2 toolTipPositionOffSet = new Vector2(25, -30);
    
    private Canvas _canvas;
    
    private bool isPickedItem = false;

    public bool IsPickedItem
    {
        get
        {
            return isPickedItem;
        }
    }
    
    private ItemUI pickedItem;

    public ItemUI PickedItem
    {
        get
        {
            return pickedItem;
        }
    }

    private void Awake()
    {
        ParseItemJson();
    }

    private void Start()
    {
        _toolTip = FindObjectOfType<ToolTip>();
        _canvas = FindObjectOfType<Canvas>();
        pickedItem = GameObject.Find("SelectedItem").GetComponent<ItemUI>();
        pickedItem.Hide();
    }

    private void Update()
    {
        if (isPickedItem)
        {
            Vector2 postion;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                Input.mousePosition, null, out postion);
            pickedItem.SetLocalPosition(postion);
        }
        else if (isToolTipShow)
        {
            Vector2 postion;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                Input.mousePosition, null, out postion);
            _toolTip.SetLocalPosition(postion+toolTipPositionOffSet);
        }

        if (isPickedItem && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)==false)
        {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }

    void ParseItemJson()
    {
        itemList = new List<Item>();
        TextAsset itemText = Resources.Load<TextAsset>("Items");
        string itemsjson = itemText.text;
        JSONObject j = new JSONObject(itemsjson);
        foreach (JSONObject temp in j.list)
        {
            string typeStr = temp["type"].stringValue;
            Item.ItemType type = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), typeStr);

            int id = temp["id"].intValue;
            string name = temp["name"].stringValue;
            Item.ItemQuality quality =
                (Item.ItemQuality)System.Enum.Parse(typeof(Item.ItemQuality), temp["quality"].stringValue);
            string description = temp["description"].stringValue;
            int capacity = temp["capacity"].intValue;
            int buyPrice = temp["buyPrice"].intValue;
            int sellPrice = temp["sellPrice"].intValue;
            string sprite = temp["sprite"].stringValue;

            Item item = null;
            switch (type)
            {
                case Item.ItemType.Consumable:
                    int hp = temp["hp"].intValue;
                    int san = temp["san"].intValue;
                    item = new Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite,
                        hp, san);
                    break;
                case Item.ItemType.Equipment:
                    int strength = temp["strength"].intValue;
                    int intelligence = temp["intelligence"].intValue;
                    int agility = temp["agility"].intValue;
                    int vitality = temp["vitality"].intValue;
                    Equipment.EqupimentType equpimentType = 
                        (Equipment.EqupimentType)System.Enum.Parse(typeof(Equipment.EqupimentType), 
                            temp["equipType"].stringValue);
                    item = new Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite,
                        strength, intelligence, agility, vitality, equpimentType);
                    break;
                case Item.ItemType.Weapon:
                    int damage = temp["damage"].intValue;
                    Weapon.WeaponType weaponType= 
                        (Weapon.WeaponType)System.Enum.Parse(typeof(Weapon.WeaponType), 
                            temp["weaponType"].stringValue);
                    item = new Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite,
                        damage, weaponType);
                    break;
                case Item.ItemType.Material:
                    item = new Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
            }
            itemList.Add(item);
        }
    }

    public Item GetItemByID(int id)
    {
        foreach (Item item in itemList)
        {
            if (item.ID==id)
            {
                return item;
            }
        }

        return null;
    }
    
    public void ShowToolTip(string content)
    {
        if (this.isPickedItem)
        {
            return;
        }
        isToolTipShow = true;
        _toolTip.Show(content);
    }

    public void HideToolTip()
    {
        isToolTipShow = false;
        _toolTip.Hide();
    }

    public void PickupItem(Item item, int amount)
    {
        PickedItem.SetItem(item,amount);
        isPickedItem = true;
        PickedItem.Show();
        this._toolTip.Hide();
    }
    
    public void RemoveItem(int amount=1)
    {
        PickedItem.ReduceAmount(amount);
        if (PickedItem.Amount <= 0)
        {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }
    
}
