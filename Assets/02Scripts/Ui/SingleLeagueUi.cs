using Cysharp.Threading.Tasks;
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

    private void OnEnable()
    {
        ShowMatchPanel();
        RefreshMatchUI();

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

    private void RefreshMatchUI()
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

            if (image_Away != null)
            {
                image_Away.gameObject.SetActive(false);
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

        string userName = DBManager.Instance.CurrentUserName;
        if (text_Home != null)
        {
            if (userName != null)
            {
                if (userName == string.Empty)
                {
                    text_Home.text = "내 팀";
                }

                else
                {
                    text_Home.text = userName;
                }
            }

            else
            {
                text_Home.text = "내 팀";
            }
        }

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
                        string awayTeamName = teams[roundIndex];

                        if (text_Away != null)
                        {
                            text_Away.text = awayTeamName;
                        }

                        LoadAwayLogoAsync(awayTeamName).Forget();
                    }
                }
            }
        }
    }

    private async UniTaskVoid LoadAwayLogoAsync(string teamName)
    {
        if (image_Away == null)
        {
            return;
        }

        string address = teamName;
        Sprite logoSprite = await ResourceManager.Instance.LoadAsset<Sprite>(address);

        if (logoSprite != null)
        {
            image_Away.sprite = logoSprite;
            image_Away.gameObject.SetActive(true);
        }

        else
        {
            image_Away.gameObject.SetActive(false);
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SingleLeagueUi>();
    }

    private void OnClickMatch()
    {
        ShowMatchPanel();
    }

    private void OnClickRank()
    {
        ShowRankPanel();
    }

    private void OnClickHistory()
    {
        ShowHistoryPanel();
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