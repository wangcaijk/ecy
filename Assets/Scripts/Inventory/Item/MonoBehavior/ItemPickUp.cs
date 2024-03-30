using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemPickUp : MonoBehaviourPun
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            //todo:将物体添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            //更新任务进度
            QuestManager.Instance.UpdateQuestProgrees(itemData.itemName, itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}