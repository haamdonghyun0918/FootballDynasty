using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamStanding
{
    public string teamName;
    public bool isPlayer;
    public int wins;
    public int draws;
    public int losses;
    public int points;

    public int GetMatchesPlayed()
    {
        return wins + draws + losses;
    }
}

public class LeagueTeamSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_Rank;
    [SerializeField] private Image image_Logo;
    [SerializeField] private TextMeshProUGUI text_Team;
    [SerializeField] private TextMeshProUGUI text_Match;
    [SerializeField] private TextMeshProUGUI text_Win;
    [SerializeField] private TextMeshProUGUI text_Draw;
    [SerializeField] private TextMeshProUGUI text_Lose;
    [SerializeField] private TextMeshProUGUI text_Point;

    public void SetSlotData(int rank, TeamStanding standing)
    {
        if (text_Rank != null)
        {
            text_Rank.text = rank.ToString();
        }

        if (text_Team != null)
        {
            text_Team.text = standing.teamName;
        }

        if (text_Match != null)
        {
            text_Match.text = standing.GetMatchesPlayed().ToString();
        }

        if (text_Win != null)
        {
            text_Win.text = standing.wins.ToString();
        }

        if (text_Draw != null)
        {
            text_Draw.text = standing.draws.ToString();
        }

        if (text_Lose != null)
        {
            text_Lose.text = standing.losses.ToString();
        }

        if (text_Point != null)
        {
            text_Point.text = standing.points.ToString();
        }

        LoadTeamLogoAsync(standing.teamName).Forget();
    }

    private async UniTaskVoid LoadTeamLogoAsync(string teamName)
    {
        if (image_Logo == null)
        {
            return;
        }

        Sprite sprite = await ResourceManager.Instance.LoadAsset<Sprite>(teamName);
        if (sprite != null)
        {
            image_Logo.sprite = sprite;
            image_Logo.gameObject.SetActive(true);
        }

        else
        {
            image_Logo.gameObject.SetActive(false);
        }
    }
}