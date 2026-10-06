using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SignUpPopUp : UiBase
{
    [SerializeField] private UiButton button_SignUp;
    [SerializeField] private UiButton button_Exit;

    [SerializeField] private TMP_InputField inputField_Id;
    [SerializeField] private TMP_InputField inputField_Pw;
    [SerializeField] private TMP_InputField inputField_Name;

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

        if (inputField_Name != null)
        {
            inputField_Name.text = "";
        }

        if (button_SignUp)
        {
            button_SignUp.BindOnClickButtonEvent(OnClickSignUp);
        }

        if (button_Exit)
        {
            button_Exit.BindOnClickButtonEvent(OnClickClose);
        }
    }

    private async void OnClickSignUp()
    {
        string id = inputField_Id.text;
        string pw = inputField_Pw.text;
        string name = inputField_Name.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pw) || string.IsNullOrEmpty(name))
        {
            UiManager.Instance.ShowWarning("아이디, 비밀번호, 닉네임을 모두 입력해주세요.").Forget();
            return;
        }

        bool isIdExists = DBManager.Instance.IsUserIdExists(id);
        if (isIdExists)
        {
            UiManager.Instance.ShowWarning("존재하는 ID입니다.").Forget();
            inputField_Id.text = "";
            return;
        }

        bool isSignUpSuccessful = DBManager.Instance.InsertUser(id, pw, name);
        if (isSignUpSuccessful)
        {
            UiManager.Instance.ShowWarning("회원가입이 완료되었습니다.").Forget();
            await UiManager.Instance.OpenUi<SignInUi>();
            UiManager.Instance.CloseUi<SignUpPopUp>();
        }

        else
        {
            UiManager.Instance.ShowWarning("회원가입에 실패했습니다. 다시 시도해주세요.").Forget();
            inputField_Id.text = "";
            inputField_Pw.text = "";
            inputField_Name.text = "";
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<SignUpPopUp>();
    }
}