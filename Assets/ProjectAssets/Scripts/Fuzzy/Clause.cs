namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic;
    public class Clause
    {
        private LinguisticVariable variable;
        private FuzzySet label;

        public LinguisticVariable Variable
        {
            get { return variable; }
        }
        
        public FuzzySet Label
        {
            get { return label; }
        }
        
        public Clause( LinguisticVariable variable, FuzzySet label )
        {
            // check if label belongs to var.
            variable.GetLabel( label.Name );
            
            // initializing attributes
            this.label    = label;
            this.variable = variable;
        }
        
        public float Evaluate( )
        {
            return label.GetMembership( variable.NumericInput );
        }
        
        public override string ToString( )
        {
            return this.variable.Name + " IS " + this.label.Name;
        }
        
    }
}