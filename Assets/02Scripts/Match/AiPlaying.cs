using System;
using System.Collections.Generic;

public class AiPlaying
{
    public enum AiPickType
    {
        BestPick,
        PressPick,
        SkillPick,
        SafePick,
        BaitPick
    }

    public class AiDecisionResult
    {
        public OpponentPlayerCard selectedCard;
        public int selectedStatType;
        public AiPickType usedPickType;
    }

    private readonly int[][] difficultyPickWeights = new int[][]
    {
        new int[] { 10, 10, 15, 35, 30 },
        new int[] { 15, 10, 15, 35, 25 },
        new int[] { 20, 15, 20, 30, 20 },
        new int[] { 25, 15, 20, 25, 15 },
        new int[] { 30, 20, 25, 20, 15 },
        new int[] { 35, 25, 25, 15, 10 },
        new int[] { 40, 30, 25, 15, 10 },
        new int[] { 45, 30, 25, 10, 10 },
        new int[] { 50, 35, 25, 10, 5 },
        new int[] { 55, 35, 20, 10, 5 },
        new int[] { 60, 40, 20, 5, 5 },
        new int[] { 65, 40, 15, 5, 5 },
        new int[] { 70, 45, 15, 5, 5 },
        new int[] { 75, 45, 10, 5, 5 },
        new int[] { 80, 50, 10, 5, 5 }
    };

    private Random randomGenerator = new Random();

    public AiDecisionResult DecideAiCard(List<OpponentPlayerCard> remainingCards, int currentTurn, int stageIndex, int userStatType, bool isFirst)
    {
        if (remainingCards == null || remainingCards.Count == 0)
        {
            return null;
        }

        AiPickType chosenPick = DeterminePickTypeByDifficulty(stageIndex);
        OpponentPlayerCard targetCard = null;
        int targetStatType = 2;

        if (chosenPick == AiPickType.BestPick)
        {
            targetCard = ExecuteBestPick(remainingCards, userStatType, out targetStatType);
        }

        else if (chosenPick == AiPickType.PressPick)
        {
            targetCard = ExecutePressPick(remainingCards, userStatType, isFirst, out targetStatType);
        }

        else if (chosenPick == AiPickType.SkillPick)
        {
            targetCard = ExecuteSkillPick(remainingCards, userStatType, out targetStatType);
        }

        else if (chosenPick == AiPickType.SafePick)
        {
            targetCard = ExecuteSafePick(remainingCards, out targetStatType);
        }

        else if (chosenPick == AiPickType.BaitPick)
        {
            targetCard = ExecuteBaitPick(remainingCards, out targetStatType);
        }

        if (targetCard == null)
        {
            targetCard = remainingCards[0];
            targetStatType = GetCounterStatType(userStatType);
        }

        AiDecisionResult result = new AiDecisionResult();
        result.selectedCard = targetCard;
        result.selectedStatType = targetStatType;
        result.usedPickType = chosenPick;

        return result;
    }

    private AiPickType DeterminePickTypeByDifficulty(int stageIndex)
    {
        int validStage = stageIndex;
        
        if (validStage < 0)
        {
            validStage = 0;
        }

        if (validStage >= difficultyPickWeights.Length)
        {
            validStage = difficultyPickWeights.Length - 1;
        }

        int[] weights = difficultyPickWeights[validStage];
        int totalWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight = totalWeight + weights[i];
        }

        int roll = randomGenerator.Next(0, totalWeight);
        int currentSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            currentSum = currentSum + weights[i];
            if (roll < currentSum)
            {
                return (AiPickType)i;
            }
        }

        return AiPickType.BestPick;
    }

    private OpponentPlayerCard ExecuteBestPick(List<OpponentPlayerCard> remainingCards, int userStatType, out int chosenStatType)
    {
        chosenStatType = GetCounterStatType(userStatType);

        OpponentPlayerCard bestCard = remainingCards[0];
        int maxVal = GetStatValueByStatType(bestCard, chosenStatType);

        for (int i = 1; i < remainingCards.Count; i++)
        {
            OpponentPlayerCard card = remainingCards[i];
            int currentVal = GetStatValueByStatType(card, chosenStatType);

            if (currentVal > maxVal)
            {
                maxVal = currentVal;
                bestCard = card;
            }
        }

        return bestCard;
    }

    private OpponentPlayerCard ExecutePressPick(List<OpponentPlayerCard> remainingCards, int userStatType, bool isAiFirst, out int chosenStatType)
    {
        if (isAiFirst)
        {
            chosenStatType = 0;
            OpponentPlayerCard bestAttCard = remainingCards[0];

            for (int i = 1; i < remainingCards.Count; i++)
            {
                if (remainingCards[i].attack > bestAttCard.attack)
                {
                    bestAttCard = remainingCards[i];
                }
            }

            return bestAttCard;
        }

        else
        {
            chosenStatType = GetCounterStatType(userStatType);
            OpponentPlayerCard bestCounterCard = remainingCards[0];
            int maxStat = GetStatValueByStatType(bestCounterCard, chosenStatType);

            for (int i = 1; i < remainingCards.Count; i++)
            {
                int val = GetStatValueByStatType(remainingCards[i], chosenStatType);
                if (val > maxStat)
                {
                    maxStat = val;
                    bestCounterCard = remainingCards[i];
                }
            }

            return bestCounterCard;
        }
    }

    private OpponentPlayerCard ExecuteSkillPick(List<OpponentPlayerCard> remainingCards, int userStatType, out int chosenStatType)
    {
        chosenStatType = GetCounterStatType(userStatType);
        OpponentPlayerCard bestSkillCard = null;
        int maxTotalEffectiveVal = -1;

        for (int i = 0; i < remainingCards.Count; i++)
        {
            OpponentPlayerCard card = remainingCards[i];
            int baseStat = GetStatValueByStatType(card, chosenStatType);
            int skillBonus = 0;

            if (card.assignedSkill != null)
            {
                skillBonus = card.assignedSkill.Increase;
            }

            int totalValue = baseStat + skillBonus;
            if (totalValue > maxTotalEffectiveVal)
            {
                maxTotalEffectiveVal = totalValue;
                bestSkillCard = card;
            }
        }

        if (bestSkillCard == null)
        {
            bestSkillCard = remainingCards[0];
        }

        return bestSkillCard;
    }

    private OpponentPlayerCard ExecuteSafePick(List<OpponentPlayerCard> remainingCards, out int chosenStatType)
    {
        float totalAtt = 0f;
        float totalMid = 0f;
        float totalDef = 0f;
        int count = remainingCards.Count;

        for (int i = 0; i < count; i++)
        {
            totalAtt = totalAtt + remainingCards[i].attack;
            totalMid = totalMid + remainingCards[i].mid;
            totalDef = totalDef + remainingCards[i].defense;
        }

        float avgAtt = totalAtt / count;
        float avgMid = totalMid / count;
        float avgDef = totalDef / count;

        OpponentPlayerCard bestSafeCard = remainingCards[0];
        chosenStatType = 1;
        float minDiff = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            OpponentPlayerCard card = remainingCards[i];

            float diffAtt = Math.Abs(card.attack - avgAtt);
            
            if (diffAtt < minDiff)
            {
                minDiff = diffAtt;
                bestSafeCard = card;
                chosenStatType = 0;
            }

            float diffMid = Math.Abs(card.mid - avgMid);
            
            if (diffMid < minDiff)
            {
                minDiff = diffMid;
                bestSafeCard = card;
                chosenStatType = 1;
            }

            float diffDef = Math.Abs(card.defense - avgDef);
           
            if (diffDef < minDiff)
            {
                minDiff = diffDef;
                bestSafeCard = card;
                chosenStatType = 2;
            }
        }

        return bestSafeCard;
    }

    private OpponentPlayerCard ExecuteBaitPick(List<OpponentPlayerCard> remainingCards, out int chosenStatType)
    {
        OpponentPlayerCard baitCard = remainingCards[0];
        chosenStatType = GetPosDefaultStatType(baitCard.position);
        int minVal = GetStatValueByStatType(baitCard, chosenStatType);

        for (int i = 1; i < remainingCards.Count; i++)
        {
            OpponentPlayerCard card = remainingCards[i];
            int defaultType = GetPosDefaultStatType(card.position);
            int val = GetStatValueByStatType(card, defaultType);

            if (val < minVal)
            {
                minVal = val;
                baitCard = card;
                chosenStatType = defaultType;
            }
        }

        return baitCard;
    }

    private int GetCounterStatType(int userStatType)
    {
        if (userStatType == 0)
        {
            return 2;
        }

        else if (userStatType == 1)
        {
            return 1;
        }

        else if (userStatType == 2)
        {
            return 0;
        }

        return 2;
    }

    private int GetPosDefaultStatType(string pos)
    {
        if (pos == "FW")
        {
            return 0;
        }

        else if (pos == "MF")
        {
            return 1;
        }

        return 2;
    }

    private int GetStatValueByStatType(OpponentPlayerCard card, int statType)
    {
        if (card == null)
        {
            return 0;
        }

        if (statType == 0)
        {
            return card.attack;
        }

        else if (statType == 1)
        {
            return card.mid;
        }

        else if (statType == 2)
        {
            return card.defense;
        }

        return 0;
    }
}