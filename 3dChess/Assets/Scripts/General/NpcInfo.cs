using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfo : MonoBehaviour
{
    public bool Defeated;
    public GameObject Excl, Check;

    public void Create(bool defeated)
    {
        Defeated = defeated;
        Excl.SetActive(!Defeated);
        Check.SetActive(Defeated);

        var currY = transform.position.y;
        Tween.Position(transform, new Vector3(transform.position.x, currY + 0.15f, transform.position.z), 1f, 0, Tween.EaseOut, loop: Tween.LoopType.PingPong);
    }

    private void Update()
    {
        transform.LookAt(GameRef.PlayerBehaviour.Camera.transform);
    }
}
