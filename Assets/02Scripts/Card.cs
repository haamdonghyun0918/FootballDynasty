using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    private bool _isFront = true;
    private bool _isFlipping = false;

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

    // 카드 도감에 있는 카드들을 위하여 사용
    public void SetUpCard(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("선수 데이터가 존재하지 않습니다.");
            return;
        }

        _isFront = true;
        _isFlipping = false;
        SetCardBase(true);

        text_Name.text = playerData.Name;
        text_Season.text = playerData.Season;
        text_Attoverall.text = playerData.Att.ToString();
        text_Midoverall.text = playerData.Mid.ToString();
        text_Defoverall.text = playerData.Def.ToString();

        if (image_Player != null)
        {
            image_Player.sprite = null;
        }

        LoadCardImagesAsync(playerData).Forget();
    }

    // 실제 내가 가진 선수들에 대하여 사용
    public void SetUpCard(UserCardData userCardData)
    {
        if (userCardData == null || userCardData.StaticData == null)
        {
            Debug.LogError("유저 카드 데이터가 존재하지 않습니다.");
            return;
        }

        TargetUserCardData = userCardData;

        SetUpCard(userCardData.StaticData);

        LoadSkillImagesAsync(userCardData).Forget();
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

        if (image_Player != null && (string.IsNullOrEmpty(playerData.ImageAddress) == false))
        {
            Sprite playerSprite = await ResourceManager.Instance.LoadAsset<Sprite>(playerData.ImageAddress);

            if (playerSprite != null)
            {
                image_Player.sprite = playerSprite;
                image_Player.gameObject.SetActive(true);
            }

            else
            {
                Debug.Log($"포지션 이미지를 찾을 수 없습니다: {playerData.Pos}");
            }
        }
    }

    private async UniTaskVoid LoadSkillImagesAsync(UserCardData userCardData)
    {
        if (skill_FW != null && userCardData.SkillFwData != null)
        {
            Sprite fwSprite = await ResourceManager.Instance.LoadAsset<Sprite>(userCardData.SkillFwData.ImageAddress);
            if (fwSprite != null)
            {
                skill_FW.sprite = fwSprite;
                skill_FW.gameObject.SetActive(true);
            }
        }

        if (skill_MF != null && userCardData.SkillMfData != null)
        {
            Sprite mfSprite = await ResourceManager.Instance.LoadAsset<Sprite>(userCardData.SkillMfData.ImageAddress);
            if (mfSprite != null)
            {
                skill_MF.sprite = mfSprite;
                skill_MF.gameObject.SetActive(true);
            }
        }

        if (skill_DF != null && userCardData.SkillDfData != null)
        {
            Sprite dfSprite = await ResourceManager.Instance.LoadAsset<Sprite>(userCardData.SkillDfData.ImageAddress);
            if (dfSprite != null)
            {
                skill_DF.sprite = dfSprite;
                skill_DF.gameObject.SetActive(true);
            }
        }
    }

    public async UniTaskVoid FlipCard()
    {
        if (_isFlipping)
        {
            return;
        }
        _isFlipping = true;

        float duration = 0.2f;
        await transform.DOScaleX(0f, duration / 2f).AsyncWaitForCompletion();

        _isFront = _isFront == false;
        SetCardBase(_isFront);

        await transform.DOScaleX(1f, duration / 2f).AsyncWaitForCompletion();

        _isFlipping = false;
    }

    private void OnClickCard()
    {
        OnClickCardAction?.Invoke(this);
    }
}