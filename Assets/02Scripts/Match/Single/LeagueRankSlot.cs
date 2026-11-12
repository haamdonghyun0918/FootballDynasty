using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LeagueRankSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Rank;
    [SerializeField] private Image image_TeamLogo;
    [SerializeField] private TMP_Text text_Team;
    [SerializeField] private TMP_Text text_Rounds;
    [SerializeField] private TMP_Text text_Wins;
    [SerializeField] private TMP_Text text_Draws;
    [SerializeField] private TMP_Text text_Losses;
    [SerializeField] private TMP_Text text_Points;

    public void SetData(int rank, string teamName, int wins, int draws, int losses, int points)
    {
        if (text_Rank != null)
        {
            text_Rank.text = rank.ToString();
        }

        if (text_Team != null)
        {
            text_Team.text = teamName;
        }

        if (text_Wins != null)
        {
            text_Wins.text = wins.ToString();
        }

        if (text_Draws != null)
        {
            text_Draws.text = draws.ToString();
        }

        if (text_Losses != null)
        {
            text_Losses.text = losses.ToString();
        }

        if (text_Points != null)
        {
            text_Points.text = points.ToString();
        }

        LoadTeamLogoAsync(teamName).Forget();
    }

    private async UniTaskVoid LoadTeamLogoAsync(string teamName)
    {
        if (image_TeamLogo == null)
        {
            return;
        }

        Sprite teamSprite = await ResourceManager.Instance.LoadAsset<Sprite>(teamName);

        if (teamSprite != null)
        {
            image_TeamLogo.sprite = teamSprite;
            image_TeamLogo.gameObject.SetActive(true);
        }

        else
        {
            image_TeamLogo.gameObject.SetActive(false);
            Debug.Log($"팀 로고를 찾을 수 없습니다: {teamName}");
        }
    }
}