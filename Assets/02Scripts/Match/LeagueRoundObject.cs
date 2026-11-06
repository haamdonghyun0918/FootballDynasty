using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeagueRoundObject : MonoBehaviour
{
    [Header("Home Team UI")]
    [SerializeField] private Image image_HomeLogo;
    [SerializeField] private TextMeshProUGUI text_HomeName;
    [SerializeField] private TextMeshProUGUI text_HomePoint;

    [Header("Away Team UI")]
    [SerializeField] private Image image_AwayLogo;
    [SerializeField] private TextMeshProUGUI text_AwayName;
    [SerializeField] private TextMeshProUGUI text_AwayPoint;

    public void SetMatchData(string homeName, string awayName, int homeScore, int awayScore)
    {
        if (text_HomeName != null)
        {
            text_HomeName.text = homeName;
        }

        if (text_AwayName != null)
        {
            text_AwayName.text = awayName;
        }

        if (text_HomePoint != null)
        {
            text_HomePoint.text = homeScore.ToString();
        }

        if (text_AwayPoint != null)
        {
            text_AwayPoint.text = awayScore.ToString();
        }

        LoadMatchLogosAsync(homeName, awayName).Forget();
    }

    private async UniTaskVoid LoadMatchLogosAsync(string homeName, string awayName)
    {
        if (image_HomeLogo != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeName);
            image_HomeLogo.sprite = homeSprite;
        }

        if (image_AwayLogo != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayName);
            image_AwayLogo.sprite = awaySprite;
        }
    }
}