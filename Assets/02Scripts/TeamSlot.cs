using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class TeamSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text_TeamName;
    [SerializeField] private Image image_TeamLogo;
    [SerializeField] private Image image_Check;
    [SerializeField] private Button button_Slot;

    private string _teamName;
    private Action<string> _onClickCallback;

    private void Awake()
    {
        if (button_Slot != null)
        {
            button_Slot.onClick.AddListener(OnClickSlot);
        }
    }

    public void SetUpTeamSlot(string teamName, string selectedTeamName, Action<string> onClickCallback)
    {
        _teamName = teamName;
        _onClickCallback = onClickCallback;

        if (text_TeamName != null)
        {
            text_TeamName.text = _teamName;
        }

        if (image_Check != null)
        {
            bool isSelected = (string.IsNullOrEmpty(_teamName) == false && string.IsNullOrEmpty(selectedTeamName) == false && _teamName.Trim() == selectedTeamName.Trim());
            image_Check.gameObject.SetActive(isSelected);
        }

        LoadTeamLogoAsync(_teamName).Forget();
    }

    private async UniTaskVoid LoadTeamLogoAsync(string teamName)
    {
        if (image_TeamLogo == null)
        {
            return;
        }

        Sprite teamSprite = await ResourceManager.Instance.LoadAsset<Sprite>(teamName);

        if (teamSprite != null)
        {
            image_TeamLogo.sprite = teamSprite;
            image_TeamLogo.gameObject.SetActive(true);
        }

        else
        {
            image_TeamLogo.gameObject.SetActive(false);
            Debug.LogWarning($"팀 로고를 찾을 수 없습니다: {teamName}");
        }
    }

    private void OnClickSlot()
    {
        _onClickCallback?.Invoke(_teamName);
    }
}