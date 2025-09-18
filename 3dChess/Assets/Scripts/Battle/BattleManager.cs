using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleUI BattleUI;
    public Dictionary<Entity, GameObject> player1Others = new(), player2Others = new();
    public Entity player1, player2;
    public GameObject P1, P2;
    public Transform Pos1, Pos2, Pos1Behind, Pos2Behind;
    public Tile contestedTile;
    public int Turn;
    public bool Winner;

    public Action<bool, Tile> OnBattleEnd;

    public void StartBattle(Entity e1, Entity e2, Tile t, bool start)
    {
        Ref.AI.Create();     
        OnBattleEnd = null;
        player1 = ((Piece)e1).side ? e1 : e2;
        player2 = ((Piece)e1).side ? e2 : e1;
        contestedTile = t;
        Turn = start ? 0 : 1;

        var player1others = FindObjectsOfType<Piece>().Where(e => e.side == ((Piece)player1).side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player1).Cast<Entity>().ToList();
        var player2others = FindObjectsOfType<Piece>().Where(e => e.side == ((Piece)player2).side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player2).Cast<Entity>().ToList();

        player1Others.Clear();
        player2Others.Clear();

        P1 = CreateNewShowPiece(player1.gameObject, Pos1);
        P2 = CreateNewShowPiece(player2.gameObject, Pos2);

        foreach (var otherPiece in player1others)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, Pos1Behind);
            player1Others.Add(otherPiece, newPiece);
        }
        foreach (var otherPiece in player2others)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, Pos2Behind);
            player2Others.Add(otherPiece, newPiece);
        }

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

        Destroy(P1);
        Destroy(P2);

        Debug.Log(Winner);
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
            attacker.GiveExp(m.Action / 10);
            if (defender.Health <= 0)
                attacker.GiveExp(30);
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
            foreach (var pair in (Winner ? player1Others : player2Others))
            {
                pair.Key.GiveExp(15);
            }
            var winnerPiece = Winner ? player1 : player2;
            winnerPiece.GiveExp(15);
            
            EndBattle();
        }
    }

    public GameObject CreateNewShowPiece(GameObject p, Transform pos)
    {
        var newPiece = Instantiate(p);
        Destroy(newPiece.GetComponent<Piece>());
        Destroy(newPiece.transform.GetChild(0).gameObject);
        newPiece.transform.SetParent(transform);
        newPiece.transform.position = new Vector3(pos.transform.position.x, newPiece.transform.position.y, pos.transform.position.z);
        return newPiece;
    }

    public void SwitchPiece(Entity e, bool side)
    {
        StartCoroutine(SwitchPieceCor(side ? player1 : player2, e, side ? P1 : P2, side ? player1Others[e] : player2Others[e], side ? Pos1 : Pos2, side ? Pos1Behind : Pos2Behind, side));
    }

    private IEnumerator SwitchPieceCor(Entity e1, Entity e2, GameObject P1, GameObject P2, Transform Pos1, Transform Pos2, bool side)
    {
        Tween.Position(P1.transform, Pos2.position, 0.5f, 0, Tween.EaseIn);
        yield return new WaitForSeconds(0.75f);
        Tween.Position(P2.transform, Pos1.position, 0.5f, 0, Tween.EaseOut);
        yield return new WaitForSeconds(0.5f);

        if (side)
        {
            player1 = e2;            
            player1Others.Remove(e2);
            player1Others.Add(e1, P1);
            this.P1 = P2;
        }
        else
        {
            player2 = e2;
            player2Others.Remove(e2);
            player2Others.Add(e1, P1);
            this.P2 = P2;
        }
        BattleUI.Create(this);
        Turn++;
        BattleUI.UpdateUI();
    }

}
