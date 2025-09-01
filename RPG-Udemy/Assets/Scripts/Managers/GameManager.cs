using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager.cs摘要
// 游戏管理器，负责管理游戏全局状态、检查点系统和丢失货币的处理
// 实现了ISaveManager接口，支持游戏存档和读档功能

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance; // 单例模式，全局访问点

    private Transform player; // 玩家的位置引用

    // 检查点系统
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab; // 丢失货币的预制体
    public int lostCurrencyAmount;                          // 丢失货币的数量
    [SerializeField] private float lostCurrencyX;           // 丢失货币的X坐标
    [SerializeField] private float lostCurrencyY;           // 丢失货币的Y坐标

    // 初始化单例并查找所有检查点
    private void Awake()
    {
        // 确保场景中只有一个GameManager实例
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        checkpoints = FindObjectsOfType<Checkpoint>(); // 查找场景中所有的检查点
    }

    // 获取玩家引用
    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }

    // 重新加载当前场景
    public void RestartScene()
    {
        SaveManager.instance.SaveGame(); // 保存游戏状态
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // 加载游戏数据（ISaveManager接口实现）
    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    // 加载检查点状态
    private void LoadCheckpoint(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints) // 遍历存档中的检查点数据
        {
            foreach (Checkpoint checkpoint in checkpoints) // 遍历场景中的所有检查点
            {
                // 如果检查点ID匹配且状态为激活，则激活该检查点
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckPoint();
            }
        }
    }

    // 加载丢失的货币
    private void LoadLostCurrency(GameData _data)
    {
        // 从存档中读取丢失货币的信息
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        // 如果有丢失的货币，则在对应位置生成货币对象
        if (lostCurrencyAmount > 0)
        {
            GameObject newlostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newlostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0; // 重置丢失的货币数量
    }

    // 延迟加载数据，确保其他系统已准备就绪
    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.5f); // 等待0.5秒

        // 按顺序加载各种数据
        LoadCheckpoint(_data);
        LoadClosetCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    // 保存游戏数据（ISaveManager接口实现）
    public void SaveData(ref GameData _data)
    {
        // 保存丢失货币的信息
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        // 保存最近检查点的ID
        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().id;

        // 保存所有检查点的状态
        _data.checkpoints.Clear();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }

    // 将玩家传送到最近的检查点
    private void LoadClosetCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckpointId = _data.closestCheckpointId;

        // 查找匹配ID的检查点并将玩家传送到该位置
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
                player.position = checkpoint.transform.position;
        }
    }

    // 查找距离玩家最近的已激活检查点
    private Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            // 只考虑已激活的检查点
            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }

    // 暂停或恢复游戏
    public void PauseGame(bool _pause)
    {
        if (_pause)
        {
            Time.timeScale = 0f; // 暂停游戏时间
        }
        else
        {
            Time.timeScale = 1f; // 恢复正常游戏时间
        }
    }
}
