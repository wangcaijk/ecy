using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;


public class CharacterStats : MonoBehaviourPun
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")] public Transform weaponSlot;

    [HideInInspector] public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }

        baseAttackData = Instantiate(attackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO;

    public int MaxHealth
    {
        get
        {
            if (characterData != null) return characterData.maxHealth;
            else return 0;
        }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null) return characterData.currentHealth;
            else return 0;
        }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null) return characterData.baseDefence;
            else return 0;
        }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null) return characterData.currentDefence;
            else return 0;
        }
        set { characterData.currentDefence = value; }
    }

    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //TODO:AnimatorUpdateMode UI
        photonView.RPC("UpdateHealthBarOnAttackRpc", RpcTarget.All);

        //TODO:经验值Update
        if (characterData.currentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    [PunRPC]
    public void UpdateHealthBarOnAttackRpc()
    {
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(int damage, CharacterStats defener)
    {
        int currentDamge = Mathf.Max(damage - defener.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamge, 0);
        photonView.RPC("UpdateHealthBarOnAttackRpc()", RpcTarget.All);
        //UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        if (defener.characterData.currentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        GameObject wp;
        attackData.ApplyWeaponData(weapon.WeaponData);
        if (attackData.isStaff)
        {
            photonView.RPC("EquipStaff", RpcTarget.All);
        }
        else
        {
            if (weapon.weaponPrefab != null)
            {
                wp = PhotonNetwork.Instantiate(weapon.PrefabName, weaponSlot.transform.position,
                    weaponSlot.transform.rotation, 0);
                wp.transform.SetParent(weaponSlot);
            }
        }


        //TODO:更新属性

        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;


        //InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                if (weaponSlot.transform.GetChild(i).GetComponent<Staff>() != null)
                {
                    photonView.RPC("UnEquipStaff", RpcTarget.All);
                }
                else
                {
                    PhotonNetwork.Destroy(weaponSlot.transform.GetChild(i).gameObject);
                }
            }
        }

        attackData.ApplyWeaponData(baseAttackData);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    #endregion

    #region Apply Data Change

    public void ApplyHealth(int amount)
    {
        if (CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }

    #endregion

    #region PUN CallBack

// public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
// {
//     if (stream.IsWriting)
//     {
//         //发送
//         stream.SendNext(characterData.currentHealth);
//     }
//     else
//     {
//         //接受
//         characterData.currentHealth = (int)stream.ReceiveNext();
//     }
// }
    [PunRPC]
    public void EquipStaff()
    {
        PlayerController pc = gameObject.GetComponent<PlayerController>();
        pc.staff.gameObject.SetActive(true);
        pc.IsMagic = true;
    }

    [PunRPC]
    public void UnEquipStaff()
    {
        PlayerController pc = gameObject.GetComponent<PlayerController>();
        pc.staff.gameObject.SetActive(false);
        pc.IsMagic = false;
    }

    #endregion
}