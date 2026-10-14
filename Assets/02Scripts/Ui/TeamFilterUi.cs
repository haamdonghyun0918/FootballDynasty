using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TeamFilterUi : UiBase
{
    [SerializeField] private Transform _teamContent;
    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonConfirm;

    private string teamSlotPrefabAddress = "TeamSlot";

    private List<string> _teamList = new List<string>();
    private List<TeamSlot> _slotPool = new List<TeamSlot>();

    private const int _initialPoolSize = 21;

    private CardDictionaryUi _cardDictionaryUi;
    private bool _isInitialized = false;

    private string _selectedTeamName = string.Empty;

    public void Init(CardDictionaryUi cardDictionaryUi, string currentSelectedTeam)
    {
        _cardDictionaryUi = cardDictionaryUi;
        _selectedTeamName = currentSelectedTeam;
        InitializeAndRefresh().Forget();
    }

    private void OnEnable()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonConfirm != null)
        {
            _buttonConfirm.BindOnClickButtonEvent(OnClickConfirm);
        }
    }

    private async UniTaskVoid InitializeAndRefresh()
    {
        var players = GameDataManager.Instance.GetAllData<PlayerData>();
        HashSet<string> uniqueTeams = new HashSet<string>();

        if (players != null)
        {
            foreach (var player in players)
            {
                if (string.IsNullOrEmpty(player.Team) == false)
                {
                    uniqueTeams.Add(player.Team);
                }
            }
        }

        _teamList.Clear();
        _teamList.AddRange(uniqueTeams);

        if (_isInitialized == false)
        {
            int createCount = Mathf.Max(_initialPoolSize, _teamList.Count);
            for (int i = 0; i < createCount; i++)
            {
                GameObject slotObj = await ResourceManager.Instance.Instantiate(teamSlotPrefabAddress, _teamContent);
                if (slotObj != null && slotObj.TryGetComponent<TeamSlot>(out var slot))
                {
                    _slotPool.Add(slot);
                    slot.gameObject.SetActive(false);
                }
            }
            _isInitialized = true;
        }

        RefreshSlots();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < _slotPool.Count; i++)
        {
            if (i < _teamList.Count)
            {
                _slotPool[i].gameObject.SetActive(true);
                _slotPool[i].SetUpTeamSlot(_teamList[i], _selectedTeamName, OnSelectedTeam);
            }

            else
            {
                _slotPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnSelectedTeam(string teamName)
    {
        _selectedTeamName = teamName;
        RefreshSlots();
    }

    private void OnClickConfirm()
    {
        if (_cardDictionaryUi != null)
        {
            _cardDictionaryUi.ApplyTeamFilter(_selectedTeamName);
        }

        UiManager.Instance.CloseUi<TeamFilterUi>();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<TeamFilterUi>();
    }
}