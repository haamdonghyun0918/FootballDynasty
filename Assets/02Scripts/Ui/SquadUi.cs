using UnityEngine;

public class SquadUi : UiBase
{
    [Header("Top Buttons")]
    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonFormation;

    [Header("Formation DropDown")]
    [SerializeField] private GameObject _formationListPanel;
    [SerializeField] private UiButton _button433;
    [SerializeField] private UiButton _button442;
    [SerializeField] private UiButton _button451;
    [SerializeField] private UiButton _button352;
    [SerializeField] private UiButton _button343;

    private bool isDropdownOpen = false;

    private void OnEnable()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonFormation != null)
        {
            _buttonFormation.BindOnClickButtonEvent(OnClickFormation);
        }

        if (_button433 != null)
        {
            _button433.BindOnClickButtonEvent(OnClick433);
        }

        if (_button442 != null)
        {
            _button442.BindOnClickButtonEvent(OnClick442);
        }

        if (_button451 != null)
        {
            _button451.BindOnClickButtonEvent(OnClick451);
        }

        if (_button352 != null)
        {
            _button352.BindOnClickButtonEvent(OnClick352);
        }

        if (_button343 != null)
        {
            _button343.BindOnClickButtonEvent(OnClick343);
        }

        if (_formationListPanel != null)
        {
            _formationListPanel.SetActive(false);
            isDropdownOpen = false;
        }
    }
    private void OnClickFormation()
    {
        if (_formationListPanel != null)
        {
            if (isDropdownOpen == false)
            {
                _formationListPanel.SetActive(true);
                isDropdownOpen = true;
            }

            else
            {
                _formationListPanel.SetActive(false);
                isDropdownOpen = false;
            }
        }
    }

    private void OnClick433()
    {
        Debug.Log("4-3-3 포메이션 선택됨");
        CloseDropdownPanel();
    }

    private void OnClick442()
    {
        Debug.Log("4-4-2 포메이션 선택됨");
        CloseDropdownPanel();
    }

    private void OnClick451()
    {
        Debug.Log("4-5-1 포메이션 선택됨");
        CloseDropdownPanel();
    }

    private void OnClick352()
    {
        Debug.Log("3-5-2 포메이션 선택됨");
        CloseDropdownPanel();
    }

    private void OnClick343()
    {
        Debug.Log("3-4-3 포메이션 선택됨");
        CloseDropdownPanel();
    }

    private void CloseDropdownPanel()
    {
        if (_formationListPanel != null)
        {
            _formationListPanel.SetActive(false);
            isDropdownOpen = false;
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SquadUi>();
    }

}