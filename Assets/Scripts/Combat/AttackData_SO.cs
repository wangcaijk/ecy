using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public int minDamage;
    public int maxDamage;
    public float criticalMultiplier;
    public float criticalChance;
    public bool isStaff;

    public void ApplyWeaponData(AttackData_SO weapon)
    {
        attackRange = weapon.attackRange;
        skillRange = weapon.skillRange;
        coolDown = weapon.coolDown;

        minDamage = weapon.minDamage;
        maxDamage = weapon.maxDamage;

        criticalChance = weapon.criticalChance;
        criticalMultiplier = weapon.criticalMultiplier;

        isStaff = weapon.isStaff;
    }
}