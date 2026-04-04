using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotel : House
{
    public int Floor = 0;
    public Hotel PreviousFloor;
    public GameObject EnterFromPrevious;
    public GameObject Exit;

    public new void Update()
    {
        var dist = Vector3.Distance(GameRef.PlayerBehaviour.transform.position, Door.transform.position);

        if (dist < Treshold && GameRef.PlayerBehaviour.HouseInRange == null)
        {
            GameRef.PlayerBehaviour.HouseInRange = this;
        }

        if (dist > Treshold && GameRef.PlayerBehaviour.HouseInRange == this)
        {
            GameRef.PlayerBehaviour.HouseInRange = null;
        }

        if (IsInside)
        {
            var closeToExit = Vector3.Distance(GameRef.PlayerBehaviour.transform.position, Exit.transform.position) < Treshold;

            GameRef.UI.ToggleExitHouse(closeToExit);

            if (closeToExit && Input.GetKey(KeyCode.E))
            {
                ExitHouse();
            }

        }
    }

    public override void ExitHouse()
    {
        IsInside = false;
        if (PreviousFloor != null)
        {
            PreviousFloor.EnterHouse(true, this);
            return;
        }      
        InsideAnyHouse = false;
        GameRef.PlayerBehaviour.TeleportTo(PlayerBeforeEnter == Vector3.zero ? Door.transform.position : PlayerBeforeEnter);
        GameRef.UI.ToggleExitHouse(false);
    }

    public override void EnterHouse(bool teleport = true, House previous = null)
    {
        if (TeleportTo == null)
        {
            Debug.LogError("No teleport location set for house " + name);
            return;
        }
        if (teleport)
        {
            if (previous != null)
            {
                GameRef.PlayerBehaviour.TeleportTo(EnterFromPrevious.transform.position);
            }
            else
            {
                PlayerBeforeEnter = GameRef.PlayerBehaviour.transform.position;
                GameRef.PlayerBehaviour.TeleportTo(TeleportTo.transform.position);
            }
            
        }
        else
        {
            PlayerBeforeEnter = Door.transform.position;
        }

        InsideAnyHouse = true;
        GameRef.PlayerBehaviour.Camera.transform.position = FixedCameraPos;

        this.ActionAfterTime(0.5f, () =>
        {
            IsInside = true;
            GameRef.PlayerBehaviour.SaveManager.SaveGame();
        });

    }
}
