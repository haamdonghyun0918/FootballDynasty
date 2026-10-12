using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;

public class CardDictionaryUi : UiBase
{
    [SerializeField] private Transform _cardContent;
    private string cardPrefabAddress = "Card";

    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonNext;
    [SerializeField] private UiButton _buttonPrevious;
    [SerializeField] private TMP_InputField inputField_Page;

    private List<PlayerData> _playerList;
    private List<Card> _cardPool = new List<Card>();

    private const int _cardsInPage = 40;
    private int _currentPage = 1;
    private bool _isInitialized = false;

    private int GetTotalPages()
    {
        if (_playerList == null)
        {
            return 1;
        }

        if (_playerList.Count == 0)
        {
            return 1;
        }

        return Mathf.CeilToInt((float)_playerList.Count / _cardsInPage);
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
    }

    private void OnDisable()
    {
        if (inputField_Page != null)
        {
            inputField_Page.onEndEdit.RemoveListener(OnInputPageEndEdit);
        }
    }

    private async UniTaskVoid InitializeAndRefresh()
    {
        _playerList = GameDataManager.Instance.GetAllData<PlayerData>();

        if (_playerList == null)
        {
            Debug.LogWarning("[CardDictionaryUI] 로드할 PlayerData가 없습니다.");
            return;
        }
        if (_playerList.Count == 0)
        {
            Debug.LogWarning("[CardDictionaryUI] 로드할 PlayerData가 없습니다.");
            return;
        }

        _currentPage = 1;

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

    private void RefreshPage()
    {
        if (_playerList == null)
        {
            return;
        }

        if (_playerList.Count == 0)
        {
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
                if (dataIndex < _playerList.Count)
                {
                    _cardPool[i].gameObject.SetActive(true);
                    _cardPool[i].SetUpCard(_playerList[dataIndex]);
                }

                else
                {
                    _cardPool[i].gameObject.SetActive(false);
                }
            }
        }

        if (inputField_Page != null)
        {
            inputField_Page.text = _currentPage + " / " + totalPages;
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
        if (string.IsNullOrEmpty(text) == true)
        {
            RefreshPage();
            return;
        }

        string cleanText = text;
        int slashIndex = text.IndexOf('/');
        
        if (slashIndex >= 0)
        {
            cleanText = text.Substring(0, slashIndex).Trim();
        }

        int page;
        
        if (int.TryParse(cleanText, out page) == true)
        {
            int totalPages = GetTotalPages();
            if (page >= 1)
            {
                if (page <= totalPages)
                {
                    _currentPage = page;
                    RefreshPage();
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
        }

        else
        {
            if (inputField_Page != null)
            {
                int totalPages = GetTotalPages();
                inputField_Page.text = _currentPage + " / " + totalPages;
            }
        }
    }
}