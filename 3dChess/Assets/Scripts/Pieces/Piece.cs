using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Piece : MonoBehaviour
{
    public bool isHeld;
    private Camera cam;
    public float heldHeight;
    public Vector3 LastPoint;
    public bool animaiton = false;
    public float normalY;
    public int moves = 0;
    public bool side = true; //true = white, false = black
    public Tile currentTile;

    public Tile orgTile;
    public List<Tile> Preview;

    

    void Start()
    {
        cam = Ref.Camera;
        normalY = transform.position.y;
        Ref.ManageTiles.GetTile(Ref.ManageTiles.GetUnderTile(transform.position)).currentPiece = this;
    }

    public abstract List<Tile> GetCurrentPreviewTiles(Tile tile);

    void Update()
    {
        if (isHeld)
        {
            foreach(var tl in Preview)
            {
                Ref.ManageTiles.SetTileState(ManageTiles.TileState.preview, tl.x, tl.y);
            }

            Ref.ManageTiles.ResetSpecificTileStates(ManageTiles.TileState.move);
            var newPos = SelectPointAtY(heldHeight);
            var delta = newPos - LastPoint;
            transform.position += new Vector3(delta.x, 0, delta.z);           
            LastPoint = newPos;

            var tile = Ref.ManageTiles.GetUnderTile(transform.position);
            Ref.ManageTiles.SetTileState(ManageTiles.TileState.move, tile);
        }
    }    

    public Vector3 SelectPointAtY(float targetY)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 origin = ray.origin;
        Vector3 direction = ray.direction;

        if (Mathf.Abs(direction.y) < 0.0001f)
        {
            Debug.LogWarning("Ray is parallel to the Y plane.");
            return Vector3.zero;
        }

        float t = (targetY - origin.y) / direction.y;
        Vector3 point = origin + t * direction;

        //Debug.DrawRay(origin, direction * t, Color.red, 2f);
        //Debug.Log("Point on ray at Y = " + targetY + ": " + point);
        return point;
    }

    private void OnMouseDown()
    {
        StartCoroutine(OnMouseDownCor());
    }

    private IEnumerator OnMouseDownCor()
    {
        if (animaiton) yield break;

        animaiton = true;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 hitPoint = hitInfo.point;
            heldHeight = hitPoint.y;
        }
        LastPoint = SelectPointAtY(heldHeight);

        GetCurrentPreviewTiles(Ref.ManageTiles.GetTile(Ref.ManageTiles.GetUnderTile(transform.position)));

        Tween.LocalPosition(transform,  new Vector3(transform.position.x, normalY + 0.15f, transform.position.z), 0.1f, 0, Tween.EaseIn);
        
        yield return new WaitForSeconds(0.1f);
        animaiton = false;
        isHeld = true;
    }

    private void OnMouseUp()
    {
        StartCoroutine(OnMouseUpCor());
    }

    private IEnumerator OnMouseUpCor()
    {
        yield return new WaitUntil(() => !animaiton);
        isHeld = false;
        animaiton = true;
        Ref.ManageTiles.ResetAllTileStates();

        var tilePos = Ref.ManageTiles.GetTile(Ref.ManageTiles.GetUnderTile(transform.position));
        var tileToGo = tilePos != null && Preview.Contains(tilePos) ? tilePos : orgTile;

        Tween.LocalPosition(transform, new Vector3(tileToGo.transform.position.x, normalY, tileToGo.transform.position.z), 0.1f, 0, Tween.EaseOut);
        
        yield return new WaitForSeconds(0.1f);
        animaiton = false;

        if(tileToGo != orgTile)
        {
            SubmitMove(tileToGo);
        }
    }

    public void SubmitMove(Tile tile)
    {
        moves++;
        if(tile.currentPiece != null)
        {
            //TODO: Start battle
            Destroy(tile.currentPiece.gameObject);
        }
        tile.currentPiece = this;
        orgTile.currentPiece = null;
        orgTile = tile;
    }
}
