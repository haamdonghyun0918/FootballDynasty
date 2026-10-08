using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("Card Text")]
    [SerializeField] private TMP_Text text_Name;
    [SerializeField] private TMP_Text text_Season;
    [SerializeField] private TMP_Text text_Attoverall;
    [SerializeField] private TMP_Text text_Midoverall;
    [SerializeField] private TMP_Text text_Defoverall;

    public void SetUpCard(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogError("선수 데이터가 존재하지 않습니다.");
            return;
        }

        text_Name.text = playerData.Name;
        text_Season.text = playerData.Season;

        text_Attoverall.text = playerData.Att.ToString();
        text_Midoverall.text = playerData.Mid.ToString();
        text_Defoverall.text = playerData.Def.ToString();
    }
}