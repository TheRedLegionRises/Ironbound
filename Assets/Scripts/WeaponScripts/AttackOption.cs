using System;
using UnityEngine;

[Serializable]
public class AttackOption
{
    public string attackName;

    [Header("Attack Properties")]
    public int attackRange = 1;
    public int attackAPCost = 2;

    [Header("Damage")]
    public float damageMultiplier = 1f;
}