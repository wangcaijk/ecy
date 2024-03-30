using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dailogue",menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();

    #if UNITY_EDITOR
    void OnValidate()//仅在编辑器内执行导致打包游戏后字典空了
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))
                dialogueIndex.Add(piece.ID, piece);
        }
    }
    #else
    void Awake()//保证在打包执行的游戏里第一时间获得对话的所有字典匹配 
    {
        dialogueIndex.Clear();
        foreach (var piece in dialoguePieces)
        {
            if (!dialogueIndex.ContainsKey(piece.ID))
                dialogueIndex.Add(piece.ID, piece);
        }
    }
    #endif

    public QuestData_SO GetQuest()
    {
        QuestData_SO currentQuest=null;
        foreach (var piece in dialoguePieces)
        {
            if (piece.quest != null)
                currentQuest = piece.quest;
        }

        return currentQuest;
    }
}
