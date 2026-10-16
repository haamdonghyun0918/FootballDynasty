using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInventoryUi : UiBase, ICardFilterable
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
    [SerializeField] private UiButton _buttonPosition;
    [SerializeField] private UiButton _buttonTeam;

    private bool _filterTotal = true;
    private bool _filterFw = false;
    private bool _filterMf = false;
    private bool _filterDf = false;
    private bool _filterGk = false;

    private string _selectedTeamName = string.Empty;

    private List<UserCardData> _myCardList;
    private List<UserCardData> _filteredCardList = new List<UserCardData>();
    private List<Card> _cardPool = new List<Card>();

    private const int _cardsInPage = 40;
    private int _currentPage = 1;
    private bool _isInitialized = false;

    private int GetTotalPages()
    {
        if (_filteredCardList == null || _filteredCardList.Count == 0)
        {
            return 1;
        }

        return Mathf.CeilToInt((float)_filteredCardList.Count / _cardsInPage);
    }

    private void OnEnable()
    {
        InitializeAndRefresh().Forget();

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

        if (_buttonPosition != null)
        {
            _buttonPosition.BindOnClickButtonEvent(OnClickPosition);
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

        ResetFilterToDefault();
    }

    private void ResetFilterToDefault()
    {
        _filterTotal = true;
        _filterFw = false;
        _filterMf = false;
        _filterDf = false;
        _filterGk = false;
        _selectedTeamName = string.Empty;
    }

    private async void OnClickPosition()
    {
        var filterUi = await UiManager.Instance.OpenUi<PositionFilterUi>();
        if (filterUi != null)
        {
            filterUi.Init(this, _filterTotal, _filterFw, _filterMf, _filterDf, _filterGk);
        }
    }

    private async void OnClickTeam()
    {
        var teamFilterUi = await UiManager.Instance.OpenUi<TeamFilterUi>();
        if (teamFilterUi != null)
        {
            teamFilterUi.Init(this, _selectedTeamName);
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<CardInventoryUi>();
    }

    public void ApplyPositionFilter(bool total, bool fw, bool mf, bool df, bool gk)
    {
        _filterTotal = total;
        _filterFw = fw;
        _filterMf = mf;
        _filterDf = df;
        _filterGk = gk;

        RefreshInventoryList();
    }

    public void ApplyTeamFilter(string teamName)
    {
        _selectedTeamName = teamName;
        RefreshInventoryList();
    }

    private void RefreshInventoryList()
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
            if (player == null) continue;

            bool matchesName = string.IsNullOrEmpty(searchKeyword) || (player.Name != null && player.Name.Contains(searchKeyword));

            bool matchesPosition = false;
            if (_filterTotal)
            {
                matchesPosition = true;
            }
            else
            {
                if (_filterFw && player.Pos == "FW") matchesPosition = true;
                if (_filterMf && player.Pos == "MF") matchesPosition = true;
                if (_filterDf && player.Pos == "DF") matchesPosition = true;
                if (_filterGk && player.Pos == "GK") matchesPosition = true;
            }

            bool matchesTeam = string.IsNullOrEmpty(_selectedTeamName) || player.Team == _selectedTeamName;

            if (matchesName && matchesPosition && matchesTeam)
            {
                _filteredCardList.Add(userCard);
            }
        }

        _currentPage = 1;
        RefreshPage();
    }

    private async UniTaskVoid InitializeAndRefresh()
    {
        string currentUserId = DBManager.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("[CardInventoryUi] 로그인된 유저 ID가 없습니다!");
            _myCardList = new List<UserCardData>();
            _filteredCardList = new List<UserCardData>();
            return;
        }

        _myCardList = DBManager.Instance.GetUserCards(currentUserId);

        if (_myCardList == null || _myCardList.Count == 0)
        {
            Debug.LogWarning("[CardInventoryUi] 보유한 카드가 없습니다.");
            _filteredCardList = new List<UserCardData>();
            return;
        }

        _filteredCardList = new List<UserCardData>(_myCardList);
        _currentPage = 1;

        if (inputField_Name != null)
        {
            inputField_Name.text = string.Empty;
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

        RefreshPage();
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
        RefreshInventoryList();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void RefreshPage()
    {
        if (_filteredCardList == null)
        {
            return;
        }

        int totalPages = GetTotalPages();
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
                    _cardPool[i].gameObject.SetActive(true);
                    _cardPool[i].SetUpCard(_filteredCardList[dataIndex].StaticData);
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

    private void OnClickNext()
    {
        int totalPages = GetTotalPages();

        if (_currentPage < totalPages)
        {
            _currentPage++;
            RefreshPage();
        }
    }

    private void OnClickPrevious()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            RefreshPage();
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
        int totalPages = GetTotalPages();

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
                RefreshPage();
            }
        }

        if (inputField_Page != null)
        {
            inputField_Page.text = _currentPage + " / " + totalPages;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}