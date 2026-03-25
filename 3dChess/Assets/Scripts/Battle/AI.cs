using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds2 = new(2);
    public Coroutine ThinkBattleMove;
    public Coroutine ThinkChessMove;
    public ChessEngine engine;
    public bool TestMode = false;

    public void CreateBattle()
    {
        if (!ChessManager.Local)
            return;
        ThinkBattleMove = StartCoroutine(MakeMove());
    }
    public void StopBattle()
    {
        StopCoroutine(ThinkBattleMove);
    }

    private IEnumerator MakeMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => Ref.BattleManager.Turn % 2 == 1);
            yield return _waitForSeconds2;
            Debug.LogError("Ai is making a move " + Ref.BattleManager.Turn);

            var decision = ChooseBattleAction();

            switch (decision.Type)
            {
                case AIActionType.Move:
                    Ref.BattleManager.PrepareUseMove(decision.Move, false);
                    break;

                case AIActionType.Potion:
                    Ref.BattleManager.PrepareUsePotion(decision.Potion, false);
                    break;

                case AIActionType.Switch:
                    Ref.BattleManager.PrepareSwitchPiece(decision.SwitchPiece, false);
                    break;

                case AIActionType.Flee:
                    Ref.BattleManager.FleeBattle(false);
                    break;
            }
        }     
    }

    public void CreateChess()
    {
        if (TestMode)
            return;
        engine.StartEngine();
        ThinkChessMove = StartCoroutine(MakeChessMove());
    }

    public void StopChess()
    {
        StopCoroutine(ThinkChessMove);
    }

    private IEnumerator MakeChessMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => ChessManager.Turn % 2 == 1 && !BattleManager.Ongoing);

            if(TryAttackWhiteKing(out var piecesToAttack, out var whiteKingTile))
            {
                yield return _waitForSeconds2;
                Ref.ChessManager.PrepareMove(false, piecesToAttack[UnityEngine.Random.Range(0, piecesToAttack.Count)], whiteKingTile);
                continue;
            }

            string fen = Ref.ManageTiles.GetFenTable() + " b - - 0 1";
            print(fen);

            var task = engine.GetBestMove(fen, 1000);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
                yield break;
            }

            string move = task.Result;

            Tile fromTile = Ref.ManageTiles.GetFenTile(move[..2]);
            Piece fromPiece = fromTile.currentPiece;
            Tile toTile = Ref.ManageTiles.GetFenTile(move[2..]);
            print(fromTile + " " + toTile);
            Ref.ChessManager.PrepareMove(false, fromPiece, toTile);
        }
    }

    public bool TryAttackWhiteKing(out List<Piece> piecesToAtack, out Tile whiteKingTile)
    {
        var whiteKing = Ref.ChessManager.WhitePieces.Find(p => p.Data.PieceType == EntityData.Type.King);
        whiteKingTile = whiteKing.currentTile;
        List<Piece> pieces = new();

        foreach (var piece in Ref.ChessManager.BlackPieces)
        {
            if(piece.gameObject.activeSelf == false)
            {
                continue;
            }
            print(piece.Name + " " + string.Join(",", piece.GetCurrentAttackTiles(piece.currentTile)));
            if(piece.GetCurrentAttackTiles(piece.currentTile).Contains(whiteKing.currentTile))
            {
                whiteKingTile = whiteKing.currentTile;
                pieces.Add(piece);
            }
        }
        piecesToAtack = pieces;
        if(pieces.Count > 0)
        {
            return true;
        }
        return false;
    }

    public AIDecision ChooseBattleAction()
    {
        const bool WHITE_SIDE = true;
        const bool BLACK_SIDE = false;

        Dictionary<Move, int> moveChoices = new();
        Dictionary<PotionData, int> potionChoices = new();
        Dictionary<Piece, int> switchPieceChoices = new();

        var whitePlayer = Ref.BattleManager.ActiveWhitePlayer;
        var blackPlayer = Ref.BattleManager.ActiveBlackPlayer;
        var effectManager = Ref.BattleManager.EffectManager;

        float botHealthQuality = 100f * blackPlayer.Health / blackPlayer.MaxHealth;
        float playerHealthQuality = 100f * whitePlayer.Health / whitePlayer.MaxHealth;

        foreach (var potion in Ref.ChessManager.BlackData.Potions)
        {    
            potionChoices[potion] = 0; 
        }

        int i = 0;
        foreach (var move in blackPlayer.Moves.Where(m => m.Count > 0))
        { 
            if(Move.MoveIndToLvlRequired(i) < blackPlayer.Level)
                moveChoices[move] = 0;
            i++;
        }

        foreach (var piece in Ref.BattleManager.blackTeam)
        {
            bool isCurrentlyBattling = piece.Value.Item2;
            if (!isCurrentlyBattling && piece.Key.Health > 0)
                switchPieceChoices[piece.Key] = 0;
        }

        // -----------------------------
        // POTIONS
        // -----------------------------

        int missingHealth = blackPlayer.MaxHealth - blackPlayer.Health;
        int healPotionValue = Math.Min(
            missingHealth,
            (int)(Variants.HealFactor * blackPlayer.MaxHealth)
        );

        TryAddScore(potionChoices, p => p.Name == "Heal", healPotionValue);

        if (botHealthQuality < 70) TryAddScore(potionChoices, p => p.Name == "Heal", 15);
        if (botHealthQuality < 50) TryAddScore(potionChoices, p => p.Name == "Heal", 30);
        if (botHealthQuality < 30) TryAddScore(potionChoices, p => p.Name == "Heal", 60);

        if (botHealthQuality > 90) TryAddScore(potionChoices, p => p.Name == "Heal", -30);
        else if (botHealthQuality > 80) TryAddScore(potionChoices, p => p.Name == "Heal", -20);
        else if (botHealthQuality > 70) TryAddScore(potionChoices, p => p.Name == "Heal", -10);

        int botNegativeEffects = effectManager.GetEffectCount(BLACK_SIDE, false, null);
        TryAddScore(potionChoices, p => p.Name == "Cleanse", 40 * botNegativeEffects);

        if (botNegativeEffects == 0)
            TryAddScore(potionChoices, p => p.Name == "Cleanse", -100);

        if (botHealthQuality < 25 && botNegativeEffects > 0)
            TryAddScore(potionChoices, p => p.Name == "Cleanse", 20);

        // -----------------------------
        // MOVES
        // -----------------------------
        foreach (var move in moveChoices.Keys.ToList())
        {
            int score = 0;

            switch (move.Type)
            {
                case MoveType.Attack:
                    {
                        int estimatedDamage = EstimateAttackDamage(blackPlayer, whitePlayer, move, BLACK_SIDE);

                        score += (int)move.Action;
                        score += 100 - (int)playerHealthQuality;

                        // Prefer attacking more when healthy
                        if (botHealthQuality > 90) score += 40;
                        else if (botHealthQuality > 80) score += 30;
                        else if (botHealthQuality > 70) score += 20;

                        // Strong lethal bias
                        if (estimatedDamage >= whitePlayer.Health)
                            score += 120;

                        // If enemy is very low, finish them
                        if (playerHealthQuality < 25)
                            score += 35;

                        // If bot is in danger but can maybe finish, still reward attack a bit
                        if (botHealthQuality < 25 && estimatedDamage < whitePlayer.Health)
                            score -= 20;

                        break;
                    }

                case MoveType.Heal:
                    {
                        int effectiveHeal = Math.Min((int)move.Action, missingHealth);

                        score += effectiveHeal;

                        if (botHealthQuality < 70) score += 20;
                        if (botHealthQuality < 50) score += 35;
                        if (botHealthQuality < 30) score += 60;

                        if (botHealthQuality > 90) score -= 30;
                        else if (botHealthQuality > 80) score -= 20;
                        else if (botHealthQuality > 70) score -= 10;

                        // If enemy is nearly dead, healing becomes less attractive
                        if (playerHealthQuality < 25)
                            score -= 25;

                        break;
                    }

                case MoveType.Poison:
                    {
                        int poisonRemaining = effectManager.HasEffect(WHITE_SIDE, Effect.Type.Poison)
                            ? effectManager.GetEffectTypeRemainingTurns(WHITE_SIDE, Effect.Type.Poison)
                            : 0;

                        score += 10;
                        score += (int)move.Action * 2;
                        score += (int)playerHealthQuality / 2;

                        if (poisonRemaining == 0)
                        {
                            score += 45;
                        }
                        else if (poisonRemaining == 1)
                        {
                            score += 5;   // refresh only when close to expiring
                        }
                        else if (poisonRemaining == 2)
                        {
                            score -= 35;  // usually not worth it
                        }
                        else // 3 or 4 turns left
                        {
                            score -= 90;  // strong penalty for wasteful refresh
                        }

                        // Poison is worse when direct attack can likely finish soon
                        if (playerHealthQuality < 35)
                            score -= 35;

                        // Poison is worse when survival is urgent
                        if (botHealthQuality < 25)
                            score -= 25;

                        break;
                    }

                case MoveType.Defense:
                    {
                        int defenseRemaining = effectManager.HasEffect(BLACK_SIDE, Effect.Type.Defense)
                            ? effectManager.GetEffectTypeRemainingTurns(BLACK_SIDE, Effect.Type.Defense)
                            : 0;

                        score += 20;
                        score += (int)move.Action * 2;
                        score += whitePlayer.Attack * 4;
                        score += 100 - (int)botHealthQuality;

                        if (defenseRemaining == 0) score += 35;
                        else if (defenseRemaining == 1) score += 20;
                        else if (defenseRemaining == 2) score += 5;
                        else score -= 40;

                        if (playerHealthQuality < 25) score -= 35; // finish instead
                        if (botHealthQuality < 25) score -= 15;    // heal/switch/flee may be better

                        break;
                    }

                case MoveType.Weaken:
                    {
                        int weakenRemaining = effectManager.HasEffect(WHITE_SIDE, Effect.Type.Weaken)
                            ? effectManager.GetEffectTypeRemainingTurns(WHITE_SIDE, Effect.Type.Weaken)
                            : 0;

                        score += 18;
                        score += (int)move.Action * 2;
                        score += whitePlayer.Attack * 5;
                        score += 100 - (int)botHealthQuality;

                        if (weakenRemaining == 0) score += 35;
                        else if (weakenRemaining == 1) score += 20;
                        else if (weakenRemaining == 2) score += 5;
                        else score -= 40;

                        if (playerHealthQuality < 25) score -= 35;
                        if (botHealthQuality < 25) score -= 15;

                        break;
                    }

                case MoveType.Slow:
                    {
                        int slowRemaining = effectManager.HasEffect(WHITE_SIDE, Effect.Type.Slow)
                            ? effectManager.GetEffectTypeRemainingTurns(WHITE_SIDE, Effect.Type.Slow)
                            : 0;

                        score += 8;
                        score += (int)move.Action;
                        score += whitePlayer.Luck * 8;
                        score += (100 - (int)botHealthQuality) / 2;

                        if (slowRemaining == 0) score += 25;
                        else if (slowRemaining == 1) score += 15;
                        else if (slowRemaining == 2) score += 5;
                        else score -= 35;

                        if (playerHealthQuality < 25) score -= 25;

                        break;
                    }

                case MoveType.Evasion:
                    {
                        int evasionRemaining = effectManager.HasEffect(BLACK_SIDE, Effect.Type.Evasion)
                            ? effectManager.GetEffectTypeRemainingTurns(BLACK_SIDE, Effect.Type.Evasion)
                            : 0;

                        score += 14;
                        score += (int)move.Action * 2;
                        score += whitePlayer.Speed * 6;
                        score += 100 - (int)botHealthQuality;

                        if (evasionRemaining == 0) score += 30;
                        else if (evasionRemaining == 1) score += 18;
                        else if (evasionRemaining == 2) score += 5;
                        else score -= 40;

                        if (playerHealthQuality < 25) score -= 30;
                        if (botHealthQuality < 25) score -= 10;

                        break;
                    }
            }

            moveChoices[move] += score;
        }

        // -----------------------------
        // SWITCH
        // -----------------------------
        foreach (var piece in switchPieceChoices.Keys.ToList())
        {
            int score = -60; // switching costs a turn, so start negative

            float reserveHealthQuality = 100f * piece.Health / piece.MaxHealth;
            float activeHealthQuality = botHealthQuality;

            int reserveCombatValue = piece.Attack * 8 + piece.Speed * 4 + piece.Luck * 3;
            int activeCombatValue = blackPlayer.Attack * 8 + blackPlayer.Speed * 4 + blackPlayer.Luck * 3;

            int combatUpgrade = reserveCombatValue - activeCombatValue;
            int healthUpgrade = piece.Health - blackPlayer.Health;

            // Reward only real upgrades, not just "another living piece"
            score += combatUpgrade;
            score += healthUpgrade / 8;

            // Main reason to switch: current piece is in trouble
            if (activeHealthQuality < 40) score += 20;
            if (activeHealthQuality < 25) score += 35;
            if (activeHealthQuality < 15) score += 50;

            // Reward healthy reserves
            if (reserveHealthQuality > 70) score += 15;
            if (reserveHealthQuality < 40) score -= 25;
            if (reserveHealthQuality < 20) score -= 50;

            // If enemy is low, switching is usually wrong: just finish them
            if (playerHealthQuality < 35) score -= 50;
            if (playerHealthQuality < 20) score -= 80;

            // If reserve is not clearly better, discourage switching
            if (combatUpgrade <= 0) score -= 25;
            if (healthUpgrade <= 0) score -= 15;

            switchPieceChoices[piece] = score;
        }

        // -----------------------------
        // FLEE
        // -----------------------------
        int fleeChoice = 0;

        if (botHealthQuality < 25) fleeChoice += 40;
        if (botHealthQuality < 15) fleeChoice += 50;
        if (playerHealthQuality > 60) fleeChoice += 20;
        if (whitePlayer.Attack >= blackPlayer.Attack + 2) fleeChoice += 20;

        bool hasUsefulHealMove = moveChoices.Any(kvp => kvp.Key.Type == MoveType.Heal && kvp.Value > 40);
        bool hasUsefulPotion = potionChoices.Any(kvp => kvp.Value > 40);
        bool hasUsefulSwitch = switchPieceChoices.Any(kvp => kvp.Value > 40);

        if (botHealthQuality < 20 && !hasUsefulHealMove && !hasUsefulPotion && !hasUsefulSwitch)
            fleeChoice += 60;

        // -----------------------------
        // DEBUG
        // -----------------------------
        print("Scores: \n" +
            "\n Moves \n" + string.Join(", ", moveChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Potions \n" + string.Join(", ", potionChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Switch \n" + string.Join(", ", switchPieceChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Flee \n" + fleeChoice
        );

        // -----------------------------
        // PICK BEST
        // -----------------------------
        int bestScore = int.MinValue;
        AIDecision decision = new();

        var bestMove = moveChoices.Count > 0
            ? moveChoices.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Name).First()
            : default;

        var bestPotion = potionChoices.Count > 0
            ? potionChoices.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Name).First()
            : default;

        var bestSwitch = switchPieceChoices.Count > 0
            ? switchPieceChoices.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Name).First()
            : default;

        if (moveChoices.Count > 0 && bestMove.Value > bestScore)
        {
            bestScore = bestMove.Value;
            decision.Type = AIActionType.Move;
            decision.Move = bestMove.Key;
            decision.Potion = null;
            decision.SwitchPiece = null;
        }

        if (potionChoices.Count > 0 && bestPotion.Value > bestScore)
        {
            bestScore = bestPotion.Value;
            decision.Type = AIActionType.Potion;
            decision.Potion = bestPotion.Key;
            decision.Move = null;
            decision.SwitchPiece = null;
        }

        if (switchPieceChoices.Count > 0 && bestSwitch.Value > bestScore)
        {
            bestScore = bestSwitch.Value;
            decision.Type = AIActionType.Switch;
            decision.SwitchPiece = bestSwitch.Key;
            decision.Move = null;
            decision.Potion = null;
        }

        if (fleeChoice > bestScore)
        {
            bestScore = fleeChoice;
            decision.Type = AIActionType.Flee;
            decision.Move = null;
            decision.Potion = null;
            decision.SwitchPiece = null;
        }

        return decision;
    }

    private int EstimateAttackDamage(Piece attacker, Piece defender, Move move, bool attackingSide)
    {
        int damage = (int)move.Action + (int)((float)attacker.Attack * 2 / 100 * move.Action);

        int defenderDefensePercent =
            Ref.BattleManager.EffectManager.HasEffect(!attackingSide, Effect.Type.Defense)
            ? (int)Ref.BattleManager.EffectManager.GetEffectTypeAction(!attackingSide, Effect.Type.Defense)
            : 0;

        int attackerWeakenPercent =
            Ref.BattleManager.EffectManager.HasEffect(attackingSide, Effect.Type.Weaken)
            ? (int)Ref.BattleManager.EffectManager.GetEffectTypeAction(attackingSide, Effect.Type.Weaken)
            : 0;

        int reductionPercent = defenderDefensePercent + attackerWeakenPercent;
        damage -= (int)(damage * reductionPercent / 100f);

        return Mathf.Max(0, damage);
    }

    private static void TryAddScore<T>(Dictionary<T, int> choices, Func<T, bool> filter, int score)
    {
        var matchingKeys = choices
            .Where(pair => filter(pair.Key))
            .Select(pair => pair.Key)
            .ToList();

        foreach (var key in matchingKeys)
        {
            choices[key] += score;
        }
    }
}

public enum AIActionType
{
    Move,
    Potion,
    Switch,
    Flee
}

public class AIDecision
{
    public AIActionType Type;

    public Move Move;
    public PotionData Potion;
    public Piece SwitchPiece;
    public bool Flee;
}
