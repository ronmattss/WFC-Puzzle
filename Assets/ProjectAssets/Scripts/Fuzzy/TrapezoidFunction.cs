/* Program Title: Trapezoid Function
* Programmers: Rivera, Ron Matthew R.
* Purpose: Creates a Trapezoid Function for the Fuzzy Logic System
* Data Structures: Class
*/
using ProjectAssets.Scripts.Fuzzy.Helpers;

namespace ProjectAssets.Scripts.Fuzzy
{

    public class TrapezoidFunction : PiecewiseFunction
    {
        public enum EdgeType
        {    // the left or right side of the Trapezoid
            Left,
            Right
        };
        
        private TrapezoidFunction( int size )
        {
            points = new Point[size];
        }
        
        public TrapezoidFunction( float m1, float m2, float m3, float m4, float max, float min )
            : this( 4 )
        {
            points[0] = new Point( m1, min );
            points[1] = new Point( m2, max );
            points[2] = new Point( m3, max );
            points[3] = new Point( m4, min );
        }
        
        public TrapezoidFunction( float m1, float m2, float m3, float m4 )
            : this( m1, m2, m3, m4, 1.0f, 0.0f )
        {
        }
        
        public TrapezoidFunction( float m1, float m2, float m3, float max, float min )
            : this( 3 )
        {
            points[0] = new Point( m1, min );
            points[1] = new Point( m2, max );
            points[2] = new Point( m3, min );
        }
        
        public TrapezoidFunction( float m1, float m2, float m3 )
            : this( m1, m2, m3, 1.0f, 0.0f )
        {
        }
        
        public TrapezoidFunction( float m1, float m2, float max, float min, EdgeType edge )
            : this( 2 )
        {
            if ( edge == EdgeType.Left )
            {
                points[0] = new Point( m1, min );
                points[1] = new Point( m2, max );
            }
            else
            {
                points[0] = new Point( m1, max );
                points[1] = new Point( m2, min );
            }
        }
        
        public TrapezoidFunction( float m1, float m2, EdgeType edge )
            : this( m1, m2, 1.0f, 0.0f, edge )
        {
        }
    }
}