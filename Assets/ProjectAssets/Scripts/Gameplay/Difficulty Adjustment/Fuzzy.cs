namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment

{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using ProjectAssets.Scripts.Util;
    using UnityEngine;
    using ProjectAssets.Scripts.Fuzzy;


    [Serializable]
    public class Fuzzy
    {
        // This class is a test for Fuzzy Logic Implementation with Animation Curve

        public MembershipFunction[] members;
        public float[] points;



        public FuzzySet fsGreat, fsAverage, fsClear;
        public LinguisticVariable clearMoves;
        public float[] moves = {15, 20, 25, 30}; // sample moves, we 
        
        public FuzzySet fsFast, fsNormal, fsSlow,fsFailed;
        public LinguisticVariable clearTime;
        public float[] time = {0, 10, 30, 40};
        
        public FuzzySet fsPlenty, fsMinimal, fsNone,fsZero;
        public LinguisticVariable newMoves;
        public float[] moveIncrease = {0, 5, 7, 15};

        private DataBank db;
        private Inference infSystem;
        public void SetMoves()
        {
            var x1 = moves[0];
            var x2 = moves[1];
            var x3 = moves[2];
            var x4 = moves[3];
            
            // Gonna Tweak This Tomorrow
            fsGreat = new FuzzySet("Great",new TrapezoidFunction(x2,x1,TrapezoidFunction.EdgeType.Left)); // m1 min  m2 max
            fsAverage = new FuzzySet("Average",new TrapezoidFunction(x2,x3,x4));
            fsClear = new FuzzySet("Clear",new TrapezoidFunction(x4,x3,TrapezoidFunction.EdgeType.Right));// m1 max  m2 min 
            
            clearMoves = new LinguisticVariable("clearMoves",15,35);
            clearMoves.AddLabel(fsGreat);
            clearMoves.AddLabel(fsAverage);
            clearMoves.AddLabel(fsClear);

        }


        
        public void SetTime()
        {
            var x1 = time[0];
            var x2 = time[1];
            var x3 = time[2];
            var x4 = time[3];

            fsFailed = new FuzzySet("Failed",new TrapezoidFunction(x1,x1,TrapezoidFunction.EdgeType.Left));
            fsSlow = new FuzzySet("Slow",new TrapezoidFunction(x1,x2, TrapezoidFunction.EdgeType.Left));
            fsNormal = new FuzzySet("Normal",new TrapezoidFunction(x2,x3,x4));
            fsFast = new FuzzySet("Fast",new TrapezoidFunction(x3,x4,TrapezoidFunction.EdgeType.Right));
            clearTime = new LinguisticVariable("clearTime",0,40);
            clearTime.AddLabel(fsFast);
            clearTime.AddLabel(fsNormal);
            clearTime.AddLabel(fsSlow);
            clearTime.AddLabel(fsFailed);

            // members[0].curve.AddKey(x1, 1);
            // members[0].curve.AddKey(x2, 0);
            // members[1].curve.AddKey(x2, 0);
            // members[1].curve.AddKey(x3, 1);
            // members[1].curve.AddKey(x4, 0);
            // members[2].curve.AddKey(x3, 0);
            // members[2].curve.AddKey(x4, 1);

        }
        
        public void SetIncrementalMoves()
        {
            var x1 = moveIncrease[0]; // 0
            var x2 = moveIncrease[1]; // 5
            var x3 = moveIncrease[2]; // 10
            var x4 = moveIncrease[3]; // 15

            fsZero = new FuzzySet("Zero",new TrapezoidFunction(x1,x1,TrapezoidFunction.EdgeType.Left));
            fsNone = new FuzzySet("None",new TrapezoidFunction(x1,x2, TrapezoidFunction.EdgeType.Left));
            fsMinimal = new FuzzySet("Minimal",new TrapezoidFunction(x2,x3,x4));
            fsPlenty = new FuzzySet("Plenty",new TrapezoidFunction(x3,x4,TrapezoidFunction.EdgeType.Right));
            
            newMoves = new LinguisticVariable("moveIncrement",0,15);
            newMoves.AddLabel(fsNone);
            newMoves.AddLabel(fsZero);
            newMoves.AddLabel(fsMinimal);
            newMoves.AddLabel(fsPlenty);
        }

        public void AddToDatabase()
        {
            db = new DataBank();
            db.AddVariable(clearMoves);
            db.AddVariable(clearTime);
            db.AddVariable(newMoves);
            
            infSystem = new Inference(db, new CentroidBasedDefuzzifier(2));
            // test, assumes that that clearMoves is opposite of the clearTime
            // infSystem.NewRule("Rule 1", "IF clearMoves IS Great THEN clearTime IS Slow");
            // infSystem.NewRule("Rule 2", "IF clearMoves IS Average THEN clearTime IS Normal");
            // infSystem.NewRule("Rule 3", "IF clearMoves IS Clear THEN clearTime IS Fast");
            // Plenty Minimal None
            
            // The system will now guess
            
            infSystem.NewRule("Rule 1", "IF clearMoves IS Great AND clearTime IS Fast THEN moveIncrement IS Plenty");
            infSystem.NewRule("Rule 2", "IF clearMoves IS Great AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 3", "IF clearMoves IS Great AND clearTime IS Slow THEN moveIncrement IS None");

            infSystem.NewRule("Rule 4", "IF clearMoves IS Average AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 5", "IF clearMoves IS Average AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 6", "IF clearMoves IS Average AND clearTime IS Slow THEN moveIncrement IS None");
            
            infSystem.NewRule("Rule 7", "IF clearMoves IS Clear AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 8", "IF clearMoves IS Clear AND clearTime IS Normal THEN moveIncrement IS None");
            infSystem.NewRule("Rule 9", "IF clearMoves IS Clear AND clearTime IS Slow THEN moveIncrement IS None");
            
            infSystem.NewRule("Rule 10", "IF clearMoves IS Great AND clearTime IS Failed THEN moveIncrement IS Zero");
            infSystem.NewRule("Rule 11", "IF clearMoves IS Average AND clearTime IS Failed THEN moveIncrement IS Zero");
            infSystem.NewRule("Rule 12", "IF clearMoves IS Clear AND clearTime IS Failed THEN moveIncrement IS Zero");




            
            
        }

        public float AcceptInput(float move,float timeInput)
        {
            infSystem.SetInput("clearMoves",move);
            infSystem.SetInput("clearTime",timeInput);

            return infSystem.Evaluate("moveIncrement");
        }
        
        
        
    }

    /*
     *         /// <summary>
        /// Accepts a Crisp input then outputs a fuzzified set of values
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<float> EvaluateInput(float input)
        {// X is time Y is Value
            var points = new List<float>();

            foreach (var curve in members)
            {
                points.Add(curve.curve.Evaluate(input));
            }

            return points;
        }
     */


    [Serializable]
    public class MembershipFunction
    {
        public AnimationCurve curve;
        public string linguisticVariable;
    }
    
    
}