using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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

        for (int x = tile.x + 1; x <= 7; x++)
        {
            if (Ref.ManageTiles.GetTile(x, tile.y).currentPiece != null && Ref.ManageTiles.GetTile(x, tile.y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, tile.y));
            if (Ref.ManageTiles.GetTile(x, tile.y).currentPiece != null) break;
        }
        for (int x = tile.x - 1; x >= 0; x--)
        {
            if (Ref.ManageTiles.GetTile(x, tile.y).currentPiece != null && Ref.ManageTiles.GetTile(x, tile.y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(x, tile.y));
            if (Ref.ManageTiles.GetTile(x, tile.y).currentPiece != null) break;
        }

        for (int y = tile.y + 1; y <= 7; y++)
        {
            if (Ref.ManageTiles.GetTile(tile.x, y).currentPiece != null && Ref.ManageTiles.GetTile(tile.x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(tile.x, y));
            if (Ref.ManageTiles.GetTile(tile.x, y).currentPiece != null) break;
        }
        for (int y = tile.y - 1; y >= 0; y--)
        {
            if (Ref.ManageTiles.GetTile(tile.x, y).currentPiece != null && Ref.ManageTiles.GetTile(tile.x, y).currentPiece.side == side) break;
            Preview.Add(Ref.ManageTiles.GetTile(tile.x, y));
            if (Ref.ManageTiles.GetTile(tile.x, y).currentPiece != null) break;
        }

        return Preview;
    }
}
