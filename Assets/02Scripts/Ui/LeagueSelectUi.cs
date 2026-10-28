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

    }

    private void OnClickAmatuer2()
    {

    }

    private void OnClickAmatuer3()
    {

    }

    private void OnClickSemiPro1()
    {

    }

    private void OnClickSemiPro2()
    {

    }

    private void OnClickSemiPro3()
    {

    }

    private void OnClickPro1()
    {

    }

    private void OnClickPro2()
    {

    }

    private void OnClickPro3()
    {

    }

    private void OnClickWorldClass1()
    {

    }

    private void OnClickWorldClass2()
    {

    }

    private void OnClickWorldClass3()
    {

    }

    private void OnClickLegendary1()
    {

    }

    private void OnClickLegendary2()
    {

    }

    private void OnClickLegendary3()
    {

    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<LeagueSelectUi>();
    }
}