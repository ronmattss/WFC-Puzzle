
/* Program Title: Rule
* Programmers: Rivera, Ron Matthew R.
* Date Created: September 20, 2021
* Purpose: Creates Rules for the Fuzzy Logic
* Data Structures: Class, stack
*/
namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic; 

    public class Rule
    {
        // name of the rule 
        private string name;
        // the original expression 
        private string rule;
        // the parsed RPN (reverse polish notation) expression
        private List<object> rpnTokenList;
        // the consequento (output) of the rule
        private Clause output;
        // the database with the linguistic variables
        private DataBank databank;
        // the norm operator
        private INorm normOperator;
        // the conorm operator
        private ICoNorm conormOperator;
        // the complement operator
        private IUnaryOperator notOperator;
        // the unary operators that the rule parser supports
        private string unaryOperators = "NOT;VERY";
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        
        public Clause Output
        {
            get { return output; }
        }
        
        public Rule( DataBank fuzzyDatabase, string name, string rule, INorm normOperator, ICoNorm coNormOperator )
        {
            // the list with the RPN expression
            rpnTokenList = new List<object>( );

            // setting attributes
            this.name           = name;
            this.rule           = rule;
            this.databank       = fuzzyDatabase;
            this.normOperator   = normOperator;
            this.conormOperator = coNormOperator;
            this.notOperator    = new NotOperator( );

            // parsing the rule to obtain RPN of the expression
            ParseRule( );
        }
        
        public Rule( DataBank fuzzyDatabase, string name, string rule ) :
            this( fuzzyDatabase, name, rule, new MinimumNorm( ), new MaximumCoNorm( ) )
        {
        }
        
        public string GetRPNExpression( )
        {
            string result = "";
            foreach ( object o in rpnTokenList )
            {
                // if its a fuzzy clause we can call clause's ToString()
                if ( o.GetType( ) == typeof( Clause ) )
                {
                    Clause c = o as Clause;
                    result += c.ToString( );
                }
                else
                    result += o.ToString( );
                result += ", ";
            }
            result += "#";
            result = result.Replace( ", #", "" );
            return result;
        }
        private int Priority( string Operator )
        {
            // if its unary
            if ( unaryOperators.IndexOf( Operator ) >= 0 )
                return 4;

            switch ( Operator )
            {
                case "(": return 1;
                case "OR": return 2;
                case "AND": return 3;
            }
            return 0;
        }
        
         private void ParseRule( )
        {
            // flag to incicate we are on consequent state
            bool consequent = false;

            // tokens like IF and THEN will be searched always in upper case
            string upRule = rule.ToUpper( );

            // the rule must start with IF, and must have a THEN somewhere
            if ( !upRule.StartsWith( "IF" ) )
                throw new ArgumentException( "A Fuzzy Rule must start with an IF statement." );
            if ( upRule.IndexOf( "THEN" ) < 0 )
                throw new ArgumentException( "Missing the consequent (THEN) statement." );

            // building a list with all the expression (rule) string tokens
            string spacedRule = rule.Replace( "(", " ( " ).Replace( ")", " ) " );
            // getting the tokens list
            string[] tokensList = GetRuleTokens( spacedRule );

            // stack to convert to RPN
            Stack<string> s = new Stack<string>( );
            // storing the last token
            string lastToken = "IF";
            // linguistic var read, used to build clause
            LinguisticVariable lingVar = null;

            // verifying each token
            for ( int i = 0; i < tokensList.Length; i++ )
            {
                // removing spaces
                string token = tokensList[i].Trim( );
                // getting upper case
                string upToken = token.ToUpper( );

                // ignoring these tokens
                if ( upToken == "" || upToken == "IF" ) continue;

                // if the THEN is found, the rule is now on consequent
                if ( upToken == "THEN" )
                {
                    lastToken = upToken;
                    consequent = true;
                    continue;
                }

                // if we got a linguistic variable, an IS statement and a label is needed
                if ( lastToken == "VAR" )
                {
                    if ( upToken == "IS" )
                        lastToken = upToken;
                    else
                        throw new ArgumentException( "An IS statement is expected after a linguistic variable." );
                }
                // if we got an IS statement, a label must follow it
                else if ( lastToken == "IS" )
                {
                    try
                    {
                        FuzzySet fs = lingVar.GetLabel( token );
                        Clause c = new Clause( lingVar, fs );
                        if ( consequent )
                            output = c;
                        else
                            rpnTokenList.Add( c );
                        lastToken = "LAB";
                    }
                    catch ( KeyNotFoundException )
                    {
                        throw new ArgumentException( "Linguistic label " + token + " was not found on the variable " + lingVar.Name + "." );
                    }
                }
                // not VAR and not IS statement 
                else
                {
                    // openning new scope
                    if ( upToken == "(" )
                    {
                        // if we are on consequent, only variables can be found
                        if ( consequent )
                            throw new ArgumentException( "Linguistic variable expected after a THEN statement." );
                        // if its a (, just push it
                        s.Push( upToken );
                        lastToken = upToken;
                    }
                    // operators
                    else if ( upToken == "AND" || upToken == "OR" || unaryOperators.IndexOf( upToken ) >= 0 )
                    {
                        // if we are on consequent, only variables can be found
                        if ( consequent )
                            throw new ArgumentException( "Linguistic variable expected after a THEN statement." );

                        // pop all the higher priority operators until the stack is empty 
                        while ( ( s.Count > 0 ) && ( Priority( s.Peek( ) ) > Priority( upToken ) ) )
                            rpnTokenList.Add( s.Pop( ) );

                        // pushing the operator    
                        s.Push( upToken );
                        lastToken = upToken;
                    }
                    // closing the scope
                    else if ( upToken == ")" )
                    {
                        // if we are on consequent, only variables can be found
                        if ( consequent )
                            throw new ArgumentException( "Linguistic variable expected after a THEN statement." );

                        // if there is nothing on the stack, an oppening parenthesis is missing.
                        if ( s.Count == 0 )
                            throw new ArgumentException( "Openning parenthesis missing." );

                        // pop the tokens and copy to output until openning is found 
                        while ( s.Peek( ) != "(" )
                        {
                            rpnTokenList.Add( s.Pop( ) );
                            if ( s.Count == 0 )
                                throw new ArgumentException( "Openning parenthesis missing." );
                        }
                        s.Pop( );

                        // saving last token...
                        lastToken = upToken;
                    }
                    // finally, the token is a variable
                    else
                    {
                        // find the variable
                        try
                        {
                            lingVar = databank.GetVariable( token );
                            lastToken = "VAR";
                        }
                        catch ( KeyNotFoundException )
                        {
                            throw new ArgumentException( "Linguistic variable " + token + " was not found on the database." );
                        }
                    }
                }
            }

            // popping all operators left in stack
            while ( s.Count > 0 )
                rpnTokenList.Add( s.Pop( ) );
        }
         
         private string[] GetRuleTokens( string rule )
         {
             // breaking in tokens
             string[] tokens = rule.Split( ' ' );

             // looking for unary operators
             for ( int i = 0; i < tokens.Length; i++ )
             {
                 // if its unary and there is an "IS" token before, we must change positions
                 if ( ( unaryOperators.IndexOf( tokens[i].ToUpper( ) ) >= 0 ) &&
                      ( i > 1 ) && ( tokens[i - 1].ToUpper( ) == "IS" ) )
                 {
                     // placing VAR name
                     tokens[i - 1] = tokens[i - 2];
                     tokens[i - 2] = tokens[i];
                     tokens[i] = "IS";
                 }
             }

             return tokens;
         }
         
         public float EvaluateFiringStrength( )
         {
             // Stack to store the operand values
             Stack<float> s = new Stack<float>( );

             // Logic to calculate the firing strength
             foreach ( object o in rpnTokenList )
             {
                 // if its a clause, then its value must be calculated and pushed
                 if ( o.GetType( ) == typeof( Clause ) )
                 {
                     Clause c = o as Clause;
                     s.Push( c.Evaluate( ) );
                 }
                 // if its an operator (AND / OR) the operation is performed and the result 
                 // returns to the stack
                 else
                 {
                     float y = s.Pop( );
                     float x = 0;

                     // unary pops only one value
                     if ( unaryOperators.IndexOf( o.ToString( ) ) < 0 )
                         x = s.Pop( );

                     // operation
                     switch ( o.ToString( ) )
                     {
                         case "AND":
                             s.Push( normOperator.Evaluate( x, y ) );
                             break;
                         case "OR":
                             s.Push( conormOperator.Evaluate( x, y ) );
                             break;
                         case "NOT":
                             s.Push( notOperator.Evaluate( y ) );
                             break;
                     }
                 }
             }

             // result on the top of stack
             return s.Pop( );
         }
        
        
    }
}