namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic;
    public class RuleBase
    {
        private Dictionary<string, Rule> rules;
        
        public RuleBase(  )
        {
            // instance of the rules list 
            this.rules = new Dictionary<string, Rule>( 20 );
        }
        
        
        public void AddRule( Rule rule )
        {
            // checking for existing name
            if ( this.rules.ContainsKey( rule.Name ) )
                throw new ArgumentException( "The fuzzy rule name already exists in the rulebase." );
            
            // adding rule
            this.rules.Add( rule.Name, rule );
        }
        
        public void ClearRules( )
        {
            this.rules.Clear( );
        }
        
        
        public Rule GetRule( string ruleName )
        {
            return rules [ruleName];
        }

        public Rule[] GetRules( )
        {
            Rule[] r = new Rule[rules.Count];

            int i = 0;
            foreach ( KeyValuePair<string, Rule> kvp in rules )
                r[i++] = kvp.Value;

            return r;
        }
    }
}