using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchHistorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Round;
    [SerializeField] private Image image_Home;
    [SerializeField] private TMP_Text text_HomeName;
    [SerializeField] private TMP_Text text_HomeScore;
    [SerializeField] private Image image_Away;
    [SerializeField] private TMP_Text text_AwayName;
    [SerializeField] private TMP_Text text_AwayScore;

    public async void SetData(int roundIndex, string homeTeam, string awayTeam, int homeScore, int awayScore)
    {
        if (text_Round != null)
        {
            text_Round.text = $"ROUND {roundIndex}";
        }

        if (text_HomeName != null)
        {
            text_HomeName.text = homeTeam;
        }

        if (text_AwayName != null)
        {
            text_AwayName.text = awayTeam;
        }

        if (text_HomeScore != null)
        {
            text_HomeScore.text = homeScore.ToString();
        }

        if (text_AwayScore != null)
        {
            text_AwayScore.text = awayScore.ToString();
        }

        if (image_Home != null)
        {
            Sprite homeSprite = await ResourceManager.Instance.LoadAsset<Sprite>(homeTeam);
            
            if (homeSprite != null)
            {
                image_Home.sprite = homeSprite;
                image_Home.gameObject.SetActive(true);
            }

            else
            {
                image_Home.gameObject.SetActive(false);
            }
        }

        if (image_Away != null)
        {
            Sprite awaySprite = await ResourceManager.Instance.LoadAsset<Sprite>(awayTeam);

            if (awaySprite != null)
            {
                image_Away.sprite = awaySprite;
                image_Away.gameObject.SetActive(true);
            }

            else
            {
                image_Away.gameObject.SetActive(false);
            }
        }
    }
}