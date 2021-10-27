namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic;
    
    public class LinguisticVariable
    {
        // name of the linguistic variable
        private string name;
        // right limit within the lingusitic variable works
        private float start;
        // left limit within the lingusitic variable works
        private float end;
        // the linguistic labels of the linguistic variable
        private Dictionary<string, FuzzySet> labels;
        // the numeric input of this variable
        private float numericInput;
        
        public float NumericInput
        {
            get { return numericInput; }
            set { numericInput = value; }
        }
        
        
        public string Name
        {
            get { return name; }
        }
        
        public float Start
        {
            get { return start; }
        }
        
        public float End
        {
            get { return end; }
        }
        
        
        public LinguisticVariable( string name, float start, float end )
        {
            this.name  = name;
            this.start = start;
            this.end   = end;

            // instance of the labels list - usually a linguistic variable has no more than 10 labels
            this.labels = new Dictionary<string, FuzzySet>( 10 );
        }
        
        public void AddLabel( FuzzySet label )
        {
            // checking for existing name
            if ( this.labels.ContainsKey( label.Name ) )
                throw new ArgumentException( "The linguistic label name already exists in the linguistic variable." );

            // checking ranges
            if ( label.LeftLimit < this.start )
                throw new ArgumentException( "The left limit of the fuzzy set can not be lower than the linguistic variable's starting point." );
            if ( label.RightLimit > this.end )
                throw new ArgumentException( "The right limit of the fuzzy set can not be greater than the linguistic variable's ending point." );

            // adding label
            this.labels.Add( label.Name, label );
        }
        
        public void ClearLabels( )
        {
            this.labels.Clear( );
        }
        
        public FuzzySet GetLabel( string labelName )
        {
            return labels[labelName];
        }
        
        
        public float GetLabelMembership( string labelName, float value )
        {
            FuzzySet fs = labels[labelName];
            return fs.GetMembership( value );
        }
        
        
        
        
    }
}