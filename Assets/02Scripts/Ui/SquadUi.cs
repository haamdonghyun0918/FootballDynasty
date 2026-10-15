using UnityEngine;
using TMPro;

public class SquadUi : UiBase
{
    [Header("Top Buttons")]
    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonFormation;
    [SerializeField] private TextMeshProUGUI _textFormationButton;
    private static string currentFormation = "";

    [Header("Formation DropDown")]
    [SerializeField] private GameObject _formationListPanel;
    [SerializeField] private UiButton _button433;
    [SerializeField] private UiButton _button442;
    [SerializeField] private UiButton _button451;
    [SerializeField] private UiButton _button352;
    [SerializeField] private UiButton _button343;

    [Header("Position Slots(From Left: 1 ~ 5)")]
    [SerializeField] private GameObject _slotGK;
    [SerializeField] private GameObject[] _slotDFs;
    [SerializeField] private GameObject[] _slotMFs;
    [SerializeField] private GameObject[] _slotFWs;
    private bool isDropdownOpen = false;

    [Header("Substitute")]
    [SerializeField] private RectTransform _panelSubRectTransform;
    [SerializeField] private UiButton _buttonSubstitute;
    private bool isSubPanelOpen = false;

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

        if (currentFormation == "")
        {
            if (_slotGK != null)
            {
                _slotGK.SetActive(false);
            }

            for (int i = 0; i < _slotDFs.Length; i++)
            {
                if (_slotDFs[i] != null)
                {
                    _slotDFs[i].SetActive(false);
                }
            }

            for (int i = 0; i < _slotMFs.Length; i++)
            {
                if (_slotMFs[i] != null)
                {
                    _slotMFs[i].SetActive(false);
                }
            }

            for (int i = 0; i < _slotFWs.Length; i++)
            {
                if (_slotFWs[i] != null)
                {
                    _slotFWs[i].SetActive(false);
                }
            }

            if (_textFormationButton != null)
            {
                _textFormationButton.text = "포메이션 선택";
            }
        }

        else
        {
            if (_textFormationButton != null)
            {
                _textFormationButton.text = currentFormation;
            }

            if (_slotGK != null)
            {
                _slotGK.SetActive(true);
            }

            if (currentFormation == "4-3-3")
            {
                SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
                SetSelectedSlotsActive(_slotMFs, new int[] { 0, 2, 4 });
                SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });
            }

            else if (currentFormation == "4-4-2")
            {
                SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
                SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
                SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });
            }

            else if (currentFormation == "4-5-1")
            {
                SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
                SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
                SetSelectedSlotsActive(_slotFWs, new int[] { 2 });
            }

            else if (currentFormation == "3-5-2")
            {
                SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
                SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
                SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });
            }

            else if (currentFormation == "3-4-3")
            {
                SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
                SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
                SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });
            }
        }

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.BindOnClickButtonEvent(OnClickSubstitute);
        }

        if (_panelSubRectTransform != null)
        {
            Vector2 closedPos = _panelSubRectTransform.anchoredPosition;
            closedPos.y = -200f;
            _panelSubRectTransform.anchoredPosition = closedPos;
            isSubPanelOpen = false;
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
        if (currentFormation == "4-3-3")
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log("4-3-3 포메이션 선택됨");
        currentFormation = "4-3-3";

        if (_textFormationButton != null)
        {
            _textFormationButton.text = "4-3-3";
        }

        if (_slotGK != null)
        {
            _slotGK.SetActive(true);
        }

        SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
        SetSelectedSlotsActive(_slotMFs, new int[] { 0, 2, 4 });
        SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });

        CloseDropdownPanel();
    }

    private void OnClick442()
    {
        if (currentFormation == "4-4-2")
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log("4-4-2 포메이션 선택됨");
        currentFormation = "4-4-2";

        if (_textFormationButton != null)
        {
            _textFormationButton.text = "4-4-2";
        }

        if (_slotGK != null)
        {
            _slotGK.SetActive(true);
        }

        SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
        SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
        SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });

        CloseDropdownPanel();
    }

    private void OnClick451()
    {
        if (currentFormation == "4-5-1")
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log("4-5-1 포메이션 선택됨");
        currentFormation = "4-5-1";

        if (_textFormationButton != null)
        {
            _textFormationButton.text = "4-5-1";
        }

        if (_slotGK != null)
        {
            _slotGK.SetActive(true);
        }

        SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
        SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
        SetSelectedSlotsActive(_slotFWs, new int[] { 2 });

        CloseDropdownPanel();
    }

    private void OnClick352()
    {
        if (currentFormation == "3-5-2")
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log("3-5-2 포메이션 선택됨");
        currentFormation = "3-5-2";

        if (_textFormationButton != null)
        {
            _textFormationButton.text = "3-5-2";
        }

        if (_slotGK != null)
        {
            _slotGK.SetActive(true);
        }

        SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
        SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
        SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });

        CloseDropdownPanel();
    }

    private void OnClick343()
    {
        if (currentFormation == "3-4-3")
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log("3-4-3 포메이션 선택됨");
        currentFormation = "3-4-3";

        if (_textFormationButton != null)
        {
            _textFormationButton.text = "3-4-3";
        }

        if (_slotGK != null)
        {
            _slotGK.SetActive(true);
        }

        SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
        SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
        SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });

        CloseDropdownPanel();
    }

    private void SetSelectedSlotsActive(GameObject[] slots, int[] actives)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].SetActive(false);
            }
        }

        for (int i = 0; i < actives.Length; i++)
        {
            int targetIndex = actives[i];
            if (targetIndex >= 0 && targetIndex < slots.Length && slots[targetIndex] != null)
            {
                slots[targetIndex].SetActive(true);
            }
        }
    }

    private void CloseDropdownPanel()
    {
        if (_formationListPanel != null)
        {
            _formationListPanel.SetActive(false);
            isDropdownOpen = false;
        }
    }

    private void OnClickSubstitute()
    {
        if (_panelSubRectTransform != null)
        {
            Vector2 currentPos = _panelSubRectTransform.anchoredPosition;

            if (isSubPanelOpen == false)
            {
                currentPos.y = 0f;
                _panelSubRectTransform.anchoredPosition = currentPos;
                isSubPanelOpen = true;
            }

            else
            {
                currentPos.y = -200f;
                _panelSubRectTransform.anchoredPosition = currentPos;
                isSubPanelOpen = false;
            }
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SquadUi>();
    }
}