using System;

[Serializable]
public class UserCardData
{
    public int InstanceId;
    public string UserId;
    public string PlayerId;

    public PlayerData StaticData
    {
        get
        {
            return GameDataManager.Instance.GetData<PlayerData>(PlayerId);
        }
    }
}