using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public List<(int, int)> pos = new() { (-1, 2), (1, 2), (2, 1), (2, -1) , (1, -2), (-1, -2), (-2, -1), (-2, 1)};
    public override List<Tile> GetCurrentPreviewTiles(Tile tile)
    {
        orgTile = tile;
        Preview.Clear();
        foreach(var newPos in pos)
        {
            int x = tile.x + newPos.Item1;
            int y = tile.y + newPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) continue;

            Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }
        return Preview;
    }

}
