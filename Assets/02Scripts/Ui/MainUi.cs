using UnityEngine;
using TMPro;

public class MainUi : UiBase
{
    [Header("LeftDown")]
    [SerializeField] private UiButton button_League;
    [SerializeField] private UiButton button_Squad;
    [SerializeField] private UiButton button_Manage;
    [SerializeField] private UiButton button_Inventory;
    [SerializeField] private UiButton button_Shop;

    [Header("RightDown")]
    [SerializeField] private UiButton button_PickUp;
    [SerializeField] private UiButton button_PlayOff;

    [Header("LeftSide")]
    [SerializeField] private UiButton button_Dictionary;

    [Header("Left Up")]
    [SerializeField] private TextMeshProUGUI text_Name;

    private void OnEnable()
    {
        if (text_Name != null && string.IsNullOrEmpty(DBManager.Instance.CurrentUserName) == false)
        {
            text_Name.text = DBManager.Instance.CurrentUserName;
        }

        if (button_Dictionary)
        {
            button_Dictionary.BindOnClickButtonEvent(OnClickDictionary);
        }

        if (button_Squad)
        {
            button_Squad.BindOnClickButtonEvent(OnClickSquad);
        }

        if (button_Manage)
        {
            button_Manage.BindOnClickButtonEvent(OnClickCard);
        }

        if (button_PickUp)
        {
            button_PickUp.BindOnClickButtonEvent(OnClickPickUp);
        }

        if (button_League)
        {
            button_League.BindOnClickButtonEvent(OnClickSingleMode);
        }
    }

    private async void OnClickDictionary()
    {
        await UiManager.Instance.OpenUi<CardDictionaryUi>();
    }

    private async void OnClickSquad()
    {
        await UiManager.Instance.OpenUi<SquadUi>();
    }

    private async void OnClickCard()
    {
        await UiManager.Instance.OpenUi<CardInventoryUi>();
    }

    private async void OnClickPickUp()
    {
        await UiManager.Instance.OpenUi<PickUpUi>();
    }

    private async void OnClickSingleMode()
    {
        await UiManager.Instance.OpenUi<LeagueSelectUi>();
    }
}