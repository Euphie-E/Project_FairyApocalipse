using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

public class GenericBoss : MonoBehaviour
{
    protected float cooldownTime = 1f;
    protected float cooldownVariation = 0f;
    protected List<Action> actionList = new List<Action>();
    IEnumerator cooldownCoroutine;
    protected void Run()
    {
        cooldownCoroutine = Cooldown();
        StartCoroutine(cooldownCoroutine);
    }
    protected void Stop()
    {
        StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = null;
    }
    void Action()
    {
        actionList[Random.Range(0, actionList.Count)]();
        StartCoroutine(cooldownCoroutine);
    }
    IEnumerator Cooldown()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldownTime + Random.Range(-cooldownVariation, cooldownVariation));
            Action();
        }
        
    }
}
