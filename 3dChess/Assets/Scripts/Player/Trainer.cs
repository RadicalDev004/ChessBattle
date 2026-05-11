using Newtonsoft.Json;
using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    private const int MaxGeneratedPieces = 8;
    private const int MaxGeneratedLevel = 20;
    private const int LevelPointCost = 2;

    private static readonly Dictionary<EntityData.Type, int> PieceTypePointCosts = new()
    {
        { EntityData.Type.Pawn, 4 },
        { EntityData.Type.Knight, 6 },
        { EntityData.Type.Bishop, 6 },
        { EntityData.Type.Rook, 8 },
        { EntityData.Type.Queen, 12 },
        { EntityData.Type.King, 10 }
    };

    private static readonly Dictionary<MoveRarity, int> PieceRarityPointCosts = new()
    {
        { MoveRarity.Common, 0 },
        { MoveRarity.Rare, 8 },
        { MoveRarity.Epic, 18 },
        { MoveRarity.Legendary, 32 }
    };

    private static readonly Dictionary<MoveRarity, int> MoveRarityPointCosts = new()
    {
        { MoveRarity.Common, 0 },
        { MoveRarity.Rare, 4 },
        { MoveRarity.Epic, 9 },
        { MoveRarity.Legendary, 16 }
    };

    private static readonly Dictionary<string, int> PotionPointCosts = new()
    {
        { "Heal", 12 },
        { "Cleanse", 14 }
    };

    private static readonly TrainerDifficultyProfile[] ZoneProfiles =
    {
        new(2, 3, 1, 5, MoveRarity.Common, MoveRarity.Common, 0, 0),
        new(3, 5, 5, 10, MoveRarity.Common, MoveRarity.Common, 0, 1),
        new(4, 6, 10, 15, MoveRarity.Rare, MoveRarity.Rare, 1, 1),
        new(5, 8, 15, 20, MoveRarity.Epic, MoveRarity.Rare, 1, 2)
    };

    public string Name;
    public int Zone;
    public int InventoryPoints;
    public int PointsRemaining;
    [TextArea(15, 15)]
    public string JsonInventory;
    public List<TrainerPieceInfo> Pieces;
    public List<TrainerPotionInfo> Potions;
    [TextArea(10, 10)]
    public string ChallengeText, DefeatedText;
    public bool Defeated;

    public NpcInfo NpcInfo;

    private Quaternion InitialRotation;

    private void Awake()
    {
        //NpcInfo.Create(false);
        InitialRotation = transform.rotation;

        if (InventoryPoints <= 0)
            LoadLegacyTemplateFromJsonInventory();
    }

    public void BuildGeneratedInventory()
    {
        InventoryData inventory = InventoryPoints > 0
            ? BuildRandomInventoryFromPoints()
            : BuildInventoryFromTemplate();

        JsonInventory = JsonConvert.SerializeObject(
            inventory,
            Formatting.Indented
        );
    }

    private InventoryData BuildInventoryFromTemplate()
    {
        if (InventoryPoints <= 0)
            LoadLegacyTemplateFromJsonInventory();

        return new InventoryData()
        {
            Name = Name,
            Pieces = new((Pieces ?? new()).Select(pieceInfo =>
            {
                var p = Variants.GetPieceByIndex(pieceInfo.PieceIndex);
                if (p == null)
                    return null;

                p.Moves = pieceInfo.MoveIndexes
                    ?.Select(i => MovePool.GetMoveByIndex(p.Variant, i))
                    .Where(m => m != null)
                    .ToList() ?? new List<Move>();

                p.Position = pieceInfo.Position;
                p.Level = Mathf.Clamp(pieceInfo.Level, 1, MaxGeneratedLevel);
                return p;
            }).Where(p => p != null)),
            Potions = BuildPotionsFromTemplate()
        };
    }

    private InventoryData BuildRandomInventoryFromPoints()
    {
        PointsRemaining = Mathf.Max(0, InventoryPoints);
        var profile = GetDifficultyProfile();
        int targetPieceCount = PickPieceCount(profile, PointsRemaining);
        var pieces = new List<EntityData>();
        var usedPositions = new HashSet<int>();

        if (PointsRemaining < PieceTypePointCosts[EntityData.Type.King])
        {
            Debug.LogWarning($"Trainer {Name} has {InventoryPoints} inventory points, less than the minimum king cost. A king will still be generated from the zone profile.");
        }

        pieces.Add(BuildRandomPiece(EntityData.Type.King, ref PointsRemaining, usedPositions, profile));

        while (pieces.Count < targetPieceCount && HasOpenBoardPosition(usedPositions))
        {
            var pieceType = CanAffordAnotherPiece(PointsRemaining)
                ? PickPieceType(PointsRemaining)
                : EntityData.Type.Pawn;

            pieces.Add(BuildRandomPiece(pieceType, ref PointsRemaining, usedPositions, profile));
        }

        return new InventoryData()
        {
            Name = Name,
            Pieces = pieces,
            Potions = BuildRandomPotions(ref PointsRemaining, profile)
        };
    }

    private EntityData BuildRandomPiece(EntityData.Type pieceType, ref int remainingPoints, HashSet<int> usedPositions, TrainerDifficultyProfile profile)
    {
        Spend(ref remainingPoints, PieceTypePointCosts[pieceType]);

        var pieceRarity = PickRarity(PieceRarityPointCosts, remainingPoints, profile.MinPieceRarity);
        Spend(ref remainingPoints, PieceRarityPointCosts[pieceRarity]);

        var piece = GetRandomPiece(pieceType, pieceRarity);
        piece.Position = TryGetOpenBoardPosition(usedPositions, out var position) ? position : -1;
        piece.HiddenStat = UnityEngine.Random.Range(0, 11);
        piece.Level = PickLevel(ref remainingPoints, profile);
        piece.Health = piece.MaxHealth;
        piece.Moves = BuildRandomMoves(piece, ref remainingPoints, profile);

        return piece;
    }

    private TrainerDifficultyProfile GetDifficultyProfile()
    {
        int zoneIndex = Mathf.Clamp(Zone, 0, ZoneProfiles.Length - 1);
        var profile = ZoneProfiles[zoneIndex];
        int pointsBonus = Mathf.Max(0, InventoryPoints / 80);

        return new TrainerDifficultyProfile(
            Mathf.Clamp(profile.MinPieces + pointsBonus / 2, 1, MaxGeneratedPieces),
            Mathf.Clamp(profile.MaxPieces + pointsBonus, 1, MaxGeneratedPieces),
            Mathf.Clamp(profile.MinLevel + pointsBonus, 1, MaxGeneratedLevel),
            Mathf.Clamp(profile.MaxLevel + pointsBonus, 1, MaxGeneratedLevel),
            profile.MinPieceRarity,
            profile.MinMoveRarity,
            profile.MinPotions,
            profile.MaxPotions
        );
    }

    private int PickPieceCount(TrainerDifficultyProfile profile, int remainingPoints)
    {
        int affordablePieces = 1;
        int pointsAfterKing = Mathf.Max(0, remainingPoints - PieceTypePointCosts[EntityData.Type.King]);
        int cheapestPieceCost = PieceTypePointCosts
            .Where(kv => kv.Key != EntityData.Type.King)
            .Min(kv => kv.Value);

        affordablePieces += pointsAfterKing / cheapestPieceCost;

        int minPieces = Mathf.Min(profile.MinPieces, MaxGeneratedPieces);
        int maxPieces = Mathf.Min(profile.MaxPieces, MaxGeneratedPieces, Mathf.Max(minPieces, affordablePieces));

        return UnityEngine.Random.Range(minPieces, maxPieces + 1);
    }

    private List<PotionData> BuildPotionsFromTemplate()
    {
        if (Potions == null)
            return new();

        return new(Potions.Select(potionInfo =>
        {
            var potion = Variants.GetPotionByIndex(potionInfo.PotionIndex);
            if (potion == null)
                return null;

            potion.Position = potionInfo.Position;
            return potion;
        }).Where(p => p != null));
    }

    private List<PotionData> BuildRandomPotions(ref int remainingPoints, TrainerDifficultyProfile profile)
    {
        int count = UnityEngine.Random.Range(profile.MinPotions, profile.MaxPotions + 1);
        var potions = new List<PotionData>();

        for (int i = 0; i < count; i++)
        {
            string potionName = PickPotionName(remainingPoints, i < profile.MinPotions);
            if (string.IsNullOrWhiteSpace(potionName))
                break;

            Spend(ref remainingPoints, PotionPointCosts[potionName]);

            var potion = Variants.PotionVariants
                .FirstOrDefault(p => p.Name == potionName)
                ?.Copy();

            if (potion == null)
                continue;

            potion.Position = i;
            potions.Add(potion);
        }

        return potions;
    }

    private string PickPotionName(int remainingPoints, bool forcePotion)
    {
        var affordablePotions = PotionPointCosts
            .Where(kv => kv.Value <= remainingPoints)
            .ToList();

        if (affordablePotions.Count == 0)
        {
            return forcePotion
                ? PotionPointCosts.OrderBy(kv => kv.Value).First().Key
                : null;
        }

        int totalWeight = affordablePotions.Sum(kv => kv.Key == "Heal" ? 2 : 1);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach (var potion in affordablePotions)
        {
            roll -= potion.Key == "Heal" ? 2 : 1;
            if (roll < 0)
                return potion.Key;
        }

        return affordablePotions[0].Key;
    }

    private bool CanAffordAnotherPiece(int remainingPoints)
    {
        int cheapestPieceCost = PieceTypePointCosts
            .Where(kv => kv.Key != EntityData.Type.King)
            .Min(kv => kv.Value);

        return remainingPoints >= cheapestPieceCost;
    }

    private bool HasOpenBoardPosition(HashSet<int> usedPositions)
    {
        return usedPositions.Count < 32;
    }

    private bool TryGetOpenBoardPosition(HashSet<int> usedPositions, out int position)
    {
        var availablePositions = Enumerable.Range(1, 32)
            .Where(p => !usedPositions.Contains(p))
            .ToList();

        if (availablePositions.Count == 0)
        {
            position = -1;
            return false;
        }

        position = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
        usedPositions.Add(position);
        return true;
    }

    private EntityData.Type PickPieceType(int points)
    {
        var candidates = PieceTypePointCosts
            .Where(kv => kv.Key != EntityData.Type.King && kv.Value <= points)
            .ToList();

        if (candidates.Count == 0)
            return EntityData.Type.Pawn;

        int totalWeight = candidates.Sum(kv => kv.Value + 1);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach (var candidate in candidates)
        {
            roll -= candidate.Value + 1;
            if (roll < 0)
                return candidate.Key;
        }

        return candidates[0].Key;
    }

    private MoveRarity PickRarity(Dictionary<MoveRarity, int> costs, int points, MoveRarity minRarity = MoveRarity.Common)
    {
        var candidates = costs
            .Where(kv => kv.Value <= points && (int)kv.Key >= (int)minRarity)
            .ToList();

        if (candidates.Count == 0)
            return minRarity;

        int totalWeight = candidates.Sum(kv => kv.Value + 1);
        int roll = UnityEngine.Random.Range(0, totalWeight);

        foreach (var candidate in candidates)
        {
            roll -= candidate.Value + 1;
            if (roll < 0)
                return candidate.Key;
        }

        return minRarity;
    }

    private EntityData GetRandomPiece(EntityData.Type type, MoveRarity rarity)
    {
        var candidates = Variants.PiecesVariants
            .Where(p => p.PieceType == type && GetPieceRarity(p.Variant) == rarity)
            .ToList();

        if (candidates.Count == 0)
        {
            candidates = Variants.PiecesVariants
                .Where(p => p.PieceType == type)
                .OrderBy(p => Mathf.Abs((int)GetPieceRarity(p.Variant) - (int)rarity))
                .GroupBy(p => Mathf.Abs((int)GetPieceRarity(p.Variant) - (int)rarity))
                .FirstOrDefault()
                ?.ToList() ?? new();
        }

        if (candidates.Count == 0)
            return Variants.GetPieceByIndex((int)type);

        return candidates[UnityEngine.Random.Range(0, candidates.Count)].Copy();
    }

    private MoveRarity GetPieceRarity(string variant)
    {
        return variant switch
        {
            "bronze" or "frost" => MoveRarity.Rare,
            "storm" or "terra" or "aqua" => MoveRarity.Epic,
            "inferno" or "void" or "radiant" => MoveRarity.Legendary,
            _ => MoveRarity.Common
        };
    }

    private int PickLevel(ref int points, TrainerDifficultyProfile profile)
    {
        int minLevel = Mathf.Clamp(profile.MinLevel, 1, MaxGeneratedLevel);
        int maxAffordableLevel = Mathf.Clamp(1 + points / LevelPointCost, minLevel, profile.MaxLevel);
        int totalWeight = 0;

        for (int level = minLevel; level <= maxAffordableLevel; level++)
            totalWeight += level;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        for (int level = minLevel; level <= maxAffordableLevel; level++)
        {
            roll -= level;
            if (roll < 0)
            {
                Spend(ref points, (level - 1) * LevelPointCost);
                return level;
            }
        }

        return minLevel;
    }

    private List<Move> BuildRandomMoves(EntityData piece, ref int points, TrainerDifficultyProfile profile)
    {
        int moveCount = GetMoveCountForLevel(piece.Level);
        var moves = new List<Move>();

        for (int i = 0; i < moveCount; i++)
        {
            var rarity = PickRarity(MoveRarityPointCosts, points, profile.MinMoveRarity);
            Spend(ref points, MoveRarityPointCosts[rarity]);

            var move = MovePool.GetRandomMove(piece.Variant, piece.PieceType, i, rarity);
            if (move != null)
                moves.Add(move);
        }

        return moves;
    }

    private int GetMoveCountForLevel(int level)
    {
        int count = 1;

        for (int i = 1; i < 4; i++)
        {
            if (Move.MoveIndToLvlRequired(i) <= level)
                count++;
        }

        return count;
    }

    private void Spend(ref int points, int cost)
    {
        points = Mathf.Max(0, points - Mathf.Max(0, cost));
    }

    private void LoadLegacyTemplateFromJsonInventory()
    {
        if (string.IsNullOrWhiteSpace(JsonInventory) || !JsonInventory.Contains("PieceIndex"))
            return;

        try
        {
            var data = JsonConvert.DeserializeObject<TrainerInventory>(JsonInventory);
            if (data?.Pieces != null && data.Pieces.Count > 0)
                Pieces = data.Pieces;
            if (data?.Potions != null)
                Potions = data.Potions;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not read legacy trainer inventory template for {Name}: {e.Message}");
        }
    }

    private bool TryGetGeneratedInventory(out InventoryData inventory)
    {
        inventory = null;
        if (string.IsNullOrWhiteSpace(JsonInventory))
            return false;

        try
        {
            inventory = JsonConvert.DeserializeObject<InventoryData>(JsonInventory);
        }
        catch
        {
            return false;
        }

        return inventory != null &&
            !string.IsNullOrWhiteSpace(inventory.Name) &&
            inventory.Pieces != null;
    }


    public void Create(TrainerData data)
    {
        Name = data.Name;
        Defeated = data.Defeated;
        if (!string.IsNullOrWhiteSpace(data.JsonInventory))
            JsonInventory = data.JsonInventory;

        if (InventoryPoints > 0 && !TryGetGeneratedInventory(out _))
            BuildGeneratedInventory();

        print("Creating trainer " + Name + " with defeated status: " + Defeated);

        NpcInfo.Create(Defeated);
    }

    public void CreateNewGame()
    {
        Defeated = false;
        BuildGeneratedInventory();

        NpcInfo.Create(Defeated);
    }

    public void Speak()
    {
        Vector3 direction = (GameRef.PlayerBehaviour.transform.position - transform.position);
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Tween.Rotation(transform, targetRotation, 0.5f, 0, Tween.EaseInOut);
    }

    public void EndSpeak()
    {
        Tween.Rotation(transform, InitialRotation, 0.5f, 0, Tween.EaseInOut);
    }

    public InventoryData GetInventory()
    {
        if (TryGetGeneratedInventory(out var generatedInventory))
            return generatedInventory;

        if (InventoryPoints > 0)
        {
            BuildGeneratedInventory();
            if (TryGetGeneratedInventory(out generatedInventory))
                return generatedInventory;
        }

        return BuildInventoryFromTemplate();
    }
}

[Serializable]
public struct TrainerPieceInfo
{
    public int PieceIndex;
    public int Position;
    public int Level;
    public List<int> MoveIndexes;
}

[Serializable]
public struct TrainerPotionInfo
{
    public int PotionIndex;
    public int Position;
}

[Serializable]
public class TrainerInventory
{
    public List<TrainerPieceInfo> Pieces;
    public List<TrainerPotionInfo> Potions;
}

public readonly struct TrainerDifficultyProfile
{
    public readonly int MinPieces;
    public readonly int MaxPieces;
    public readonly int MinLevel;
    public readonly int MaxLevel;
    public readonly MoveRarity MinPieceRarity;
    public readonly MoveRarity MinMoveRarity;
    public readonly int MinPotions;
    public readonly int MaxPotions;

    public TrainerDifficultyProfile(int minPieces, int maxPieces, int minLevel, int maxLevel, MoveRarity minPieceRarity, MoveRarity minMoveRarity, int minPotions, int maxPotions)
    {
        MinPieces = minPieces;
        MaxPieces = maxPieces;
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        MinPieceRarity = minPieceRarity;
        MinMoveRarity = minMoveRarity;
        MinPotions = minPotions;
        MaxPotions = maxPotions;
    }
}
