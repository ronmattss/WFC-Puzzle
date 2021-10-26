namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment

{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ProjectAssets.Scripts.Util;
    using UnityEngine;
    using AForge.Fuzzy;


    [Serializable]
    public class Fuzzy
    {
        // This class is a test for Fuzzy Logic Implementation with Animation Curve

        public AnimationCurve[] curve;
        public string curveRule;
        public float[] points;

        public void GetCoordinate()
        {// X is time Y is Value
           var x1 = curve[0].Evaluate(.23f);
           var x2 = curve[1].Evaluate(.23f);
           var x3 = curve[2].Evaluate(.23f);
        }
    }
}