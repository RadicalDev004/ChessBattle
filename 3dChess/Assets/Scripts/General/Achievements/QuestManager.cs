using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> Quests = new();
    public List<QuestUI> QuestUIs = new();

    public TMP_Text T_QuestsCount;
    public QuestUI OriginalQuestUI;

    public GameObject QuestIcon;

    private void Awake()
    {
        var currY = QuestIcon.transform.position.y;
        Tween.Position(QuestIcon.transform, new Vector3(QuestIcon.transform.position.x, currY + 0.15f, QuestIcon.transform.position.z), 1f, 0, Tween.EaseOut, loop: Tween.LoopType.PingPong);
    }

    public List<QuestData> GetQuestsData()
    {
        var questDatas = new List<QuestData>();
        foreach (var quest in Quests)
        {
            var questData = new QuestData
            {
                Index = quest.Index,
                Type = quest.Type
            };
            questDatas.Add(questData);
        }
        return questDatas;
    }

    public void RefreshQuestData()
    {
        foreach (var qui in QuestUIs)
        {
            qui.Refresh();
        }
    }

    public void SetQuestsData(List<QuestData> questsData)
    {
        QuestUIs.ClearObjects();
        foreach (var quest in Quests)
        {
            var questData = questsData.Find(qd => qd.Index == quest.Index);
            if (questData != null)
            {
                quest.Type = questData.Type;
            }
            CreateQuestUI(quest);
        }
        UpdateQuestInfo();
    }

    public void UpdateQuestInfo()
    {
        T_QuestsCount.text = $"{Quests.Count(q => q.Type == Quest.QuestType.Completed)}/{Quests.Count}";
    }

    public void CreateQuestUI(Quest quest)
    {
        var questUI = Instantiate(OriginalQuestUI, OriginalQuestUI.transform.parent);
        questUI.gameObject.SetActive(true);
        questUI.Create(quest);
        QuestUIs.Add(questUI);
    }
}
