using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    private Quest thisQuest;
    public TMP_Text T_Name, T_Description, T_Reward;
    public Slider S_Progress;
    public Button B_Claim;
    public GameObject CompletedOverlay;

    public void Create(Quest quest)
    {
        thisQuest = quest;

        T_Name.text = quest.Name;
        T_Description.text = quest.Description;
        T_Reward.text = $"Reward: <b>{quest.Reward.ToSpacedNumber()}</b>";

        S_Progress.minValue = quest.ProgressProvider.GetMin();
        S_Progress.maxValue = quest.ProgressProvider.GetMax();
        S_Progress.value = quest.ProgressProvider.GetProgress();

        if (quest.ProgressProvider.GetProgress() >= quest.ProgressProvider.GetMax())
        {
            quest.Type = Quest.QuestType.Completed;
        }

        switch (quest.Type)
        {
            case Quest.QuestType.InProgress:
                B_Claim.gameObject.SetActive(false);
                break;

            case Quest.QuestType.Completed:
                B_Claim.gameObject.SetActive(true);
                break;

            case Quest.QuestType.Claimed:
                B_Claim.gameObject.SetActive(false);
                CompletedOverlay.SetActive(true);
                S_Progress.value = S_Progress.maxValue;
                break;

        }
    }

    public void Refresh()
    {
        Create(thisQuest);
    }
}
