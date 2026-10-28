using System;

[Serializable]
public class GameDataBase
{
    public string Id;
    public string Name;
}

[Serializable]
public class PlayerData : GameDataBase
{
    public int Att;
    public int Mid;
    public int Def;
    public string Pos;
    public string League;
    public string Team;
    public string Season;
    public string ImageAddress;
}

[Serializable]
public class SkillData : GameDataBase
{
    public string Pos;
    public string Description;
    public int Increase;
    public int Decrease;
    public string ImageAddress;
}