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

        string playerTeamName = "내 팀";
        string userName = DBManager.Instance.CurrentUserName;

        if (userName != null)
        {
            if (userName != string.Empty)
            {
                playerTeamName = userName;
            }
        }

        string opponentTeamName = "상대 팀";

        if (seasonData != null)
        {
            string teamsCsv = seasonData.selectedTeamsCsv;
            if (teamsCsv != null)
            {
                if (teamsCsv != string.Empty)
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
        if (string.IsNullOrEmpty(userId) == true)
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

        if (savedSlots.ContainsKey(dbKey) == true)
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

        if (isViewingOpponent == true)
        {
            ShowOpponentSquadView();
        }

        else
        {
            string userId = DBManager.Instance.CurrentUserId;
            LoadAndApplyUserSquad(userId);
        }
    }

    private void ShowOpponentSquadView()
    {
        SingleLeagueData seasonData = SingleModeManager.Instance.CurrentSeasonData;
        if (seasonData == null)
        {
            return;
        }

        string teamsCsv = seasonData.selectedTeamsCsv;
        if (string.IsNullOrEmpty(teamsCsv) == true)
        {
            return;
        }

        string[] teams = teamsCsv.Split(',');
        int roundIndex = seasonData.currentRound;
        string opponentTeamName = "상대 팀";

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

            if (slot_GK != null)
            {
                slot_GK.ClearSlot();
            }
        }
    }
}