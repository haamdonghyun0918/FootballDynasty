using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleLeagueUi : UiBase
{
    [Header("Panel Button")]
    [SerializeField] private UiButton button_PanelMatch;
    [SerializeField] private UiButton button_PanelRank;
    [SerializeField] private UiButton button_PanelHistory;

    [Header("Panels")]
    [SerializeField] private GameObject panel_Match;
    [SerializeField] private GameObject panel_Rank;
    [SerializeField] private GameObject panel_History;
    [SerializeField] private GameObject panel_Result;
    [SerializeField] private GameObject panel_Round;

    [Header("Common Button")]
    [SerializeField] private UiButton button_Close;

    [Header("Panel Match")]
    [SerializeField] private TextMeshProUGUI text_Difficulty;
    [SerializeField] private TextMeshProUGUI text_Round;
    [SerializeField] private TextMeshProUGUI text_Home;
    [SerializeField] private TextMeshProUGUI text_Away;
    [SerializeField] private Image image_Home;
    [SerializeField] private Image image_Away;
    [SerializeField] private UiButton button_Sim;
    [SerializeField] private UiButton button_Start;

    [Header("Panel Result (Simulation)")]
    [SerializeField] private TextMeshProUGUI text_SimLeagueInfo;
    [SerializeField] private TextMeshProUGUI text_SimRound;
    [SerializeField] private Image image_SimHomeLogo;
    [SerializeField] private TextMeshProUGUI text_SimHomeTeam;
    [SerializeField] private TextMeshProUGUI text_SimHomePoint;
    [SerializeField] private TextMeshProUGUI text_SimHomeResult;
    [SerializeField] private Image image_SimAwayLogo;
    [SerializeField] private TextMeshProUGUI text_SimAwayTeam;
    [SerializeField] private TextMeshProUGUI text_SimAwayPoint;
    [SerializeField] private TextMeshProUGUI text_SimAwayResult;
    [SerializeField] private UiButton button_SimResultCheck;

    [Header("Panel Round (All Matches)")]
    [SerializeField] private TextMeshProUGUI text_SimRoundLeagueInfo;
    [SerializeField] private TextMeshProUGUI text_SimRoundTitle;
    [SerializeField] private LeagueRoundObject[] roundRows;
    [SerializeField] private UiButton button_SimRoundCheck;

    [Header("Panel Rank")]
    [SerializeField] private TextMeshProUGUI text_LeagueInfo;
    [SerializeField] private Transform transform_RankContent;
    private string leagueTeamSlotPrefab = "LeagueTeamSlot";
    private List<LeagueTeamSlot> _rankSlotPool = new List<LeagueTeamSlot>();

    [Header("Panel History")]
    [SerializeField] private Transform transform_HistoryContent;
    private string leagueHistorySlotPrefab = "LeagueHistorySlot";
    private List<LeagueHistorySlot> _historySlotPool = new List<LeagueHistorySlot>();

    public static Action<int, int, bool> OnManualMatchFinished;
    private AiPlaying aiPlaying = new AiPlaying();
    private int cachedHomeScore = 0;
    private int cachedAwayScore = 0;
    private string cachedOpponentName = string.Empty;
    private bool cachedIsHome = true;
    private List<RoundMatchData> cachedRoundMatches = new List<RoundMatchData>();

    public class RoundMatchData
    {
        public string homeTeam;
        public string awayTeam;
        public int homeScore;
        public int awayScore;
    }

    private void OnEnable()
    {
        ShowMatchPanel();
        RefreshMatchUi();

        if (button_PanelMatch != null)
        {
            button_PanelMatch.BindOnClickButtonEvent(OnClickMatch);
        }

        if (button_PanelRank != null)
        {
            button_PanelRank.BindOnClickButtonEvent(OnClickRank);
        }

        if (button_PanelHistory != null)
        {
            button_PanelHistory.BindOnClickButtonEvent(OnClickHistory);
        }

        if (button_Close != null)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }

        if (button_Sim != null)
        {
            button_Sim.BindOnClickButtonEvent(OnClickSkip);
        }

        if (button_Start != null)
        {
            button_Start.BindOnClickButtonEvent(OnClickStart);
        }

        if (button_SimResultCheck != null)
        {
            button_SimResultCheck.BindOnClickButtonEvent(OnClickSimResultCheck);
        }

        if (button_SimRoundCheck != null)
        {
            button_SimRoundCheck.BindOnClickButtonEvent(OnClickSimRoundCheck);
        }

        OnManualMatchFinished += HandleManualMatchFinished;

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_Round != null)
        {
            panel_Round.SetActive(false);
        }
    }

    private void OnDisable()
    {
        OnManualMatchFinished -= HandleManualMatchFinished;
    }

    private void RefreshMatchUi()
    {
        string userId = DBManager.Instance.CurrentUserId;
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData == null)
        {
            seasonData = DBManager.Instance.GetSingleLeagueSeason(userId);
            if (seasonData != null)
            {
                SingleModeManager.Instance.CurrentSeasonData = seasonData;
            }
        }

        if (seasonData == null)
        {
            if (text_Difficulty != null)
            {
                text_Difficulty.text = string.Empty;
            }

            if (text_Round != null)
            {
                text_Round.text = "ROUND 1";
            }

            if (text_Home != null)
            {
                text_Home.text = "내 팀";
            }

            if (text_Away != null)
            {
                text_Away.text = "상대 팀";
            }

            if (image_Home != null)
            {
                image_Home.sprite = null;
            }

            if (image_Away != null)
            {
                image_Away.sprite = null;
            }

            return;
        }

        string[] difficultyNames = new string[]
        {
            "Amateur 1", "Amateur 2", "Amateur 3",
            "SemiPro 1", "SemiPro 2", "SemiPro 3",
            "Pro 1", "Pro 2", "Pro 3",
            "WorldClass 1", "WorldClass 2", "WorldClass 3",
            "Legendary 1", "Legendary 2", "Legendary 3"
        };

        int diffIndex = seasonData.difficultyIndex;
        if (text_Difficulty != null)
        {

            if (diffIndex >= 0)
            {
                if (diffIndex < difficultyNames.Length)
                {
                    text_Difficulty.text = difficultyNames[diffIndex];
                }

                else
                {
                    text_Difficulty.text = string.Empty;
                }
            }

            else
            {
                text_Difficulty.text = string.Empty;
            }
        }

        int roundNumber = seasonData.currentRound + 1;

        if (text_Round != null)
        {
            text_Round.text = "ROUND " + roundNumber;
        }

        string playerTeamName = "내 팀";
        string userName = DBManager.Instance.CurrentUserName;

        if (userName != null)
        {
            if (userName != string.Empty)
            {
                playerTeamName = userName;
            }
        }

        string teamsCsv = seasonData.selectedTeamsCsv;
        string opponentTeamName = "상대 팀";

        if (teamsCsv != null)
        {
            if (teamsCsv != string.Empty)
            {
                string[] teams = teamsCsv.Split(',');
                int roundIndex = seasonData.currentRound;

                if (roundIndex >= 0)
                {
                    if (roundIndex < teams.Length)
                    {
                        opponentTeamName = teams[roundIndex];
                    }
                }
            }
        }

        cachedOpponentName = opponentTeamName;
        cachedIsHome = (seasonData.currentRound % 2 == 0);

        string homeTeamName = string.Empty;
        string awayTeamName = string.Empty;

        if (cachedIsHome)
        {
            homeTeamName = playerTeamName;
            awayTeamName = opponentTeamName;
        }

        else
        {
            homeTeamName = opponentTeamName;
            awayTeamName = playerTeamName;
        }

        if (text_Home != null)
        {
            text_Home.text = homeTeamName;
        }

        if (text_Away != null)
        {
            text_Away.text = awayTeamName;
        }

        LoadMatchLogosAsync(homeTeamName, awayTeamName, image_Home, image_Away).Forget();
    }

    private async UniTaskVoid LoadMatchLogosAsync(string homeName, string awayName, Image homeImg, Image awayImg)
    {
        if (homeImg != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            homeImg.sprite = homeSprite;
        }

        if (awayImg != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            awayImg.sprite = awaySprite;
        }
    }

    private void OnClickSkip()
    {
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        if (seasonData == null)
        {
            return;
        }

        int difficultyIndex = seasonData.difficultyIndex;

        SingleLeagueMode leagueMode = new SingleLeagueMode();
        SingleLeagueMode.OpponentSquadResult aiSquad = leagueMode.BuildOpponentSquad(cachedOpponentName, difficultyIndex);
        List<OpponentPlayerCard> aiStartingList = aiSquad != null ? new List<OpponentPlayerCard>(aiSquad.startingLineup) : new List<OpponentPlayerCard>();

        SimulatePlayerMatch11Turns(aiStartingList, difficultyIndex, out cachedHomeScore, out cachedAwayScore, cachedIsHome);
        cachedRoundMatches = GenerateAllRoundMatches(seasonData, cachedIsHome, cachedHomeScore, cachedAwayScore);

        OpenSimulationResultPanel(seasonData, cachedHomeScore, cachedAwayScore, cachedIsHome);
    }

    private void SimulatePlayerMatch11Turns(List<OpponentPlayerCard> aiCards, int difficultyIndex, out int homeScore, out int awayScore, bool isHome)
    {
        int pScore = 0;
        int aScore = 0;

        string userId = DBManager.Instance.CurrentUserId;
        Dictionary<string, int> savedSlots = DBManager.Instance.GetUserSquadSlots(userId);
        List<UserCardData> userCards = DBManager.Instance.GetUserCards(userId);

        List<UserCardData> myStartingCards = new List<UserCardData>();
        string[] posKeys = { "GK", "DF_0", "DF_1", "DF_2", "DF_3", "DF_4", "MF_0", "MF_1", "MF_2", "MF_3", "MF_4", "FW_0", "FW_1", "FW_2", "FW_3", "FW_4" };
        foreach (var key in posKeys)
        {
            if (savedSlots.ContainsKey(key) && savedSlots[key] != -1)
            {
                int instanceId = savedSlots[key];
                var card = userCards.Find(c => c != null && c.InstanceId == instanceId);
                if (card != null && !myStartingCards.Contains(card))
                {
                    myStartingCards.Add(card);
                }
            }
        }

        for (int turn = 1; turn <= 11; turn++)
        {
            if (myStartingCards.Count == 0 || aiCards.Count == 0)
            {
                break;
            }

            UserCardData myCard = myStartingCards[(turn - 1) % myStartingCards.Count];
            PlayerData myPlayerData = GameDataManager.Instance.GetData<PlayerData>(myCard.PlayerId);

            int myStatType = (turn - 1) % 3;
            int myBaseVal = 0;

            if (myPlayerData != null)
            {
                if (myStatType == 0)
                {
                    myBaseVal = myPlayerData.Att;
                }

                else if (myStatType == 1)
                {
                    myBaseVal = myPlayerData.Mid;
                }

                else
                {
                    myBaseVal = myPlayerData.Def;
                }
            }

            var aiResult = aiPlaying.DecideAiCard(aiCards, turn, difficultyIndex, myStatType, false);
            OpponentPlayerCard aiCard = aiResult != null ? aiResult.selectedCard : aiCards[0];
            int aiStatType = aiResult != null ? aiResult.selectedStatType : 2;
            aiCards.Remove(aiCard);

            int aiBaseVal = 0;

            if (aiCard != null)
            {
                if (aiStatType == 0)
                {
                    aiBaseVal = aiCard.attack;
                }

                else if (aiStatType == 1)
                {
                    aiBaseVal = aiCard.mid;
                }

                else
                {
                    aiBaseVal = aiCard.defense;
                }
            }

            SkillData myCameraSkill = GetSkillDataByStatType(myCard, myStatType);
            int myIncrease = myCameraSkill != null ? myCameraSkill.Increase : 0;
            int aiDecrease = myCameraSkill != null ? myCameraSkill.Decrease : 0;
            int aiSkillIncrease = (aiCard != null && aiCard.assignedSkill != null) ? aiCard.assignedSkill.Increase : 0;

            int myFinal = myBaseVal + myIncrease;
            int aiFinal = aiBaseVal + aiSkillIncrease - aiDecrease;

            if (aiFinal < 0)
            {
                aiFinal = 0;
            }

            if (myFinal > aiFinal)
            {
                pScore++;
            }

            else if (myFinal < aiFinal)
            {
                aScore++;
            }

            else
            {
                int myOtherSum = myPlayerData != null ? (myPlayerData.Att + myPlayerData.Mid + myPlayerData.Def) : 0;
                int aiOtherSum = aiBaseVal * 3;
                if (myOtherSum > aiOtherSum)
                {
                    pScore++;
                }

                else if (myOtherSum < aiOtherSum)
                {
                    aScore++;
                }
            }
        }

        if (isHome)
        {
            homeScore = pScore;
            awayScore = aScore;
        }

        else
        {
            homeScore = aScore;
            awayScore = pScore;
        }
    }

    private SkillData GetSkillDataByStatType(UserCardData cardData, int statType)
    {
        if (statType == 0)
        {
            return cardData.SkillFwData;
        }

        else if (statType == 1)
        {
            return cardData.SkillMfData;
        }

        else if (statType == 2)
        {
            return cardData.SkillDfData;
        }

        return null;
    }

    private List<RoundMatchData> GenerateAllRoundMatches(SingleLeagueData seasonData, bool isPlayerHome, int playerHomeScore, int playerAwayScore)
    {
        List<RoundMatchData> matches = new List<RoundMatchData>();
        string playerTeamName = DBManager.Instance.CurrentUserName;
        if (string.IsNullOrEmpty(playerTeamName))
        {
            playerTeamName = "내 팀";
        }

        List<string> allTeams = new List<string> { playerTeamName };
        if (!string.IsNullOrEmpty(seasonData.selectedTeamsCsv))
        {
            allTeams.AddRange(seasonData.selectedTeamsCsv.Split(','));
        }

        int round = seasonData.currentRound;
        int numTeams = allTeams.Count;
        int half = numTeams / 2;

        List<string> rotatingTeams = new List<string>(allTeams);
        rotatingTeams.RemoveAt(0);

        int shift = round % rotatingTeams.Count;
        List<string> currentRoundTeams = new List<string> { allTeams[0] };
        for (int i = 0; i < rotatingTeams.Count; i++)
        {
            currentRoundTeams.Add(rotatingTeams[(i + shift) % rotatingTeams.Count]);
        }

        for (int i = 0; i < half; i++)
        {
            string t1 = currentRoundTeams[i];
            string t2 = currentRoundTeams[numTeams - 1 - i];

            RoundMatchData record = new RoundMatchData();

            if (i == 0 && t1 == playerTeamName)
            {
                record.homeTeam = t1;
                record.awayTeam = t2;
                record.homeScore = playerHomeScore;
                record.awayScore = playerAwayScore;
            }

            else if (i == 0 && t2 == playerTeamName)
            {
                record.homeTeam = t2;
                record.awayTeam = t1;
                record.homeScore = playerHomeScore;
                record.awayScore = playerAwayScore;
            }

            else
            {
                record.homeTeam = (i % 2 == 0) ? t1 : t2;
                record.awayTeam = (i % 2 == 0) ? t2 : t1;
                System.Random rand = new System.Random((seasonData.currentRound + 1) * 100 + i);
                record.homeScore = rand.Next(0, 4);
                record.awayScore = rand.Next(0, 3);
            }

            matches.Add(record);
        }

        return matches;
    }

    private void OpenSimulationResultPanel(SingleLeagueData seasonData, int homeScore, int awayScore, bool isHome)
    {
        if (panel_Result != null)
        {
            panel_Result.SetActive(true);
        }

        string diffName = text_Difficulty != null ? text_Difficulty.text : "";
        int roundNum = seasonData.currentRound + 1;
        if (text_SimLeagueInfo != null)
        {
            text_SimLeagueInfo.text = "리그 난이도: " + diffName;
        }
        if (text_SimRound != null)
        {
            text_SimRound.text = "ROUND " + roundNum;
        }

        string playerTeamName = DBManager.Instance.CurrentUserName;
        if (string.IsNullOrEmpty(playerTeamName))
        {
            playerTeamName = "내 팀";
        }

        string homeName = isHome ? playerTeamName : cachedOpponentName;
        string awayName = isHome ? cachedOpponentName : playerTeamName;

        if (text_SimHomeTeam != null)
        {
            text_SimHomeTeam.text = homeName;
        }

        if (text_SimAwayTeam != null)
        {
            text_SimAwayTeam.text = awayName;
        }

        if (text_SimHomePoint != null)
        {
            text_SimHomePoint.text = homeScore.ToString();
        }

        if (text_SimAwayPoint != null)
        {
            text_SimAwayPoint.text = awayScore.ToString();
        }

        LoadMatchLogosAsync(homeName, awayName, image_SimHomeLogo, image_SimAwayLogo).Forget();

        int playerGoals = isHome ? homeScore : awayScore;
        int opponentGoals = isHome ? awayScore : homeScore;

        if (playerGoals > opponentGoals)
        {
            if (text_SimHomeResult != null) { text_SimHomeResult.text = isHome ? "Win" : "Lose"; text_SimHomeResult.color = isHome ? Color.red : Color.blue; }
            if (text_SimAwayResult != null) { text_SimAwayResult.text = isHome ? "Lose" : "Win"; text_SimAwayResult.color = isHome ? Color.blue : Color.red; }
        }

        else if (playerGoals < opponentGoals)
        {
            if (text_SimHomeResult != null) { text_SimHomeResult.text = isHome ? "Lose" : "Win"; text_SimHomeResult.color = isHome ? Color.blue : Color.red; }
            if (text_SimAwayResult != null) { text_SimAwayResult.text = "Win"; text_SimAwayResult.color = isHome ? Color.red : Color.blue; }
        }

        else
        {
            if (text_SimHomeResult != null) { text_SimHomeResult.text = "Draw"; text_SimHomeResult.color = Color.white; }
            if (text_SimAwayResult != null) { text_SimAwayResult.text = "Draw"; text_SimAwayResult.color = Color.white; }
        }
    }

    private void HandleManualMatchFinished(int homeScore, int awayScore, bool isHome)
    {
        cachedHomeScore = homeScore;
        cachedAwayScore = awayScore;
        cachedIsHome = isHome;

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData != null)
        {
            cachedRoundMatches = GenerateAllRoundMatches(seasonData, cachedIsHome, cachedHomeScore, cachedAwayScore);
            OpenSimulationRoundPanel();
        }
    }

    private void OnClickSimResultCheck()
    {
        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        OpenSimulationRoundPanel();
    }

    private void OpenSimulationRoundPanel()
    {
        if (panel_Round != null)
        {
            panel_Round.SetActive(true);
        }

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        string diffName = text_Difficulty != null ? text_Difficulty.text : "";
        int roundNum = seasonData != null ? seasonData.currentRound + 1 : 1;

        if (text_SimRoundLeagueInfo != null)
        {
            text_SimRoundLeagueInfo.text = "리그 난이도: " + diffName;
        }

        if (text_SimRoundTitle != null)
        {
            text_SimRoundTitle.text = "ROUND " + roundNum;
        }

        if (roundRows != null)
        {
            for (int i = 0; i < roundRows.Length; i++)
            {
                if (i < cachedRoundMatches.Count)
                {
                    if (roundRows[i] != null)
                    {
                        roundRows[i].SetMatchData(cachedRoundMatches[i].homeTeam, cachedRoundMatches[i].awayTeam, cachedRoundMatches[i].homeScore, cachedRoundMatches[i].awayScore);
                    }
                }
            }
        }
    }

    private void OnClickSimRoundCheck()
    {
        if (panel_Round != null)
        {
            panel_Round.SetActive(false);
        }

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        if (seasonData != null)
        {
            string userId = DBManager.Instance.CurrentUserId;
            int playerGoals = cachedIsHome ? cachedHomeScore : cachedAwayScore;
            int opponentGoals = cachedIsHome ? cachedAwayScore : cachedHomeScore;

            if (playerGoals > opponentGoals)
            {
                seasonData.wins++;
                seasonData.points += 3;
            }

            else if (playerGoals == opponentGoals)
            {
                seasonData.draws++;
                seasonData.points += 1;
            }

            else
            {
                seasonData.losses++;
            }

            string newRecord = cachedHomeScore + ":" + cachedAwayScore;
            if (string.IsNullOrEmpty(seasonData.matchHistoryCsv))
            {
                seasonData.matchHistoryCsv = newRecord;
            }

            else
            {
                seasonData.matchHistoryCsv += "," + newRecord;
            }

            seasonData.currentRound++;

            DBManager.Instance.UpdateSingleLeagueProgress(
                userId,
                seasonData.currentRound,
                seasonData.wins,
                seasonData.draws,
                seasonData.losses,
                seasonData.points,
                seasonData.matchHistoryCsv,
                seasonData.aiResultsCsv
            );
        }

        RefreshMatchUi();
        RefreshRankUi().Forget();
        RefreshHistoryUi().Forget();
    }

    private async UniTaskVoid RefreshRankUi()
    {
        string userId = DBManager.Instance.CurrentUserId;
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData == null)
        {
            seasonData = DBManager.Instance.GetSingleLeagueSeason(userId);
            if (seasonData != null)
            {
                SingleModeManager.Instance.CurrentSeasonData = seasonData;
            }
        }

        if (seasonData == null)
        {
            return;
        }

        string[] difficultyNames = new string[]
        {
            "Amateur 1", "Amateur 2", "Amateur 3",
            "SemiPro 1", "SemiPro 2", "SemiPro 3",
            "Pro 1", "Pro 2", "Pro 3",
            "WorldClass 1", "WorldClass 2", "WorldClass 3",
            "Legendary 1", "Legendary 2", "Legendary 3"
        };

        string diffText = string.Empty;
        int diffIndex = seasonData.difficultyIndex;

        if (diffIndex >= 0)
        {
            if (diffIndex < difficultyNames.Length)
            {
                diffText = difficultyNames[diffIndex];
            }
        }

        List<TeamStanding> standings = new List<TeamStanding>();

        TeamStanding playerStanding = new TeamStanding();
        string userName = DBManager.Instance.CurrentUserName;

        if (userName != null)
        {
            if (userName == string.Empty)
            {
                playerStanding.teamName = "내 팀";
            }

            else
            {
                playerStanding.teamName = userName;
            }
        }

        playerStanding.isPlayer = true;
        playerStanding.wins = seasonData.wins;
        playerStanding.draws = seasonData.draws;
        playerStanding.losses = seasonData.losses;
        playerStanding.points = seasonData.points;
        standings.Add(playerStanding);

        string teamsCsv = seasonData.selectedTeamsCsv;
        if (teamsCsv != null)
        {
            if (teamsCsv != string.Empty)
            {
                string[] aiTeams = teamsCsv.Split(',');
                for (int i = 0; i < aiTeams.Length; i++)
                {
                    TeamStanding aiStanding = new TeamStanding();
                    aiStanding.teamName = aiTeams[i];
                    aiStanding.isPlayer = false;
                    aiStanding.wins = seasonData.currentRound > 0 ? (i % (seasonData.currentRound + 1)) : 0;
                    aiStanding.points = aiStanding.wins * 3;

                    standings.Add(aiStanding);
                }
            }
        }

        for (int i = 0; i < standings.Count - 1; i++)
        {
            for (int j = 0; j < standings.Count - 1 - i; j++)
            {
                bool needSwap = false;
                if (standings[j].points < standings[j + 1].points)
                {
                    needSwap = true;
                }

                else if (standings[j].points == standings[j + 1].points)
                {
                    if (standings[j].wins < standings[j + 1].wins)
                    {
                        needSwap = true;
                    }
                }

                if (needSwap == true)
                {
                    TeamStanding temp = standings[j];
                    standings[j] = standings[j + 1];
                    standings[j + 1] = temp;
                }
            }
        }

        int playerRank = 1;
        for (int i = 0; i < standings.Count; i++)
        {
            if (standings[i].isPlayer == true)
            {
                playerRank = i + 1;
                break;
            }
        }

        if (text_LeagueInfo != null)
        {
            text_LeagueInfo.text = "리그 난이도: " + diffText + " / 리그 순위: " + playerRank;
        }

        if (transform_RankContent != null)
        {
            while (_rankSlotPool.Count < standings.Count)
            {
                GameObject slotObj = await ResourceManager.Instance.Instantiate(leagueTeamSlotPrefab, transform_RankContent);
                if (slotObj != null)
                {
                    LeagueTeamSlot slotScript;

                    if (slotObj.TryGetComponent<LeagueTeamSlot>(out slotScript))
                    {
                        _rankSlotPool.Add(slotScript);
                    }
                }
            }

            for (int i = 0; i < _rankSlotPool.Count; i++)
            {
                if (i < standings.Count)
                {
                    _rankSlotPool[i].gameObject.SetActive(true);
                    int rank = i + 1;
                    TeamStanding standing = standings[i];
                    _rankSlotPool[i].SetSlotData(rank, standing);
                }

                else
                {
                    _rankSlotPool[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private async UniTaskVoid RefreshHistoryUi()
    {
        string userId = DBManager.Instance.CurrentUserId;
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData == null)
        {
            seasonData = DBManager.Instance.GetSingleLeagueSeason(userId);
            if (seasonData != null)
            {
                SingleModeManager.Instance.CurrentSeasonData = seasonData;
            }
        }

        if (seasonData == null)
        {
            return;
        }

        string playerTeamName = "내 팀";
        string userName = DBManager.Instance.CurrentUserName;
        if (userName != null)
        {
            if (userName != string.Empty)
            {
                playerTeamName = userName;
            }
        }

        List<string> historyList = new List<string>();
        string historyCsv = seasonData.matchHistoryCsv;

        if (historyCsv != null)
        {
            if (historyCsv != string.Empty)
            {
                string[] splitHistory = historyCsv.Split(',');
                for (int i = 0; i < splitHistory.Length; i++)
                {
                    if (string.IsNullOrEmpty(splitHistory[i]) == false)
                    {
                        historyList.Add(splitHistory[i]);
                    }
                }
            }
        }

        string teamsCsv = seasonData.selectedTeamsCsv;
        string[] teams = null;
        if (teamsCsv != null)
        {
            if (teamsCsv != string.Empty)
            {
                teams = teamsCsv.Split(',');
            }
        }

        if (transform_HistoryContent != null)
        {
            while (_historySlotPool.Count < historyList.Count)
            {
                GameObject slotObj = await ResourceManager.Instance.Instantiate(leagueHistorySlotPrefab, transform_HistoryContent);
                if (slotObj != null)
                {
                    LeagueHistorySlot slotScript;
                    if (slotObj.TryGetComponent<LeagueHistorySlot>(out slotScript))
                    {
                        _historySlotPool.Add(slotScript);
                    }
                }
            }

            for (int i = 0; i < _historySlotPool.Count; i++)
            {
                if (i < historyList.Count)
                {
                    _historySlotPool[i].gameObject.SetActive(true);

                    string record = historyList[i];
                    string[] scores = record.Split(':');
                    int savedHomeScore = 0;
                    int savedAwayScore = 0;

                    if (scores.Length >= 2)
                    {
                        int.TryParse(scores[0], out savedHomeScore);
                        int.TryParse(scores[1], out savedAwayScore);
                    }

                    string opponentTeamName = "상대 팀";
                    if (teams != null)
                    {
                        if (i < teams.Length)
                        {
                            opponentTeamName = teams[i];
                        }
                    }

                    string homeName = string.Empty;
                    string awayName = string.Empty;
                    int homeScore = 0;
                    int awayScore = 0;

                    if (i % 2 == 0)
                    {
                        homeName = playerTeamName;
                        awayName = opponentTeamName;
                        homeScore = savedHomeScore;
                        awayScore = savedAwayScore;
                    }

                    else
                    {
                        homeName = opponentTeamName;
                        awayName = playerTeamName;
                        homeScore = savedAwayScore;
                        awayScore = savedHomeScore;
                    }

                    _historySlotPool[i].SetSlotData(homeName, awayName, homeScore, awayScore, playerTeamName);
                }

                else
                {
                    _historySlotPool[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SingleLeagueUi>();
    }

    private void OnClickMatch()
    {
        ShowMatchPanel();
        RefreshMatchUi();
    }

    private void OnClickRank()
    {
        ShowRankPanel();
        RefreshRankUi().Forget();
    }

    private void OnClickHistory()
    {
        ShowHistoryPanel();
        RefreshHistoryUi().Forget();
    }

    private async void OnClickStart()
    {
        await UiManager.Instance.OpenUi<SingleFatalUi>();
    }

    private void ShowMatchPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(true);
        }

        if (panel_Rank != null)
        {
            panel_Rank.SetActive(false);
        }

        if (panel_History != null)
        {
            panel_History.SetActive(false);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_Round != null)
        {
            panel_Round.SetActive(false);
        }
    }

    private void ShowRankPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(false);
        }

        if (panel_Rank != null)
        {
            panel_Rank.SetActive(true);
        }

        if (panel_History != null)
        {
            panel_History.SetActive(false);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_Round != null)
        {
            panel_Round.SetActive(false);
        }
    }

    private void ShowHistoryPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(false);
        }

        if (panel_Rank != null)
        {
            panel_Rank.SetActive(false);
        }

        if (panel_History != null)
        {
            panel_History.SetActive(true);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_Round != null)
        {
            panel_Round.SetActive(false);
        }
    }
}