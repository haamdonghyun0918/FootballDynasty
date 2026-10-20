using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private GameObject[] _subSlots;
    private bool isSubPanelOpen = false;

    private string cardPrefabAddress = "Card";

    private Dictionary<GameObject, UserCardData> _squadSlotDataMap = new Dictionary<GameObject, UserCardData>();
    private Dictionary<GameObject, Card> _squadSlotCardMap = new Dictionary<GameObject, Card>();
    private GameObject _currentSelectedSlot = null;

    private void OnEnable()
    {
        BindSlotButtons();

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

            if (_buttonSubstitute != null)
            {
                _buttonSubstitute.gameObject.SetActive(false);
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

            if (_buttonSubstitute != null)
            {
                _buttonSubstitute.gameObject.SetActive(true);
            }

            ApplyFormationSlots(currentFormation);
        }

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.BindOnClickButtonEvent(OnClickSubstitute);
        }

        if (_panelSubRectTransform != null)
        {
            Vector2 closedPos = _panelSubRectTransform.anchoredPosition;
            closedPos.y = -250f;
            _panelSubRectTransform.anchoredPosition = closedPos;
            isSubPanelOpen = false;
        }
    }

    private void BindSlotButtons()
    {
        BindSingleSlot(_slotGK);

        if (_slotDFs != null)
        {
            for (int i = 0; i < _slotDFs.Length; i++) BindSingleSlot(_slotDFs[i]);
        }

        if (_slotMFs != null)
        {
            for (int i = 0; i < _slotMFs.Length; i++) BindSingleSlot(_slotMFs[i]);
        }

        if (_slotFWs != null)
        {
            for (int i = 0; i < _slotFWs.Length; i++) BindSingleSlot(_slotFWs[i]);
        }

        if (_subSlots != null)
        {
            for (int i = 0; i < _subSlots.Length; i++) BindSingleSlot(_subSlots[i]);
        }
    }

    private void BindSingleSlot(GameObject slotObj)
    {
        if (slotObj == null)
        {
            return;
        }

        UiButton slotButton = slotObj.GetComponent<UiButton>();
        if (slotButton != null)
        {
            slotButton.BindOnClickButtonEvent(OnSlotClickedHandler);
        }
    }

    private void OnSlotClickedHandler()
    {
        GameObject clickedObj = EventSystem.current.currentSelectedGameObject;
        if (clickedObj == null)
        {
            return;
        }

        Transform curTransform = clickedObj.transform;

        while (curTransform != null && curTransform.GetComponent<UiButton>() == null)
        {
            curTransform = curTransform.parent;
        }

        if (curTransform != null)
        {
            OpenFilterForSlot(curTransform.gameObject);
        }
    }

    private void OpenFilterForSlot(GameObject slotObj)
    {
        _currentSelectedSlot = slotObj;
        string posFilter = GetPositionOfSlot(slotObj);

        List<string> excludedIds = new List<string>();

        foreach (var pair in _squadSlotDataMap)
        {
            if (pair.Key != slotObj && pair.Value != null)
            {
                excludedIds.Add(pair.Value.InstanceId.ToString());
            }
        }

        OpenFilterUiAsync(posFilter, excludedIds).Forget();
    }

    private async UniTaskVoid OpenFilterUiAsync(string posFilter, List<string> excludedIds)
    {
        var filterUi = await UiManager.Instance.OpenUi<SquadFilterUi>();
        if (filterUi != null)
        {
            filterUi.Init(posFilter, excludedIds, OnCardSelectedFromFilter);
        }
    }

    private void OnCardSelectedFromFilter(UserCardData selectedUserCard)
    {
        if (_currentSelectedSlot == null || selectedUserCard == null)
        {
            return;
        }

        SetCardToSlot(_currentSelectedSlot, selectedUserCard).Forget();
    }

    private async UniTaskVoid SetCardToSlot(GameObject slotObj, UserCardData userCardData)
    {
        _squadSlotDataMap[slotObj] = userCardData;

        Card card;
        if (_squadSlotCardMap.ContainsKey(slotObj) && _squadSlotCardMap[slotObj] != null)
        {
            card = _squadSlotCardMap[slotObj];
        }

        else
        {
            GameObject cardObj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, slotObj.transform);
            card = cardObj.GetComponent<Card>();
            _squadSlotCardMap[slotObj] = card;
        }

        card.gameObject.SetActive(true);
        card.SetUpCard(userCardData.StaticData);
        card.OnClickCardAction = OnPlacedCardClicked;
    }

    private void OnPlacedCardClicked(Card clickedCard)
    {
        foreach (var pair in _squadSlotCardMap)
        {
            if (pair.Value == clickedCard)
            {
                OpenFilterForSlot(pair.Key);
                break;
            }
        }
    }

    private string GetPositionOfSlot(GameObject slotObj)
    {
        if (slotObj == _slotGK)
        {
            return "GK";
        }

        if (_slotDFs != null)
        {
            for (int i = 0; i < _slotDFs.Length; i++)
            {
                if (_slotDFs[i] == slotObj) return "DF";
            }
        }

        if (_slotMFs != null)
        {
            for (int i = 0; i < _slotMFs.Length; i++)
            {
                if (_slotMFs[i] == slotObj) return "MF";
            }
        }

        if (_slotFWs != null)
        {
            for (int i = 0; i < _slotFWs.Length; i++)
            {
                if (_slotFWs[i] == slotObj) return "FW";
            }
        }

        if (_subSlots != null)
        {
            for (int i = 0; i < _subSlots.Length; i++)
            {
                if (_subSlots[i] == slotObj)
                {
                    string slotName = slotObj.name;

                    if (slotName.Contains("GK"))
                    {
                        return "GK";
                    }

                    else if (slotName.Contains("DF"))
                    {
                        return "DF";
                    }

                    else if (slotName.Contains("MF"))
                    {
                        return "MF";
                    }

                    else if (slotName.Contains("FW"))
                    {
                        return "FW";
                    }
                }
            }
        }

        return string.Empty;
    }

    private void ResetSquadData()
    {
        _squadSlotDataMap.Clear();

        foreach (var pair in _squadSlotCardMap)
        {
            if (pair.Value != null)
            {
                pair.Value.gameObject.SetActive(false);
            }
        }

        _squadSlotCardMap.Clear();
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

    private void ApplyFormationSlots(string formationName)
    {
        ResetSquadData();

        if (formationName == "4-3-3")
        {
            SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(_slotMFs, new int[] { 0, 2, 4 });
            SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });
        }

        else if (formationName == "4-4-2")
        {
            SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });
        }

        else if (formationName == "4-5-1")
        {
            SetSelectedSlotsActive(_slotDFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
            SetSelectedSlotsActive(_slotFWs, new int[] { 2 });
        }

        else if (formationName == "3-5-2")
        {
            SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
            SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 2, 3, 4 });
            SetSelectedSlotsActive(_slotFWs, new int[] { 1, 3 });
        }

        else if (formationName == "3-4-3")
        {
            SetSelectedSlotsActive(_slotDFs, new int[] { 1, 2, 3 });
            SetSelectedSlotsActive(_slotMFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(_slotFWs, new int[] { 0, 2, 4 });
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

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots("4-3-3");
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

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots("4-4-2");
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

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots("4-5-1");
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

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots("3-5-2");
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

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots("3-4-3");
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
                currentPos.y = -250f;
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