using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ski : MonoBehaviour
{
    public List<GameObject> Chairs = new();

    public float P1, P2;
    public GameObject SkiMachine;
    public GameObject Wheel1, Wheel2;
    public float time = 2;

    private void Start()
    {  
        this.ActionAfterTime(0.5f, () =>
        {
            Tween.Rotate(Wheel1.transform, new Vector3(0, -360, 0), Space.Self, time, 0, loop: Tween.LoopType.Loop);
            Tween.Rotate(Wheel2.transform, new Vector3(0, -360, 0), Space.Self, time, 0, loop: Tween.LoopType.Loop);

            foreach (GameObject obj in Chairs)
            {
                StartCoroutine(MoveChair(obj));
            }
        });   
    }

    public IEnumerator MoveChair(GameObject chair)
    {
        bool side = chair.transform.localPosition.x <= 13;
        var toTravel = Mathf.Abs((side ? P1 : P2) - chair.transform.localPosition.z);
        var timePerDistance = time * 7 / Mathf.Abs(P2 - P1);
        
        bool first = true;

        while (true)
        {
            Tween.LocalPosition(chair.transform, new Vector3(chair.transform.localPosition.x, chair.transform.localPosition.y, side ? P1 : P2), first ? timePerDistance * toTravel : time * 7, 0);
            yield return new WaitForSecondsRealtime(first ? timePerDistance * toTravel : time * 7);

            chair.transform.SetParent(side ? Wheel1.transform : Wheel2.transform);
            yield return new WaitForSecondsRealtime(time / 2);

            chair.transform.SetParent(SkiMachine.transform);

            first = false;
            side = !side;
        }        
    }
}
