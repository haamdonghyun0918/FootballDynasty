using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadFilterUi : UiBase, ICardFilterable
{
    [SerializeField] private Transform _cardContent;
    private string cardPrefabAddress = "Card";

    [SerializeField] private UiButton _buttonClose;

    [Header("Pages")]
    [SerializeField] private UiButton _buttonNext;
    [SerializeField] private UiButton _buttonPrevious;
    [SerializeField] private TMP_InputField inputField_Page;

    [Header("Filters")]
    [SerializeField] private TMP_InputField inputField_Name;
    [SerializeField] private UiButton _buttonTeam;

    private string _targetPosition = string.Empty;
    private List<string> _excludedUserCardIds = new List<string>();
    private Action<UserCardData> _onSelectCardCallback;

    private string _selectedTeamName = string.Empty;

    private List<UserCardData> _myCardList;
    private List<UserCardData> _filteredCardList = new List<UserCardData>();
    private List<Card> _cardPool = new List<Card>();

    private const int _cardsInPage = 40;
    private int _currentPage = 1;
    private bool _isInitialized = false;

    private int GetTotalFilterPages()
    {
        if (_filteredCardList == null || _filteredCardList.Count == 0)
        {
            return 1;
        }

        return Mathf.CeilToInt((float)_filteredCardList.Count / _cardsInPage);
    }

    private void OnEnable()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonNext != null)
        {
            _buttonNext.BindOnClickButtonEvent(OnClickNext);
        }

        if (_buttonPrevious != null)
        {
            _buttonPrevious.BindOnClickButtonEvent(OnClickPrevious);
        }

        if (inputField_Page != null)
        {
            inputField_Page.onSelect.AddListener(OnInputPageSelect);
            inputField_Page.onEndEdit.AddListener(OnInputPageEndEdit);
        }

        if (inputField_Name != null)
        {
            inputField_Name.text = string.Empty;
            inputField_Name.onSelect.AddListener(OnNameSelect);
            inputField_Name.onEndEdit.AddListener(OnNameEndEdit);
        }

        if (_buttonTeam != null)
        {
            _buttonTeam.BindOnClickButtonEvent(OnClickTeam);
        }
    }

    private void OnDisable()
    {
        if (inputField_Page != null)
        {
            inputField_Page.onSelect.RemoveListener(OnInputPageSelect);
            inputField_Page.onEndEdit.RemoveListener(OnInputPageEndEdit);
        }

        if (inputField_Name != null)
        {
            inputField_Name.onEndEdit.RemoveListener(OnNameEndEdit);
        }

        _targetPosition = string.Empty;
        _excludedUserCardIds.Clear();
        _onSelectCardCallback = null;
        _selectedTeamName = string.Empty;
    }

    public void Init(string position, List<string> excludedCardIds, Action<UserCardData> onSelectCallback)
    {
        _targetPosition = position;

        if (excludedCardIds != null)
        {
            _excludedUserCardIds = new List<string>(excludedCardIds);
        }
        else
        {
            _excludedUserCardIds.Clear();
        }

        _onSelectCardCallback = onSelectCallback;

        InitializeAndRefresh().Forget();
    }

    private async UniTaskVoid InitializeAndRefresh()
    {
        string currentUserId = DBManager.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("[SquadFilterUi] 로그인된 유저 ID가 없습니다!");
            _myCardList = new List<UserCardData>();
            _filteredCardList = new List<UserCardData>();
            return;
        }

        _myCardList = DBManager.Instance.GetUserCards(currentUserId);

        if (_myCardList == null || _myCardList.Count == 0)
        {
            Debug.LogWarning("[SquadFilterUi] 보유한 카드가 없습니다.");
            _filteredCardList = new List<UserCardData>();
            return;
        }

        if (_isInitialized == false)
        {
            for (int i = 0; i < _cardsInPage; i++)
            {
                GameObject cardObj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, _cardContent);
                if (cardObj != null)
                {
                    Card card;
                    if (cardObj.TryGetComponent<Card>(out card))
                    {
                        _cardPool.Add(card);
                    }
                }
            }
            _isInitialized = true;
        }

        RefreshFilterList();
    }

    private void RefreshFilterList()
    {
        if (_myCardList == null)
        {
            return;
        }

        _filteredCardList.Clear();

        string searchKeyword = string.Empty;

        if (inputField_Name != null)
        {
            searchKeyword = inputField_Name.text.Trim();
        }

        foreach (var userCard in _myCardList)
        {
            PlayerData player = userCard.StaticData;

            if (player == null)
            {
                continue;
            }

            if (_excludedUserCardIds.Contains(userCard.InstanceId.ToString()))
            {
                continue;
            }

            bool matchesPosition = string.IsNullOrEmpty(_targetPosition) || player.Pos == _targetPosition;
            bool matchesName = string.IsNullOrEmpty(searchKeyword) || (player.Name != null && player.Name.Contains(searchKeyword));
            bool matchesTeam = string.IsNullOrEmpty(_selectedTeamName) || player.Team == _selectedTeamName;

            if (matchesPosition && matchesName && matchesTeam)
            {
                _filteredCardList.Add(userCard);
            }
        }

        _currentPage = 1;
        RefreshFilterPage();
    }

    private void RefreshFilterPage()
    {
        if (_filteredCardList == null)
        {
            return;
        }

        int totalPages = GetTotalFilterPages();
        _currentPage = Mathf.Clamp(_currentPage, 1, totalPages);

        if (_filteredCardList.Count == 0)
        {
            for (int i = 0; i < _cardPool.Count; i++)
            {
                _cardPool[i].gameObject.SetActive(false);
            }

            if (inputField_Page != null && inputField_Page.isFocused == false)
            {
                inputField_Page.text = "1 / 1";
            }
            return;
        }

        int startIndex = (_currentPage - 1) * _cardsInPage;

        for (int i = 0; i < _cardsInPage; i++)
        {
            int dataIndex = startIndex + i;

            if (i < _cardPool.Count)
            {
                if (dataIndex < _filteredCardList.Count)
                {
                    UserCardData cardData = _filteredCardList[dataIndex];

                    _cardPool[i].gameObject.SetActive(true);
                    _cardPool[i].SetTargetUserCardData(cardData);
                    _cardPool[i].SetUpCard(cardData.StaticData);
                    _cardPool[i].EnableCardButton(OnSelectCardFromFilter);
                }

                else
                {
                    _cardPool[i].gameObject.SetActive(false);
                }
            }
        }

        if (inputField_Page != null && inputField_Page.isFocused == false)
        {
            inputField_Page.text = _currentPage + " / " + totalPages;
        }
    }

    private void OnSelectCardFromFilter(Card clickedCard)
    {
        if (clickedCard == null) return;

        UserCardData selectedData = clickedCard.TargetUserCardData;
        if (selectedData != null)
        {
            _onSelectCardCallback?.Invoke(selectedData);
        }

        UiManager.Instance.CloseUi<SquadFilterUi>();
    }

    private async void OnClickTeam()
    {
        var teamFilterUi = await UiManager.Instance.OpenUi<TeamFilterUi>();
        if (teamFilterUi != null)
        {
            teamFilterUi.Init(this, _selectedTeamName);
        }
    }

    public void ApplyPositionFilter(bool total, bool fw, bool mf, bool df, bool gk)
    {
    }

    public void ApplyTeamFilter(string teamName)
    {
        _selectedTeamName = teamName;
        RefreshFilterList();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SquadFilterUi>();
    }

    private void OnClickNext()
    {
        int totalPages = GetTotalFilterPages();
        if (_currentPage < totalPages)
        {
            _currentPage++;
            RefreshFilterPage();
        }
    }

    private void OnClickPrevious()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            RefreshFilterPage();
        }
    }

    private void OnInputPageSelect(string text)
    {
        if (inputField_Page != null)
        {
            inputField_Page.text = string.Empty;
        }
    }

    private void OnInputPageEndEdit(string text)
    {
        int totalPages = GetTotalFilterPages();

        if (string.IsNullOrEmpty(text))
        {
            if (inputField_Page != null)
            {
                inputField_Page.text = _currentPage + " / " + totalPages;
            }
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        string cleanText = text;
        int slashIndex = text.IndexOf('/');

        if (slashIndex >= 0)
        {
            cleanText = text.Substring(0, slashIndex).Trim();
        }

        int page;

        if (int.TryParse(cleanText, out page))
        {
            if (page >= 1 && page <= totalPages)
            {
                _currentPage = page;
                RefreshFilterPage();
            }
        }

        if (inputField_Page != null)
        {
            inputField_Page.text = _currentPage + " / " + totalPages;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnNameSelect(string text)
    {
        if (inputField_Name != null)
        {
            inputField_Name.text = string.Empty;
        }
    }

    private void OnNameEndEdit(string keyword)
    {
        RefreshFilterList();
        EventSystem.current.SetSelectedGameObject(null);
    }
}