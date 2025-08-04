using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Coroutine Think;
    public void Create()
    {
        Think = StartCoroutine(MakeMove());
    }
    public void Stop()
    {
        StopCoroutine(Think);
    }

    private IEnumerator MakeMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => Ref.BattleManager.Turn % 2 == 1);
            yield return new WaitForSeconds(2);
            Ref.BattleManager.ProcessMove(Ref.BattleManager.player2.Moves[Random.Range(0, Ref.BattleManager.player2.Moves.Count)]);
        }
        

    }
}
