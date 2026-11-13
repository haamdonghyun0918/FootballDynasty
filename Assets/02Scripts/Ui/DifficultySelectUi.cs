using UnityEngine;

public class DifficultySelectUi : UiBase
{
    [Header("Difficulty Button")]
    [SerializeField] private UiButton button_Diff1;
    [SerializeField] private UiButton button_Diff2;
    [SerializeField] private UiButton button_Diff3;
    [SerializeField] private UiButton button_Diff4;
    [SerializeField] private UiButton button_Diff5;
    [SerializeField] private UiButton button_Diff6;
    [SerializeField] private UiButton button_Diff7;
    [SerializeField] private UiButton button_Diff8;
    [SerializeField] private UiButton button_Diff9;
    [SerializeField] private UiButton button_Diff10;
    [SerializeField] private UiButton button_Diff11;
    [SerializeField] private UiButton button_Diff12;
    [SerializeField] private UiButton button_Diff13;
    [SerializeField] private UiButton button_Diff14;
    [SerializeField] private UiButton button_Diff15;

    [Header("Block Difficulty")]
    [SerializeField] private GameObject block_Diff2;
    [SerializeField] private GameObject block_Diff3;
    [SerializeField] private GameObject block_Diff4;
    [SerializeField] private GameObject block_Diff5;
    [SerializeField] private GameObject block_Diff6;
    [SerializeField] private GameObject block_Diff7;
    [SerializeField] private GameObject block_Diff8;
    [SerializeField] private GameObject block_Diff9;
    [SerializeField] private GameObject block_Diff10;
    [SerializeField] private GameObject Block_Diff11;
    [SerializeField] private GameObject Block_Diff12;
    [SerializeField] private GameObject Block_Diff13;
    [SerializeField] private GameObject Block_Diff14;
    [SerializeField] private GameObject Block_Diff15;

    [Header("Close")]
    [SerializeField] private UiButton button_Close;

    private void OnEnable()
    {
        if (button_Diff1 != null)
        {
            button_Diff1.BindOnClickButtonEvent(OnClickDiff1);
        }

        if (button_Diff2 != null)
        {
            button_Diff2.BindOnClickButtonEvent(OnClickDiff2);
        }

        if (button_Diff3 != null)
        {
            button_Diff3.BindOnClickButtonEvent(OnClickDiff3);
        }

        if (button_Diff4 != null)
        {
            button_Diff4.BindOnClickButtonEvent(OnClickDiff4);
        }

        if (button_Diff5 != null)
        {
            button_Diff5.BindOnClickButtonEvent(OnClickDiff5);
        }

        if (button_Diff6 != null)
        {
            button_Diff6.BindOnClickButtonEvent(OnClickDiff6);
        }

        if (button_Diff7 != null)
        {
            button_Diff7.BindOnClickButtonEvent(OnClickDiff7);
        }

        if (button_Diff8 != null)
        {
            button_Diff8.BindOnClickButtonEvent(OnClickDiff8);
        }

        if (button_Diff9 != null)
        {
            button_Diff9.BindOnClickButtonEvent(OnClickDiff9);
        }

        if (button_Diff10 != null)
        {
            button_Diff10.BindOnClickButtonEvent(OnClickDiff10);
        }

        if (button_Diff11 != null)
        {
            button_Diff11.BindOnClickButtonEvent(OnClickDiff11);
        }

        if (button_Diff12 != null)
        {
            button_Diff12.BindOnClickButtonEvent(OnClickDiff12);
        }

        if (button_Diff13 != null)
        {
            button_Diff13.BindOnClickButtonEvent(OnClickDiff13);
        }

        if (button_Diff14 != null)
        {
            button_Diff14.BindOnClickButtonEvent(OnClickDiff14);
        }

        if (button_Diff15 != null)
        {
            button_Diff15.BindOnClickButtonEvent(OnClickDiff15);
        }

        if (button_Close != null)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }

        int unlockedLevel = DBManager.Instance.GetUserLeagueDifficulty(DBManager.Instance.CurrentUserId);
        UpdateDifficulty(unlockedLevel);
    }

    private void UpdateDifficulty(int unlockedLevel)
    {
        if (block_Diff2 != null)
        {
            block_Diff2.SetActive(unlockedLevel < 2);
        }

        if (block_Diff3 != null)
        {
            block_Diff3.SetActive(unlockedLevel < 3);
        }

        if (block_Diff4 != null)
        {
            block_Diff4.SetActive(unlockedLevel < 4);
        }

        if (block_Diff5 != null)
        {
            block_Diff5.SetActive(unlockedLevel < 5);
        }

        if (block_Diff6 != null)
        {
            block_Diff6.SetActive(unlockedLevel < 6);
        }

        if (block_Diff7 != null)
        {
            block_Diff7.SetActive(unlockedLevel < 7);
        }

        if (block_Diff8 != null)
        {
            block_Diff8.SetActive(unlockedLevel < 8);
        }

        if (block_Diff9 != null)
        {
            block_Diff9.SetActive(unlockedLevel < 9);
        }

        if (block_Diff10 != null)
        {
            block_Diff10.SetActive(unlockedLevel < 10);
        }

        if (Block_Diff11 != null)
        {
            Block_Diff11.SetActive(unlockedLevel < 11);
        }

        if (Block_Diff12 != null)
        {
            Block_Diff12.SetActive(unlockedLevel < 12);
        }

        if (Block_Diff13 != null)
        {
            Block_Diff13.SetActive(unlockedLevel < 13);
        }

        if (Block_Diff14 != null)
        {
            Block_Diff14.SetActive(unlockedLevel < 14);
        }

        if (Block_Diff15 != null)
        {
            Block_Diff15.SetActive(unlockedLevel < 15);
        }
    }

    private async void OnClickDiff1()
    {
        SingleModeManager.Instance.StartSingleMode(1);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff2()
    {
        SingleModeManager.Instance.StartSingleMode(2);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff3()
    {
        SingleModeManager.Instance.StartSingleMode(3);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff4()
    {
        SingleModeManager.Instance.StartSingleMode(4);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff5()
    {
        SingleModeManager.Instance.StartSingleMode(5);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff6()
    {
        SingleModeManager.Instance.StartSingleMode(6);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff7()
    {
        SingleModeManager.Instance.StartSingleMode(7);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff8()
    {
        SingleModeManager.Instance.StartSingleMode(8);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff9()
    {
        SingleModeManager.Instance.StartSingleMode(9);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff10()
    {
        SingleModeManager.Instance.StartSingleMode(10);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff11()
    {
        SingleModeManager.Instance.StartSingleMode(11);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff12()
    {
        SingleModeManager.Instance.StartSingleMode(12);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff13()
    {
        SingleModeManager.Instance.StartSingleMode(13);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff14()
    {
        SingleModeManager.Instance.StartSingleMode(14);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickDiff15()
    {
        SingleModeManager.Instance.StartSingleMode(15);
        await UiManager.Instance.OpenUi<LeagueMainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }

    private async void OnClickClose()
    {
        await UiManager.Instance.OpenUi<MainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }
}