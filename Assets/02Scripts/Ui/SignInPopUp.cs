using UnityEngine;
using TMPro;

public class SignInPopUp : UiBase
{
    [SerializeField] private UiButton button_LogIn;
    [SerializeField] private UiButton button_SignUp;
    [SerializeField] private UiButton button_Exit;

    [SerializeField] private TMP_InputField inputField_Id;
    [SerializeField] private TMP_InputField inputField_Pw;

    private void OnEnable()
    {
        if (inputField_Id != null)
        {
            inputField_Id.text = "";
        }

        if (inputField_Pw != null)
        {
            inputField_Pw.text = "";
        }

        if (button_LogIn)
        {
            button_LogIn.BindOnClickButtonEvent(AttemptLogin);
        }

        if (button_SignUp)
        {
            button_SignUp.BindOnClickButtonEvent(OpenSignUpUi);
        }
        
        if (button_Exit)
        {
            button_Exit.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private async void AttemptLogin()
    {
        string id = inputField_Id.text;
        string pw = inputField_Pw.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw))
        {
            Debug.LogWarning("아이디와 비밀번호를 모두 입력해주세요.");
            return;
        }

        bool isLoginSuccessful = DBManager.Instance.ValidateUser(id, pw);

        if (isLoginSuccessful)
        {
            Debug.Log("로그인 성공");
            //await UiManager.Instance.OpenUi<MainUi>();
            //UiManager.Instance.CloseUi<SignInPopUp>();
        }

        else
        {
            Debug.LogWarning("로그인 실패");
            inputField_Id.text = "";
            inputField_Pw.text = "";
        }
    }

    private async void OpenSignUpUi()
    {
        await UiManager.Instance.OpenUi<SignUpPopUp>();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SignInPopUp>();
    }
}