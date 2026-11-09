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

    private AiPlaying aiPlaying = new AiPlaying();
    private OpponentPlayerCard preDecidedAiCard = null;
    private int preDecidedAiStatType = 0;
    private bool isHomeAttackTurn = true;
    private HashSet<int> usedCardInstanceIds = new HashSet<int>();
    private Dictionary<SquadSlot, GameObject> usingBlockDict = new Dictionary<SquadSlot, GameObject>();

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

        usedCardInstanceIds.Clear();
        CacheUsingBlocks();

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
        int difficultyIndex = 0;

        if (seasonData != null)
        {
            difficultyIndex = seasonData.difficultyIndex;
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

        if (string.IsNullOrEmpty(opponentTeamName) == false)
        {
            SingleLeagueMode leagueMode = new SingleLeagueMode();
            SingleLeagueMode.OpponentSquadResult aiSquad = leagueMode.BuildOpponentSquad(opponentTeamName, difficultyIndex);

            if (aiSquad != null)
            {
                currentAiStartingList = aiSquad.startingLineup;
                currentAiBenchList = aiSquad.benchList;
            }
        }

        int currentRound = 0;
        if (seasonData != null)
        {
            currentRound = seasonData.currentRound;
        }

        bool isPlayerHome = (currentRound % 2 == 0);

        string homeTeamName = string.Empty;
        string awayTeamName = string.Empty;

        if (isPlayerHome)
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

        isHomeAttackTurn = true;

        if (home_Attack != null)
        {
            home_Attack.SetActive(isHomeAttackTurn);
        }

        if (away_Attack != null)
        {
            away_Attack.SetActive(isHomeAttackTurn == false);
        }

        LoadMatchLogosAsync(homeTeamName, awayTeamName).Forget();
        LoadAndApplyUserSquad(userId);
        UpdateAllSlotUsingBlocks();

        bool isPlayerAttackTurn = (isPlayerHome == isHomeAttackTurn);
        if (isPlayerAttackTurn == false)
        {
            ProcessAiAttackTurn();
        }

        else
        {
            if (object_OppositeChoosen != null)
            {
                object_OppositeChoosen.SetActive(false);
            }
        }
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
            if (panel_Sub.activeSelf)
            {
                HandleSubSlotSelection(clickedSlot);
                return;
            }
        }

        if (clickedSlot.TargetUserCardData != null)
        {
            if (usedCardInstanceIds.Contains(clickedSlot.TargetUserCardData.InstanceId))
            {
                Debug.LogWarning("[SingleFatalUi] 이미 사용 완료된 선수 카드입니다.");
                return;
            }

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

        if (currentTurn > 11)
        {
            ShowEndPanel();
            return;
        }

        if (text_Turn != null)
        {
            text_Turn.text = "현재 턴: " + currentTurn;
        }

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        int currentRound = 0;
        if (seasonData != null)
        {
            currentRound = seasonData.currentRound;
        }

        bool isPlayerHome = (currentRound % 2 == 0);
        bool isPlayerAttackTurn = (isPlayerHome == isHomeAttackTurn);

        if (isPlayerAttackTurn == false)
        {
            ProcessAiAttackTurn();
        }

        else
        {
            if (object_OppositeChoosen != null)
            {
                object_OppositeChoosen.SetActive(false);
            }
        }

        if (home_Attack != null)
        {
            home_Attack.SetActive(isHomeAttackTurn);
        }

        if (away_Attack != null)
        {
            away_Attack.SetActive(isHomeAttackTurn == false);
        }

        if (button_Substitute != null)
        {
            if (currentTurn == 2 || currentTurn == 5 || currentTurn == 8)
            {
                button_Substitute.SetInteractable(true);
            }

            else
            {
                button_Substitute.SetInteractable(false);
            }
        }
    }

    private void ProcessAiAttackTurn()
    {
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        int stageIndex = 0;
        if (seasonData != null)
        {
            stageIndex = seasonData.difficultyIndex;
        }

        var aiResult = aiPlaying.DecideAiCard(currentAiStartingList, currentTurn, stageIndex, 0, true);

        if (aiResult != null)
        {
            preDecidedAiCard = aiResult.selectedCard;
            preDecidedAiStatType = aiResult.selectedStatType;

            TriggerOppositeChoosenUI(preDecidedAiCard, preDecidedAiStatType);
        }
    }

    private void OnClickSubstitute()
    {
        if (currentTurn == 3 || currentTurn == 6 || currentTurn == 9)
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
            if (panel_Sub.activeSelf)
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
        isViewingOpponent = (isViewingOpponent == false);

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

        if (mySelectedCardData != null && usedCardInstanceIds.Contains(mySelectedCardData.InstanceId) == false)
        {
            usedCardInstanceIds.Add(mySelectedCardData.InstanceId);
            UpdateAllSlotUsingBlocks();
        }

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        int stageIndex = 0;
        if (seasonData != null)
        {
            stageIndex = seasonData.difficultyIndex;
        }

        int currentRound = 0;
        if (seasonData != null)
        {
            currentRound = seasonData.currentRound;
        }

        bool isPlayerHome = (currentRound % 2 == 0);
        bool isPlayerAttackTurn = (isPlayerHome == isHomeAttackTurn);

        UserCardData playerCard = mySelectedCardData;
        int playerStatType = statType;

        OpponentPlayerCard aiCard = null;
        int aiStatType = 2;

        if (isPlayerAttackTurn)
        {
            if (object_OppositeChoosen != null)
            {
                object_OppositeChoosen.SetActive(false);
            }

            var aiResult = aiPlaying.DecideAiCard(currentAiStartingList, currentTurn, stageIndex, playerStatType, false);
            if (aiResult != null)
            {
                aiCard = aiResult.selectedCard;
                aiStatType = aiResult.selectedStatType;
                currentAiStartingList.Remove(aiCard);
            }
        }

        else
        {
            if (preDecidedAiCard != null)
            {
                aiCard = preDecidedAiCard;
                aiStatType = preDecidedAiStatType;
                currentAiStartingList.Remove(aiCard);
                preDecidedAiCard = null;
            }

            else
            {
                var aiResult = aiPlaying.DecideAiCard(currentAiStartingList, currentTurn, stageIndex, playerStatType, true);
                if (aiResult != null)
                {
                    aiCard = aiResult.selectedCard;
                    aiStatType = aiResult.selectedStatType;
                    currentAiStartingList.Remove(aiCard);
                }
            }

            TriggerOppositeChoosenUI(aiCard, aiStatType);
        }

        UserCardData homeUserCard = null;
        int homeUserStatType = 0;
        if (isPlayerHome)
        {
            homeUserCard = playerCard;
            homeUserStatType = playerStatType;
        }

        OpponentPlayerCard homeAiCard = null;
        int homeAiStatType = 0;
        if (isPlayerHome == false)
        {
            homeAiCard = aiCard;
            homeAiStatType = aiStatType;
        }

        UserCardData awayUserCard = null;
        int awayUserStatType = 0;
        if (isPlayerHome == false)
        {
            awayUserCard = playerCard;
            awayUserStatType = playerStatType;
        }

        OpponentPlayerCard awayAiCard = null;
        int awayAiStatType = 0;
        if (isPlayerHome)
        {
            awayAiCard = aiCard;
            awayAiStatType = aiStatType;
        }

        OpenResultPanelAsync(homeUserCard, homeUserStatType, homeAiCard, homeAiStatType, awayUserCard, awayUserStatType, awayAiCard, awayAiStatType, isPlayerHome).Forget();
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

    private async UniTaskVoid OpenResultPanelAsync(
        UserCardData homeUserCard, int homeUserStatType,
        OpponentPlayerCard homeAiCard, int homeAiStatType,
        UserCardData awayUserCard, int awayUserStatType,
        OpponentPlayerCard awayAiCard, int awayAiStatType,
        bool isPlayerHome)
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

        int homeBaseVal = 0;
        SkillData homeSkill = null;
        PlayerData homePlayerData = null;
        int homeAiBase = 0;

        if (isPlayerHome)
        {
            if (homeUserCard != null)
            {
                homePlayerData = GameDataManager.Instance.GetData<PlayerData>(homeUserCard.PlayerId);
                if (homePlayerData != null)
                {
                    if (homeUserStatType == 0)
                    {
                        homeBaseVal = homePlayerData.Att;
                    }

                    else if (homeUserStatType == 1)
                    {
                        homeBaseVal = homePlayerData.Mid;
                    }

                    else if (homeUserStatType == 2)
                    {
                        homeBaseVal = homePlayerData.Def;
                    }
                }
                homeSkill = GetSkillDataByStatType(homeUserCard, homeUserStatType);
            }
        }

        else
        {
            if (homeAiCard != null)
            {
                if (homeAiStatType == 0)
                {
                    homeAiBase = homeAiCard.attack;
                }

                else if (homeAiStatType == 1)
                {
                    homeAiBase = homeAiCard.mid;
                }

                else if (homeAiStatType == 2)
                {
                    homeAiBase = homeAiCard.defense;
                }

                homeBaseVal = homeAiBase;
            }
        }

        if (text_Overall_Home != null)
        {
            text_Overall_Home.text = homeBaseVal.ToString();
        }

        int awayBaseVal = 0;
        SkillData awaySkill = null;
        PlayerData awayPlayerData = null;
        int awayAiBase = 0;

        if (isPlayerHome == false)
        {
            if (awayUserCard != null)
            {
                awayPlayerData = GameDataManager.Instance.GetData<PlayerData>(awayUserCard.PlayerId);
                if (awayPlayerData != null)
                {
                    if (awayUserStatType == 0)
                    {
                        awayBaseVal = awayPlayerData.Att;
                    }

                    else if (awayUserStatType == 1)
                    {
                        awayBaseVal = awayPlayerData.Mid;
                    }

                    else if (awayUserStatType == 2)
                    {
                        awayBaseVal = awayPlayerData.Def;
                    }
                }
                awaySkill = GetSkillDataByStatType(awayUserCard, awayUserStatType);
            }
        }

        else
        {
            if (awayAiCard != null)
            {
                if (awayAiStatType == 0)
                {
                    awayAiBase = awayAiCard.attack;
                }

                else if (awayAiStatType == 1)
                {
                    awayAiBase = awayAiCard.mid;
                }

                else if (awayAiStatType == 2)
                {
                    awayAiBase = awayAiCard.defense;
                }

                awayBaseVal = awayAiBase;
            }
        }

        if (text_Overall_Away != null)
        {
            text_Overall_Away.text = awayBaseVal.ToString();
        }

        int homeIncrease = 0;
        int homeDecrease = 0;

        if (homeSkill != null)
        {
            if (text_Skill_Home != null)
            {
                text_Skill_Home.text = homeSkill.Description;
            }

            homeIncrease = homeSkill.Increase;
            homeDecrease = homeSkill.Decrease;
        }

        else
        {
            if (isPlayerHome == false)
            {
                if (homeAiCard != null)
                {
                    if (homeAiCard.assignedSkill != null)
                    {
                        if (text_Skill_Home != null)
                        {
                            text_Skill_Home.text = homeAiCard.assignedSkill.Description;
                        }

                        homeIncrease = homeAiCard.assignedSkill.Increase;
                    }
                }
            }
        }

        int awayIncrease = 0;
        int awayDecrease = 0;

        if (awaySkill != null)
        {
            if (text_Skill_Away != null)
            {
                text_Skill_Away.text = awaySkill.Description;
            }

            awayIncrease = awaySkill.Increase;
            awayDecrease = awaySkill.Decrease;
        }

        else
        {
            if (isPlayerHome)
            {
                if (awayAiCard != null)
                {
                    if (awayAiCard.assignedSkill != null)
                    {
                        if (text_Skill_Away != null)
                        {
                            text_Skill_Away.text = awayAiCard.assignedSkill.Description;
                        }

                        awayIncrease = awayAiCard.assignedSkill.Increase;
                    }
                }
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        int netHome = homeIncrease - homeDecrease;
        if (netHome != 0)
        {
            if (text_Plus_Home != null)
            {
                if (netHome >= 0)
                {
                    text_Plus_Home.text = "+" + netHome;
                }

                else
                {
                    text_Plus_Home.text = netHome.ToString();
                }
            }
        }

        int netAway = awayIncrease - awayDecrease;
        if (netAway != 0)
        {
            if (text_Plus_Away != null)
            {
                if (netAway >= 0)
                {
                    text_Plus_Away.text = "+" + netAway;
                }

                else
                {
                    text_Plus_Away.text = netAway.ToString();
                }
            }
        }

        int homeFinalVal = homeBaseVal + homeIncrease - homeDecrease;
        int awayFinalVal = awayBaseVal + awayIncrease - awayDecrease;

        if (homeFinalVal < 0)
        {
            homeFinalVal = 0;
        }

        if (awayFinalVal < 0)
        {
            awayFinalVal = 0;
        }

        if (text_Overall_Home != null)
        {
            text_Overall_Home.text = homeFinalVal.ToString();
        }

        if (text_Overall_Away != null)
        {
            text_Overall_Away.text = awayFinalVal.ToString();
        }

        string judgeResult = EvaluateHomeAwayWinner(homeFinalVal, awayFinalVal, homePlayerData, homeAiBase, awayPlayerData, awayAiBase);

        if (judgeResult == "HOME_WIN")
        {
            if (text_Result_Home != null)
            {
                text_Result_Home.text = "Win";
                text_Result_Home.color = Color.red;
            }

            if (text_Result_Away != null)
            {
                text_Result_Away.text = "Lose";
                text_Result_Away.color = Color.blue;
            }

            ApplyHomeScoreDelta(homeSkill, true);
            isHomeAttackTurn = true;
        }

        else if (judgeResult == "AWAY_WIN")
        {
            if (text_Result_Home != null)
            {
                text_Result_Home.text = "Lose";
                text_Result_Home.color = Color.blue;
            }

            if (text_Result_Away != null)
            {
                text_Result_Away.text = "Win";
                text_Result_Away.color = Color.red;
            }

            ApplyAwayScoreDelta(awaySkill, true);
            isHomeAttackTurn = false;
        }

        else
        {
            if (text_Result_Home != null)
            {
                text_Result_Home.text = "Draw";
                text_Result_Home.color = Color.white;
            }

            if (text_Result_Away != null)
            {
                text_Result_Away.text = "Draw";
                text_Result_Away.color = Color.white;
            }
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

    private string EvaluateHomeAwayWinner(int homeFinal, int awayFinal, PlayerData homePlayer, int homeAiBase, PlayerData awayPlayer, int awayAiBase)
    {
        if (homeFinal > awayFinal)
        {
            return "HOME_WIN";
        }

        else if (homeFinal < awayFinal)
        {
            return "AWAY_WIN";
        }

        else
        {
            int homeOtherSum = 0;
            if (homePlayer != null)
            {
                homeOtherSum = homePlayer.Att + homePlayer.Mid + homePlayer.Def;
            }

            else
            {
                homeOtherSum = homeAiBase * 3;
            }

            int awayOtherSum = 0;
            if (awayPlayer != null)
            {
                awayOtherSum = awayPlayer.Att + awayPlayer.Mid + awayPlayer.Def;
            }

            else
            {
                awayOtherSum = awayAiBase * 3;
            }

            if (homeOtherSum > awayOtherSum)
            {
                return "HOME_WIN";
            }

            else if (homeOtherSum < awayOtherSum)
            {
                return "AWAY_WIN";
            }

            else
            {
                int homeMin = 0;
                if (homePlayer != null)
                {
                    homeMin = Mathf.Min(homePlayer.Att, Mathf.Min(homePlayer.Mid, homePlayer.Def));
                }

                else
                {
                    homeMin = homeAiBase;
                }

                int awayMin = 0;
                if (awayPlayer != null)
                {
                    awayMin = Mathf.Min(awayPlayer.Att, Mathf.Min(awayPlayer.Mid, awayPlayer.Def));
                }

                else
                {
                    awayMin = awayAiBase;
                }

                if (homeMin > awayMin)
                {
                    return "HOME_WIN";
                }

                else if (homeMin < awayMin)
                {
                    return "AWAY_WIN";
                }

                else
                {
                    return "DRAW";
                }
            }
        }
    }

    private void ApplyHomeScoreDelta(SkillData usedSkill, bool isHomeWin)
    {
        int homeScoreDelta = 1;
        bool isCaptainSkill = false;

        if (usedSkill != null)
        {
            if (string.IsNullOrEmpty(usedSkill.Name) == false)
            {
                if (usedSkill.Name.Contains("주장의 품격"))
                {
                    isCaptainSkill = true;
                }
            }
        }

        if (isHomeWin)
        {
            if (isCaptainSkill)
            {
                homeScoreDelta = 2;
            }

            AddHomeScore(homeScoreDelta);
        }
    }

    private void ApplyAwayScoreDelta(SkillData usedSkill, bool isAwayWin)
    {
        int awayScoreDelta = 1;
        bool isCaptainSkill = false;

        if (usedSkill != null)
        {
            if (string.IsNullOrEmpty(usedSkill.Name) == false)
            {
                if (usedSkill.Name.Contains("주장의 품격"))
                {
                    isCaptainSkill = true;
                }
            }
        }

        if (isAwayWin)
        {
            if (isCaptainSkill)
            {
                awayScoreDelta = 2;
            }

            AddAwayScore(awayScoreDelta);
        }
    }

    private void AddHomeScore(int delta)
    {
        if (point_Home != null)
        {
            int currentScore = 0;
            int.TryParse(point_Home.text, out currentScore);
            currentScore = currentScore + delta;
            if (currentScore < 0)
            {
                currentScore = 0;
            }

            point_Home.text = currentScore.ToString();
        }
    }

    private void AddAwayScore(int delta)
    {
        if (point_Away != null)
        {
            int currentScore = 0;
            int.TryParse(point_Away.text, out currentScore);
            currentScore = currentScore + delta;
            if (currentScore < 0)
            {
                currentScore = 0;
            }

            point_Away.text = currentScore.ToString();
        }
    }

    private void CacheUsingBlocks()
    {
        usingBlockDict.Clear();
        CacheSlotBlock(slot_GK);
        CacheSlotArrayBlocks(slot_DFs);
        CacheSlotArrayBlocks(slot_MFs);
        CacheSlotArrayBlocks(slot_FWs);
    }

    private void CacheSlotBlock(SquadSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        for (int i = 0; i < slot.transform.childCount; i++)
        {
            Transform child = slot.transform.GetChild(i);
            if (child.name == "UsingBlock")
            {
                usingBlockDict[slot] = child.gameObject;
                break;
            }
        }
    }

    private void CacheSlotArrayBlocks(SquadSlot[] slots)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            CacheSlotBlock(slots[i]);
        }
    }

    private void UpdateSlotUsingBlock(SquadSlot slot)
    {
        if (slot == null)
        {
            return;
        }

        if (usingBlockDict.TryGetValue(slot, out GameObject usingBlockObj))
        {
            bool isUsed = false;

            if (slot.TargetUserCardData != null)
            {
                isUsed = usedCardInstanceIds.Contains(slot.TargetUserCardData.InstanceId);
            }

            usingBlockObj.SetActive(isUsed);

            if (isUsed)
            {
                usingBlockObj.transform.SetAsLastSibling();
            }
        }
    }

    private void UpdateAllSlotUsingBlocks()
    {
        UpdateSlotUsingBlock(slot_GK);

        if (slot_DFs != null)
        {
            for (int i = 0; i < slot_DFs.Length; i++)
            {
                UpdateSlotUsingBlock(slot_DFs[i]);
            }
        }

        if (slot_MFs != null)
        {
            for (int i = 0; i < slot_MFs.Length; i++)
            {
                UpdateSlotUsingBlock(slot_MFs[i]);
            }
        }

        if (slot_FWs != null)
        {
            for (int i = 0; i < slot_FWs.Length; i++)
            {
                UpdateSlotUsingBlock(slot_FWs[i]);
            }
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
            if (result_Home_End != null)
            {
                result_Home_End.text = "Win";
                result_Home_End.color = Color.red;
            }

            if (result_Away_End != null)
            {
                result_Away_End.text = "Lose";
                result_Away_End.color = Color.blue;
            }
        }

        else if (homeScore < awayScore)
        {
            if (result_Home_End != null)
            {
                result_Home_End.text = "Lose";
                result_Home_End.color = Color.blue;
            }

            if (result_Away_End != null)
            {
                result_Away_End.text = "Win";
                result_Away_End.color = Color.red;
            }
        }

        else
        {
            if (result_Home_End != null)
            {
                result_Home_End.text = "Draw";
                result_Home_End.color = Color.white;
            }

            if (result_Away_End != null)
            {
                result_Away_End.text = "Draw";
                result_Away_End.color = Color.white;
            }
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
            if (panel_End.activeSelf)
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

        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        bool isHome = false;

        if (seasonData != null)
        {
            isHome = (seasonData.currentRound % 2 == 0);
        }

        SingleLeagueUi.OnManualMatchFinished?.Invoke(homeScore, awayScore, isHome);
        UiManager.Instance.CloseUi<SingleFatalUi>();
        Debug.Log("[SingleFatalUi] 경기 종료, Panel_Round 호출 후 UI 닫힘.");
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