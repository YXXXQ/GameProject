using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// 物品类型枚举
public enum ItemType
{
    Material,   // 材料类物品（可堆叠，存放在仓库）
    Equipment   // 装备类物品（不可堆叠，存放在背包）
}

// ItemData.cs摘要
// 物品数据基类，定义了所有物品共有的属性和方法
// 作为ScriptableObject，可以在Unity编辑器中创建和配置物品数据

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;   // 物品类型（材料或装备）
    public string itemName;     // 物品名称
    public Sprite itemIcon;     // 物品图标
    public string itemId;       // 物品唯一ID（自动生成）

    [Range(0, 100)]
    public int dropChance;      // 物品掉落几率（0-100%）

    protected StringBuilder sb = new StringBuilder();  // 用于构建描述文本

    // 在编辑器中验证时自动生成物品ID
    private void OnValidate()
    {
#if UNITY_EDITOR
        // 使用资源路径的GUID作为物品的唯一ID
        string path = AssetDatabase.GetAssetPath(this);
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    // 获取物品描述，由子类重写以提供具体描述
    public virtual string GetDescription()
    {
        return "";
    }
}
