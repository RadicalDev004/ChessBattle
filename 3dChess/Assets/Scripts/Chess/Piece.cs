using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Piece : Entity
{
    public bool isHeld;
    private Camera cam;
    public float heldHeight;
    public Vector3 LastPoint;
    public bool animaiton = false;
    public float normalY;
    public int movesCnt = 0;
    public bool side = true; //true = white, false = black
    public Tile currentTile;

    public Tile orgTile;
    public List<Tile> Preview;

    public PieceUI pieceUI;
    public float pieceUIHeight;

    
    public override void UpdateUI()
    {
        pieceUI.UpdateHealth(Health);
    }
    

    void Start()
    {
        
    }

    public void Create(int index, EntityData e)
    {
        var tile = Ref.ManageTiles.GetTile(index);

        cam = Ref.Camera;
        normalY = transform.position.y;

        tile.currentPiece = this;
        currentTile = tile;
        orgTile = currentTile;
        transform.position = new Vector3(tile.transform.position.x, normalY, tile.transform.position.z);

        Data = e;
        /*
        Name = e.Name;
        MaxHealth = e.MaxHealth;
        */
        Health = e.Health;
        Level = e.Level;
        //Moves = e.Moves;

        ActivatePieceUI();

        GetComponent<MeshRenderer>().material = !side && e.Variant == "basic" ? Resources.Load<Material>($"Materials/black") :  Resources.Load<Material>($"Materials/{e.Variant}");
        //MakeMoves();
    }

    public abstract List<Tile> GetCurrentPreviewTiles(Tile tile);
    public abstract List<Tile> GetCurrentAttackTiles(Tile tile);
    public abstract List<Tile> GetCurrentHelpingTiles(Tile tile);

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
        if(side != (ChessManager.Turn % 2 == 0)) yield break;
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
        currentTile = tileToGo;

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
        ChessManager.Turn++;
        if (tile.currentPiece != null)
        {
            Ref.BattleManager.StartBattle(this, tile.currentPiece, tile, side);           
            Ref.BattleManager.OnBattleEnd += BattleEndListener;
        }
        else
        {
            Debug.Log("No contest for tile.");
            movesCnt++;
            tile.currentPiece = this;
            orgTile.currentPiece = null;
            orgTile = tile;
        }       
    }

    public void BattleEndListener(BattleManager.BattleResult result, Tile tile)
    {        
        int sideToInt = side ? 1 : 0;
        Debug.Log("Winner from piece: " + result + " " + side + " " + sideToInt);
        if (sideToInt != (int)result)
        {
            Tween.LocalPosition(transform, new Vector3(orgTile.transform.position.x, normalY, orgTile.transform.position.z), 0.25f, 0, Tween.EaseOut);
        }
        else
        {
            movesCnt++;
            tile.currentPiece = this;
            orgTile.currentPiece = null;
            orgTile = tile;
        }
    }

    public void ActivatePieceUI()
    {
        pieceUI = Instantiate(Ref.OrgPieceUI);
        pieceUI.transform.SetParent(transform);
        pieceUI.transform.localPosition = new Vector3(0, 0, pieceUIHeight);
        pieceUI.gameObject.SetActive(true);
        //pieceUI.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0.05f);
        pieceUI.Create(Name == "" ? GetType().Name : Name, Level.ToString(), Health, MaxHealth, side);
    }

    public void MakeMoves()
    {
        //Moves = MovePool.Basic;
    }

    public override void OnIncludedBattleEnd()
    {
        if(Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
