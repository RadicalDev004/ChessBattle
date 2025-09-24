using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceUI : MonoBehaviour
{
    public TMP_Text T_PieceName, T_Level;
    public Slider S_Health;
    public bool Type;
    public Color C1, C2;
    public Transform target;
    

    public void Create(string name, string level, int health, int maxHealth, bool type)
    {
        T_PieceName.text = name;
        S_Health.maxValue = maxHealth;
        S_Health.value = health;
        Type = type;
        T_Level.text = "lvl " + level;
        S_Health.fillRect.GetComponentInChildren<Image>().color = Type ? C1 : C2;
    }

    public void UpdateHealth(int newHealth)
    {
        S_Health.value = newHealth;
    }

    public void TakeDamage(int amount)
    {
        S_Health.value -= amount;
    }

    private void Update()
    {
        Transform target = Ref.Camera.transform;

        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;

        transform.LookAt(targetPosition);
    }
}
