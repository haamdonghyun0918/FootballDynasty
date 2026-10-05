using UnityEngine;

public class SignInUi : UiBase
{
    [SerializeField] private UiButton _buttonSignIn;
    [SerializeField] private UiButton _buttonSignUp;

    private void OnEnable()
    {
        if (_buttonSignIn)
        {
            _buttonSignIn.BindOnClickButtonEvent(OpenMainUi);
        }

        if (_buttonSignUp)
        {
            _buttonSignUp.BindOnClickButtonEvent(OpenSignUpUi);
        }
    }

    private async void OpenMainUi()
    {
        //TODO: MainUi만들어질 시 주석 해제 10/05
        //await UiManager.Instance.OpenUi<MainUi>();
    }

    private async void OpenSignUpUi()
    {
        //TODO: SignUpUi만들어질 시 주석 해제 10/05
        //await UiManager.Instance.OpenUi<SignUpUi>();
    }
}