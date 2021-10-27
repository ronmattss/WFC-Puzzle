using System;
using System.Collections.Generic;
namespace ProjectAssets.Scripts.Fuzzy
{
    public class DataBank
    {
        private Dictionary<string, LinguisticVariable> variables;

        public DataBank(  )
        {
            // instance of the variables list 
            this.variables = new Dictionary<string, LinguisticVariable>( 10 );
        }
        
        public void AddVariable( LinguisticVariable variable )
        {
            // checking for existing name
            if ( this.variables.ContainsKey( variable.Name ) )
                throw new ArgumentException( "The linguistic variable name already exists in the database." );
            
            // adding label
            this.variables.Add( variable.Name, variable );
        }
        
        public void ClearVariables( )
        {
            this.variables.Clear( );
        }
        
        public LinguisticVariable GetVariable( string variableName )
        {
            return variables [variableName];
        }
    }
}