using UnityEngine;

public class CardInventoryUi : UiBase
{
    [SerializeField] private UiButton button_Close;

    private void OnEnable()
    {
        if (button_Close != null)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<CardInventoryUi>();
    }
}