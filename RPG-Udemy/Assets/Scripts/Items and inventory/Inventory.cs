using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// Inventory.cs摘要
// 背包系统核心类，负责管理玩家的物品、装备和仓库
// 实现了ISaveManager接口，支持背包数据的存档和读档

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;                                // 单例实例
    
    // 物品和装备列表
    public List<ItemData> startingItems;                             // 游戏开始时的初始物品
    public List<InventoryItem> equipment;                            // 已装备的物品列表
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictianory; // 装备字典，用于快速查找
    
    public List<InventoryItem> inventory;                            // 背包物品列表
    public Dictionary<ItemData, InventoryItem> inventoryDictianory;  // 背包物品字典，用于快速查找
    
    public List<InventoryItem> stash;                                // 仓库物品列表
    public Dictionary<ItemData, InventoryItem> stashDictianory;      // 仓库物品字典，用于快速查找

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;          // 背包槽位父物体
    [SerializeField] private Transform stashSloatParent;             // 仓库槽位父物体
    [SerializeField] private Transform equipmentSlotParent;          // 装备槽位父物体
    [SerializeField] private Transform statSlotParent;               // 属性槽位父物体

    // UI槽位引用
    private UI_itemSlot[] inventoryItemSlot;                         // 背包UI槽位数组
    private UI_itemSlot[] stashItemSlot;                             // 仓库UI槽位数组
    private UI_EquipmentSlot[] equipmentSlot;                        // 装备UI槽位数组
    private UI_StatSlot[] statSlot;                                  // 属性UI槽位数组

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;                                 // 上次使用药水的时间
    private float lastTimeUsedAmulet;                                // 上次使用护符的时间

    public float flaskCooldown { get; private set; }                 // 药水冷却时间
    private float armorCooldown;                                     // 护甲冷却时间

    [Header("数据库")]
    public List<InventoryItem> loadedItems;                          // 从存档加载的物品
    public List<ItemData_Equipment> loadedEquipment;                 // 从存档加载的装备

    // 初始化单例
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        // 确保场景中只有一个Inventory实例
    }

    // 初始化背包系统
    public void Start()
    {
        // 初始化物品列表和字典
        inventory = new List<InventoryItem>();
        inventoryDictianory = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictianory = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictianory = new Dictionary<ItemData_Equipment, InventoryItem>();

        // 获取UI槽位引用
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_itemSlot>();
        stashItemSlot = stashSloatParent.GetComponentsInChildren<UI_itemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        // 添加初始物品
        AddStartingItems();

        // 初始化物品冷却时间
        lastTimeUsedFlask = -999f;  // 设置为很久以前，确保游戏开始时可以立即使用
        lastTimeUsedAmulet = -999f;
    }

    // 添加初始物品和从存档加载的物品
    private void AddStartingItems()
    {
        // 首先装备所有已加载的装备
        foreach (ItemData_Equipment item in loadedEquipment)
        {
            EquipItem(item);
        }

        // 如果有已加载的物品，添加它们
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }

        // 如果没有已加载的物品，添加初始物品
        for (int i = 0; i < startingItems.Count; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i]);
        }
    }

    // 装备物品
    public void EquipItem(ItemData _item)
    {
        // 转换为装备类型
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        // 查找同类型的已装备物品
        ItemData_Equipment oldEquipment = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictianory)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
                break;  // 找到后立即跳出循环
            }
        }

        // 装备流程：
        // 1. 从背包中移除新装备
        RemoveItem(_item);
        
        // 2. 如果已有同类型装备，先卸下并添加到背包
        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }
        
        // 3. 装备新物品并应用其属性修饰符
        equipment.Add(newItem);
        equipmentDictianory.Add(newEquipment, newItem);
        newEquipment.AddModifiers();  // 应用装备的属性加成
        
        // 4. 更新UI显示
        UpdateSloatUI();
    }

    // 卸下装备
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        // 检查装备是否存在
        if (equipmentDictianory.TryGetValue(itemToRemove, out InventoryItem value))
        {
            // 从装备列表和字典中移除
            equipment.Remove(value);
            equipmentDictianory.Remove(itemToRemove);
            
            // 移除该装备提供的属性修饰符
            itemToRemove.RemoveModifiers();
        }
    }

    // 更新所有UI槽位显示
    private void UpdateSloatUI()
    {
        // 1. 更新装备槽位
        // 首先清理所有装备槽位
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            equipmentSlot[i].CleanUpSloat();
        }

        // 然后更新有装备的槽位
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictianory)
            {
                // 将装备放入对应类型的槽位
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                {
                    equipmentSlot[i].UpdateSlots(item.Value);
                }
            }
        }

        // 2. 更新背包和仓库槽位
        // 清理背包槽位
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].CleanUpSloat();
        }
        // 清理仓库槽位
        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].CleanUpSloat();
        }

        // 更新背包槽位显示
        for (int i = 0; i < inventory.Count && i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].UpdateSlots(inventory[i]);
        }

        // 更新仓库槽位显示
        for (int i = 0; i < stash.Count && i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].UpdateSlots(stash[i]);
        }

        // 3. 更新角色属性UI
        UpdateStatsUI();
    }

    // 更新角色属性UI显示
    public void UpdateStatsUI()
    {
        // 更新所有属性槽位的值
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    // 添加物品到背包系统
    public void AddItem(ItemData _item)
    {
        // 根据物品类型决定添加到背包还是仓库
        if (_item.itemType == ItemType.Equipment && CanAddItem())
            AddToInventory(_item);  // 装备类物品添加到背包
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);      // 材料类物品添加到仓库

        // 更新UI显示
        UpdateSloatUI();
    }

    // 添加物品到仓库
    private void AddToStash(ItemData _item)
    {
        // 检查是否已有该物品，如果有则增加堆叠数量
        if (stashDictianory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();  // 增加堆叠数量
        }
        else
        {
            // 如果没有，创建新物品条目
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictianory.Add(_item, newItem);
        }
    }

    // 添加物品到背包
    private void AddToInventory(ItemData _item)
    {
        // 检查是否已有该物品，如果有则增加堆叠数量
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();  // 增加堆叠数量
        }
        else
        {
            // 如果没有，创建新物品条目
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictianory.Add(_item, newItem);
        }
    }

    // 从背包或仓库中移除物品
    public void RemoveItem(ItemData _item)
    {
        // 检查背包中是否有该物品
        if (inventoryDictianory.TryGetValue(_item, out InventoryItem value))
        {
            // 如果堆叠数量为1，完全移除物品
            if (value.stackSize <= 1)
            {
                inventory.Remove(value);
                inventoryDictianory.Remove(_item);
            }
            else
                value.RemoveStack();  // 减少堆叠数量
        }
        
        // 检查仓库中是否有该物品
        if (stashDictianory.TryGetValue(_item, out InventoryItem stashValue))
        {
            // 如果堆叠数量为1，完全移除物品
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictianory.Remove(_item);
            }
            else
                stashValue.RemoveStack();  // 减少堆叠数量
        }
        
        // 更新UI显示
        UpdateSloatUI();
    }

    // 检查背包是否有空间添加新物品
    public bool CanAddItem()
    {
        // 如果背包已满，返回false
        if (inventory.Count >= inventoryItemSlot.Length)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        return true;
    }
    // 检查是否可以制作物品，并在可以时消耗材料并添加制作的物品
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requireMaterials)
    {
        // 用于记录需要移除的材料
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();
        
        // 检查是否有足够的材料
        for (int i = 0; i < _requireMaterials.Count; i++)
        {
            // 检查仓库中是否有该材料
            if (stashDictianory.TryGetValue(_requireMaterials[i].data, out InventoryItem stashValue))
            {
                // 检查数量是否足够
                if (stashValue.stackSize < _requireMaterials[i].stackSize)
                {
                    return false;  // 材料数量不足
                }
                else
                {
                    // 记录需要移除的材料
                    materialsToRemove.Add(stashValue);
                }
            }
            else
            {
                return false;  // 缺少所需材料
            }
        }
        
        // 消耗材料
        for (int i = 0; i < materialsToRemove.Count; i++)
        {
            RemoveItem(materialsToRemove[i].data);
        }
        
        // 添加制作的物品
        AddItem(_itemToCraft);
        return true;
    }

    // 获取已装备物品列表
    public List<InventoryItem> GetEquipmentList() => equipment;

    // 获取仓库物品列表
    public List<InventoryItem> GetStashList() => stash;

    // 获取指定类型的已装备物品
    public ItemData_Equipment GetEquipment(EquipmentType _type)
    {
        ItemData_Equipment equipedItem = null;

        // 遍历装备字典查找指定类型的装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictianory)
        {
            if (item.Key.equipmentType == _type)
            {
                equipedItem = item.Key;
                break;  // 找到后立即跳出循环
            }
        }
        return equipedItem;
    }
    
    // 使用药水
    public void UseFlask()
    {
        // 获取当前装备的药水
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);
        if (currentFlask == null)
        {
            return;  // 没有装备药水
        }

        // 检查药水是否冷却完毕
        bool canUseFlask = Time.time > lastTimeUsedFlask + flaskCooldown;

        if (canUseFlask)
        {
            // 设置新的冷却时间
            flaskCooldown = currentFlask.itemCooldown;
            // 触发药水效果
            currentFlask.Effect(null);
            // 记录使用时间
            lastTimeUsedFlask = Time.time;
        }
    }
    
    // 检查是否可以使用护甲特效
    public bool canUseArmor()
    {
        // 获取当前装备的护甲
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if (currentArmor == null)
        {
            return false;  // 没有装备护甲
        }
        
        // 检查护甲是否冷却完毕
        if (Time.time > lastTimeUsedAmulet + armorCooldown)
        {
            // 设置新的冷却时间
            armorCooldown = currentArmor.itemCooldown;
            // 记录使用时间
            lastTimeUsedAmulet = Time.time;
            return true;
        }
        return false;
    }

    // 从存档加载背包数据（ISaveManager接口实现）
    public void LoadData(GameData _data)
    {
        // 加载背包和仓库物品
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in GetItemDataBase())
            {
                // 根据物品ID匹配物品数据
                if (item != null && item.itemId == pair.Key)
                {
                    // 创建物品并设置堆叠数量
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    // 添加到加载的物品列表
                    loadedItems.Add(itemToLoad);
                }
            }
        }
        
        // 加载装备物品
        foreach (string loadedItemId in _data.equipmentId)
        {
            foreach (var item in GetItemDataBase())
            {
                // 根据物品ID匹配装备数据
                if (item != null && loadedItemId == item.itemId)
                {
                    // 添加到加载的装备列表
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    // 保存背包数据到存档（ISaveManager接口实现）
    public void SaveData(ref GameData _data)
    {
        // 清空现有数据
        _data.inventory.Clear();
        _data.equipmentId.Clear();

        // 合并背包和仓库物品，避免重复添加相同itemId
        Dictionary<string, int> combinedInventory = new Dictionary<string, int>();

        // 添加背包物品
        foreach (var pair in inventoryDictianory)
        {
            if (combinedInventory.ContainsKey(pair.Key.itemId))
                combinedInventory[pair.Key.itemId] += pair.Value.stackSize;
            else
                combinedInventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        // 添加仓库物品
        foreach (var pair in stashDictianory)
        {
            if (combinedInventory.ContainsKey(pair.Key.itemId))
                combinedInventory[pair.Key.itemId] += pair.Value.stackSize;
            else
                combinedInventory.Add(pair.Key.itemId, pair.Value.stackSize);
        }

        // 将合并后的数据写入_data.inventory
        foreach (var pair in combinedInventory)
        {
            _data.inventory.Add(pair.Key, pair.Value);
        }

        // 保存装备ID列表
        foreach (var pair in equipmentDictianory)
        {
            _data.equipmentId.Add(pair.Key.itemId);
        }
    }
    
    // 获取物品数据库
    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();

#if UNITY_EDITOR
        // 在编辑器模式下，从资源文件夹加载所有物品数据
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }
#else
        // 在构建版本中使用一个预定义的物品清单
        // 这里简单返回 startingItems 作为后备方案
        foreach (ItemData item in startingItems)
        {
            if (item != null && !itemDataBase.Contains(item))
                itemDataBase.Add(item);
        }
#endif

        return itemDataBase;
    }
}
