using UnityEngine;

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
}