using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class SquadSlot : MonoBehaviour
{
    [SerializeField] private UiButton _buttonSlot;
    [SerializeField] private string _slotPosition;

    public string SlotPosition
    {
        get { return _slotPosition; }
    }

    public UserCardData TargetUserCardData { get; private set; }
    public Card SpawnedCard { get; private set; }

    private Action<SquadSlot> _onClickSlotCallback;
    private string cardPrefabAddress = "Card";

    private void OnEnable()
    {
        if (_buttonSlot != null)
        {
            _buttonSlot.BindOnClickButtonEvent(OnClickSlot);
        }
    }

    private void OnDisable()
    {
        if (_buttonSlot != null)
        {
            _buttonSlot.UnBindOnClickButtonEvent(OnClickSlot);
        }
    }

    public void Init(string position, Action<SquadSlot> onClickCallback)
    {
        _slotPosition = position;
        _onClickSlotCallback = onClickCallback;
    }

    private async UniTaskVoid SetCardData(UserCardData userCardData)
    {
        TargetUserCardData = userCardData;

        if (userCardData == null)
        {
            ClearSlot();
            return;
        }

        if (SpawnedCard == null)
        {
            GameObject cardobj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, transform);

            if (cardobj != null)
            {
                SpawnedCard = cardobj.GetComponent<Card>();
            }
        }

        if (SpawnedCard != null)
        {
            SpawnedCard.gameObject.SetActive(true);
            SpawnedCard.SetTargetUserCardData(userCardData);
            SpawnedCard.SetUpCard(userCardData.StaticData);
        }

    }

    private void ClearSlot()
    {
        TargetUserCardData = null;

        if (SpawnedCard != null)
        {
            SpawnedCard.gameObject.SetActive(false);
        }
    }

    private void OnClickSlot()
    {
        if (_onClickSlotCallback != null)
        {
            _onClickSlotCallback.Invoke(this);
        }
    }
}