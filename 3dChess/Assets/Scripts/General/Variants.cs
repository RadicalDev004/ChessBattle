using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Variants
{
    public static List<EntityData> PiecesVariants = new()
    {
        new EntityData("Pawn", EntityData.Type.Pawn, -1, 250, 250, 2, 5, 4, 0, "basic", null),
        new EntityData("King", EntityData.Type.King, -1, 300, 300, 4, 2, 3, 0, "basic", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 275, 275, 3, 4, 3, 0, "basic", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 300, 300, 3, 3, 4, 0, "basic", null),
        new EntityData("Rook", EntityData.Type.Rook, -1, 450, 450, 2, 2, 4, 0, "basic", null),
        new EntityData("Queen", EntityData.Type.Queen, -1, 300, 300, 5, 1, 2, 0, "basic", null),
    };

    public static EntityData GetRandomOfType(EntityData.Type type)
    {
        var candidates = PiecesVariants.Where(p => p.PieceType == type).ToList();

        if (candidates.Count == 0)
            return null;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        chosen.Moves.Add(MovePool.GetRandomMove());
        return chosen;
    }

    public static EntityData GetRandom()
    {
        var chosen = PiecesVariants[Random.Range(0, PiecesVariants.Count)];
        chosen.Moves.Add(MovePool.GetRandomMove());
        return chosen;
    }
}
