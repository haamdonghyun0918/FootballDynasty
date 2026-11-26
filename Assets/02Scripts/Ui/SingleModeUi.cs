
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
        InitializeMatchAsync().Forget();
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

        RectTransform rect = cardObj.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
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
}