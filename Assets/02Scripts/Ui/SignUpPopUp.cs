using UnityEngine;
using TMPro;

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
            Debug.LogWarning("아이디와 비밀번호와 닉네임을 모두 입력해주세요.");
            return;
        }

        bool isSignUpSuccessful = DBManager.Instance.InsertUser(id, pw, name);

        if (isSignUpSuccessful)
        {
            Debug.Log("회원가입 성공");
            await UiManager.Instance.OpenUi<SignInUi>();
            UiManager.Instance.CloseUi<SignUpPopUp>();
        }

        else
        {
            Debug.LogWarning("회원가입 실패");
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