using Cysharp.Threading.Tasks;
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

    [Header("Panel Rank")]
    [SerializeField] private TextMeshProUGUI text_LeagueInfo;
    [SerializeField] private Transform transform_RankContent;
    private string leagueTeamSlotPrefab = "LeagueTeamSlot";
    private List<LeagueTeamSlot> _rankSlotPool = new List<LeagueTeamSlot>();

    [Header("Panel History")]
    [SerializeField] private Transform transform_HistoryContent;
    private string leagueHistorySlotPrefab = "LeagueHistorySlot";
    private List<LeagueHistorySlot> _historySlotPool = new List<LeagueHistorySlot>();

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

        string homeTeamName = string.Empty;
        string awayTeamName = string.Empty;

        if (seasonData.currentRound % 2 == 0)
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

        LoadMatchLogosAsync(homeTeamName, awayTeamName).Forget();
    }

    private async UniTaskVoid LoadMatchLogosAsync(string homeName, string awayName)
    {
        if (image_Home != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            image_Home.sprite = homeSprite;
        }

        if (image_Away != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            image_Away.sprite = awaySprite;
        }
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
                    aiStanding.wins = 0;
                    aiStanding.draws = 0;
                    aiStanding.losses = 0;
                    aiStanding.points = 0;

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

    private void OnClickSkip()
    {

    }

    private void OnClickStart()
    {

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
    }
}