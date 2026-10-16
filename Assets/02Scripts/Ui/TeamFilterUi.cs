using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TeamFilterUi : UiBase
{
    [SerializeField] private Transform _teamContent;
    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonConfirm;
    [SerializeField] private TMP_InputField inputField_TeamName;

    private string teamSlotPrefabAddress = "TeamSlot";

    private List<string> _teamList = new List<string>();
    private List<TeamSlot> _slotPool = new List<TeamSlot>();

    private const int _initialPoolSize = 21;

    private ICardFilterable _applyTarget;
    private bool _isInitialized = false;

    private string _selectedTeamName = string.Empty;

    public void Init(ICardFilterable applyTarget, string currentSelectedTeam)
    {
        _applyTarget = applyTarget;
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

        if (inputField_TeamName != null)
        {
            inputField_TeamName.text = string.Empty;
            inputField_TeamName.onSelect.AddListener(OnTeamNameSelect);
            inputField_TeamName.onEndEdit.AddListener(OnTeamNameEndEdit);
        }
    }

    private void OnDisable()
    {
        if (inputField_TeamName != null)
        {
            inputField_TeamName.onSelect.RemoveListener(OnTeamNameSelect);
            inputField_TeamName.onEndEdit.RemoveListener(OnTeamNameEndEdit);
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
        string searchKeyword = string.Empty;

        if (inputField_TeamName != null)
        {
            searchKeyword = inputField_TeamName.text.Trim();
        }

        List<string> filteredTeams = new List<string>();

        foreach (var team in _teamList)
        {
            bool matchesTeam = string.IsNullOrEmpty(searchKeyword) || (team != null && team.Contains(searchKeyword));
            if (matchesTeam)
            {
                filteredTeams.Add(team);
            }
        }

        for (int i = 0; i < _slotPool.Count; i++)
        {
            if (i < filteredTeams.Count)
            {
                _slotPool[i].gameObject.SetActive(true);
                _slotPool[i].SetUpTeamSlot(filteredTeams[i], _selectedTeamName, OnSelectedTeam);
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

    private void OnTeamNameSelect(string text)
    {
        if (inputField_TeamName != null)
        {
            inputField_TeamName.text = string.Empty;
        }
    }

    private void OnTeamNameEndEdit(string keyword)
    {
        RefreshSlots();
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnClickConfirm()
    {
        if (_applyTarget != null)
        {
            _applyTarget.ApplyTeamFilter(_selectedTeamName);
        }

        UiManager.Instance.CloseUi<TeamFilterUi>();
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<TeamFilterUi>();
    }
}