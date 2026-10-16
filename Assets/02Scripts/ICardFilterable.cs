public interface ICardFilterable
{
    void ApplyPositionFilter(bool total, bool fw, bool mf, bool df, bool gk);
    void ApplyTeamFilter(string teamName);
}