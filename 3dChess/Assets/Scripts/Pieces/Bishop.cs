using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Tile> GetCurrentPreviewTiles(Tile tile)
    {
        orgTile = tile;
        Preview.Clear();

        for (int x = tile.x + 1, y = tile.y + 1; x <= 7 && y <= 7; x++, y++)
        {
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null) break;
        }
        for (int x = tile.x - 1, y = tile.y + 1; x >= 0 && y <= 7; x--, y++)
        {
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null) break;
        }

        for (int x = tile.x + 1, y = tile.y - 1; x <= 7 && y >= 0; x++, y--)
        {
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null) break;
        }
        for (int x = tile.x - 1, y = tile.y - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null) break;
        }
        return Preview;
    }
}
