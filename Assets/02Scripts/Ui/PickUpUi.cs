using UnityEngine;
using System.Collections.Generic;

public class PickUpUi : UiBase
{
    [SerializeField] private UiButton button_Close;
    [SerializeField] private UiButton button_Once;
    [SerializeField] private UiButton button_Ten;
    private List<PlayerData> _playerList;

    private void OnEnable()
    {
        LoadPlayer();

        if (button_Close)
        {
            button_Close.BindOnClickButtonEvent(OnClickClose);
        }

        if (button_Once)
        {
            button_Once.BindOnClickButtonEvent(OnClickOnce);
        }

        if ( button_Ten)
        {
            button_Ten.BindOnClickButtonEvent(OnClickTen);
        }
    }

    private void LoadPlayer()
    {
        _playerList = GameDataManager.Instance.GetAllData<PlayerData>();
        if (_playerList == null || _playerList.Count == 0)
        {
            Debug.LogError("선수 데이터가 존재하지 않습니다.");
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<PickUpUi>();
    }

    private void OnClickOnce()
    {
        PickUpCard(1);
    }

    private void OnClickTen()
    {
        PickUpCard(10);
    }

    private void PickUpCard(int count)
    {
        string currentUserId = DBManager.Instance.CurrentUserId;

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, _playerList.Count);
            string randomPlayerId = _playerList[randomIndex].Id;

            DBManager.Instance.InsertUserCard(currentUserId, randomPlayerId);
        }

        Debug.Log($"[PickUpUi] {count}연차 뽑기 완료!");
    }
}