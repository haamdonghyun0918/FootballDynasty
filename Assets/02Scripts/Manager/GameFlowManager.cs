using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    private async void Start()
    {
        await UiManager.Instance.OpenUi<SignInUi>();
    }
}