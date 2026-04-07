using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Searchable : MonoBehaviour
{
    public string Name;
    public bool isEmpty = false;
    public float distanceToPlayer;
    public float Range = 2f;
    public int Zone;
    [HideInInspector]
    public float sqrRange;

    private void Awake()
    {
        sqrRange = Range * Range;
        ChangeState(true);
    }

    public void ChangeState(bool state)
    {
        isEmpty = state;
        GetComponent<Outline>().enabled = !isEmpty;
    }


    public void Search()
    {
        isEmpty = true;
        GetComponent<Outline>().enabled = false;

        GameRef.PlayerBehaviour.BoxBehaviour.PrepareBox();
    }
}
