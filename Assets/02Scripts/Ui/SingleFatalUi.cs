using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleFatalUi : UiBase
{
    [Header("Panels")]
    [SerializeField] private GameObject panel_Match;
    [SerializeField] private GameObject panel_Sub;
    [SerializeField] private GameObject panel_Result;
    [SerializeField] private GameObject panel_End;

    [Header("Panel Match - Info")]
    [SerializeField] private TextMeshProUGUI text_Formation;
    [SerializeField] private TextMeshProUGUI text_Turn;
    [SerializeField] private TextMeshProUGUI text_SubPossible;
    [SerializeField] private TextMeshProUGUI text_SubCompleted;
    [SerializeField] private TextMeshProUGUI text_Sub;
    [SerializeField] private UiButton button_Change;

    [Header("Panel Match - ScorePanel")]
    [SerializeField] private GameObject home_Attack;
    [SerializeField] private GameObject away_Attack;
    [SerializeField] private Image logo_Home;
    [SerializeField] private TextMeshProUGUI name_Home;
    [SerializeField] private TextMeshProUGUI point_Home;
    [SerializeField] private Image logo_Away;
    [SerializeField] private TextMeshProUGUI name_Away;
    [SerializeField] private TextMeshProUGUI point_Away;

    private int currentTurn = 1;
    private int subPossibleCount = 5;
    private int subCompletedCount = 0;

    private void OnEnable()
    {
        ShowMatchPanel();
        InitMatchData();
    }

    private void ShowMatchPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(true);
        }

        if (panel_Sub != null)
        {
            panel_Sub.SetActive(false);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_End != null)
        {
            panel_End.SetActive(false);
        }
    }

    private void InitMatchData()
    {
        currentTurn = 1;

        if (text_Turn != null)
        {
            text_Turn.text = "현재 턴: " + currentTurn;
        }

        subPossibleCount = 5;
        subCompletedCount = 0;

        if (text_SubPossible != null)
        {
            text_SubPossible.text = "교체 가능한 인원 수: " + subPossibleCount;
        }

        if (text_SubCompleted != null)
        {
            text_SubCompleted.text = "교체한 인원 수: " + subCompletedCount;
        }

        string userId = DBManager.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(userId) == false)
        {
            string savedFormation = DBManager.Instance.GetUserFormation(userId);
            if (text_Formation != null)
            {
                if (string.IsNullOrEmpty(savedFormation) == false)
                {
                    text_Formation.text = savedFormation;
                }
                else
                {
                    text_Formation.text = "포메이션 선택";
                }
            }
        }

        if (home_Attack != null)
        {
            home_Attack.SetActive(true);
        }

        if (away_Attack != null)
        {
            away_Attack.SetActive(false);
        }

        if (point_Home != null)
        {
            point_Home.text = "0";
        }

        if (point_Away != null)
        {
            point_Away.text = "0";
        }

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        if (seasonData == null)
        {
            seasonData = DBManager.Instance.GetSingleLeagueSeason(userId);
            if (seasonData != null)
            {
                SingleModeManager.Instance.CurrentSeasonData = seasonData;
            }
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

        string opponentTeamName = "상대 팀";

        if (seasonData != null)
        {
            string teamsCsv = seasonData.selectedTeamsCsv;
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
        }

        string homeTeamName = string.Empty;
        string awayTeamName = string.Empty;

        int currentRound = 0;

        if (seasonData != null)
        {
            currentRound = seasonData.currentRound;
        }

        if (currentRound % 2 == 0)
        {
            homeTeamName = playerTeamName;
            awayTeamName = opponentTeamName;
        }

        else
        {
            homeTeamName = opponentTeamName;
            awayTeamName = playerTeamName;
        }

        if (name_Home != null)
        {
            name_Home.text = homeTeamName;
        }

        if (name_Away != null)
        {
            name_Away.text = awayTeamName;
        }

        LoadMatchLogosAsync(homeTeamName, awayTeamName).Forget();
    }

    private async UniTaskVoid LoadMatchLogosAsync(string homeName, string awayName)
    {
        if (logo_Home != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            logo_Home.sprite = homeSprite;
        }

        if (logo_Away != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            logo_Away.sprite = awaySprite;
        }
    }
}