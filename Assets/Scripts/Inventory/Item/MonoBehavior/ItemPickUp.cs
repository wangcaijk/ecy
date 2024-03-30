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
            //todo:��������ӵ�����
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            //װ������
            //GameManager.Instance.playerStats.EquipWeapon(itemData);
            //�����������
            QuestManager.Instance.UpdateQuestProgrees(itemData.itemName, itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}