using UnityEngine;

public class SignInUi : UiBase
{
    [SerializeField] private UiButton _buttonSignIn;
    [SerializeField] private UiButton _buttonSignUp;

    private void OnEnable()
    {
        if (_buttonSignIn)
        {
            _buttonSignIn.BindOnClickButtonEvent(OpenSignInPopUp);
        }

        if (_buttonSignUp)
        {
            _buttonSignUp.BindOnClickButtonEvent(OpenSignUpUi);
        }
    }

    private async void OpenSignInPopUp()
    {
        await UiManager.Instance.OpenUi<SignInPopUp>();
    }

    private async void OpenSignUpUi()
    {
        await UiManager.Instance.OpenUi<SignUpPopUp>();
    }
}