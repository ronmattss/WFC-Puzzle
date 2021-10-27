namespace ProjectAssets.Scripts.Fuzzy
{
    public interface IDefuzzify
    {
        float Defuzzify( FuzzyOutput fuzzyOutput, INorm normOperator );

    }
}