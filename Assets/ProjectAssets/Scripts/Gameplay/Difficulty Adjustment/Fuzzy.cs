namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment

{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ProjectAssets.Scripts.Util;
    using UnityEngine;
    
    
    [Serializable]
    public class Fuzzy
    {
        // This class is a test for Fuzzy Logic Implementation with Animation Curve

        public AnimationCurve curve;
        public string curveRule;
        public float[] points;
    }
}