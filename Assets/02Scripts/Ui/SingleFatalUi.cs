using Cysharp.Threading.Tasks;
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

    private int currentTurn = 1;
    private int subPossibleCount = 5;
    private int subCompletedCount = 0;
    private bool isViewingOpponent = false;
    private List<OpponentPlayerCard> currentAiStartingList = new List<OpponentPlayerCard>();
    private List<OpponentPlayerCard> currentAiBenchList = new List<OpponentPlayerCard>();
    private List<bool> aiStartingSettledList = new List<bool>();
    private List<bool> aiBenchSettledList = new List<bool>();
    private Dictionary<string, bool> aiPlayerSettledDict = new Dictionary<string, bool>();

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
                return;
            }
        }

        if (clickedSlot.TargetUserCardData != null)
        {
            OpenCardInfo(clickedSlot.TargetUserCardData);
        }
    }

    public void ProgressTurn()
    {
        currentTurn = currentTurn + 1;
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
            panel_Sub.SetActive(isOpen == false);
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

        LoadCardSkillInfoAsync().Forget();
    }

    private async UniTaskVoid LoadCardSkillInfoAsync()
    {
        SkillData attSkill = FindSkillDataByPos("Att");
        if (attSkill != null)
        {
            if (text_SkillAttDesc != null)
            {
                text_SkillAttDesc.text = attSkill.Description;
            }

            if (img_SkillAtt != null && string.IsNullOrEmpty(attSkill.ImageAddress) == false)
            {
                Sprite skillSprite = await ResourceManager.Instance.LoadAsset<Sprite>(attSkill.ImageAddress);
                if (img_SkillAtt != null)
                {
                    img_SkillAtt.sprite = skillSprite;
                }
            }
        }

        SkillData midSkill = FindSkillDataByPos("Mid");
        if (midSkill != null)
        {
            if (text_SkillMidDesc != null)
            {
                text_SkillMidDesc.text = midSkill.Description;
            }

            if (img_SkillMid != null && string.IsNullOrEmpty(midSkill.ImageAddress) == false)
            {
                Sprite skillSprite = await ResourceManager.Instance.LoadAsset<Sprite>(midSkill.ImageAddress);
                if (img_SkillMid != null)
                {
                    img_SkillMid.sprite = skillSprite;
                }
            }
        }

        SkillData defSkill = FindSkillDataByPos("Def");
        if (defSkill != null)
        {
            if (text_SkillDefDesc != null)
            {
                text_SkillDefDesc.text = defSkill.Description;
            }

            if (img_SkillDef != null && string.IsNullOrEmpty(defSkill.ImageAddress) == false)
            {
                Sprite skillSprite = await ResourceManager.Instance.LoadAsset<Sprite>(defSkill.ImageAddress);
                if (img_SkillDef != null)
                {
                    img_SkillDef.sprite = skillSprite;
                }
            }
        }
    }

    private SkillData FindSkillDataByPos(string targetPos)
    {
        List<SkillData> allSkills = GameDataManager.Instance.GetAllData<SkillData>();
        if (allSkills == null)
        {
            return null;
        }

        for (int i = 0; i < allSkills.Count; i++)
        {
            SkillData skill = allSkills[i];
            if (skill != null)
            {
                if (skill.Pos == targetPos)
                {
                    return skill;
                }
            }
        }

        return null;
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
        Debug.Log("[SingleFatalUi] 공격 능력치 선택됨");
    }

    private void OnClickSelectMid()
    {
        Debug.Log("[SingleFatalUi] 미드필더 능력치 선택됨");
    }

    private void OnClickSelectDef()
    {
        Debug.Log("[SingleFatalUi] 수비 능력치 선택됨");
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

            OpponentPlayerCard aiGk = null;
            List<OpponentPlayerCard> aiDfs = new List<OpponentPlayerCard>();
            List<OpponentPlayerCard> aiMfs = new List<OpponentPlayerCard>();
            List<OpponentPlayerCard> aiFws = new List<OpponentPlayerCard>();

            for (int i = 0; i < aiSquad.startingLineup.Count; i++)
            {
                OpponentPlayerCard player = aiSquad.startingLineup[i];
                if (player != null)
                {
                    if (player.position == "GK")
                    {
                        aiGk = player;
                    }

                    else if (player.position == "DF")
                    {
                        aiDfs.Add(player);
                    }

                    else if (player.position == "MF")
                    {
                        aiMfs.Add(player);
                    }

                    else if (player.position == "FW")
                    {
                        aiFws.Add(player);
                    }

                    else
                    {
                        aiDfs.Add(player);
                    }
                }
            }

            if (slot_GK != null)
            {
                if (aiGk != null)
                {
                    bool isSettled = false;
                    if (aiPlayerSettledDict.ContainsKey(aiGk.playerId))
                    {
                        isSettled = aiPlayerSettledDict[aiGk.playerId];
                    }

                    if (isSettled)
                    {

                    }

                    else
                    {
                        slot_GK.ClearSlot();
                    }
                }

                else
                {
                    slot_GK.ClearSlot();
                }
            }

            if (slot_DFs != null)
            {
                for (int i = 0; i < slot_DFs.Length; i++)
                {
                    if (slot_DFs[i] != null && slot_DFs[i].gameObject.activeSelf)
                    {
                        if (i < aiDfs.Count)
                        {
                            OpponentPlayerCard dfPlayer = aiDfs[i];
                            bool isSettled = false;
                            if (aiPlayerSettledDict.ContainsKey(dfPlayer.playerId))
                            {
                                isSettled = aiPlayerSettledDict[dfPlayer.playerId];
                            }

                            if (isSettled)
                            {

                            }

                            else
                            {
                                slot_DFs[i].ClearSlot();
                            }
                        }

                        else
                        {
                            slot_DFs[i].ClearSlot();
                        }
                    }
                }
            }

            if (slot_MFs != null)
            {
                for (int i = 0; i < slot_MFs.Length; i++)
                {
                    if (slot_MFs[i] != null && slot_MFs[i].gameObject.activeSelf)
                    {
                        if (i < aiMfs.Count)
                        {
                            OpponentPlayerCard mfPlayer = aiMfs[i];
                            bool isSettled = false;
                            if (aiPlayerSettledDict.ContainsKey(mfPlayer.playerId))
                            {
                                isSettled = aiPlayerSettledDict[mfPlayer.playerId];
                            }

                            if (isSettled)
                            {

                            }

                            else
                            {
                                slot_MFs[i].ClearSlot();
                            }
                        }

                        else
                        {
                            slot_MFs[i].ClearSlot();
                        }
                    }
                }
            }

            if (slot_FWs != null)
            {
                for (int i = 0; i < slot_FWs.Length; i++)
                {
                    if (slot_FWs[i] != null && slot_FWs[i].gameObject.activeSelf)
                    {
                        if (i < aiFws.Count)
                        {
                            OpponentPlayerCard fwPlayer = aiFws[i];
                            bool isSettled = false;
                            if (aiPlayerSettledDict.ContainsKey(fwPlayer.playerId))
                            {
                                isSettled = aiPlayerSettledDict[fwPlayer.playerId];
                            }

                            if (isSettled)
                            {

                            }

                            else
                            {
                                slot_FWs[i].ClearSlot();
                            }
                        }

                        else
                        {
                            slot_FWs[i].ClearSlot();
                        }
                    }
                }
            }

            if (slot_Subs != null)
            {
                SetSlotArrayActive(slot_Subs, true);
                for (int i = 0; i < slot_Subs.Length; i++)
                {
                    if (slot_Subs[i] != null)
                    {
                        if (i < aiSquad.benchList.Count)
                        {
                            OpponentPlayerCard benchPlayer = aiSquad.benchList[i];
                            bool isSettled = false;
                            if (aiPlayerSettledDict.ContainsKey(benchPlayer.playerId))
                            {
                                isSettled = aiPlayerSettledDict[benchPlayer.playerId];
                            }

                            if (isSettled)
                            {

                            }

                            else
                            {
                                slot_Subs[i].ClearSlot();
                            }
                        }

                        else
                        {
                            slot_Subs[i].ClearSlot();
                        }
                    }
                }
            }
        }
    }
}