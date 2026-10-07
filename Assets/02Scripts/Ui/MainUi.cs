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
    }
}