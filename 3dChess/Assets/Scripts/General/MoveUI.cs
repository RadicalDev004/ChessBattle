using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveUI : MonoBehaviour
{
    public TMP_Text T_Name, T_Description, T_Type, T_Action;
    private Move thisMove;

    public void Create(Move m)
    {
        thisMove = m;
        T_Name.text = m.Name;
        T_Description.text = m.Description;
        T_Type.text = m.Type.ToString();
        T_Action.text = m.Action.ToString();
    }
}
