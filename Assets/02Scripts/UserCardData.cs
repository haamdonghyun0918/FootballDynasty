using System;

[Serializable]
public class UserCardData
{
    public int InstanceId;
    public string UserId;
    public string PlayerId;
    public string SkillFWId;
    public string SkillMFId;
    public string SkillDFId;

    public PlayerData StaticData
    {
        get
        {
            return GameDataManager.Instance.GetData<PlayerData>(PlayerId);
        }
    }

    public SkillData SkillFwData
    {
        get
        {
            return GameDataManager.Instance.GetData<SkillData>(SkillFWId);
        }
    }

    public SkillData SkillMfData
    {
        get
        {
            return GameDataManager.Instance.GetData<SkillData>(SkillMFId);
        }
    }

    public SkillData SkillDfData
    {
        get
        {
            return GameDataManager.Instance.GetData<SkillData>(SkillDFId);
        }
    }
}