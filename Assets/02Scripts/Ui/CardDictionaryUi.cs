using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDictionaryUi : UiBase
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

    private List<PlayerData> _playerList;
    private List<PlayerData> _filteredPlayerList = new List<PlayerData>();
    private List<Card> _cardPool = new List<Card>();

    private const int _cardsInPage = 40;
    private int _currentPage = 1;
    private bool _isInitialized = false;

    private int GetTotalPages()
    {
        if (_filteredPlayerList == null)
        {
            return 1;
        }

        if (_filteredPlayerList.Count == 0)
        {
            return 1;
        }

        return Mathf.CeilToInt((float)_filteredPlayerList.Count / _cardsInPage);
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
    }

    private async void OnClickPosition()
    {
        await UiManager.Instance.OpenUi<PositionFilterUi>();
    }

    private async UniTaskVoid InitializeAndRefresh()
    {
        _playerList = GameDataManager.Instance.GetAllData<PlayerData>();

        if (_playerList == null || _playerList.Count == 0)
        {
            Debug.LogWarning("[CardDictionaryUI] 로드할 PlayerData가 없습니다.");
            _filteredPlayerList = new List<PlayerData>();
            return;
        }

        _filteredPlayerList = new List<PlayerData>(_playerList);
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
        if (_playerList == null)
        {
            return;
        }

        _filteredPlayerList.Clear();

        string searchKeyword = keyword;
        if (string.IsNullOrEmpty(searchKeyword) && inputField_Name != null)
        {
            searchKeyword = inputField_Name.text;
        }

        if (string.IsNullOrEmpty(searchKeyword) == false)
        {
            searchKeyword = searchKeyword.Trim();
        }

        if (string.IsNullOrEmpty(searchKeyword))
        {
            _filteredPlayerList = new List<PlayerData>(_playerList);
        }

        else
        {
            foreach (var player in _playerList)
            {
                if (player.Name != null && player.Name.Contains(searchKeyword))
                {
                    _filteredPlayerList.Add(player);
                }
            }
        }

        _currentPage = 1;
        RefreshPage();
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void RefreshPage()
    {
        if (_filteredPlayerList == null)
        {
            return;
        }

        if (_filteredPlayerList.Count == 0)
        {
            for (int i = 0; i < _cardPool.Count; i++)
            {
                _cardPool[i].gameObject.SetActive(false);
            }

            if (inputField_Page != null)
            {
                if (inputField_Page.isFocused == false)
                {
                    inputField_Page.text = "1 / 1";
                }
            }
            return;
        }

        int totalPages = GetTotalPages();
        _currentPage = Mathf.Clamp(_currentPage, 1, totalPages);

        int startIndex = (_currentPage - 1) * _cardsInPage;

        for (int i = 0; i < _cardsInPage; i++)
        {
            int dataIndex = startIndex + i;

            if (i < _cardPool.Count)
            {
                if (dataIndex < _filteredPlayerList.Count)
                {
                    _cardPool[i].gameObject.SetActive(true);
                    _cardPool[i].SetUpCard(_filteredPlayerList[dataIndex]);
                }
                else
                {
                    _cardPool[i].gameObject.SetActive(false);
                }
            }
        }

        if (inputField_Page != null)
        {
            if (inputField_Page.isFocused == false)
            {
                inputField_Page.text = _currentPage + " / " + totalPages;
            }
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<CardDictionaryUi>();
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
                if (inputField_Page != null)
                {
                    inputField_Page.text = _currentPage + " / " + totalPages;
                }
            }

            else
            {
                if (inputField_Page != null)
                {
                    inputField_Page.text = _currentPage + " / " + totalPages;
                }
            }
        }

        else
        {
            if (inputField_Page != null)
            {
                inputField_Page.text = _currentPage + " / " + totalPages;
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}