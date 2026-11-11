using UnityEngine;

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


    private async void OnClickClose()
    {
        await UiManager.Instance.OpenUi<MainUi>();
        UiManager.Instance.CloseUi<LeagueMainUi>();
    }
}