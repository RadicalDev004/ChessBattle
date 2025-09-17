using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPiece : MonoBehaviour
{
    private Piece currentHighlight;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider.gameObject.TryGetComponent(out Piece p) && p.side && !p.isHeld)
        {
            if(currentHighlight != null)
            {
                currentHighlight.GetComponent<Outline>().enabled = false;
            }

            currentHighlight = p;
            p.GetComponent<Outline>().enabled = true;
        }
        else
        {
            if (currentHighlight != null)
            {
                currentHighlight.GetComponent<Outline>().enabled = false;
                currentHighlight = null;
            }
        }
    }
}
