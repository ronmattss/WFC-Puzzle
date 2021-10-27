namespace ProjectAssets.Scripts.Fuzzy
{
    using System;
    using System.Collections.Generic;

    public class FuzzyOutput
    {
        public class OutputConstraint
        {
            private string label;
            private float firingStrength;

            internal OutputConstraint(string label, float firingStrength)
            {
                this.label = label;
                this.firingStrength = firingStrength;
            }

            public string Label
            {
                get { return label; }
            }

            public float FiringStrength
            {
                get { return firingStrength; }
            }
        }

        private List<OutputConstraint> outputList;
        private LinguisticVariable outputVar;

        public List<OutputConstraint> OutputList
        {
            get { return outputList; }
        }

        public LinguisticVariable OutputVariable
        {
            get { return outputVar; }
        }

        internal FuzzyOutput(LinguisticVariable outputVar)
        {
            // instance of the constraints list 
            this.outputList = new List<OutputConstraint>(20);

            // output linguistic variable
            this.outputVar = outputVar;
        }

        internal void AddOutput(string labelName, float firingStrength)
        {
            // check if the label exists in the linguistic variable
            this.outputVar.GetLabel(labelName);

            // adding label
            this.outputList.Add(new OutputConstraint(labelName, firingStrength));
        }

        internal void ClearOutput()
        {
            this.outputList.Clear();
        }
    }
}