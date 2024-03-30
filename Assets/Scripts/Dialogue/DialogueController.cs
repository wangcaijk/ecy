using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
     bool canTalk = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialougePanel.SetActive(false);
            canTalk = false;
        }
    }

    void Update()
    {
        if (canTalk && Input.GetMouseButtonDown(1))
        {
            OpenDialogue();
        }
    }

    void OpenDialogue()
    {
        //打开UI
        //传输对话内容
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);
    }
}
