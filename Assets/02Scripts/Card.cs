using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Card Front, Back")]
    [SerializeField] private GameObject card_Front;
    [SerializeField] private GameObject card_Back;


    [Header("Card Common")]
    [SerializeField] private Image image_Player;
    [SerializeField] private TMP_Text text_Name;
    [SerializeField] private TMP_Text text_Attoverall;
    [SerializeField] private TMP_Text text_Midoverall;
    [SerializeField] private TMP_Text text_Defoverall;
    [SerializeField] private UiButton _buttonCard;
    public Action<Card> OnClickCardAction;

    [Header("Card Front Image")]
    [SerializeField] private TMP_Text text_Season;
    [SerializeField] private Image image_Team;
    [SerializeField] private Image image_Pos;

    [Header("Card Back Skill")]
    [SerializeField] private Image skill_FW;
    [SerializeField] private Image skill_MF;
    [SerializeField] private Image skill_DF;

    public UserCardData TargetUserCardData { get; private set; }

    public void SetTargetUserCardData(UserCardData userCardData)
    {
        TargetUserCardData = userCardData;
    }

    private void OnEnable()
    {
        if (_buttonCard != null)
        {
            _buttonCard.BindOnClickButtonEvent(OnClickCard);
            _buttonCard.SetInteractable(false);
        }
    }

    private void OnDisable()
    {
        OnClickCardAction = null;
        TargetUserCardData = null;
    }

    public void EnableCardButton(Action<Card> clickAction)
    {
        OnClickCardAction = clickAction;

        if (_buttonCard != null)
        {
            _buttonCard.SetInteractable(true);
        }
    }

    public void DisableCardButton()
    {
        OnClickCardAction = null;

        if (_buttonCard != null)
        {
            _buttonCard.SetInteractable(false);
        }
    }

    public void SetCardBase(bool isFront)
    {
        if (card_Front != null)
        {
            card_Front.SetActive(isFront);
        }

        if (card_Back != null)
        {
            card_Back.SetActive(isFront == false);
        }
    }

    public void SetUpCard(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("선수 데이터가 존재하지 않습니다.");
            return;
        }

        SetCardBase(true);

        text_Name.text = playerData.Name;
        text_Season.text = playerData.Season;

        text_Attoverall.text = playerData.Att.ToString();
        text_Midoverall.text = playerData.Mid.ToString();
        text_Defoverall.text = playerData.Def.ToString();

        LoadCardImagesAsync(playerData).Forget();
    }

    private async UniTaskVoid LoadCardImagesAsync(PlayerData playerData)
    {
        if (image_Team != null && (string.IsNullOrEmpty(playerData.Team) == false))
        {
            Sprite teamSprite = await ResourceManager.Instance.LoadAsset<Sprite>(playerData.Team);
            
            if (teamSprite != null)
            {
                image_Team.sprite = teamSprite;
                image_Team.gameObject.SetActive(true);
            }

            else
            {
                Debug.LogWarning($"팀 로고를 찾을 수 없습니다: {playerData.Team}");
            }
        }

        if (image_Pos != null && (string.IsNullOrEmpty(playerData.Pos) == false))
        {
            Sprite posSprite = await ResourceManager.Instance.LoadAsset<Sprite>(playerData.Pos);

            if (posSprite != null)
            {
                image_Pos.sprite = posSprite;
                image_Pos.gameObject.SetActive(true);
            }

            else
            {
                Debug.LogWarning($"포지션 이미지를 찾을 수 없습니다: {playerData.Pos}");
            }
        }
    }

    private void OnClickCard()
    {
        OnClickCardAction?.Invoke(this);
    }
}