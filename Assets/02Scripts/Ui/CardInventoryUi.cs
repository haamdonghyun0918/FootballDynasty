using UnityEngine;

public class CardInventoryUi : UiBase
{
    [SerializeField] private UiButton _buttonClose;

    private void OnEnable()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<CardInventoryUi>();
    }
}