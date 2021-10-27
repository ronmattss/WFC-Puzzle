namespace ProjectAssets.Scripts.Fuzzy
{    using System;

    public class MaximumCoNorm : ICoNorm
    {
        
        public float Evaluate(float membershipA, float membershipB)
        {
            return Math.Max( membershipA, membershipB );

        }
    }
}