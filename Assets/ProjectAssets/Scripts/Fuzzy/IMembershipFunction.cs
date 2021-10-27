namespace ProjectAssets.Scripts.Fuzzy
{
    public interface IMembershipFunction
    {
        float GetMembership(float x);
        float LeftLimit { get; }
        float RightLimit { get; }
    }
}