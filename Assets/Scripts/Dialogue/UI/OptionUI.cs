using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    public Button thisButton;
    public DialoguePiece currentPiece;

    private bool takeQuest;
    private string nextPieceID;
    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionCliked);
        
    }
    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
    }

    public void OnOptionCliked()
    {
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(currentPiece.quest)
            };
            if (takeQuest)
            {
                //��������б�
                //�ж��Ƿ��Ѿ����ڸ�����
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //�ж��Ƿ�����˽���
                }
                else
                {
                    //û������ ����������
                    QuestManager.Instance.tasks.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }
        
        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialougePanel.SetActive(false);
            return;
        }
        else
        {
           DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
}
