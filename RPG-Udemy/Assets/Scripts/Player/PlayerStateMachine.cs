using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerStateMachine.cs摘要
// 玩家状态机，负责管理和切换玩家的不同状态（如闲置、移动、跳跃等）

public class PlayerStateMachine : MonoBehaviour
{
    // 当前活动的玩家状态
    public PlayerState currentState {  get; private set; }

    // 初始化状态机，设置起始状态
    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;    // 设置当前状态
        currentState.Enter();          // 调用状态的进入方法
    }
    
    // 切换到新状态
    public void ChangeState(PlayerState _newState)
    {
        currentState.Exit();           // 退出当前状态
        currentState = _newState;      // 更新当前状态
        currentState.Enter();          // 进入新状态
    }
}
