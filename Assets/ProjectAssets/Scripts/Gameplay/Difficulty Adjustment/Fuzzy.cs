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
        
        
        public FuzzySet fsLow, fsMid, fsHigh;
        public LinguisticVariable levelRating;
       // public float[] time = {0, 10, 30, 40};

        private DataBank db;
        private Inference infSystem;
        
        
        
        
        
        // level Rating?
        // Board Size
        // Minimum Moves Per Board

        // Maximum Moves Per Board
        public void SetLevelRating()
        {
            var x1 = 10; // EM
            var x2 = 30; // EM / .93
            var x3 = 40; // EM / .75
            var x4 =60; // EM /.62
            var x5 =70; // EM / .6
            var x6 =80; // EM / .51
            var x7 = 90; // EM / .5
            
            fsLow = new FuzzySet("Great",new TrapezoidFunction(x1,x4,TrapezoidFunction.EdgeType.Right)); // m1 min  m2 max
            fsMid = new FuzzySet("Average",new TrapezoidFunction(x2,x3,x6,x7));
            fsHigh = new FuzzySet("Clear",new TrapezoidFunction(x5,x7,TrapezoidFunction.EdgeType.Left));// m1 max  m2 min 
            
            
        }
        public void SetBoardSize()
        {
            
        }
        public void SetMoves(int em)
        {
            // Set specified moves based on percentage of EM from the previous level
            // match the player's move 
            var x1 = em; // EM
            var x2 = (em / .93f); // EM / .93
            var x3 = (em / .75f); // EM / .75
            var x4 = (em / .62f); // EM /.62
            var x5 = (em / .60f); // EM / .6
            var x6 = (em / .51f); // EM / .51
            var x7 = (em / .50f); // EM / .5


                // Gonna Tweak This Tomorrow
            fsGreat = new FuzzySet("Great",new TrapezoidFunction(x1,x3,TrapezoidFunction.EdgeType.Right)); // m1 min  m2 max
            fsAverage = new FuzzySet("Average",new TrapezoidFunction(x2,x3,x6,x7));
            fsClear = new FuzzySet("Clear",new TrapezoidFunction(x5,x7,TrapezoidFunction.EdgeType.Left));// m1 max  m2 min 
            // fsSlowClear = new FuzzySet("SlowClear",new TrapezoidFunction(x4,x10,x6,x7));// m1 max  m2 min 

            clearMoves = new LinguisticVariable("clearMoves",0,em/.5f);
            clearMoves.AddLabel(fsGreat);
            clearMoves.AddLabel(fsAverage);

            clearMoves.AddLabel(fsClear);
            // clearMoves.AddLabel(fsSlowClear);
        }


        
        public void SetTime(float allottedTime)
        {
            //{0, 10, 30, 40}
            var x1 = 0; // 0
            var x2 = allottedTime *.125f; // 5

            // trapezoid
            var x3 = allottedTime *.175f; // 10
            var x4 = allottedTime *.375f;
            // 15
            
            // second Trapezoid
            var x5 = allottedTime *.4f; // 15
            //  x4 = 4
            var x6 = allottedTime *.5f; // 15
            var x7 = allottedTime *.65f; // 15
            
            var x8 = allottedTime *.62f; // 15
            var x9 = allottedTime *.7f;
            var x10 = allottedTime * .25f;
            var x11 = allottedTime * .45f;

            fsFailed = new FuzzySet("Failed",new TrapezoidFunction(x1,x10,TrapezoidFunction.EdgeType.Right));
            fsSlow = new FuzzySet("Slow",new TrapezoidFunction(x3,x10,x4,x11));
            fsNormal = new FuzzySet("Normal",new TrapezoidFunction(x4,x5,x6,x7));
            fsFast = new FuzzySet("Fast",new TrapezoidFunction(x6,x9,TrapezoidFunction.EdgeType.Left));
            clearTime = new LinguisticVariable("clearTime",0,allottedTime);
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
        
        // Tweak this to your liking
        public void SetIncrementalMoves()
        {
            var x1 = -5; // 0
            var x2 = 1; // 5

            // trapezoid
            var x3 = 3; // 10
            var x4 = 4;
            // 15
            
            // second Trapezoid
            var x5 = 3.5f; // 15
            //  x4 = 4
            var x6 = 5; // 15
            var x7 = 6; // 15
            
            var x8 = 5.5f; // 15
            var x9 = 9;
            // m1 min  m2 max
            fsZero = new FuzzySet("Zero",new TrapezoidFunction(x1,x2,TrapezoidFunction.EdgeType.Right));
            fsNone = new FuzzySet("None",new TrapezoidFunction(x1,x2,x3,x4));
            fsMinimal = new FuzzySet("Minimal",new TrapezoidFunction(x5,x4,x6,x7));
            fsPlenty = new FuzzySet("Plenty",new TrapezoidFunction(x8,x9,TrapezoidFunction.EdgeType.Left));
            
            newMoves = new LinguisticVariable("moveIncrement",-5,15);
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
            
            infSystem = new Inference(db, new CentroidBasedDefuzzifier(40));
            // test, assumes that that clearMoves is opposite of the clearTime
            // infSystem.NewRule("Rule 1", "IF clearMoves IS Great THEN clearTime IS Slow");
            // infSystem.NewRule("Rule 2", "IF clearMoves IS Average THEN clearTime IS Normal");
            // infSystem.NewRule("Rule 3", "IF clearMoves IS Clear THEN clearTime IS Fast");
            // Plenty Minimal None
            
            // The system will now guess
            // Inference Class uses string manipulation (using stack and RPN)
            // 
            infSystem.NewRule("Rule 1", "IF clearMoves IS Great AND clearTime IS Fast THEN moveIncrement IS Plenty");
            infSystem.NewRule("Rule 2", "IF clearMoves IS Great AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 3", "IF clearMoves IS Great AND clearTime IS Slow THEN moveIncrement IS None");

            infSystem.NewRule("Rule 4", "IF clearMoves IS Average AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 5", "IF clearMoves IS Average AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 6", "IF clearMoves IS Average AND clearTime IS Slow THEN moveIncrement IS Minimal");
            
            // infSystem.NewRule("Rule 7", "IF clearMoves IS SlowClear AND clearTime IS Fast THEN moveIncrement IS Minimal");
            // infSystem.NewRule("Rule 8", "IF clearMoves IS SlowClear AND clearTime IS Normal THEN moveIncrement IS Minimal");
            // infSystem.NewRule("Rule 9", "IF clearMoves IS SlowClear AND clearTime IS Slow THEN moveIncrement IS None");
            //
            infSystem.NewRule("Rule 7", "IF clearMoves IS Clear AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 8", "IF clearMoves IS Clear AND clearTime IS Normal THEN moveIncrement IS None");
            infSystem.NewRule("Rule 9", "IF clearMoves IS Clear AND clearTime IS Slow THEN moveIncrement IS None");
            
             infSystem.NewRule("Rule 10", "IF clearMoves IS Great OR clearTime IS Failed THEN moveIncrement IS Zero");
             infSystem.NewRule("Rule 11", "IF clearMoves IS Average OR clearTime IS Failed THEN moveIncrement IS Zero");
           //  infSystem.NewRule("Rule 15", "IF clearMoves IS SlowClear OR clearTime IS Failed THEN moveIncrement IS Zero");
             infSystem.NewRule("Rule 12", "IF clearMoves IS Clear OR clearTime IS Failed THEN moveIncrement IS Zero");








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