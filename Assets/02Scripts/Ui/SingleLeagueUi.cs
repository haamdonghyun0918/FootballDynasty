using UnityEngine;
using TMPro;
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