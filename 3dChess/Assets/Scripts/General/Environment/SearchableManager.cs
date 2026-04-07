using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchableManager : MonoBehaviour
{
    public List<Searchable> Searchables = new();
    public List<Searchable> AvailableSearchables = new();
    public HashSet<Searchable> InRangeSearchables = new();
    public List<BeachBall> BeachBalls = new();
    public Searchable ClosestSearchable;
    public int LossStatistic;

    private void Start()
    {
        Searchables = new(FindObjectsOfType<Searchable>());
        BeachBalls = new(FindObjectsOfType<BeachBall>(true));

        foreach (Searchable searchable in Searchables)
        {
            if (searchable.Zone <= GameRef.StoryManager.CurrentZone)
            {
                AvailableSearchables.Add(searchable);
            }
        }
        
        LossStatistic = GameRef.PlayerBehaviour.SaveManager.GetLossesToWinsBalance() + 1;

        var activeSearchables = AvailableSearchables.GetRandomElements(LossStatistic);
        activeSearchables.ForEach(searchable => { searchable.ChangeState(false); });

        if (GameRef.StoryManager.CurrentZone >= 2)
        {
            BeachBalls.ForEach(ball => { ball.gameObject.SetActive(false); });
            var balls = BeachBalls.GetRandomElements(Mathf.Min(LossStatistic, BeachBalls.Count));
            balls.ForEach(ball => { ball.gameObject.SetActive(true); });
        }

        print($"Activated {LossStatistic} searchables");
    }

    private void Update()
    {
        Vector3 playerPos = GameRef.PlayerBehaviour.transform.position;

        ClosestSearchable = null;
        float closestSqrDist = float.MaxValue;

        foreach (var searchable in AvailableSearchables)
        {
            if (searchable.isEmpty)
                continue;

            float sqrDist = (searchable.transform.position - playerPos).sqrMagnitude;
            if (sqrDist <= searchable.sqrRange)
            {
                InRangeSearchables.Add(searchable);

                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    ClosestSearchable = searchable;
                }
            }
            else
            {
                InRangeSearchables.Remove(searchable);
            }
        }

        if (ClosestSearchable != null)
        {
            GameRef.UI.ToggleSearchItem(true, ClosestSearchable.Name);
            if (Input.GetKeyDown(KeyCode.F))
            {
                Search(ClosestSearchable);
            }
        }
        else
        {
            GameRef.UI.ToggleSearchItem(false);
        }
    }

    public void Search(Searchable searchable)
    {
        InRangeSearchables.Remove(searchable);
        searchable.Search();
    }
}
