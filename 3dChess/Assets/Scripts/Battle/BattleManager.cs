using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleUI BattleUI;
    public Entity player1, player2;
    public GameObject P1, P2;
    public Transform Pos1, Pos2;
    public Tile contestedTile;
    public int Turn;
    public bool Winner;

    public Action<bool, Tile> OnBattleEnd;

    public void StartBattle(Entity e1, Entity e2, Tile t)
    {
        Ref.AI.Create();
        OnBattleEnd = null;
        player1 = e1;
        player2 = e2;
        contestedTile = t;
        Turn = 0;

        P1 = Instantiate(player1.gameObject);
        Destroy(P1.GetComponent<Piece>());
        Destroy(P1.transform.GetChild(0).gameObject);
        P1.transform.SetParent(transform);
        P1.transform.position = new Vector3(Pos1.transform.position.x, P1.transform.position.y, Pos1.transform.position.z);

        P2 = Instantiate(player2.gameObject);
        Destroy(P2.GetComponent<Piece>());
        Destroy(P2.transform.GetChild(0).gameObject);
        P2.transform.SetParent(transform);
        P2.transform.position = new Vector3(Pos2.transform.position.x, P2.transform.position.y, Pos2.transform.position.z);

        Ref.Camera.gameObject.SetActive(false);
        Ref.BattleCamera.gameObject.SetActive(true);

        BattleUI.gameObject.SetActive(true);
        BattleUI.Create(this);
    }

    public void EndBattle()
    {
        BattleUI.gameObject.SetActive(false);

        Ref.Camera.gameObject.SetActive(true);
        Ref.BattleCamera.gameObject.SetActive(false);
        Ref.AI.Stop();
        player1.UpdateUI();
        player2.UpdateUI();

        OnBattleEnd?.Invoke(Winner, contestedTile);
    }


    public void ProcessMove(Move m)
    {
        GameObject attackerObj = Turn % 2 == 0 ? P1! : P2;
        GameObject defenderObj = Turn % 2 != 0 ? P1 : P2;

        Entity attacker = Turn % 2 == 0 ? player1 : player2;
        Entity defender = Turn % 2 != 0 ? player1 : player2;
        Turn++;
        BattleUI.UpdateUI();

        //Move Animation

        StartCoroutine(StraightAttack(attacker, defender, attackerObj, defenderObj, m));
    }

    private IEnumerator StraightAttack(Entity attacker, Entity defender, GameObject attackerObj, GameObject defenderObj, Move m)
    {
        Vector3 initialPos = attackerObj.gameObject.transform.position;
        Tween.Position(attackerObj.gameObject.transform, defenderObj.gameObject.transform.position, 0.5f, 0, Tween.EaseIn);

        if(m.Type == MoveType.Attack)
        {
            defender.Health -= (int)m.Action;
        }
        else if(m.Type == MoveType.Heal)
        {
            attacker.Health += (int)m.Action;
        }
            BattleUI.UpdateHealth();

        yield return new WaitForSeconds(0.5f);
        Tween.Position(attackerObj.gameObject.transform, initialPos, 0.5f, 0, Tween.EaseOut);
        if(player1.Health <= 0 || player2.Health <= 0)
        {
            Winner = player2.Health <= 0;
            EndBattle();
        }
    }
}
