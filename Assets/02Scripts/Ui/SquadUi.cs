using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] private SquadSlot _slotGK;
    [SerializeField] private SquadSlot[] _slotDFs;
    [SerializeField] private SquadSlot[] _slotMFs;
    [SerializeField] private SquadSlot[] _slotFWs;
    private bool isDropdownOpen = false;

    [Header("Substitute")]
    [SerializeField] private RectTransform _panelSubRectTransform;
    [SerializeField] private UiButton _buttonSubstitute;
    [SerializeField] private SquadSlot[] _subSlots;
    private bool isSubPanelOpen = false;

    private SquadSlot _currentSelectedSlot = null;
    private List<SquadSlot> _allSlots = new List<SquadSlot>();

    private void Awake()
    {
        CollectAllSlots();
    }

    private void OnEnable()
    {
        InitSlots();
        BindButtons();

        if (_formationListPanel != null)
        {
            _formationListPanel.SetActive(false);
            isDropdownOpen = false;
        }

        if (string.IsNullOrEmpty(currentFormation))
        {
            SetAllSlotsActive(false);

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
                _slotGK.gameObject.SetActive(true);
            }

            if (_buttonSubstitute != null)
            {
                _buttonSubstitute.gameObject.SetActive(true);
            }

            ApplyFormationSlots(currentFormation);
        }

        if (_panelSubRectTransform != null)
        {
            Vector2 closedPos = _panelSubRectTransform.anchoredPosition;
            closedPos.y = -250f;
            _panelSubRectTransform.anchoredPosition = closedPos;
            isSubPanelOpen = false;
        }

        LoadSquadData();
    }

    private void OnDisable()
    {
        UnbindButtons();
    }

    private void CollectAllSlots()
    {
        _allSlots.Clear();

        if (_slotGK != null)
        {
            _allSlots.Add(_slotGK);
        }

        if (_slotDFs != null)
        {
            for (int i = 0; i < _slotDFs.Length; i++)
            {
                if (_slotDFs[i] != null)
                {
                    _allSlots.Add(_slotDFs[i]);
                }
            }
        }

        if (_slotMFs != null)
        {
            for (int i = 0; i < _slotMFs.Length; i++)
            {
                if (_slotMFs[i] != null)
                {
                    _allSlots.Add(_slotMFs[i]);
                }
            }
        }

        if (_slotFWs != null)
        {
            for (int i = 0; i < _slotFWs.Length; i++)
            {
                if (_slotFWs[i] != null)
                {
                    _allSlots.Add(_slotFWs[i]);
                }
            }
        }

        if (_subSlots != null)
        {
            for (int i = 0; i < _subSlots.Length; i++)
            {
                if (_subSlots[i] != null)
                {
                    _allSlots.Add(_subSlots[i]);
                }
            }
        }
    }

    private void InitSlots()
    {
        for (int i = 0; i < _allSlots.Count; i++)
        {
            if (_allSlots[i] != null)
            {
                _allSlots[i].Init(_allSlots[i].SlotPosition, OnSlotClicked);
            }
        }
    }

    private void OnSlotClicked(SquadSlot clickedSlot)
    {
        if (clickedSlot == null)
        {
            return;
        }

        _currentSelectedSlot = clickedSlot;
        List<string> excludedIds = new List<string>();

        for (int i = 0; i < _allSlots.Count; i++)
        {
            SquadSlot slot = _allSlots[i];

            if (slot != clickedSlot && slot.TargetUserCardData != null)
            {
                excludedIds.Add(slot.TargetUserCardData.InstanceId.ToString());
            }
        }

        OpenFilterUiAsync(clickedSlot.SlotPosition, excludedIds).Forget();
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

        _currentSelectedSlot.SetCardData(selectedUserCard).Forget();
    }

    private void SetAllSlotsActive(bool isActive)
    {
        if (_slotGK != null)
        {
            _slotGK.gameObject.SetActive(isActive);
        }

        SetSlotArrayActive(_slotDFs, isActive);
        SetSlotArrayActive(_slotMFs, isActive);
        SetSlotArrayActive(_slotFWs, isActive);
        SetSlotArrayActive(_subSlots, isActive);
    }

    private void SetSlotArrayActive(SquadSlot[] slots, bool isActive)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(isActive);
            }
        }
    }

    private void SetSelectedSlotsActive(SquadSlot[] slots, int[] activeIndices)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(false);
            }
        }

        if (activeIndices == null)
        {
            return;
        }

        for (int i = 0; i < activeIndices.Length; i++)
        {
            int targetIndex = activeIndices[i];
            if (targetIndex >= 0 && targetIndex < slots.Length && slots[targetIndex] != null)
            {
                slots[targetIndex].gameObject.SetActive(true);
            }
        }
    }

    private void ResetSquadData()
    {
        for (int i = 0; i < _allSlots.Count; i++)
        {
            if (_allSlots[i] != null)
            {
                _allSlots[i].ClearSlot();
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

    private void BindButtons()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonFormation != null)
        {
            _buttonFormation.BindOnClickButtonEvent(OnClickFormation);
        }
        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.BindOnClickButtonEvent(OnClickSubstitute);
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
    }

    private void UnbindButtons()
    {
        if (_buttonClose != null)
        {
            _buttonClose.UnBindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonFormation != null)
        {
            _buttonFormation.UnBindOnClickButtonEvent(OnClickFormation);
        }

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.UnBindOnClickButtonEvent(OnClickSubstitute);
        }

        if (_button433 != null)
        {
            _button433.UnBindOnClickButtonEvent(OnClick433);
        }

        if (_button442 != null)
        {
            _button442.UnBindOnClickButtonEvent(OnClick442);
        }

        if (_button451 != null)
        {
            _button451.UnBindOnClickButtonEvent(OnClick451);
        }

        if (_button352 != null)
        {
            _button352.UnBindOnClickButtonEvent(OnClick352);
        }

        if (_button343 != null)
        {
            _button343.UnBindOnClickButtonEvent(OnClick343);
        }
    }

    private void OnClickFormation()
    {
        if (_formationListPanel != null)
        {
            isDropdownOpen = !isDropdownOpen;
            _formationListPanel.SetActive(isDropdownOpen);
        }
    }

    private void OnClick433()
    {
        ChangeFormation("4-3-3");
    }

    private void OnClick442()
    {
        ChangeFormation("4-4-2");
    }

    private void OnClick451()
    {
        ChangeFormation("4-5-1");
    }

    private void OnClick352()
    {
        ChangeFormation("3-5-2");
    }

    private void OnClick343()
    {
        ChangeFormation("3-4-3");
    }

    private void ChangeFormation(string formationName)
    {
        if (currentFormation == formationName)
        {
            CloseDropdownPanel();
            return;
        }

        Debug.Log($"{formationName} 포메이션 선택됨");
        currentFormation = formationName;

        if (_textFormationButton != null)
        {
            _textFormationButton.text = formationName;
        }

        if (_slotGK != null)
        {
            _slotGK.gameObject.SetActive(true);
        }

        if (_buttonSubstitute != null)
        {
            _buttonSubstitute.gameObject.SetActive(true);
        }

        ApplyFormationSlots(formationName);
        SetSlotArrayActive(_subSlots, true);
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

    private void OnClickSubstitute()
    {
        if (_panelSubRectTransform != null)
        {
            Vector2 currentPos = _panelSubRectTransform.anchoredPosition;
            isSubPanelOpen = !isSubPanelOpen;
            currentPos.y = isSubPanelOpen ? 0f : -250f;
            _panelSubRectTransform.anchoredPosition = currentPos;
        }
    }

    private void SaveSquadData()
    {
        string currentUserId = DBManager.Instance.CurrentUserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            return;
        }

        DBManager.Instance.SaveUserFormation(currentUserId, currentFormation);

        if (_slotGK != null)
        {
            int instanceId = (_slotGK.TargetUserCardData != null) ? _slotGK.TargetUserCardData.InstanceId : -1;
            DBManager.Instance.SaveUserSquadSlot(currentUserId, "GK", instanceId);
        }

        if (_slotDFs != null)
        {
            for (int i = 0; i < _slotDFs.Length; i++)
            {
                if (_slotDFs[i] != null)
                {
                    int instanceId = (_slotDFs[i].TargetUserCardData != null) ? _slotDFs[i].TargetUserCardData.InstanceId : -1;
                    DBManager.Instance.SaveUserSquadSlot(currentUserId, $"DF_{i}", instanceId);
                }
            }
        }

        if (_slotMFs != null)
        {
            for (int i = 0; i < _slotMFs.Length; i++)
            {
                if (_slotMFs[i] != null)
                {
                    int instanceId = (_slotMFs[i].TargetUserCardData != null) ? _slotMFs[i].TargetUserCardData.InstanceId : -1;
                    DBManager.Instance.SaveUserSquadSlot(currentUserId, $"MF_{i}", instanceId);
                }
            }
        }

        if (_slotFWs != null)
        {
            for (int i = 0; i < _slotFWs.Length; i++)
            {
                if (_slotFWs[i] != null)
                {
                    int instanceId = (_slotFWs[i].TargetUserCardData != null) ? _slotFWs[i].TargetUserCardData.InstanceId : -1;
                    DBManager.Instance.SaveUserSquadSlot(currentUserId, $"FW_{i}", instanceId);
                }
            }
        }

        if (_subSlots != null)
        {
            for (int i = 0; i < _subSlots.Length; i++)
            {
                if (_subSlots[i] != null)
                {
                    int instanceId = (_subSlots[i].TargetUserCardData != null) ? _subSlots[i].TargetUserCardData.InstanceId : -1;
                    DBManager.Instance.SaveUserSquadSlot(currentUserId, $"Sub_{i}", instanceId);
                }
            }
        }

        Debug.Log("[SquadUi] 스쿼드 데이터 저장 완료!");
    }

    private void LoadSquadData()
    {
        string currentUserId = DBManager.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            return;
        }

        string savedFormation = DBManager.Instance.GetUserFormation(currentUserId);
        
        if (string.IsNullOrEmpty(savedFormation) == false)
        {
            currentFormation = savedFormation;

            if (_textFormationButton != null)
            {
                _textFormationButton.text = currentFormation;
            }

            if (_slotGK != null)
            {
                _slotGK.gameObject.SetActive(true);
            }

            if (_buttonSubstitute != null)
            {
                _buttonSubstitute.gameObject.SetActive(true);
            }

            ApplyFormationSlots(currentFormation);
            SetSlotArrayActive(_subSlots, true);
        }

        else
        {
            currentFormation = string.Empty;
            SetAllSlotsActive(false);

            if (_textFormationButton != null)
            {
                _textFormationButton.text = "포메이션 선택";
            }

            if (_buttonSubstitute != null)
            {
                _buttonSubstitute.gameObject.SetActive(false);
            }

            return;
        }

        Dictionary<string, int> savedSlots = DBManager.Instance.GetUserSquadSlots(currentUserId);
        List<UserCardData> userCards = DBManager.Instance.GetUserCards(currentUserId);

        RestoreSlot(_slotGK, "GK", savedSlots, userCards);

        if (_slotDFs != null)
        {
            for (int i = 0; i < _slotDFs.Length; i++)
            {
                RestoreSlot(_slotDFs[i], $"DF_{i}", savedSlots, userCards);
            }
        }

        if (_slotMFs != null)
        {
            for (int i = 0; i < _slotMFs.Length; i++)
            {
                RestoreSlot(_slotMFs[i], $"MF_{i}", savedSlots, userCards);
            }
        }

        if (_slotFWs != null)
        {
            for (int i = 0; i < _slotFWs.Length; i++)
            {
                RestoreSlot(_slotFWs[i], $"FW_{i}", savedSlots, userCards);
            }
        }

        if (_subSlots != null)
        {
            for (int i = 0; i < _subSlots.Length; i++)
            {
                RestoreSlot(_subSlots[i], $"Sub_{i}", savedSlots, userCards);
            }
        }
    }

    private void RestoreSlot(SquadSlot slot, string dbKey, Dictionary<string, int> savedSlots, List<UserCardData> userCards)
    {
        if (slot == null)
        {
            return;
        }

        if (savedSlots.ContainsKey(dbKey))
        {
            int instanceId = savedSlots[dbKey];

            if (instanceId != -1)
            {
                UserCardData targetCard = null;

                for (int i = 0; i < userCards.Count; i++)
                {
                    if (userCards[i] != null && userCards[i].InstanceId == instanceId)
                    {
                        targetCard = userCards[i];
                        break;
                    }
                }

                if (targetCard != null)
                {
                    slot.SetCardData(targetCard).Forget();
                }

                else
                {
                    slot.ClearSlot();
                }
            }

            else
            {
                slot.ClearSlot();
            }
        }
    }

    private void OnClickClose()
    {
        SaveSquadData();
        UiManager.Instance.CloseUi<SquadUi>();
    }
}