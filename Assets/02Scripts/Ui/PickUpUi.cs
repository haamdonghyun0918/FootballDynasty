using UnityEngine;

public class PickUpUi : UiBase
{
    [SerializeField] private UiButton button_Close;
    [SerializeField] private UiButton button_Once;
    [SerializeField] private UiButton button_Ten;

    private void OnEnable()
    {
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

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<PickUpUi>();
    }

    private void OnClickOnce()
    {
        var allPlayers = GameDataManager.Instance.GetAllData<PlayerData>();
        if (allPlayers == null || allPlayers.Count == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, allPlayers.Count);
        string randomPlayerId = allPlayers[randomIndex].Id;


    }

    private void OnClickTen()
    {

    }
}