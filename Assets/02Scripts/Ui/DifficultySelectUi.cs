using System.Runtime.CompilerServices;
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
    }

    private void OnClickDiff1()
    {
        SingleModeManager.Instance.StartSingleMode(1);
    }

    private void OnClickDiff2()
    {
        SingleModeManager.Instance.StartSingleMode(2);
    }

    private void OnClickDiff3()
    {
        SingleModeManager.Instance.StartSingleMode(3);
    }

    private void OnClickDiff4()
    {
        SingleModeManager.Instance.StartSingleMode(4);
    }

    private void OnClickDiff5()
    {
        SingleModeManager.Instance.StartSingleMode(5);
    }

    private void OnClickDiff6()
    {
        SingleModeManager.Instance.StartSingleMode(6);
    }

    private void OnClickDiff7()
    {
        SingleModeManager.Instance.StartSingleMode(7);
    }

    private void OnClickDiff8()
    {
        SingleModeManager.Instance.StartSingleMode(8);
    }

    private void OnClickDiff9()
    {
        SingleModeManager.Instance.StartSingleMode(9);
    }

    private void OnClickDiff10()
    {
        SingleModeManager.Instance.StartSingleMode(10);
    }

    private void OnClickDiff11()
    {
        SingleModeManager.Instance.StartSingleMode(11);
    }

    private void OnClickDiff12()
    {
        SingleModeManager.Instance.StartSingleMode(12);
    }

    private void OnClickDiff13()
    {
        SingleModeManager.Instance.StartSingleMode(13);
    }

    private void OnClickDiff14()
    {
        SingleModeManager.Instance.StartSingleMode(14);
    }

    private void OnClickDiff15()
    {
        SingleModeManager.Instance.StartSingleMode(15);
    }

    private async void OnClickClose()
    {
        await UiManager.Instance.OpenUi<MainUi>();
        UiManager.Instance.CloseUi<DifficultySelectUi>();
    }
}