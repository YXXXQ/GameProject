using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// EnemyStateMachine.cs摘要
// 敌人状态机，负责管理和切换敌人的不同状态（如巡逻、追击、攻击等）

public class EnemyStateMachine 
{
    // 当前活动的敌人状态
    public EnemyState currentState {  get; private set; }

    // 初始化状态机，设置起始状态
    public void Initialize(EnemyState _startState)
    {
        currentState = _startState;    // 设置当前状态
        currentState.Enter();          // 调用状态的进入方法
    }

    // 切换到新状态
    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();           // 退出当前状态
        currentState = _newState;      // 更新当前状态
        currentState.Enter();          // 进入新状态
    }
}
