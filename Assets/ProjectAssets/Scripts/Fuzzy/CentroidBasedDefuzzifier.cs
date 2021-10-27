namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic;
    public class CentroidBasedDefuzzifier : IDefuzzify
    {
        
        private int intervals;
        
        public CentroidBasedDefuzzifier( int intervals )
        {
            this.intervals = intervals;
        }
        
        public float Defuzzify( FuzzyOutput fuzzyOutput, INorm normOperator )
        {
            // results and accumulators
            float weightSum = 0, membershipSum = 0;

            // speech universe
            float start = fuzzyOutput.OutputVariable.Start;
            float end = fuzzyOutput.OutputVariable.End;

            // increment
            float increment = ( end - start ) / this.intervals;

            // running through the speech universe and evaluating the labels at each point
            for ( float x = start; x < end; x += increment )
            {
                // we must evaluate x membership to each one of the output labels
                foreach ( FuzzyOutput.OutputConstraint oc in fuzzyOutput.OutputList )
                {
                    // getting the membership for X and constraining it with the firing strength
                    float membership = fuzzyOutput.OutputVariable.GetLabelMembership( oc.Label, x );
                    float constrainedMembership = normOperator.Evaluate( membership, oc.FiringStrength );

                    weightSum += x * constrainedMembership;
                    membershipSum += constrainedMembership;
                }
            }

            // if no membership was found, then the membershipSum is zero and the numerical output is unknown.
            if ( membershipSum == 0 )
                throw new Exception( "The numerical output in unavaliable. All memberships are zero." );

            return weightSum / membershipSum;
        }

    }
}