namespace ProjectAssets.Scripts.Fuzzy
{    using System;
    public class MinimumNorm : INorm
    {
        public float Evaluate( float membershipA, float membershipB )
        {
            return Math.Min( membershipA, membershipB );
        }
    }
}