using System;

[Serializable]
public class GameDataBase
{
    public string Id;
}

[Serializable]
public class PlayerData : GameDataBase
{
    public string Name;
    public int Att;
    public int Mid;
    public int Def;
    public string Pos;
    public string League;
    public string Team;
    public string Season;
}