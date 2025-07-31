using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Entity player1, player2;
    public GameObject P1, P2;
    public Transform Pos1, Pos2;
    public Tile contestedTile;
    public int Turn;
    public bool Winner;

    public Action<bool, Tile> OnBattleEnd;

    public void StartBattle(Entity e1, Entity e2, Tile t)
    {
        OnBattleEnd = null;
        player1 = e1;
        player2 = e2;
        contestedTile = t;
        Turn = 0;

        P1 = Instantiate(player1.gameObject);
        Destroy(P1.GetComponent<Piece>());
        P1.transform.SetParent(transform);
        P1.transform.position = new Vector3(Pos1.transform.position.x, P1.transform.position.y, Pos1.transform.position.z);

        P2 = Instantiate(player2.gameObject);
        Destroy(P2.GetComponent<Piece>());
        P2.transform.SetParent(transform);
        P2.transform.position = new Vector3(Pos2.transform.position.x, P2.transform.position.y, Pos2.transform.position.z);

        Ref.Camera.gameObject.SetActive(false);
        Ref.BattleCamera.gameObject.SetActive(true);
    }

    public void EndBattle()
    {
        Ref.Camera.gameObject.SetActive(true);
        Ref.BattleCamera.gameObject.SetActive(false);

        OnBattleEnd?.Invoke(Winner, contestedTile);
    }
}
