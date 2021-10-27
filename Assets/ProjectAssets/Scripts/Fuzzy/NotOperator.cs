namespace ProjectAssets.Scripts.Fuzzy
{
    public class NotOperator : IUnaryOperator
    {
        public float Evaluate( float membership )
        {
            return ( 1 - membership );
        }
    }
}