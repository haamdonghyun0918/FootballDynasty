
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleModeUi : UiBase
{
    [Header("Turn Info")]
    [SerializeField] private TMP_Text text_Turns;

    [Header("Match Info")]
    [SerializeField] private Image image_AttackHome;
    [SerializeField] private Image image_AttackAway;

    [SerializeField] private Image image_LogoHome;
    [SerializeField] private TMP_Text text_NameHome;
    [SerializeField] private TMP_Text text_PointsHome;

    [SerializeField] private Image image_LogoAway;
    [SerializeField] private TMP_Text text_NameAway;
    [SerializeField] private TMP_Text text_PointsAway;

    [Header("User Squad")]
    [SerializeField] private Transform _userSlotGK;
    [SerializeField] private Transform _userLineDF;
    [SerializeField] private Transform _userLineMF;
    [SerializeField] private Transform _userLineFW;

    [Header("Opponent Squad")]
    [SerializeField] private Transform _opponentSlotGK;
    [SerializeField] private Transform _opponentLineDF;
    [SerializeField] private Transform _opponentLineMF;
    [SerializeField] private Transform _opponentLineFW;

    [Header("Squad Panels")]
    [SerializeField] private GameObject panel_UserSquad;
    [SerializeField] private GameObject panel_OpponentSquad;
    [SerializeField] private UiButton button_Opponent;

    [Header("Opponent Choice")]
    [SerializeField] private GameObject panel_OpponentChoice;
    [SerializeField] private Transform opponentCard_Locate;
    [SerializeField] private TMP_Text text_ChooseOverall;

    [Header("User Choice")]
    [SerializeField] private GameObject panel_Choice;
    [SerializeField] private Transform card_Spawn;

    [SerializeField] private UiButton button_OverallFW;
    [SerializeField] private UiButton button_OverallMF;
    [SerializeField] private UiButton button_OverallDF;

    [SerializeField] private TMP_Text text_OverallFW;
    [SerializeField] private TMP_Text text_OverallMF;
    [SerializeField] private TMP_Text text_OverallDF;

    [SerializeField] private TMP_Text text_SkillFW;
    [SerializeField] private TMP_Text text_SkillMF;
    [SerializeField] private TMP_Text text_SkillDF;
    [SerializeField] private UiButton button_Cancel;

    private string cardPrefabAddress = "Card";

    private LeagueMatchData _currentMatch;

    private TeamSquad _opponentSquad;

    private string _userTeamName;
    private string _opponentTeamName;

    private bool _isPlayerHome;
    private bool _isHomeTeamAttacking = true;
    private bool _isInitializing = false;

    private int _currentTurn = 1;

    private Dictionary<Transform, Card> _userCardViews = new Dictionary<Transform, Card>();

    private Dictionary<Transform, Card> _opponentCardViews = new Dictionary<Transform, Card>();

    private Dictionary<string, UserCardData> _userCardsBySlot = new Dictionary<string, UserCardData>();

    private Dictionary<string, PlayerData> _opponentPlayersBySlot = new Dictionary<string, PlayerData>();

    private bool _isShowingOpponentSquad = false;

    private GameObject _opponentChoiceSlotObject;

    private Card _choicePreviewCard;

    private UserCardData _selectedUserCardData;
    private string _selectedUserOverall = string.Empty;

    private PlayerData _selectedOpponentPlayer;
    private string _selectedOpponentOverall = string.Empty;

    public LeagueMatchData CurrentMatch
    {
        get { return _currentMatch; }
    }

    public TeamSquad OpponentSquad
    {
        get { return _opponentSquad; }
    }

    public bool IsPlayerHome
    {
        get { return _isPlayerHome; }
    }

    public bool IsHomeTeamAttacking
    {
        get { return _isHomeTeamAttacking; }
    }

    public bool IsUserTeamAttacking
    {
        get { return _isPlayerHome == _isHomeTeamAttacking; }
    }

    public int CurrentTurn
    {
        get { return _currentTurn; }
    }

    private void OnEnable()
    {
        BindButtons();

        if (panel_UserSquad != null)
        {
            panel_UserSquad.SetActive(true);
        }

        if (panel_OpponentSquad != null)
        {
            panel_OpponentSquad.SetActive(false);
        }

        if (panel_Choice != null)
        {
            panel_Choice.SetActive(false);
        }

        if (panel_OpponentChoice != null)
        {
            panel_OpponentChoice.SetActive(false);
        }

        _isShowingOpponentSquad = false;

        InitializeMatchAsync().Forget();
    }

    private void OnDisable()
    {
        UnbindButtons();
    }

    private void BindButtons()
    {
        if (button_Opponent != null)
        {
            button_Opponent.BindOnClickButtonEvent(OnClickOpponent);
        }

        if (button_OverallFW != null)
        {
            button_OverallFW.BindOnClickButtonEvent(OnClickOverallFW);
        }

        if (button_OverallMF != null)
        {
            button_OverallMF.BindOnClickButtonEvent(OnClickOverallMF);
        }

        if (button_OverallDF != null)
        {
            button_OverallDF.BindOnClickButtonEvent(OnClickOverallDF);
        }

        if (button_Cancel != null)
        {
            button_Cancel.BindOnClickButtonEvent(OnClickCancel);
        }
    }

    private void UnbindButtons()
    {
        if (button_Opponent != null)
        {
            button_Opponent.UnBindOnClickButtonEvent(OnClickOpponent);
        }

        if (button_OverallFW != null)
        {
            button_OverallFW.UnBindOnClickButtonEvent(OnClickOverallFW);
        }

        if (button_OverallMF != null)
        {
            button_OverallMF.UnBindOnClickButtonEvent(OnClickOverallMF);
        }

        if (button_OverallDF != null)
        {
            button_OverallDF.UnBindOnClickButtonEvent(OnClickOverallDF);
        }

        if (button_Cancel != null)
        {
            button_Cancel.UnBindOnClickButtonEvent(OnClickCancel);
        }
    }

    private void OnClickOpponent()
    {
        if (_isShowingOpponentSquad)
        {
            SetSquadView(false);
        }

        else
        {
            SetSquadView(true);
        }

        if (panel_Choice != null)
        {
            panel_Choice.SetActive(false);
        }

        _selectedUserCardData = null;
        _selectedUserOverall = string.Empty;
    }

    private void SetSquadView(bool showOpponent)
    {
        _isShowingOpponentSquad = showOpponent;

        if (panel_UserSquad != null)
        {
            panel_UserSquad.SetActive(showOpponent == false);
        }

        if (panel_OpponentSquad != null)
        {
            panel_OpponentSquad.SetActive(showOpponent);
        }

        if (showOpponent == false)
        {
            RefreshUserCardViews();
        }
    }

    private void RefreshUserCardViews()
    {
        foreach (KeyValuePair<string, UserCardData> item in _userCardsBySlot)
        {
            string slotKey = item.Key;
            UserCardData userCard = item.Value;

            if (userCard == null)
            {
                continue;
            }

            Transform slot = GetUserSlotFromKey(slotKey);

            if (slot == null)
            {
                continue;
            }

            Card card = null;

            if (_userCardViews.TryGetValue(slot, out card) == false)
            {
                continue;
            }

            if (card == null)
            {
                continue;
            }

            card.gameObject.SetActive(true);
            card.SetUpCard(userCard);
            card.EnableCardButton(OnClickUserSquadCard);
        }
    }

    private Transform GetUserSlotFromKey(string slotKey)
    {
        if (slotKey == "GK")
        {
            return _userSlotGK;
        }

        int separatorIndex = slotKey.IndexOf('_');

        if (separatorIndex < 0)
        {
            return null;
        }

        string position = slotKey.Substring(0, separatorIndex);
        string indexText = slotKey.Substring(separatorIndex + 1);

        int slotIndex;

        if (int.TryParse(indexText, out slotIndex) == false)
        {
            return null;
        }

        return GetSquadSlot(true, position, slotIndex);
    }

    private async UniTaskVoid InitializeMatchAsync()
    {
        if (_isInitializing)
        {
            return;
        }

        _isInitializing = true;

        try
        {
            if (DBManager.Instance == null)
            {
                Debug.LogError("[SingleModeUi] DBManager가 존재하지 않습니다.");
                return;
            }

            if (ResourceManager.Instance == null)
            {
                Debug.LogError("[SingleModeUi] ResourceManager가 존재하지 않습니다.");
                return;
            }

            string userId = DBManager.Instance.CurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("[SingleModeUi] 로그인된 유저가 없습니다.");
                return;
            }

            _userTeamName = DBManager.Instance.CurrentUserName;
            _currentMatch = DBManager.Instance.GetCurrentMatchInfo(userId);

            if (_currentMatch == null)
            {
                Debug.LogError("[SingleModeUi] 현재 진행할 경기가 없습니다.");
                return;
            }

            _isPlayerHome = _currentMatch.isPlayerHome;

            if (_isPlayerHome)
            {
                _opponentTeamName = _currentMatch.awayTeam;
            }

            else
            {
                _opponentTeamName = _currentMatch.homeTeam;
            }

            UpdateMatchInfo();
            SetAttackOwner(true);
            SetCurrentTurn(1);

            await LoadTeamLogoAsync(image_LogoHome, _currentMatch.homeTeam);
            await LoadTeamLogoAsync(image_LogoAway, _currentMatch.awayTeam);

            ResetCardViews();

            await LoadUserSquadAsync(userId);
            await LoadOpponentSquadAsync(userId);
            SetSquadView(false);

            Debug.Log($"[SingleModeUi] 경기 준비 완료! " + $"{_currentMatch.homeTeam} VS {_currentMatch.awayTeam}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SingleModeUi] 경기 초기화 실패: {ex.Message}");
        }
        finally
        {
            _isInitializing = false;
        }
    }

    private void UpdateMatchInfo()
    {
        if (_currentMatch == null)
        {
            return;
        }

        if (text_NameHome != null)
        {
            text_NameHome.text = _currentMatch.homeTeam;
        }

        if (text_NameAway != null)
        {
            text_NameAway.text = _currentMatch.awayTeam;
        }

        UpdateMatchScore(
            _currentMatch.homeScore,
            _currentMatch.awayScore
        );
    }

    private async UniTask LoadTeamLogoAsync(Image targetImage, string teamName)
    {
        if (targetImage == null || string.IsNullOrEmpty(teamName))
        {
            return;
        }

        targetImage.sprite = null;
        targetImage.gameObject.SetActive(false);

        Sprite teamSprite = await ResourceManager.Instance.LoadAsset<Sprite>(teamName);

        if (targetImage == null)
        {
            return;
        }

        if (teamSprite != null)
        {
            targetImage.sprite = teamSprite;
            targetImage.gameObject.SetActive(true);
        }

        else
        {
            Debug.LogWarning($"[SingleModeUi] 팀 로고를 찾을 수 없습니다: {teamName}");
        }
    }

    private async UniTask LoadUserSquadAsync(string userId)
    {
        string formation = DBManager.Instance.GetUserFormation(userId);

        if (string.IsNullOrEmpty(formation))
        {
            Debug.LogWarning("[SingleModeUi] 저장된 유저 포메이션이 없습니다.");

            HideSquadSlots(_userSlotGK, _userLineDF, _userLineMF, _userLineFW);

            return;
        }

        int[] dfIndices;
        int[] mfIndices;
        int[] fwIndices;

        if (TryGetFormationIndices(formation, out dfIndices, out mfIndices, out fwIndices) == false)
        {
            Debug.LogError($"[SingleModeUi] 지원하지 않는 유저 포메이션: {formation}");
            return;
        }

        SetFormationVisibility(_userSlotGK, _userLineDF, _userLineMF, _userLineFW, dfIndices, mfIndices, fwIndices);

        Dictionary<string, int> savedSlots = DBManager.Instance.GetUserSquadSlots(userId);
        List<UserCardData> userCards = DBManager.Instance.GetUserCards(userId);
        Dictionary<int, UserCardData> cardDict = new Dictionary<int, UserCardData>();

        for (int i = 0; i < userCards.Count; i++)
        {
            if (userCards[i] != null)
            {
                cardDict[userCards[i].InstanceId] = userCards[i];
            }
        }

        _userCardsBySlot.Clear();

        await LoadUserCardToSlot("GK", _userSlotGK, savedSlots, cardDict);
        await LoadUserLineCards("DF", _userLineDF, dfIndices, savedSlots, cardDict);
        await LoadUserLineCards("MF", _userLineMF, mfIndices, savedSlots, cardDict);
        await LoadUserLineCards("FW", _userLineFW, fwIndices, savedSlots, cardDict);
    }

    private async UniTask LoadUserLineCards(string position, Transform line, int[] activeIndices, Dictionary<string, int> savedSlots, Dictionary<int, UserCardData> cardDict)
    {
        if (line == null)
        {
            return;
        }

        for (int i = 0; i < line.childCount; i++)
        {
            Transform slot = line.GetChild(i);
            bool isActive = ContainsIndex(activeIndices, i);
            slot.gameObject.SetActive(isActive);

            if (isActive)
            {
                string slotKey = $"{position}_{i}";
                await LoadUserCardToSlot(slotKey, slot, savedSlots, cardDict);
            }
        }
    }

    private async UniTask LoadUserCardToSlot(string slotKey, Transform slot, Dictionary<string, int> savedSlots, Dictionary<int, UserCardData> cardDict)
    {
        if (slot == null)
        {
            return;
        }

        UserCardData targetCard = null;

        if (savedSlots.TryGetValue(slotKey, out int instanceId))
        {
            if (instanceId != -1)
            {
                cardDict.TryGetValue(instanceId, out targetCard);
            }
        }

        if (targetCard == null || targetCard.StaticData == null)
        {
            return;
        }

        _userCardsBySlot[slotKey] = targetCard;

        Card cardView = await GetOrCreateCardView(slot, _userCardViews);

        if (cardView != null)
        {
            cardView.gameObject.SetActive(true);
            cardView.SetUpCard(targetCard);
            cardView.EnableCardButton(OnClickUserSquadCard);
        }
    }

    private async UniTask LoadOpponentSquadAsync(string userId)
    {
        _opponentSquad =SingleModeManager.Instance.GetLeagueTeamSquad(_opponentTeamName);

        if (_opponentSquad == null)
        {
            Debug.LogError($"[SingleModeUi] 상대 팀({_opponentTeamName})을 찾을 수 없습니다.");
            HideSquadSlots(_opponentSlotGK, _opponentLineDF, _opponentLineMF, _opponentLineFW);

            return;
        }

        string formation = _opponentSquad.FormationType;

        if (string.IsNullOrEmpty(formation))
        {
            formation = GetFormationFromSquad(_opponentSquad);
        }

        int[] dfIndices;
        int[] mfIndices;
        int[] fwIndices;

        if (TryGetFormationIndices(formation, out dfIndices, out mfIndices, out fwIndices) == false)
        {
            Debug.LogError($"[SingleModeUi] 상대 팀 포메이션을 확인할 수 없습니다: {formation}");
            return;
        }

        SetFormationVisibility(_opponentSlotGK, _opponentLineDF, _opponentLineMF, _opponentLineFW, dfIndices, mfIndices, fwIndices);
       
        BuildOpponentSlotData(_opponentSquad, dfIndices, mfIndices, fwIndices);

        await UniTask.CompletedTask;
    }

    private void BuildOpponentSlotData(TeamSquad squad, int[] dfIndices, int[] mfIndices, int[] fwIndices)
    {
        _opponentPlayersBySlot.Clear();

        List<PlayerData> gks = new List<PlayerData>();
        List<PlayerData> dfs = new List<PlayerData>();
        List<PlayerData> mfs = new List<PlayerData>();
        List<PlayerData> fws = new List<PlayerData>();

        foreach (PlayerData player in squad.StartingLineup)
        {
            if (player.Pos == "GK")
            {
                gks.Add(player);
            }

            else if (player.Pos == "DF")
            {
                dfs.Add(player);
            }

            else if (player.Pos == "MF")
            {
                mfs.Add(player);
            }

            else if (player.Pos == "FW")
            {
                fws.Add(player);
            }
        }

        if (gks.Count > 0)
        {
            _opponentPlayersBySlot["GK"] = gks[0];
        }

        MapOpponentPositionPlayers("DF", dfs, dfIndices);
        MapOpponentPositionPlayers("MF", mfs, mfIndices);
        MapOpponentPositionPlayers("FW", fws, fwIndices);
    }

    private void MapOpponentPositionPlayers(string position, List<PlayerData> players, int[] activeIndices)
    {
        int count = Mathf.Min(players.Count, activeIndices.Length);

        for (int i = 0; i < count; i++)
        {
            string key = $"{position}_{activeIndices[i]}";
            _opponentPlayersBySlot[key] = players[i];
        }
    }

    private async UniTask<Card> GetOrCreateCardView(Transform slot, Dictionary<Transform, Card> cardPool)
    {
        if (slot == null)
        {
            return null;
        }

        if (cardPool.TryGetValue(slot, out Card existingCard))
        {
            if (existingCard != null)
            {
                return existingCard;
            }

            cardPool.Remove(slot);
        }

        GameObject cardObj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, slot, true);

        if (cardObj == null)
        {
            return null;
        }

        Card card = cardObj.GetComponent<Card>();

        if (card == null)
        {
            Debug.LogError("[SingleModeUi] Card 프리팹에 Card 스크립트가 없습니다.");

            Destroy(cardObj);
            return null;
        }

        cardPool.Add(slot, card);

        return card;
    }

    private void ResetCardViews()
    {
        foreach (Card card in _userCardViews.Values)
        {
            if (card != null)
            {
                card.gameObject.SetActive(false);
            }
        }

        foreach (Card card in _opponentCardViews.Values)
        {
            if (card != null)
            {
                card.gameObject.SetActive(false);
            }
        }

        _userCardsBySlot.Clear();
        _opponentPlayersBySlot.Clear();
    }

    public async UniTask<bool> RevealOpponentCard(string position, int slotIndex)
    {
        string slotKey = GetSlotKey(position, slotIndex);

        if (_opponentPlayersBySlot.TryGetValue(slotKey, out PlayerData playerData) == false)
        {
            Debug.LogWarning($"[SingleModeUi] 상대 슬롯의 선수를 찾을 수 없습니다: {slotKey}");
            return false;
        }

        Transform slot = GetSquadSlot(false, position, slotIndex);

        if (slot == null)
        {
            return false;
        }

        Card cardView = await GetOrCreateCardView(slot, _opponentCardViews);

        if (cardView == null)
        {
            return false;
        }

        cardView.gameObject.SetActive(true);
        cardView.SetUpCard(playerData);

        Debug.Log($"[SingleModeUi] 상대 선수 공개: {playerData.Name}");

        return true;
    }

    public UserCardData GetUserCardAt(string position, int slotIndex)
    {
        string slotKey = GetSlotKey(position, slotIndex);

        if (_userCardsBySlot.TryGetValue(slotKey, out UserCardData cardData))
        {
            return cardData;
        }

        return null;
    }

    public PlayerData GetOpponentPlayerAt(string position, int slotIndex)
    {
        string slotKey = GetSlotKey(position, slotIndex);

        if (_opponentPlayersBySlot.TryGetValue(slotKey, out PlayerData playerData))
        {
            return playerData;
        }

        return null;
    }

    private Transform GetSquadSlot(bool isUser, string position, int slotIndex)
    {
        Transform targetGK = isUser ? _userSlotGK : _opponentSlotGK;

        Transform targetDF = isUser ? _userLineDF : _opponentLineDF;

        Transform targetMF = isUser ? _userLineMF : _opponentLineMF;

        Transform targetFW = isUser ? _userLineFW : _opponentLineFW;

        if (position == "GK")
        {
            return targetGK;
        }

        Transform line = null;

        if (position == "DF")
        {
            line = targetDF;
        }

        else if (position == "MF")
        {
            line = targetMF;
        }

        else if (position == "FW")
        {
            line = targetFW;
        }

        if (line == null || slotIndex < 0 || slotIndex >= line.childCount)
        {
            return null;
        }

        return line.GetChild(slotIndex);
    }

    private string GetSlotKey(string position, int slotIndex)
    {
        if (position == "GK")
        {
            return "GK";
        }

        return $"{position}_{slotIndex}";
    }

    private bool TryGetFormationIndices(string formation, out int[] dfIndices, out int[] mfIndices, out int[] fwIndices)
    {
        dfIndices = null;
        mfIndices = null;
        fwIndices = null;

        if (formation == "4-3-3")
        {
            dfIndices = new int[] { 0, 1, 3, 4 };
            mfIndices = new int[] { 0, 2, 4 };
            fwIndices = new int[] { 0, 2, 4 };
        }

        else if (formation == "4-4-2")
        {
            dfIndices = new int[] { 0, 1, 3, 4 };
            mfIndices = new int[] { 0, 1, 3, 4 };
            fwIndices = new int[] { 1, 3 };
        }

        else if (formation == "4-5-1")
        {
            dfIndices = new int[] { 0, 1, 3, 4 };
            mfIndices = new int[] { 0, 1, 2, 3, 4 };
            fwIndices = new int[] { 2 };
        }

        else if (formation == "3-5-2")
        {
            dfIndices = new int[] { 1, 2, 3 };
            mfIndices = new int[] { 0, 1, 2, 3, 4 };
            fwIndices = new int[] { 1, 3 };
        }

        else if (formation == "3-4-3")
        {
            dfIndices = new int[] { 1, 2, 3 };
            mfIndices = new int[] { 0, 1, 3, 4 };
            fwIndices = new int[] { 0, 2, 4 };
        }

        return dfIndices != null;
    }

    private void SetFormationVisibility(Transform slotGK, Transform lineDF, Transform lineMF, Transform lineFW, int[] dfIndices, int[] mfIndices, int[] fwIndices)
    {
        if (slotGK != null)
        {
            slotGK.gameObject.SetActive(true);
        }

        SetLineVisibility(lineDF, dfIndices);
        SetLineVisibility(lineMF, mfIndices);
        SetLineVisibility(lineFW, fwIndices);
    }

    private void SetLineVisibility(Transform line, int[] activeIndices)
    {
        if (line == null)
        {
            return;
        }

        for (int i = 0; i < line.childCount; i++)
        {
            bool isActive = ContainsIndex(activeIndices, i);
            line.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    private bool ContainsIndex(int[] indices, int targetIndex)
    {
        if (indices == null)
        {
            return false;
        }

        for (int i = 0; i < indices.Length; i++)
        {
            if (indices[i] == targetIndex)
            {
                return true;
            }
        }

        return false;
    }

    private void HideSquadSlots(Transform slotGK, Transform lineDF, Transform lineMF, Transform lineFW)
    {
        if (slotGK != null)
        {
            slotGK.gameObject.SetActive(false);
        }

        HideLine(lineDF);
        HideLine(lineMF);
        HideLine(lineFW);
    }

    private void HideLine(Transform line)
    {
        if (line == null)
        {
            return;
        }

        for (int i = 0; i < line.childCount; i++)
        {
            line.GetChild(i).gameObject.SetActive(false);
        }
    }

    private string GetFormationFromSquad(TeamSquad squad)
    {
        int df = 0;
        int mf = 0;
        int fw = 0;

        foreach (PlayerData player in squad.StartingLineup)
        {
            if (player.Pos == "DF")
            {
                df++;
            }

            else if (player.Pos == "MF")
            {
                mf++;
            }

            else if (player.Pos == "FW")
            {
                fw++;
            }
        }

        if (df == 4 && mf == 3 && fw == 3)
        {
            return "4-3-3";
        }

        if (df == 4 && mf == 4 && fw == 2)
        {
            return "4-4-2";
        }

        if (df == 4 && mf == 5 && fw == 1)
        {
            return "4-5-1";
        }

        if (df == 3 && mf == 5 && fw == 2)
        {
            return "3-5-2";
        }

        if (df == 3 && mf == 4 && fw == 3)
        {
            return "3-4-3";
        }

        return string.Empty;
    }

    public void SetCurrentTurn(int turn)
    {
        _currentTurn = turn;

        if (text_Turns != null)
        {
            text_Turns.text = $"현재 턴: {_currentTurn}";
        }
    }

    public void UpdateMatchScore(int homeScore, int awayScore)
    {
        if (text_PointsHome != null)
        {
            text_PointsHome.text = homeScore.ToString();
        }

        if (text_PointsAway != null)
        {
            text_PointsAway.text = awayScore.ToString();
        }
    }

    public void SetAttackOwner(bool isHomeTeam)
    {
        _isHomeTeamAttacking = isHomeTeam;

        if (image_AttackHome != null)
        {
            image_AttackHome.gameObject.SetActive(isHomeTeam);
        }

        if (image_AttackAway != null)
        {
            image_AttackAway.gameObject.SetActive(!isHomeTeam);
        }
    }

    private void OnClickUserSquadCard(Card clickedCard)
    {
        if (_isShowingOpponentSquad)
        {
            return;
        }

        if (clickedCard == null)
        {
            return;
        }

        if (clickedCard.TargetUserCardData == null)
        {
            return;
        }

        _selectedUserCardData = clickedCard.TargetUserCardData;
        _selectedUserOverall = string.Empty;

        OpenUserChoicePanelAsync().Forget();
    }

    private async UniTaskVoid OpenUserChoicePanelAsync()
    {
        if (_selectedUserCardData == null)
        {
            return;
        }

        PlayerData playerData = _selectedUserCardData.StaticData;

        if (playerData == null)
        {
            Debug.LogError("[SingleModeUi] 선수 데이터가 없습니다.");
            return;
        }

        if (panel_Choice == null || card_Spawn == null)
        {
            Debug.LogError("[SingleModeUi] Panel_Choice 연결을 확인해주세요.");
            return;
        }

        panel_Choice.SetActive(true);

        if (_choicePreviewCard == null)
        {
            GameObject cardObj = await ResourceManager.Instance.Instantiate(cardPrefabAddress, card_Spawn, true);

            if (cardObj == null)
            {
                return;
            }

            _choicePreviewCard = cardObj.GetComponent<Card>();
        }

        if (_choicePreviewCard == null)
        {
            return;
        }

        _choicePreviewCard.gameObject.SetActive(true);
        _choicePreviewCard.SetUpCard(_selectedUserCardData);
        _choicePreviewCard.DisableCardButton();

        if (text_OverallFW != null)
        {
            text_OverallFW.text = "FW";
        }

        if (text_OverallMF != null)
        {
            text_OverallMF.text = "MF";
        }

        if (text_OverallDF != null)
        {
            text_OverallDF.text = "DF";
        }

        if (text_SkillFW != null)
        {
            text_SkillFW.text = GetSkillDescription(_selectedUserCardData.SkillFwData);
        }

        if (text_SkillMF != null)
        {
            text_SkillMF.text = GetSkillDescription(_selectedUserCardData.SkillMfData);
        }

        if (text_SkillDF != null)
        {
            text_SkillDF.text = GetSkillDescription(_selectedUserCardData.SkillDfData);
        }
    }

    private string GetSkillDescription(SkillData skillData)
    {
        if (skillData == null)
        {
            return "스킬 정보 없음";
        }

        return skillData.Description;
    }


    private void OnClickOverallFW()
    {
        SelectUserOverall("FW");
    }

    private void OnClickOverallMF()
    {
        SelectUserOverall("MF");
    }

    private void OnClickOverallDF()
    {
        SelectUserOverall("DF");
    }

    private void SelectUserOverall(string overallType)
    {
        if (_selectedUserCardData == null)
        {
            return;
        }

        _selectedUserOverall = overallType;

        if (panel_Choice != null)
        {
            panel_Choice.SetActive(false);
        }

        Debug.Log("[SingleModeUi] 유저 능력치 선택: " + _selectedUserCardData.StaticData.Name +" / " + overallType);
    }

    private void OnClickCancel()
    {
        _selectedUserCardData = null;
        _selectedUserOverall = string.Empty;

        if (panel_Choice != null)
        {
            panel_Choice.SetActive(false);
        }
    }


    public void ShowOpponentChoice(PlayerData playerData, string overallType)
    {
        if (playerData == null)
        {
            return;
        }

        if (IsUserTeamAttacking)
        {
            return;
        }

        if (overallType != "FW" && overallType != "MF" && overallType != "DF")
        {
            Debug.LogError("[SingleModeUi] 잘못된 능력치 선택입니다.");
            return;
        }

        Transform sourceSlot = FindOpponentPlayerSlot(playerData);

        if (sourceSlot == null)
        {
            Debug.LogError("[SingleModeUi] 상대 포지션 슬롯을 찾지 못했습니다.");
            return;
        }

        if (opponentCard_Locate == null)
        {
            Debug.LogError("[SingleModeUi] OpponentCard_Locate가 연결되지 않았습니다.");
            return;
        }

        _selectedOpponentPlayer = playerData;
        _selectedOpponentOverall = overallType;

        if (_opponentChoiceSlotObject != null)
        {
            Destroy(_opponentChoiceSlotObject);
            _opponentChoiceSlotObject = null;
        }

        _opponentChoiceSlotObject = Instantiate(sourceSlot.gameObject, opponentCard_Locate, false);

        _opponentChoiceSlotObject.name = "OpponentChoicePositionSlot";

        RectTransform copiedRect = _opponentChoiceSlotObject.GetComponent<RectTransform>();

        if (copiedRect != null)
        {
            copiedRect.anchorMin = Vector2.zero;
            copiedRect.anchorMax = Vector2.one;
            copiedRect.offsetMin = Vector2.zero;
            copiedRect.offsetMax = Vector2.zero;
        }

        UpdateOpponentOverallText(overallType);

        if (panel_OpponentChoice != null)
        {
            panel_OpponentChoice.SetActive(true);
        }
    }

    private Transform FindOpponentPlayerSlot(PlayerData playerData)
    {
        foreach (KeyValuePair<string, PlayerData> item in _opponentPlayersBySlot)
        {
            if (item.Value == null)
            {
                continue;
            }

            if (item.Value.Id == playerData.Id)
            {
                if (item.Key == "GK")
                {
                    return _opponentSlotGK;
                }

                int separatorIndex = item.Key.IndexOf('_');

                if (separatorIndex < 0)
                {
                    return null;
                }

                string position = item.Key.Substring(0, separatorIndex);
                string indexText = item.Key.Substring(separatorIndex + 1);

                int slotIndex;

                if (int.TryParse(indexText, out slotIndex) == false)
                {
                    return null;
                }

                return GetSquadSlot(false, position, slotIndex);
            }
        }

        return null;
    }

    private void UpdateOpponentOverallText(string overallType)
    {
        if (text_ChooseOverall == null)
        {
            return;
        }

        if (overallType == "FW")
        {
            text_ChooseOverall.text = "FW";
            text_ChooseOverall.color = Color.red;
        }

        else if (overallType == "MF")
        {
            text_ChooseOverall.text = "MF";
            text_ChooseOverall.color = Color.green;
        }

        else if (overallType == "DF")
        {
            text_ChooseOverall.text = "DF";
            text_ChooseOverall.color = Color.blue;
        }
    }

}