using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CardDictionaryUi : UiBase
{
    [SerializeField] private Transform _cardContent;
    private string cardPrefabAddress = "Card";

    [SerializeField] private UiButton _buttonClose;

    private void OnEnable()
    {
        RefreshDictionary().Forget();
        if (_buttonClose)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private async UniTaskVoid RefreshDictionary()
    {
        foreach (Transform child in _cardContent)
        {
            Destroy(child.gameObject);
        }

        List<PlayerData> playerList = GameDataManager.Instance.GetAllData<PlayerData>();

        if (playerList == null || playerList.Count == 0)
        {
            Debug.LogWarning("[CardDictionaryUI] 로드할 PlayerData가 없습니다.");
            return;
        }

        foreach (PlayerData playerData in playerList)
        {
            GameObject cardObj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, _cardContent);

            if (cardObj != null && cardObj.TryGetComponent<Card>(out var card))
            {
                card.SetUpCard(playerData);
            }
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<CardDictionaryUi>();
    }
}