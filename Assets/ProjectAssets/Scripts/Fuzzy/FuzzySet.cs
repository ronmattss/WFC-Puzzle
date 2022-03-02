/* Program Title: Fuzzy Logic Set
 * Programmers: Rivera, Ron Matthew R.
 * Date Created: September 20, 2021
 * Purpose: Gets the Membership a Fuzzy Logic set
 * Data Structures: Classes, Interfaces, Enums
 */
namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    
    

    public class FuzzySet
    {
        // name of the Fuzzy Set
        private string _name;
        // What type of Function will this be on
        private IMembershipFunction membershipFunction;

        public string Name => _name;

        public float LeftLimit => membershipFunction.LeftLimit;
        public float RightLimit => membershipFunction.RightLimit;

        public FuzzySet(string name, IMembershipFunction function)
        {
            this._name = name;
            this.membershipFunction = function;
        }

        public float GetMembership(float x)
        {
            return membershipFunction.GetMembership(x);
        }
    }
    
}