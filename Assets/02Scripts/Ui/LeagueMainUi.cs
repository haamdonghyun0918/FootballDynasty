using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        if (panel_Match != null || panel_Rank != null || panel_History != null)
        {
            panel_Match.SetActive(false);
            panel_Rank.SetActive(true);
            panel_History.SetActive(false);
        }
    }

    private void OnClickHistory()
    {
        if (panel_Match != null || panel_Rank != null || panel_History != null)
        {
            panel_Match.SetActive(false);
            panel_Rank.SetActive(false);
            panel_History.SetActive(true);
        }
    }

    private void RefreshPanel()
    {
        if (panel_Match != null || panel_Rank != null || panel_History != null)
        {
            panel_Match.SetActive(true);
            panel_Rank.SetActive(false);
            panel_History.SetActive(false);
        }
    }

    private void OnClickPlay()
    {

    }

    private void OnClickSim()
    {

    }

    private async void OnClickClose()
    {
        await UiManager.Instance.OpenUi<MainUi>();
        UiManager.Instance.CloseUi<LeagueMainUi>();
    }
}