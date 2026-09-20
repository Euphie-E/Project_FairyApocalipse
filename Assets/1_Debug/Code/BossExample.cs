using UnityEngine;
using System.Collections.Generic;
using System;

public class BossExample : GenericBoss
{
    public static BossExample Self;
    public enum BossStates
    {
        Disabled,
        Alive,
        Gay,
        Dead
    }
    BossStates currentState = BossStates.Disabled;
    List<List<Action>> listernerList = new List<List<Action>>();
    public enum BossEvents
    {
        OnDeath,
        OnAttack,
        OnAttack2
    }
    void Awake()
    {
        Self = this;
        for(int i = 0; i < Enum.GetNames(typeof(BossEvents)).Length; i++)
        {
            //Debug.Log("Adding listener list for event: " + ((BossEvents)i).ToString());
            listernerList.Add(new List<Action>());
        }
    }
    void Start()
    {
        Debug.Log("Tt Start");
        actionList.Add(Attack);
        actionList.Add(Attack2);
        currentState = BossStates.Alive;
        base.Run();
        cooldownTime = 3f;
        cooldownVariation = 1f;
    }
    public bool clicked = false;
    void Update()
    {
        if(clicked)
        {
            clicked = false;
            BroadcastEvent(BossEvents.OnDeath);
            base.Stop();
        }
    }
    void Attack()
    {
        if(currentState == BossStates.Alive)
        {
            Debug.Log("Tt Attack");
            BroadcastEvent(BossEvents.OnAttack);
        }
    }
    void Attack2()
    {
        if(currentState == BossStates.Alive)
        {
            Debug.Log("Tt Attack2");
            BroadcastEvent(BossEvents.OnAttack2);
        }
    }

    void BroadcastEvent(BossEvents bossEvent)
    {
        //Debug.Log("BroadcastEvent: " + bossEvent.ToString());
        //Debug.Log("BroadcastEvent: " + listernerList[(int)bossEvent].Count.ToString());
        foreach(Action action in listernerList[(int)bossEvent])
        {
            action();
        }
    }
    public void AddListener(BossEvents bossEvent, Action action)
    {
        listernerList[(int)bossEvent].Add(action);
    }
    public void RemoveListener(BossEvents bossEvent, Action action)
    {
        listernerList[(int)bossEvent].Remove(action);
    }
}
