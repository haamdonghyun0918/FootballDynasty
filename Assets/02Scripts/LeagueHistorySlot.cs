using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class LeagueHistorySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_Home;
    [SerializeField] private TextMeshProUGUI text_Away;
    [SerializeField] private TextMeshProUGUI text_HomePoint;
    [SerializeField] private TextMeshProUGUI text_AwayPoint;

    [SerializeField] private TextMeshProUGUI text_HomeResult;
    [SerializeField] private TextMeshProUGUI text_AwayResult;

    [SerializeField] private Image image_Home;
    [SerializeField] private Image image_Away;

    public void SetSlotData(string homeName, string awayName, int homeScore, int awayScore, string playerTeamName)
    {
        if (text_Home != null)
        {
            text_Home.text = homeName;
        }

        if (text_Away != null)
        {
            text_Away.text = awayName;
        }

        if (text_HomePoint != null)
        {
            text_HomePoint.text = homeScore.ToString();
        }

        if (text_AwayPoint != null)
        {
            text_AwayPoint.text = awayScore.ToString();
        }

        if (text_HomeResult != null)
        {
            if (homeScore > awayScore)
            {
                text_HomeResult.text = "Win";
                text_HomeResult.color = Color.red;
            }

            else if (homeScore < awayScore)
            {
                text_HomeResult.text = "Lose";
                text_HomeResult.color = Color.blue;
            }

            else
            {
                text_HomeResult.text = "Draw";
                text_HomeResult.color = Color.white;
            }
        }

        if (text_AwayResult != null)
        {
            if (awayScore > homeScore)
            {
                text_AwayResult.text = "Win";
                text_AwayResult.color = Color.red;
            }

            else if (awayScore < homeScore)
            {
                text_AwayResult.text = "Lose";
                text_AwayResult.color = Color.blue;
            }

            else
            {
                text_AwayResult.text = "Draw";
                text_AwayResult.color = Color.white;
            }
        }

        LoadLogosAsync(homeName, awayName).Forget();
    }

    private async UniTaskVoid LoadLogosAsync(string homeName, string awayName)
    {
        if (image_Home != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            image_Home.sprite = homeSprite;
        }

        if (image_Away != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            image_Away.sprite = awaySprite;
        }
    }
}