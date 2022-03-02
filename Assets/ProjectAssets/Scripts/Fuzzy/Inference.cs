/* Program Title: Inference
 * Programmers: Rivera, Ron Matthew R.
 * Date Created: September 20, 2021
 * Purpose: Translates a given sentence into a logical expression.
 * Data Structures: Classes, Interfaces, Enums
 */

namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    public class Inference
    {
        private DataBank database;
        // The fuzzy rules of this system
        private RuleBase rulebase;
        // The defuzzifier method choosen 
        private IDefuzzify defuzzifier;
        // Norm operator used in rules and deffuzification
        private INorm normOperator;
        // CoNorm operator used in rules
        private ICoNorm coNormOperator;
        
        public Inference( DataBank database, IDefuzzify defuzzifier )
            : this( database, defuzzifier, new MinimumNorm( ), new MaximumCoNorm( ) )
        {
        }
        
        public Inference( DataBank database, IDefuzzify defuzzifier, INorm normOperator, ICoNorm conormOperator )
        {
            this.database = database;
            this.defuzzifier = defuzzifier;
            this.normOperator = normOperator;
            this.coNormOperator = conormOperator;
            this.rulebase = new RuleBase( );
        }
        
        public Rule NewRule( string name, string rule )
        {
            Rule r = new Rule( database, name, rule, normOperator, coNormOperator );
            this.rulebase.AddRule( r );
            return r;
        }
        public void SetInput( string variableName, float value )
        {
            this.database.GetVariable( variableName ).NumericInput = value;
        }
        
        public LinguisticVariable GetLinguisticVariable( string variableName )
        {
            return this.database.GetVariable( variableName );
        }
        
        public Rule GetRule( string ruleName )
        {
            return this.rulebase.GetRule ( ruleName );
        }
        
        public float Evaluate( string variableName )
        {
            // call the defuzzification on fuzzy output 
            FuzzyOutput fuzzyOutput = ExecuteInference( variableName );
            float res = defuzzifier.Defuzzify( fuzzyOutput, normOperator );
            return res;
        }
        
        public FuzzyOutput ExecuteInference( string variableName )
        {
            // gets the variable
            LinguisticVariable lingVar = database.GetVariable( variableName );

            // object to store the fuzzy output
            FuzzyOutput fuzzyOutput = new FuzzyOutput( lingVar );

            // select only rules with the variable as output
            Rule[] rules = rulebase.GetRules( );
            foreach ( Rule r in rules )
            {
                if ( r.Output.Variable.Name == variableName )
                {
                    string labelName = r.Output.Label.Name;
                    float firingStrength = r.EvaluateFiringStrength( );
                    if ( firingStrength > 0 )
                        fuzzyOutput.AddOutput( labelName, firingStrength );
                    
//                    Debug.Log($"Firing Strength of: {labelName}: {firingStrength}");
                }
            }

            // returns the fuzzy output obtained
            return fuzzyOutput;
        }
    }
}