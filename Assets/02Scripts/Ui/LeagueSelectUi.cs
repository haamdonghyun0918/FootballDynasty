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

    private void OnClickAmatuer1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(0);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickAmatuer2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(1);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickAmatuer3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(2);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickSemiPro1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(3);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickSemiPro2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(4);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickSemiPro3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(5);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickPro1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(6);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickPro2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(7);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickPro3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(8);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickWorldClass1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(9);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickWorldClass2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(10);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickWorldClass3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(11);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickLegendary1()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(12);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickLegendary2()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(13);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickLegendary3()
    {
        SingleModeManager.Instance.StartNewLeagueSeason(14);
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }
}