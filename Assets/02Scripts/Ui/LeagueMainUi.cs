using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeagueMainUi : UiBase
{
    [Header("Main Buttons")]
    [SerializeField] private UiButton button_Match;
    [SerializeField] private UiButton button_Rank;
    [SerializeField] private UiButton button_History;
    [SerializeField] private UiButton button_Close;

    [Header("Panel")]
    [SerializeField] private GameObject panel_Match;
    [SerializeField] private GameObject panel_Rank;
    [SerializeField] private GameObject panel_History;

    [Header("Panel_Main")]
    [SerializeField] private UiButton button_Sim;
    [SerializeField] private UiButton button_Play;
    [SerializeField] private Image image_Home;
    [SerializeField] private Image image_Away;
    [SerializeField] private TMP_Text text_HomeName;
    [SerializeField] private TMP_Text text_AwayName;
    [SerializeField] private TMP_Text text_Round;

    [Header("Panel_Rank")]
    [SerializeField] private TMP_Text text_Difficulty;
    [SerializeField] private Transform content_TeamView;
    private string rankSlotPrefabAddress = "LeagueRankSlot";
    private List<GameObject> spawnedRankSlots = new List<GameObject>();

    [Header("Panel_History")]
    [SerializeField] private Transform content_HistoryView;
    private string historySlotPrefabAddress = "MatchHistorySlot";
    private List<GameObject> spawnedHistorySlots = new List<GameObject>();


    private void OnEnable()
    {
        if (button_Match != null)
        {
            button_Match.BindOnClickButtonEvent(OnClickMatch);
        }

        if (button_Rank != null)
        {
            button_Rank.BindOnClickButtonEvent(OnClickRank);
        }

        if (button_History != null)
        {
            button_History.BindOnClickButtonEvent(OnClickHistory);
        }

        if (button_Close != null)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }

        if (button_Play != null)
        {
            button_Play.BindOnClickButtonEvent(OnClickPlay);
        }

        if (button_Sim != null)
        {
            button_Sim.BindOnClickButtonEvent(OnClickSim);
        }

        RefreshPanel();
    }

    private void OnClickMatch()
    {
        RefreshPanel();
    }

    private void OnClickRank()
    {
        RefreshRankPanel();
    }

    private void OnClickHistory()
    {
        RefreshHistoryPanel();
    }

    private async void RefreshPanel()
    {
        if (panel_Match != null || panel_Rank != null || panel_History != null)
        {
            panel_Match.SetActive(true);
            panel_Rank.SetActive(false);
            panel_History.SetActive(false);
        }

        string userId = DBManager.Instance.CurrentUserId;
        LeagueMatchData currentMatch = DBManager.Instance.GetCurrentMatchInfo(userId);

        if (currentMatch != null)
        {
            if (text_Round != null)
            {
                text_Round.text = $"ROUND {currentMatch.roundIndex}";
            }

            if (text_HomeName != null)
            {
                text_HomeName.text = currentMatch.homeTeam;
            }

            if (text_AwayName != null)
            {
                text_AwayName.text = currentMatch.awayTeam;
            }

            if (image_Home != null)
            {
                Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(currentMatch.homeTeam);

                if (homeSprite != null)
                {
                    image_Home.sprite = homeSprite;
                    image_Home.gameObject.SetActive(true);
                }

                else
                {
                    image_Home.gameObject.SetActive(false);
                }
            }

            if (image_Away != null)
            {
                Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(currentMatch.awayTeam);

                if (awaySprite != null)
                {
                    image_Away.sprite = awaySprite;
                    image_Away.gameObject.SetActive(true);
                }

                else
                {
                    image_Away.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnClickPlay()
    {

    }

    private void OnClickSim()
    {

    }

    private async void RefreshRankPanel()
    {
        if (panel_Match != null || panel_Rank != null || panel_History != null)
        {
            panel_Match.SetActive(false);
            panel_Rank.SetActive(true);
            panel_History.SetActive(false);
        }

        for (int i = 0; i < spawnedRankSlots.Count; i++)
        {
            if (spawnedRankSlots[i] != null)
            {
                Destroy(spawnedRankSlots[i]);
            }
        }

        spawnedRankSlots.Clear();

        List<RankedTeamData> rankedList = SingleModeManager.Instance.GetRankedTeamList();

        for (int i = 0; i < rankedList.Count; i++)
        {
            int rank = rankedList[i].rank;
            LeagueTeamRecord record = rankedList[i].record;

            GameObject slotObj = await ResourceManager.Instance.Instantiate(rankSlotPrefabAddress, content_TeamView);

            if (slotObj != null)
            {
                spawnedRankSlots.Add(slotObj);
                LeagueRankSlot rankSlot = slotObj.GetComponent<LeagueRankSlot>();

                if (rankSlot != null)
                {
                    rankSlot.SetData(rank, record.teamName, record.wins, record.draws, record.losses, record.points);
                }
            }
        }
    }

    private async void RefreshHistoryPanel()
    {
        if (panel_Match != null && panel_Rank != null && panel_History != null)
        {
            panel_Match.SetActive(false);
            panel_Rank.SetActive(false);
            panel_History.SetActive(true);
        }

        for (int i = 0; i < spawnedHistorySlots.Count; i++)
        {
            if (spawnedHistorySlots[i] != null)
            {
                Destroy(spawnedHistorySlots[i]);
            }
        }

        spawnedHistorySlots.Clear();

        string userId = DBManager.Instance.CurrentUserId;
        string userTeamName = DBManager.Instance.CurrentUserName;
        List<LeagueMatchData> historyList = DBManager.Instance.GetUserMatchHistory(userId, userTeamName);

        for (int i = 0; i < historyList.Count; i++)
        {
            LeagueMatchData match = historyList[i];

            GameObject slotObj = await ResourceManager.Instance.Instantiate(historySlotPrefabAddress, content_HistoryView);

            if (slotObj != null)
            {
                spawnedHistorySlots.Add(slotObj);
                MatchHistorySlot historySlot = slotObj.GetComponent<MatchHistorySlot>();

                if (historySlot != null)
                {
                    historySlot.SetData(match.roundIndex, match.homeTeam, match.awayTeam, match.homeScore, match.awayScore);
                }
            }
        }
    }

    private async void OnClickClose()
    {
        await UiManager.Instance.OpenUi<MainUi>();
        UiManager.Instance.CloseUi<LeagueMainUi>();
    }
}