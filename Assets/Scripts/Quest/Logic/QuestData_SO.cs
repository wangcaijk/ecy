using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest",menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
   [System.Serializable]
   public class QuestRequire
   {
      public string name;
      public int requirAmount;
      public int currentAmount;
   }
   public string questName;
   [TextArea] public string description;

   public bool isStarted;
   public bool isComplete;
   public bool isFinished;

   public List<QuestRequire> questRequires = new List<QuestRequire>();
   public List<InventoryItem> rewards = new List<InventoryItem>();

   public void CheckQuestProgress()
   {
      var finishRequires = questRequires.Where(r => r.requirAmount <= r.currentAmount);
      isComplete = finishRequires.Count() == questRequires.Count;

      if (isComplete)
      {
         Debug.Log("任务完成");
      }
   }
   //当前任务需要的收集消灭目标的名字列表
   public List<string> RequireTargetName()
   {
      List<string> targetNameList = new List<string>();

      foreach (var require in questRequires)
      {
         targetNameList.Add(require.name);   
      }

      return targetNameList;
   }
}
