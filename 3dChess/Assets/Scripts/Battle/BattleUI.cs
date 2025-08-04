using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public Slider S_P1, S_P2;
    public TMP_Text T_Name1, T_Name2, T_Lvl1, T_Lvl2;

    public BattleManager BattleManager;

    public GameObject OrgMove;
    public List<GameObject> MoveUI;
    public GameObject PlayerUIOverlay, AttackUI, DialogueUI;
    

    public void Create(BattleManager battleManager)
    {

        BattleManager = battleManager;

        T_Name1.text = BattleManager.player1.Name == "" ? BattleManager.player1.GetType().ToString() : BattleManager.player1.Name;
        T_Name2.text = BattleManager.player2.Name == "" ? BattleManager.player2.GetType().ToString() : BattleManager.player2.Name;

        T_Lvl1.text = "lvl " +  BattleManager.player1.Level.ToString();
        T_Lvl2.text = "lvl" + BattleManager.player2.Level.ToString();

        S_P1.maxValue = BattleManager.player1.MaxHealth;
        S_P1.value = S_P1.maxValue;

        S_P2.maxValue = BattleManager.player2.MaxHealth;
        S_P2.value = S_P2.maxValue;

        foreach(var m in BattleManager.player1.Moves)
        {
            GameObject g = Instantiate(OrgMove, OrgMove.transform.parent);
            g.GetComponentInChildren<TMP_Text>().text = m.Name;
            MoveUI.Add(g);
            g.SetActive(true);
            g.GetComponent<Button>().onClick.AddListener(() => { BattleManager.ProcessMove(m); });
        }
    }

    public void UpdateHealth()
    {
        S_P1.value = BattleManager.player1.Health;
        S_P2.value = BattleManager.player2.Health;
    }

    public void UpdateUI()
    {
        bool b = BattleManager.Turn % 2 != 0;
        PlayerUIOverlay.SetActive(b);
        AttackUI.SetActive(b);
        DialogueUI.SetActive(!b);
    }
}
