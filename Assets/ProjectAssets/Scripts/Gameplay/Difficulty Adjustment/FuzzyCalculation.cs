/*
 Title: Fuzzy Calculation
 Author: Ron Matthew Rivera
 Sub-System: Part of Dynamic Difficulty Adjustment System
 Date Written/Revised: Dec. 15, 2021
 Purpose: Implements a Fuzzy Calculation based on the given input from the game
 Data Structures, algorithms, and control: Class. Lists
 */

namespace ProjectAssets.Scripts.Gameplay.Difficulty_Adjustment
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Util;
    using UnityEngine;
    using Fuzzy;


    [Serializable]
    public class FuzzyCalculation
    {
        #region Variables

        public MembershipFunction[] members;
        public float[] points;

        public FuzzySet fsGreat,
            fsAverage,
            fsClear;

        public LinguisticVariable clearMoves;
        //public  float[] moves = {15, 20, 25, 30}; // sample moves, we 

        public FuzzySet fsFast,
            fsNormal,
            fsSlow,
            fsFailed;

        public LinguisticVariable clearTime;
        // public float[] time = {0, 10, 30, 40};

        public FuzzySet fsPlenty,
            fsMinimal,
            fsNone,
            fsZero;

        public LinguisticVariable newMoves;
        // public float[] moveIncrease = {0, 5, 7, 15};

        public FuzzySet fsLow,
            fsMid,
            fsHigh;

        public LinguisticVariable levelRating;
        // public float[] time = {0, 10, 30, 40};

        private DataBank db;
        private Inference infSystem;

        #endregion

        // level Rating?
        // Board Size
        // Minimum Moves Per Board

        // Maximum Moves Per Board
        public void SetLevelRating()
        {
            var x1 = 10; // EM
            var x2 = 30; // EM / .93
            var x3 = 40; // EM / .75
            var x4 = 60; // EM /.62
            var x5 = 70; // EM / .6
            var x6 = 80; // EM / .51
            var x7 = 90; // EM / .5
            fsLow = new FuzzySet("Great",
                new TrapezoidFunction(x1,
                    x4,
                    TrapezoidFunction.EdgeType.Right)); // m1 min  m2 max
            fsMid = new FuzzySet("Average",
                new TrapezoidFunction(x2,
                    x3,
                    x6,
                    x7));
            fsHigh = new FuzzySet("Clear",
                new TrapezoidFunction(x5,
                    x7,
                    TrapezoidFunction.EdgeType.Left)); // m1 max  m2 min 
        }

        public void SetMoves(int em)
        {
            // Set specified moves based on percentage of EM from the previous level
            // match the player's move 
            var x1 = em; // EM
            var x2 = em / .93f; // EM / .93
            var x3 = em / .75f; // EM / .75
            var x4 = em / .62f; // EM /.62
            var x5 = em / .60f; // EM / .6
            var x6 = em / .51f; // EM / .51
            var x7 = em / .50f; // EM / .5

            // Fuzzy Sets
            // 
            fsGreat = new FuzzySet("Great",
                new TrapezoidFunction(x1,
                    x3,
                    TrapezoidFunction.EdgeType.Right)); // m1 min  m2 max
            fsAverage = new FuzzySet("Average",
                new TrapezoidFunction(x2,
                    x3,
                    x6,
                    x7));
            fsClear = new FuzzySet("Clear",
                new TrapezoidFunction(x5,
                    x7,
                    TrapezoidFunction.EdgeType.Left)); // m1 max  m2 min 
            // fsSlowClear = new FuzzySet("SlowClear",new TrapezoidFunction(x4,x10,x6,x7));// m1 max  m2 min 
            clearMoves = new LinguisticVariable("clearMoves",
                0,
                em / .5f);
            clearMoves.AddLabel(fsGreat);
            clearMoves.AddLabel(fsAverage);
            clearMoves.AddLabel(fsClear);
            // clearMoves.AddLabel(fsSlowClear);
        }

        public void SetTime(float allottedTime)
        {
            //{0, 10, 30, 40}
            var x1 = 0; // 0
            var x2 = allottedTime * .125f; // 5

            // trapezoid
            var x3 = allottedTime * .175f; // 10
            var x4 = allottedTime * .375f;
            // 15

            // second Trapezoid
            var x5 = allottedTime * .4f; // 15
            //  x4 = 4
            var x6 = allottedTime * .5f; // 15
            var x7 = allottedTime * .65f; // 15
            var x8 = allottedTime * .62f; // 15
            var x9 = allottedTime * .7f;
            var x10 = allottedTime * .25f;
            var x11 = allottedTime * .40f;
            fsFailed = new FuzzySet("Failed",
                new TrapezoidFunction(x1,
                    x10,
                    TrapezoidFunction.EdgeType.Right));
            fsSlow = new FuzzySet("Slow",
                new TrapezoidFunction(x3,
                    x10,
                    x4,
                    x11));
            fsNormal = new FuzzySet("Normal",
                new TrapezoidFunction(x10,
                    x5,
                    x6,
                    x7));
            fsFast = new FuzzySet("Fast",
                new TrapezoidFunction(5,
                    x9,
                    TrapezoidFunction.EdgeType.Left));
            clearTime = new LinguisticVariable("clearTime",
                0,
                allottedTime);
            clearTime.AddLabel(fsFast);
            clearTime.AddLabel(fsNormal);
            clearTime.AddLabel(fsSlow);
            clearTime.AddLabel(fsFailed);
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
            fsZero = new FuzzySet("Zero",
                new TrapezoidFunction(x1,
                    x2,
                    TrapezoidFunction.EdgeType.Right));
            fsNone = new FuzzySet("None",
                new TrapezoidFunction(x1,
                    x2,
                    x3,
                    x4));
            fsMinimal = new FuzzySet("Minimal",
                new TrapezoidFunction(x5,
                    x4,
                    x6,
                    x7));
            fsPlenty = new FuzzySet("Plenty",
                new TrapezoidFunction(x8,
                    x9,
                    TrapezoidFunction.EdgeType.Left));
            newMoves = new LinguisticVariable("moveIncrement",
                -5,
                15);
            newMoves.AddLabel(fsNone);
            newMoves.AddLabel(fsZero);
            newMoves.AddLabel(fsMinimal);
            newMoves.AddLabel(fsPlenty);
        }

        // add Rules to the databank where rules will be applied
        public void AddToDataBank()
        {
            db = new DataBank();
            db.AddVariable(clearMoves);
            db.AddVariable(clearTime);
            db.AddVariable(newMoves);
            infSystem = new Inference(db,
                new CentroidBasedDefuzzifier(40));
            infSystem.NewRule("Rule 1",
                "IF clearMoves IS Great AND clearTime IS Fast THEN moveIncrement IS Plenty");
            infSystem.NewRule("Rule 2",
                "IF clearMoves IS Great AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 3",
                "IF clearMoves IS Great AND clearTime IS Slow THEN moveIncrement IS None");
            infSystem.NewRule("Rule 4",
                "IF clearMoves IS Average AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 5",
                "IF clearMoves IS Average AND clearTime IS Normal THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 6",
                "IF clearMoves IS Average AND clearTime IS Slow THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 7",
                "IF clearMoves IS Clear AND clearTime IS Fast THEN moveIncrement IS Minimal");
            infSystem.NewRule("Rule 8",
                "IF clearMoves IS Clear AND clearTime IS Normal THEN moveIncrement IS None");
            infSystem.NewRule("Rule 9",
                "IF clearMoves IS Clear AND clearTime IS Slow THEN moveIncrement IS None");
            infSystem.NewRule("Rule 10",
                "IF clearMoves IS Great OR clearTime IS Failed THEN moveIncrement IS Zero");
            infSystem.NewRule("Rule 11",
                "IF clearMoves IS Average OR clearTime IS Failed THEN moveIncrement IS Zero");
            infSystem.NewRule("Rule 12",
                "IF clearMoves IS Clear OR clearTime IS Failed THEN moveIncrement IS Zero");
        }

        // accept input from the Difficulty modifier
        public float AcceptInput(float move,
            float timeInput)
        {
            infSystem.SetInput("clearMoves",
                move);
            infSystem.SetInput("clearTime",
                timeInput);
            return infSystem.Evaluate("moveIncrement");
        }
    }

    [Serializable]
    public class MembershipFunction
    {
        public AnimationCurve curve;
        public string linguisticVariable;
    }
}