using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SerializableDictionary.cs摘要
/// 可序列化的字典类，解决Unity无法直接序列化Dictionary的问题
/// 通过实现ISerializationCallbackReceiver接口，将字典的键值对转换为可序列化的列表
/// </summary>
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // 用于序列化的键列表
    [SerializeField] private List<TKey> keys = new List<TKey>();
    // 用于序列化的值列表
    [SerializeField] private List<TValue> values = new List<TValue>();

    /// <summary>
    /// 序列化前调用，将字典中的键值对转换为两个并行列表
    /// </summary>
    public void OnBeforeSerialize()
    {
        // 清空现有列表
        keys.Clear();
        values.Clear();

        // 将字典中的每个键值对添加到对应的列表中
        foreach (KeyValuePair<TKey, TValue> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    /// <summary>
    /// 反序列化后调用，将两个并行列表转换回字典
    /// </summary>
    public void OnAfterDeserialize()
    {
        // 清空字典
        this.Clear();

        // 验证键列表和值列表长度是否一致
        if (keys.Count != values.Count)
        {
            Debug.LogError("序列化错误：键数和值数不相等");
        }

        // 重建字典
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
