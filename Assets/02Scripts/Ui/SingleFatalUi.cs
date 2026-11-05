using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleFatalUi : UiBase
{
    [Header("Panels")]
    [SerializeField] private GameObject panel_Match;
    [SerializeField] private GameObject panel_Sub;
    [SerializeField] private GameObject panel_Result;
    [SerializeField] private GameObject panel_End;

    [Header("Panel Match - Info")]
    [SerializeField] private TextMeshProUGUI text_Formation;
    [SerializeField] private TextMeshProUGUI text_Turn;
    [SerializeField] private TextMeshProUGUI text_SubPossible;
    [SerializeField] private TextMeshProUGUI text_SubCompleted;
    [SerializeField] private UiButton button_Change;
    [SerializeField] private UiButton button_Substitute;

    [Header("Panel Sub - Controls")]
    [SerializeField] private TextMeshProUGUI text_Second;
    [SerializeField] private UiButton button_SubConfirm;
    [SerializeField] private UiButton button_SubCancel;

    [Header("Card Info Panel")]
    [SerializeField] private GameObject panel_CardInfo;
    [SerializeField] private UiButton button_CardInfoClose;
    [SerializeField] private Transform transform_LocateCard;
    [SerializeField] private TextMeshProUGUI text_OverallAtt;
    [SerializeField] private TextMeshProUGUI text_OverallMid;
    [SerializeField] private TextMeshProUGUI text_OverallDef;
    [SerializeField] private Image img_SkillAtt;
    [SerializeField] private TextMeshProUGUI text_SkillAttDesc;
    [SerializeField] private Image img_SkillMid;
    [SerializeField] private TextMeshProUGUI text_SkillMidDesc;
    [SerializeField] private Image img_SkillDef;
    [SerializeField] private TextMeshProUGUI text_SkillDefDesc;
    [SerializeField] private UiButton btn_SelectAtt;
    [SerializeField] private UiButton btn_SelectMid;
    [SerializeField] private UiButton btn_SelectDef;

    [Header("Panel Match - ScorePanel")]
    [SerializeField] private GameObject home_Attack;
    [SerializeField] private GameObject away_Attack;
    [SerializeField] private Image logo_Home;
    [SerializeField] private TextMeshProUGUI name_Home;
    [SerializeField] private TextMeshProUGUI point_Home;
    [SerializeField] private Image logo_Away;
    [SerializeField] private TextMeshProUGUI name_Away;
    [SerializeField] private TextMeshProUGUI point_Away;

    [Header("Panel Match - Ground Slots")]
    [SerializeField] private SquadSlot slot_GK;
    [SerializeField] private SquadSlot[] slot_DFs;
    [SerializeField] private SquadSlot[] slot_MFs;
    [SerializeField] private SquadSlot[] slot_FWs;

    [Header("Panel Match - Substitute Slots")]
    [SerializeField] private SquadSlot[] slot_Subs;

    [Header("Panel Result - UI Elements")]
    [SerializeField] private Transform card_Home_Point;
    [SerializeField] private TextMeshProUGUI text_Skill_Home;
    [SerializeField] private TextMeshProUGUI text_Overall_Home;
    [SerializeField] private TextMeshProUGUI text_Plus_Home;
    [SerializeField] private TextMeshProUGUI text_Result_Home;

    [SerializeField] private Transform card_Away_Point;
    [SerializeField] private TextMeshProUGUI text_Skill_Away;
    [SerializeField] private TextMeshProUGUI text_Overall_Away;
    [SerializeField] private TextMeshProUGUI text_Plus_Away;
    [SerializeField] private TextMeshProUGUI text_Result_Away;

    [SerializeField] private TextMeshProUGUI text_Time_Result;

    [Header("Opposite Choosen UI (Defense/Reaction Turn)")]
    [SerializeField] private GameObject object_OppositeChoosen;
    [SerializeField] private TextMeshProUGUI text_ChoosenSlotPos;
    [SerializeField] private Image image_ChoosenOverallBg;
    [SerializeField] private TextMeshProUGUI text_ChoosenOverallName;

    [Header("Panel End - Final Result UI")]
    [SerializeField] private Image logo_Home_End;
    [SerializeField] private TextMeshProUGUI name_Home_End;
    [SerializeField] private TextMeshProUGUI score_Home_End;
    [SerializeField] private TextMeshProUGUI result_Home_End;

    [SerializeField] private Image logo_Away_End;
    [SerializeField] private TextMeshProUGUI name_Away_End;
    [SerializeField] private TextMeshProUGUI score_Away_End;
    [SerializeField] private TextMeshProUGUI result_Away_End;

    [SerializeField] private TextMeshProUGUI text_Time_End;
    [SerializeField] private UiButton button_Check;

    private int currentTurn = 1;
    private int subPossibleCount = 5;
    private int subCompletedCount = 0;
    private bool isViewingOpponent = false;
    private List<OpponentPlayerCard> currentAiStartingList = new List<OpponentPlayerCard>();
    private List<OpponentPlayerCard> currentAiBenchList = new List<OpponentPlayerCard>();
    private List<bool> aiStartingSettledList = new List<bool>();
    private List<bool> aiBenchSettledList = new List<bool>();
    private Dictionary<string, bool> aiPlayerSettledDict = new Dictionary<string, bool>();

    private SquadSlot selectedSubSourceSlot = null;
    private int currentSubCountThisSession = 0;
    private float subRemainingTime = 30f;

    private UserCardData mySelectedCardData = null;
    private int mySelectedStatType = 0;
    private float resultTimer = 5f;
    private float endTimer = 30f;

    private void OnEnable()
    {
        ShowMatchPanel();
        InitMatchData();
        BindMatchButtons();
    }

    private void OnDisable()
    {
        UnbindMatchButtons();
    }

    private void ShowMatchPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(true);
        }

        if (panel_Sub != null)
        {
            panel_Sub.SetActive(false);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_End != null)
        {
            panel_End.SetActive(false);
        }

        if (panel_CardInfo != null)
        {
            panel_CardInfo.SetActive(false);
        }

        if (object_OppositeChoosen != null)
        {
            object_OppositeChoosen.SetActive(false);
        }
    }

    private void BindMatchButtons()
    {
        if (button_Change != null)
        {
            button_Change.BindOnClickButtonEvent(OnClickChange);
        }

        if (button_Substitute != null)
        {
            button_Substitute.BindOnClickButtonEvent(OnClickSubstitute);
        }

        if (button_CardInfoClose != null)
        {
            button_CardInfoClose.BindOnClickButtonEvent(OnClickCardInfoClose);
        }

        if (btn_SelectAtt != null)
        {
            btn_SelectAtt.BindOnClickButtonEvent(OnClickSelectAtt);
        }

        if (btn_SelectMid != null)
        {
            btn_SelectMid.BindOnClickButtonEvent(OnClickSelectMid);
        }

        if (btn_SelectDef != null)
        {
            btn_SelectDef.BindOnClickButtonEvent(OnClickSelectDef);
        }

        if (button_SubConfirm != null)
        {
            button_SubConfirm.BindOnClickButtonEvent(OnClickSubConfirm);
        }

        if (button_SubCancel != null)
        {
            button_SubCancel.BindOnClickButtonEvent(OnClickSubCancel);
        }

        if (button_Check != null)
        {
            button_Check.BindOnClickButtonEvent(OnClickButtonCheck);
        }
    }

    private void UnbindMatchButtons()
    {
        if (button_Change != null)
        {
            button_Change.UnBindOnClickButtonEvent(OnClickChange);
        }

        if (button_Substitute != null)
        {
            button_Substitute.UnBindOnClickButtonEvent(OnClickSubstitute);
        }

        if (button_CardInfoClose != null)
        {
            button_CardInfoClose.UnBindOnClickButtonEvent(OnClickCardInfoClose);
        }

        if (btn_SelectAtt != null)
        {
            btn_SelectAtt.UnBindOnClickButtonEvent(OnClickSelectAtt);
        }

        if (btn_SelectMid != null)
        {
            btn_SelectMid.UnBindOnClickButtonEvent(OnClickSelectMid);
        }

        if (btn_SelectDef != null)
        {
            btn_SelectDef.UnBindOnClickButtonEvent(OnClickSelectDef);
        }

        if (button_SubConfirm != null)
        {
            button_SubConfirm.UnBindOnClickButtonEvent(OnClickSubConfirm);
        }

        if (button_SubCancel != null)
        {
            button_SubCancel.UnBindOnClickButtonEvent(OnClickSubCancel);
        }

        if (button_Check != null)
        {
            button_Check.UnBindOnClickButtonEvent(OnClickButtonCheck);
        }
    }

    private void InitMatchData()
    {
        currentTurn = 1;

        if (text_Turn != null)
        {
            text_Turn.text = "현재 턴: " + currentTurn;
        }

        subPossibleCount = 5;
        subCompletedCount = 0;

        if (text_SubPossible != null)
        {
            text_SubPossible.text = "교체 가능한 인원 수: " + subPossibleCount;
        }

        if (text_SubCompleted != null)
        {
            text_SubCompleted.text = "교체한 인원 수: " + subCompletedCount;
        }

        if (button_Substitute != null)
        {
            button_Substitute.SetInteractable(false);
        }

        if (home_Attack != null)
        {
            home_Attack.SetActive(true);
        }

        if (away_Attack != null)
        {
            away_Attack.SetActive(false);
        }

        if (point_Home != null)
        {
            point_Home.text = "0";
        }

        if (point_Away != null)
        {
            point_Away.text = "0";
        }

        isViewingOpponent = false;

        string userId = DBManager.Instance.CurrentUserId;
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData == null)
        {
            seasonData = DBManager.Instance.GetSingleLeagueSeason(userId);
            if (seasonData != null)
            {
                SingleModeManager.Instance.CurrentSeasonData = seasonData;
            }
        }

        string playerTeamName = DBManager.Instance.CurrentUserName;
        if (string.IsNullOrEmpty(playerTeamName))
        {
            playerTeamName = "내 팀";
        }

        string opponentTeamName = string.Empty;

        if (seasonData != null)
        {
            string teamsCsv = seasonData.selectedTeamsCsv;
            if (string.IsNullOrEmpty(teamsCsv) == false)
            {
                string[] teams = teamsCsv.Split(',');
                int roundIndex = seasonData.currentRound;

                if (roundIndex >= 0)
                {
                    if (roundIndex < teams.Length)
                    {
                        opponentTeamName = teams[roundIndex];
                    }
                }
            }
        }

        string homeTeamName = string.Empty;
        string awayTeamName = string.Empty;

        int currentRound = 0;

        if (seasonData != null)
        {
            currentRound = seasonData.currentRound;
        }

        if (currentRound % 2 == 0)
        {
            homeTeamName = playerTeamName;
            awayTeamName = opponentTeamName;
        }

        else
        {
            homeTeamName = opponentTeamName;
            awayTeamName = playerTeamName;
        }

        if (name_Home != null)
        {
            name_Home.text = homeTeamName;
        }

        if (name_Away != null)
        {
            name_Away.text = awayTeamName;
        }

        LoadMatchLogosAsync(homeTeamName, awayTeamName).Forget();
        LoadAndApplyUserSquad(userId);
    }

    private async UniTaskVoid LoadMatchLogosAsync(string homeName, string awayName)
    {
        if (logo_Home != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            logo_Home.sprite = homeSprite;
        }

        if (logo_Away != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            logo_Away.sprite = awaySprite;
        }
    }

    private void LoadAndApplyUserSquad(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        string savedFormation = DBManager.Instance.GetUserFormation(userId);
        if (text_Formation != null)
        {
            if (string.IsNullOrEmpty(savedFormation) == false)
            {
                text_Formation.text = savedFormation;
            }

            else
            {
                text_Formation.text = "포메이션 선택";
                return;
            }
        }

        ApplyMatchFormationSlots(savedFormation);

        Dictionary<string, int> savedSlots = DBManager.Instance.GetUserSquadSlots(userId);
        List<UserCardData> userCards = DBManager.Instance.GetUserCards(userId);

        RestoreMatchSlot(slot_GK, "GK", savedSlots, userCards);

        if (slot_DFs != null)
        {
            for (int i = 0; i < slot_DFs.Length; i++)
            {
                RestoreMatchSlot(slot_DFs[i], "DF_" + i, savedSlots, userCards);
            }
        }

        if (slot_MFs != null)
        {
            for (int i = 0; i < slot_MFs.Length; i++)
            {
                RestoreMatchSlot(slot_MFs[i], "MF_" + i, savedSlots, userCards);
            }
        }

        if (slot_FWs != null)
        {
            for (int i = 0; i < slot_FWs.Length; i++)
            {
                RestoreMatchSlot(slot_FWs[i], "FW_" + i, savedSlots, userCards);
            }
        }

        if (slot_Subs != null)
        {
            SetSlotArrayActive(slot_Subs, true);
            for (int i = 0; i < slot_Subs.Length; i++)
            {
                RestoreMatchSlot(slot_Subs[i], "Sub_" + i, savedSlots, userCards);
            }
        }
    }

    private void ApplyMatchFormationSlots(string formationName)
    {
        SetSlotArrayActive(slot_DFs, false);
        SetSlotArrayActive(slot_MFs, false);
        SetSlotArrayActive(slot_FWs, false);

        if (slot_GK != null)
        {
            slot_GK.gameObject.SetActive(true);
        }

        if (formationName == "4-3-3")
        {
            SetSelectedSlotsActive(slot_DFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(slot_MFs, new int[] { 0, 2, 4 });
            SetSelectedSlotsActive(slot_FWs, new int[] { 0, 2, 4 });
        }

        else if (formationName == "4-4-2")
        {
            SetSelectedSlotsActive(slot_DFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(slot_MFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(slot_FWs, new int[] { 1, 3 });
        }

        else if (formationName == "4-5-1")
        {
            SetSelectedSlotsActive(slot_DFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(slot_MFs, new int[] { 0, 1, 2, 3, 4 });
            SetSelectedSlotsActive(slot_FWs, new int[] { 2 });
        }

        else if (formationName == "3-5-2")
        {
            SetSelectedSlotsActive(slot_DFs, new int[] { 1, 2, 3 });
            SetSelectedSlotsActive(slot_MFs, new int[] { 0, 1, 2, 3, 4 });
            SetSelectedSlotsActive(slot_FWs, new int[] { 1, 3 });
        }

        else if (formationName == "3-4-3")
        {
            SetSelectedSlotsActive(slot_DFs, new int[] { 1, 2, 3 });
            SetSelectedSlotsActive(slot_MFs, new int[] { 0, 1, 3, 4 });
            SetSelectedSlotsActive(slot_FWs, new int[] { 0, 2, 4 });
        }
    }

    private void SetSlotArrayActive(SquadSlot[] slots, bool isActive)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].gameObject.SetActive(isActive);
            }
        }
    }

    private void SetSelectedSlotsActive(SquadSlot[] slots, int[] activeIndices)
    {
        if (slots == null)
        {
            return;
        }

        if (activeIndices == null)
        {
            return;
        }

        for (int i = 0; i < activeIndices.Length; i++)
        {
            int targetIndex = activeIndices[i];
            if (targetIndex >= 0)
            {
                if (targetIndex < slots.Length)
                {
                    if (slots[targetIndex] != null)
                    {
                        slots[targetIndex].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void RestoreMatchSlot(SquadSlot slot, string dbKey, Dictionary<string, int> savedSlots, List<UserCardData> userCards)
    {
        if (slot == null)
        {
            return;
        }

        slot.Init(dbKey, OnClickMatchSquadSlot);

        if (savedSlots.ContainsKey(dbKey))
        {
            int instanceId = savedSlots[dbKey];

            if (instanceId != -1)
            {
                UserCardData targetCard = null;

                for (int i = 0; i < userCards.Count; i++)
                {
                    if (userCards[i] != null)
                    {
                        if (userCards[i].InstanceId == instanceId)
                        {
                            targetCard = userCards[i];
                            break;
                        }
                    }
                }

                if (targetCard != null)
                {
                    slot.SetCardData(targetCard).Forget();
                }

                else
                {
                    slot.ClearSlot();
                }
            }

            else
            {
                slot.ClearSlot();
            }
        }

        else
        {
            slot.ClearSlot();
        }
    }

    private void OnClickMatchSquadSlot(SquadSlot clickedSlot)
    {
        if (clickedSlot == null)
        {
            return;
        }

        if (panel_Sub != null)
        {
            if (panel_Sub.activeSelf == true)
            {
                HandleSubSlotSelection(clickedSlot);
                return;
            }
        }

        if (clickedSlot.TargetUserCardData != null)
        {
            mySelectedCardData = clickedSlot.TargetUserCardData;
            OpenCardInfo(mySelectedCardData);
        }
    }

    private void HandleSubSlotSelection(SquadSlot clickedSlot)
    {
        if (selectedSubSourceSlot == null)
        {
            selectedSubSourceSlot = clickedSlot;
        }

        else
        {
            if (selectedSubSourceSlot == clickedSlot)
            {
                selectedSubSourceSlot = null;
                return;
            }

            UserCardData sourceData = selectedSubSourceSlot.TargetUserCardData;
            UserCardData targetData = clickedSlot.TargetUserCardData;

            if (sourceData != null)
            {
                clickedSlot.SetCardData(sourceData).Forget();
            }

            else
            {
                clickedSlot.ClearSlot();
            }

            if (targetData != null)
            {
                selectedSubSourceSlot.SetCardData(targetData).Forget();
            }

            else
            {
                selectedSubSourceSlot.ClearSlot();
            }

            currentSubCountThisSession = currentSubCountThisSession + 1;
            selectedSubSourceSlot = null;
        }
    }

    public void ProgressTurn()
    {
        currentTurn = currentTurn + 1;

        // 예시로 최대 턴을 9턴으로 가정 (9턴이 끝나면 Panel_End 호출)
        if (currentTurn > 9)
        {
            ShowEndPanel();
            return;
        }

        if (text_Turn != null)
        {
            text_Turn.text = "현재 턴: " + currentTurn;
        }

        if (button_Substitute != null)
        {
            if (currentTurn == 3)
            {
                button_Substitute.SetInteractable(true);
            }

            else if (currentTurn == 6)
            {
                button_Substitute.SetInteractable(true);
            }

            else if (currentTurn == 9)
            {
                button_Substitute.SetInteractable(true);
            }

            else
            {
                button_Substitute.SetInteractable(false);
            }
        }
    }

    private void OnClickSubstitute()
    {
        if (currentTurn == 3)
        {
            ToggleSubPanel();
        }

        else if (currentTurn == 6)
        {
            ToggleSubPanel();
        }

        else if (currentTurn == 9)
        {
            ToggleSubPanel();
        }

        else
        {
            Debug.LogWarning("[SingleFatalUi] 교체 가능한 타이밍이 아닙니다.");
        }
    }

    private void ToggleSubPanel()
    {
        if (panel_Sub != null)
        {
            bool isOpen = panel_Sub.activeSelf;
            if (isOpen == false)
            {
                currentSubCountThisSession = 0;
                selectedSubSourceSlot = null;
                panel_Sub.SetActive(true);
                RunSubTimerAsync().Forget();
            }

            else
            {
                panel_Sub.SetActive(false);
            }
        }
    }

    private async UniTaskVoid RunSubTimerAsync()
    {
        subRemainingTime = 30f;

        while (subRemainingTime > 0f)
        {
            if (panel_Sub == null)
            {
                break;
            }

            if (panel_Sub.activeSelf == false)
            {
                break;
            }

            if (text_Second != null)
            {
                text_Second.text = Mathf.CeilToInt(subRemainingTime).ToString();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            subRemainingTime = subRemainingTime - 1f;
        }

        if (panel_Sub != null)
        {
            if (panel_Sub.activeSelf == true)
            {
                OnClickSubCancel();
            }
        }
    }

    private void OnClickSubConfirm()
    {
        if (currentSubCountThisSession > 0)
        {
            subPossibleCount = subPossibleCount - currentSubCountThisSession;
            subCompletedCount = subCompletedCount + currentSubCountThisSession;

            if (text_SubPossible != null)
            {
                text_SubPossible.text = "교체 가능한 인원 수: " + subPossibleCount;
            }

            if (text_SubCompleted != null)
            {
                text_SubCompleted.text = "교체한 인원 수: " + subCompletedCount;
            }

            ProgressTurn();
        }

        CloseSubPanel();
    }

    private void OnClickSubCancel()
    {
        string userId = DBManager.Instance.CurrentUserId;
        if (string.IsNullOrEmpty(userId) == false)
        {
            LoadAndApplyUserSquad(userId);
        }

        CloseSubPanel();
    }

    private void CloseSubPanel()
    {
        currentSubCountThisSession = 0;
        selectedSubSourceSlot = null;

        if (panel_Sub != null)
        {
            panel_Sub.SetActive(false);
        }
    }

    private void OnClickChange()
    {
        isViewingOpponent = isViewingOpponent == false;

        if (isViewingOpponent)
        {
            ShowOpponentSquadView();
        }

        else
        {
            string userId = DBManager.Instance.CurrentUserId;
            LoadAndApplyUserSquad(userId);
        }
    }

    public void OpenCardInfo(UserCardData selectedCardData)
    {
        if (panel_CardInfo != null)
        {
            panel_CardInfo.SetActive(true);
        }

        if (selectedCardData == null)
        {
            return;
        }

        PlayerData playerData = GameDataManager.Instance.GetData<PlayerData>(selectedCardData.PlayerId);
        if (playerData != null)
        {
            if (text_OverallAtt != null)
            {
                text_OverallAtt.text = playerData.Att.ToString();
            }

            if (text_OverallMid != null)
            {
                text_OverallMid.text = playerData.Mid.ToString();
            }

            if (text_OverallDef != null)
            {
                text_OverallDef.text = playerData.Def.ToString();
            }
        }

        LoadCardSkillInfoAsync(selectedCardData).Forget();
    }

    private async UniTaskVoid LoadCardSkillInfoAsync(UserCardData cardData)
    {
        if (cardData.SkillFwData != null && text_SkillAttDesc != null)
        {
            text_SkillAttDesc.text = cardData.SkillFwData.Description;
        }

        if (cardData.SkillMfData != null && text_SkillMidDesc != null)
        {
            text_SkillMidDesc.text = cardData.SkillMfData.Description;
        }

        if (cardData.SkillDfData != null && text_SkillDefDesc != null)
        {
            text_SkillDefDesc.text = cardData.SkillDfData.Description;
        }

        if (cardData.SkillFwData != null && img_SkillAtt != null && string.IsNullOrEmpty(cardData.SkillFwData.ImageAddress) == false)
        {
            img_SkillAtt.sprite = await ResourceManager.Instance.LoadAsset<Sprite>(cardData.SkillFwData.ImageAddress);
        }

        if (cardData.SkillMfData != null && img_SkillMid != null && string.IsNullOrEmpty(cardData.SkillMfData.ImageAddress) == false)
        {
            img_SkillMid.sprite = await ResourceManager.Instance.LoadAsset<Sprite>(cardData.SkillMfData.ImageAddress);
        }

        if (cardData.SkillDfData != null && img_SkillDef != null && string.IsNullOrEmpty(cardData.SkillDfData.ImageAddress) == false)
        {
            img_SkillDef.sprite = await ResourceManager.Instance.LoadAsset<Sprite>(cardData.SkillDfData.ImageAddress);
        }
    }

    private void OnClickCardInfoClose()
    {
        ClearCardInfoData();

        if (panel_CardInfo != null)
        {
            panel_CardInfo.SetActive(false);
        }
    }

    private void ClearCardInfoData()
    {
        if (text_OverallAtt != null)
        {
            text_OverallAtt.text = string.Empty;
        }

        if (text_OverallMid != null)
        {
            text_OverallMid.text = string.Empty;
        }

        if (text_OverallDef != null)
        {
            text_OverallDef.text = string.Empty;
        }

        if (text_SkillAttDesc != null)
        {
            text_SkillAttDesc.text = string.Empty;
        }

        if (text_SkillMidDesc != null)
        {
            text_SkillMidDesc.text = string.Empty;
        }

        if (text_SkillDefDesc != null)
        {
            text_SkillDefDesc.text = string.Empty;
        }

        if (img_SkillAtt != null)
        {
            img_SkillAtt.sprite = null;
        }

        if (img_SkillMid != null)
        {
            img_SkillMid.sprite = null;
        }

        if (img_SkillDef != null)
        {
            img_SkillDef.sprite = null;
        }
    }

    private void OnClickSelectAtt()
    {
        mySelectedStatType = 0;
        ExecuteMatchBattleSimulation(mySelectedStatType);
    }

    private void OnClickSelectMid()
    {
        mySelectedStatType = 1;
        ExecuteMatchBattleSimulation(mySelectedStatType);
    }

    private void OnClickSelectDef()
    {
        mySelectedStatType = 2;
        ExecuteMatchBattleSimulation(mySelectedStatType);
    }

    private void ExecuteMatchBattleSimulation(int statType)
    {
        if (mySelectedCardData == null)
        {
            Debug.LogWarning("[SingleFatalUi] 선택된 내 카드가 없습니다.");
            return;
        }

        OnClickCardInfoClose();

        int opponentStatType = 2;
        if (statType == 0)
        {
            opponentStatType = 2;
        }

        else if (statType == 1)
        {
            opponentStatType = 1;
        }

        else if (statType == 2)
        {
            opponentStatType = 0;
        }

        OpponentPlayerCard aiCard = GetCurrentRoundAiCard(opponentStatType);

        TriggerOppositeChoosenUI(aiCard, opponentStatType);

        OpenResultPanelAsync(mySelectedCardData, statType, aiCard, opponentStatType).Forget();
    }

    private OpponentPlayerCard GetCurrentRoundAiCard(int targetStatType)
    {
        if (currentAiStartingList == null)
        {
            return null;
        }

        if (currentAiStartingList.Count == 0)
        {
            return null;
        }

        return currentAiStartingList[0];
    }

    private void TriggerOppositeChoosenUI(OpponentPlayerCard aiCard, int opponentStatType)
    {
        if (object_OppositeChoosen == null)
        {
            return;
        }

        object_OppositeChoosen.SetActive(true);

        if (aiCard != null && text_ChoosenSlotPos != null)
        {
            string pos = aiCard.position;
            text_ChoosenSlotPos.text = pos;
            if (pos == "DF")
            {
                text_ChoosenSlotPos.color = Color.blue;
            }

            else if (pos == "MF")
            {
                text_ChoosenSlotPos.color = Color.green;
            }

            else if (pos == "FW")
            {
                text_ChoosenSlotPos.color = Color.red;
            }

            else if (pos == "GK")
            {
                text_ChoosenSlotPos.color = Color.yellow;
            }

            else
            {
                text_ChoosenSlotPos.color = Color.white;
            }
        }

        if (text_ChoosenOverallName != null && image_ChoosenOverallBg != null)
        {
            if (opponentStatType == 0)
            {
                text_ChoosenOverallName.text = "ATT";
                image_ChoosenOverallBg.color = Color.red;
            }

            else if (opponentStatType == 1)
            {
                text_ChoosenOverallName.text = "MID";
                image_ChoosenOverallBg.color = Color.green;
            }

            else if (opponentStatType == 2)
            {
                text_ChoosenOverallName.text = "DEF";
                image_ChoosenOverallBg.color = Color.blue;
            }
        }
    }

    private async UniTaskVoid OpenResultPanelAsync(UserCardData myCard, int myType, OpponentPlayerCard aiCard, int aiType)
    {
        if (panel_Result != null)
        {
            panel_Result.SetActive(true);
        }

        if (text_Skill_Home != null)
        {
            text_Skill_Home.text = string.Empty;
        }

        if (text_Overall_Home != null)
        {
            text_Overall_Home.text = "0";
        }

        if (text_Plus_Home != null)
        {
            text_Plus_Home.text = string.Empty;
        }

        if (text_Result_Home != null)
        {
            text_Result_Home.text = string.Empty;
        }

        if (text_Skill_Away != null)
        {
            text_Skill_Away.text = string.Empty;
        }

        if (text_Overall_Away != null)
        {
            text_Overall_Away.text = "0";
        }

        if (text_Plus_Away != null)
        {
            text_Plus_Away.text = string.Empty;
        }

        if (text_Result_Away != null)
        {
            text_Result_Away.text = string.Empty;
        }

        PlayerData myPlayerData = GameDataManager.Instance.GetData<PlayerData>(myCard.PlayerId);
        
        int myBaseVal = 0;
        
        if (myPlayerData != null)
        {
            if (myType == 0)
            {
                myBaseVal = myPlayerData.Att;
            }

            else if (myType == 1)
            {
                myBaseVal = myPlayerData.Mid;
            }

            else if (myType == 2)
            {
                myBaseVal = myPlayerData.Def;
            }
        }

        if (text_Overall_Home != null)
        {
            text_Overall_Home.text = myBaseVal.ToString();
        }

        int aiBaseVal = 50;
        if (text_Overall_Away != null)
        {
            text_Overall_Away.text = aiBaseVal.ToString();
        }

        SkillData myCameraSkill = GetSkillDataByStatType(myCard, myType);
        int myIncrease = 0;
        int aiDecrease = 0;

        if (myCameraSkill != null)
        {
            if (text_Skill_Home != null)
            {
                text_Skill_Home.text = myCameraSkill.Description;
            }

            myIncrease = myCameraSkill.Increase;
            aiDecrease = myCameraSkill.Decrease;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        if (myIncrease > 0 && text_Plus_Home != null)
        {
            text_Plus_Home.text = "+" + myIncrease;
        }

        if (aiDecrease > 0 && text_Plus_Away != null)
        {
            text_Plus_Away.text = "-" + aiDecrease;
        }

        int myFinalVal = myBaseVal + myIncrease;
        int aiFinalVal = aiBaseVal - aiDecrease;

        if (text_Overall_Home != null)
        {
            text_Overall_Home.text = myFinalVal.ToString();
        }

        if (text_Overall_Away != null)
        {
            text_Overall_Away.text = aiFinalVal.ToString();
        }

        string judgeResult = EvaluateMatchWinner(myFinalVal, aiFinalVal, myPlayerData, aiBaseVal);

        if (judgeResult == "WIN")
        {
            if (text_Result_Home != null) { text_Result_Home.text = "Win"; text_Result_Home.color = Color.red; }
            if (text_Result_Away != null) { text_Result_Away.text = "Lose"; text_Result_Away.color = Color.blue; }
            ApplyScoreDelta(myCameraSkill, true);
        }

        else if (judgeResult == "LOSE")
        {
            if (text_Result_Home != null) { text_Result_Home.text = "Lose"; text_Result_Home.color = Color.blue; }
            if (text_Result_Away != null) { text_Result_Away.text = "Win"; text_Result_Away.color = Color.red; }
            ApplyScoreDelta(myCameraSkill, false);
        }

        else
        {
            if (text_Result_Home != null) { text_Result_Home.text = "Draw"; text_Result_Home.color = Color.white; }
            if (text_Result_Away != null) { text_Result_Away.text = "Draw"; text_Result_Away.color = Color.white; }
            ApplyScoreDelta(null, false);
        }

        RunResultTimerAsync().Forget();
    }

    private SkillData GetSkillDataByStatType(UserCardData cardData, int statType)
    {
        if (statType == 0)
        {
            return cardData.SkillFwData;
        }

        else if (statType == 1)
        {
            return cardData.SkillMfData;
        }

        else if (statType == 2)
        {
            return cardData.SkillDfData;
        }

        return null;
    }

    private string EvaluateMatchWinner(int myFinal, int aiFinal, PlayerData myData, int aiBase)
    {
        if (myFinal > aiFinal)
        {
            return "WIN";
        }

        else if (myFinal < aiFinal)
        {
            return "LOSE";
        }

        else
        {
            int myOtherSum = myData.Att + myData.Mid + myData.Def;
            int aiOtherSum = aiBase * 3;

            if (myOtherSum > aiOtherSum)
            {
                return "WIN";
            }

            else if (myOtherSum < aiOtherSum)
            {
                return "LOSE";
            }

            else
            {
                int myMinStat = Mathf.Min(myData.Att, Mathf.Min(myData.Mid, myData.Def));
                int aiMinStat = aiBase;

                if (myMinStat > aiMinStat)
                {
                    return "WIN";
                }

                else if (myMinStat < aiMinStat)
                {
                    return "LOSE";
                }

                else
                {
                    return "DRAW";
                }
            }
        }
    }

    private void ApplyScoreDelta(SkillData usedSkill, bool isMyWin)
    {
        int homeScoreDelta = 1;
        bool isCaptainSkill = false;
        int penaltyLoss = 1;

        if (usedSkill != null && usedSkill.Name.Contains("주장의 품격"))
        {
            isCaptainSkill = true;
        }

        if (isMyWin)
        {
            if (isCaptainSkill)
            {
                homeScoreDelta = 2;
            }

            AddHomeScore(homeScoreDelta);
        }

        else
        {
            if (isCaptainSkill)
            {
                AddHomeScore(-penaltyLoss);
            }
        }
    }

    private void AddHomeScore(int delta)
    {
        if (point_Home != null)
        {
            int currentScore = 0;
            int.TryParse(point_Home.text, out currentScore);
            currentScore = currentScore + delta;
            if (currentScore < 0) currentScore = 0;
            point_Home.text = currentScore.ToString();
        }
    }

    private async UniTaskVoid RunResultTimerAsync()
    {
        resultTimer = 5f;

        while (resultTimer > 0f)
        {
            if (text_Time_Result != null)
            {
                text_Time_Result.text = Mathf.CeilToInt(resultTimer).ToString();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            resultTimer = resultTimer - 1f;
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (object_OppositeChoosen != null)
        {
            object_OppositeChoosen.SetActive(false);
        }

        ProgressTurn();
    }

    private void ShowEndPanel()
    {
        if (panel_Match != null)
        {
            panel_Match.SetActive(false);
        }

        if (panel_Result != null)
        {
            panel_Result.SetActive(false);
        }

        if (panel_End != null)
        {
            panel_End.SetActive(true);
        }

        if (name_Home_End != null && name_Home != null)
        {
            name_Home_End.text = name_Home.text;
        }

        if (name_Away_End != null && name_Away != null)
        {
            name_Away_End.text = name_Away.text;
        }

        int homeScore = 0;
        int awayScore = 0;

        if (point_Home != null)
        {
            int.TryParse(point_Home.text, out homeScore);
        }

        if (point_Away != null)
        {
            int.TryParse(point_Away.text, out awayScore);
        }

        if (score_Home_End != null)
        {
            score_Home_End.text = homeScore.ToString();
        }

        if (score_Away_End != null)
        {
            score_Away_End.text = awayScore.ToString();
        }

        if (logo_Home_End != null && logo_Home != null)
        {
            logo_Home_End.sprite = logo_Home.sprite;
        }

        if (logo_Away_End != null && logo_Away != null)
        {
            logo_Away_End.sprite = logo_Away.sprite;
        }

        if (homeScore > awayScore)
        {
            if (result_Home_End != null) { result_Home_End.text = "Win"; result_Home_End.color = Color.red; }
            if (result_Away_End != null) { result_Away_End.text = "Lose"; result_Away_End.color = Color.blue; }
        }

        else if (homeScore < awayScore)
        {
            if (result_Home_End != null) { result_Home_End.text = "Lose"; result_Home_End.color = Color.blue; }
            if (result_Away_End != null) { result_Away_End.text = "Win"; result_Away_End.color = Color.red; }
        }

        else
        {
            if (result_Home_End != null) { result_Home_End.text = "Draw"; result_Home_End.color = Color.white; }
            if (result_Away_End != null) { result_Away_End.text = "Draw"; result_Away_End.color = Color.white; }
        }

        RunEndTimerAsync().Forget();
    }

    private async UniTaskVoid RunEndTimerAsync()
    {
        endTimer = 30f;

        while (endTimer > 0f)
        {
            if (panel_End == null)
            {
                break;
            }

            if (panel_End.activeSelf == false)
            {
                break;
            }

            if (text_Time_End != null)
            {
                text_Time_End.text = Mathf.CeilToInt(endTimer).ToString();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            endTimer = endTimer - 1f;
        }

        if (panel_End != null)
        {
            if (panel_End.activeSelf == true)
            {
                CloseEndPanel();
            }
        }
    }

    private void OnClickButtonCheck()
    {
        CloseEndPanel();
    }

    private void CloseEndPanel()
    {
        if (panel_End != null)
        {
            panel_End.SetActive(false);
        }

        string userId = DBManager.Instance.CurrentUserId;
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData != null)
        {
            int homeScore = 0;
            int awayScore = 0;

            if (score_Home_End != null)
            {
                int.TryParse(score_Home_End.text, out homeScore);
            }

            if (score_Away_End != null)
            {
                int.TryParse(score_Away_End.text, out awayScore);
            }

            bool isHome = (seasonData.currentRound % 2 == 0);
            int playerGoals = 0;
            int opponentGoals = 0;

            if (isHome)
            {
                playerGoals = homeScore;
                opponentGoals = awayScore;
            }

            else
            {
                playerGoals = awayScore;
                opponentGoals = homeScore;
            }

            if (playerGoals > opponentGoals)
            {
                seasonData.wins = seasonData.wins + 1;
                seasonData.points = seasonData.points + 3;
            }

            else if (playerGoals == opponentGoals)
            {
                seasonData.draws = seasonData.draws + 1;
                seasonData.points = seasonData.points + 1;
            }

            else
            {
                seasonData.losses = seasonData.losses + 1;
            }

            string newRecord = homeScore + ":" + awayScore;
            if (string.IsNullOrEmpty(seasonData.matchHistoryCsv))
            {
                seasonData.matchHistoryCsv = newRecord;
            }

            else
            {
                seasonData.matchHistoryCsv = seasonData.matchHistoryCsv + "," + newRecord;
            }

            seasonData.currentRound = seasonData.currentRound + 1;

            DBManager.Instance.UpdateSingleLeagueProgress(
                userId,
                seasonData.currentRound,
                seasonData.wins,
                seasonData.draws,
                seasonData.losses,
                seasonData.points,
                seasonData.matchHistoryCsv,
                seasonData.aiResultsCsv
            );
        }

        UiManager.Instance.CloseUi<SingleFatalUi>();
        Debug.Log("[SingleFatalUi] 경기 종료 및 데이터 정산 완료, UI 닫힘.");
    }

    private void ShowOpponentSquadView()
    {
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;

        if (seasonData == null)
        {
            return;
        }

        string teamsCsv = seasonData.selectedTeamsCsv;
        if (string.IsNullOrEmpty(teamsCsv))
        {
            return;
        }

        string[] teams = teamsCsv.Split(',');
        int roundIndex = seasonData.currentRound;
        string opponentTeamName = string.Empty;

        if (roundIndex >= 0)
        {
            if (roundIndex < teams.Length)
            {
                opponentTeamName = teams[roundIndex];
            }
        }

        SingleLeagueMode leagueMode = new SingleLeagueMode();
        SingleLeagueMode.OpponentSquadResult aiSquad = leagueMode.BuildOpponentSquad(opponentTeamName, seasonData.difficultyIndex);

        if (aiSquad != null)
        {
            if (text_Formation != null)
            {
                text_Formation.text = aiSquad.assignedFormation.formationName;
            }

            ApplyMatchFormationSlots(aiSquad.assignedFormation.formationName);

            currentAiStartingList = aiSquad.startingLineup;
            currentAiBenchList = aiSquad.benchList;

            if (aiStartingSettledList.Count != currentAiStartingList.Count)
            {
                aiStartingSettledList.Clear();
                for (int i = 0; i < currentAiStartingList.Count; i++)
                {
                    aiStartingSettledList.Add(false);
                }
            }

            if (aiBenchSettledList.Count != currentAiBenchList.Count)
            {
                aiBenchSettledList.Clear();
                for (int i = 0; i < currentAiBenchList.Count; i++)
                {
                    aiBenchSettledList.Add(false);
                }
            }
        }
    }
}