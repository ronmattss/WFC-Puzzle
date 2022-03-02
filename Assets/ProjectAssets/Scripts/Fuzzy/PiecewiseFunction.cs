/* Program Title: Piecewise Function
* Programmers: Rivera, Ron Matthew R.
* Purpose: Creates a Piecewise Function for the Fuzzy Logic System
* Data Structures: Class
*/
using ProjectAssets.Scripts.Fuzzy.Helpers;
using System;
namespace ProjectAssets.Scripts.Fuzzy
{
    

    public class PiecewiseFunction : IMembershipFunction
    {
        protected Point[] points;


        public float LeftLimit
        {
            get
            {
                return points[0].X;
            }
        }
        public float RightLimit
        {
            get
            {
                return points[points.Length - 1].X;
            }
        }
        
        protected PiecewiseFunction( )
        {
            points = null;
        }
        
        public PiecewiseFunction( Point[] points )
        {
            this.points = points;

            // check if X points are in a sequence and if Y values are in [0..1] range
            for ( int i = 0, n = points.Length; i < n; i++ )
            {
                if ( ( points[i].Y < 0 ) || ( points[i].Y > 1 ) )
                    throw new ArgumentException( "Y value of points must be in the range of [0, 1]." );

                if ( i == 0 )
                    continue;

                if ( points[i - 1].X > points[i].X )
                    throw new ArgumentException( "Points must be in crescent order on X axis." );
            }
        }
        
        public float GetMembership( float x )
        {
            // no values belong to the fuzzy set, if there are no points in the piecewise function
            if ( points.Length == 0 )
                return 0.0f;

            // if X value is less than the first point, so first point's Y will be returned as membership
            if ( x < points[0].X )
                return points[0].Y;

            // looking for the line that contais the X value
            for ( int i = 1, n = points.Length; i < n; i++ )
            {
                // the line with X value starts in points[i-1].X and ends at points[i].X
                if ( x < points[i].X )
                {
                    // points to calculate line's equation
                    float y1 = points[i].Y;
                    float y0 = points[i - 1].Y;
                    float x1 = points[i].X;
                    float x0 = points[i - 1].X;
                    // angular coefficient
                    float m = ( y1 - y0 ) / ( x1 - x0 );
                    // returning the membership - the Y value for this X
                    return m * ( x - x0 ) + y0;
                }
            }

            // X value is more than last point, so last point Y will be returned as membership
            return points[points.Length - 1].Y;
        }
        
        
    }
    
    
}