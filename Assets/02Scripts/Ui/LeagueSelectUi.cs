using UnityEngine;

public class LeagueSelectUi : UiBase
{
    [SerializeField] private UiButton button_Close;

    [Header("Amatuer")]
    [SerializeField] private UiButton button_Amatuer_1;
    [SerializeField] private UiButton button_Amatuer_2;
    [SerializeField] private UiButton button_Amatuer_3;

    [Header("SemiPro")]
    [SerializeField] private UiButton button_SemiPro_1;
    [SerializeField] private UiButton button_SemiPro_2;
    [SerializeField] private UiButton button_SemiPro_3;

    [Header("Pro")]
    [SerializeField] private UiButton button_Pro_1;
    [SerializeField] private UiButton button_Pro_2;
    [SerializeField] private UiButton button_Pro_3;

    [Header("WorldClass")]
    [SerializeField] private UiButton button_WorldClass_1;
    [SerializeField] private UiButton button_WorldClass_2;
    [SerializeField] private UiButton button_WorldClass_3;

    [Header("Legendary")]
    [SerializeField] private UiButton button_Legendary_1;
    [SerializeField] private UiButton button_Legendary_2;
    [SerializeField] private UiButton button_Legendary_3;

    private void OnEnable()
    {
        if (button_Amatuer_1 != null)
        {
            button_Amatuer_1.BindOnClickButtonEvent(OnClickAmatuer1);
        }

        if (button_Amatuer_2 != null)
        {
            button_Amatuer_2.BindOnClickButtonEvent(OnClickAmatuer2);
        }

        if (button_Amatuer_3 != null)
        {
            button_Amatuer_3.BindOnClickButtonEvent(OnClickAmatuer3);
        }

        if (button_SemiPro_1 != null)
        {
            button_SemiPro_1.BindOnClickButtonEvent(OnClickSemiPro1);
        }

        if (button_SemiPro_2 != null)
        {
            button_SemiPro_2.BindOnClickButtonEvent(OnClickSemiPro2);
        }

        if (button_SemiPro_3 != null)
        {
            button_SemiPro_3.BindOnClickButtonEvent(OnClickSemiPro3);
        }

        if (button_Pro_1 != null)
        {
            button_Pro_1.BindOnClickButtonEvent(OnClickPro1);
        }

        if (button_Pro_2 != null)
        {
            button_Pro_2.BindOnClickButtonEvent(OnClickPro2);
        }

        if (button_Pro_3 != null)
        {
            button_Pro_3.BindOnClickButtonEvent(OnClickPro3);
        }

        if (button_WorldClass_1 != null)
        {
            button_WorldClass_1.BindOnClickButtonEvent(OnClickWorldClass1);
        }

        if (button_WorldClass_2 != null)
        {
            button_WorldClass_2.BindOnClickButtonEvent(OnClickWorldClass2);
        }

        if (button_WorldClass_3 != null)
        {
            button_WorldClass_3.BindOnClickButtonEvent(OnClickWorldClass3);
        }

        if (button_Legendary_1 != null)
        {
            button_Legendary_1.BindOnClickButtonEvent(OnClickLegendary1);
        }

        if (button_Legendary_2 != null)
        {
            button_Legendary_2.BindOnClickButtonEvent(OnClickLegendary2);
        }

        if (button_Legendary_3 != null)
        {
            button_Legendary_3.BindOnClickButtonEvent(OnClickLegendary3);
        }

        if (button_Close != null)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private async void OnClickAmatuer1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(0);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickAmatuer2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(1);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickAmatuer3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(2);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickSemiPro1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(3);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickSemiPro2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(4);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickSemiPro3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(5);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickPro1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(6);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickPro2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(7);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickPro3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(8);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickWorldClass1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(9);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickWorldClass2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(10);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickWorldClass3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(11);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickLegendary1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(12);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickLegendary2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(13);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private async void OnClickLegendary3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(14);
        await UiManager.Instance.OpenUi<SingleLeagueUi>();
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }
}