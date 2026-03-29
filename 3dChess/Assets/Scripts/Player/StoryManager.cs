using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public int TrainersPerZone = 5;
    public int ZoneCount = 4;

    public List<Trainer> Trainers;
    public List<Gate> Gates;

    private void Start()
    {
        Trainers = new(FindObjectsOfType<Trainer>().OrderBy(t => t.Zone));
        Gates = new(FindObjectsOfType<Gate>().OrderBy(g => g.Index));

        int completedZones = 0;

        for (int i = 0; i < ZoneCount; i++)
        {
            int trainersDefeatedInZone = Trainers.Count(t => t.Defeated && t.Zone == i);
            print(i + " " + trainersDefeatedInZone);
            if (trainersDefeatedInZone >= TrainersPerZone)
            {
                var gate = Gates.Where(g => g.Index == i).First();
                gate.Open();
                completedZones++;
            }
        }
    }

    public int GetTrainersDefeatedbyZone(int zone)
    {
        return Trainers.Where(t => t.Defeated && t.Zone == zone).Count();
    }
}
