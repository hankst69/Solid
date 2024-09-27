//----------------------------------------------------------------------------------
// File: "Vector3D.cs"
// Author: Steffen Hanke
// Date: 2019
// based on: syngo.Services.ImageProcessing/Maths/MathsVector3d.cs
// made into an immutable class and reduced to minimum set of members
//----------------------------------------------------------------------------------
using System;
using System.Globalization;

namespace Solid.Infrastructure.Math
{
    /// <apiflag>Yes</apiflag>
    /// <summary>API:YES 
    /// A class which defines a 3D vector by its X, Y and Z component of type double.
    /// </summary>
    /// <remarks>
    /// X, Y  and Z component are placed within a struct DicomPoint3d 
    /// to ensure a sequential ordering within the physical memory (X before Y before Z).
    /// There are APIs to set and get these components and for vector arithmetic.
    /// 
    /// A vector is defined to be "Nil" or "IsNil" if the
    /// length of the vector is less than a specified tolerance.
    /// </remarks>
    public class Vector3D
    {
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The double constant which should be used as general tolerance argument
        /// within a Maths-application if no extraordinary tolerance is needed for 
        /// special tasks. The value is 1 * 10^-6 
        /// </summary>
        public const double DEFAULT_TOLERANCE = 0.000001;

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Defines orientation enum.
        /// </summary>
        public enum Orientation : short
        {
            /// <apiflag>Yes</apiflag>
            /// <summary>API:YES 
            /// Orthogonal
            /// </summary>
            Orthogonal = 0,
            /// <apiflag>Yes</apiflag>
            /// <summary>API:YES 
            /// Same direction but not parallel
            /// </summary>
            SameDirection,
            /// <apiflag>Yes</apiflag>
            /// <summary>API:YES 
            /// Opposite direction Z axis
            /// </summary>
            OppositeDirection,
            /// <apiflag>Yes</apiflag>
            /// <summary>API:YES 
            /// Parallel and same direction
            /// </summary>
            Parallel,
            /// <apiflag>Yes</apiflag>
            /// <summary>API:YES 
            /// Parallel and opposite direction
            /// </summary>
            AntiParallel
        };

        
        // --- Attributes (readonly due immutable concept) ----------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The x coordinate.
        /// </summary>
        private readonly double m_Xval;

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The y coordinate.
        /// </summary>
        private readonly double m_Yval;

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The z coordinate.
        /// </summary>
        private readonly double m_Zval;


        // --- Component handling ---------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Property to get the x component.
        /// </summary>
        public double X => m_Xval;

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Property to get the y component.
        /// </summary>
        public double Y => m_Yval;

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Property to get the z component.
        /// </summary>
        public double Z => m_Zval;

        ///// <apiflag>Yes</apiflag>
        ///// <summary>API:YES 
        ///// Gets the x, y and z component of this Vector3D object.
        ///// </summary>
        ///// <param name="xval">The x component.</param>
        ///// <param name="yval">The y component.</param>
        ///// <param name="zval">The z component.</param>
        //public void GetComponents(out double xval, out double yval, out double zval)
        //{
        //    xval = m_Xval;
        //    yval = m_Yval;
        //    zval = m_Zval;
        //}


        // --- Construction -----------------------------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Default constructor to set the components to 0.0.
        /// </summary>
        public Vector3D()
        {
            m_Xval = 0.0;
            m_Yval = 0.0;
            m_Zval = 0.0;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Copy constructor to set the components from a specified Vector3D object. 
        /// </summary>
        /// <param name="vec3D">The specified Vector3D object.</param>
        public Vector3D(Vector3D vec3D)
        {
            m_Xval = vec3D.m_Xval;
            m_Yval = vec3D.m_Yval;
            m_Zval = vec3D.m_Zval;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Constructor to set the x, y and z component.
        /// </summary>
        /// <param name="xValue">The x component.</param>
        /// <param name="yValue">The y component.</param>
        /// <param name="zValue">The z component.</param>
        public Vector3D(double xValue, double yValue, double zValue)
        {
            m_Xval = xValue;
            m_Yval = yValue;
            m_Zval = zValue;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Constructor to set the x, y and z component with common intital value.
        /// </summary>
        /// <param name="initVal">The value for the x, y and z component.</param>
        public Vector3D(double initVal)
        {
            m_Xval = initVal;
            m_Yval = initVal;
            m_Zval = initVal;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Constructs a new Vector3D instance from given string
        /// </summary>
        /// <remarks>
        /// The specified string must look like:
        /// <pre>
        /// [-1.0E+000 2.0E+000 -3.0E+000]
        /// </pre>
        /// i.e. Exx format,
        /// and was usually generated before by the ToString() method.
        /// The threads default 'CultureInfo' is used to parse the string.
        /// </remarks>
        /// <param name="vector3D">The string which specifies the Vector3D object.</param>
        public Vector3D(string vector3D)
            : this(CreateFromString(vector3D, null))
        {
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Constructs a new Vector3D instance from given string
        /// </summary>
        /// <remarks>
        /// The specified string must look like:
        /// <pre>
        /// [-1.0E+000 2.0E+000 -3.0E+000] 
        /// </pre>
        /// i.e. Exx format,
        /// and was usually generated before by the ToString(Format, FormatProvider) method.
        /// The specified FormatProvider is used to parse the string.
        /// </remarks>
        /// <param name="vector3D">The string which specifies the Vector3D object.</param>
        /// <param name="formatProvider">The format provider for example System.Globalization.CultureInfo("de-DE").</param>
        public Vector3D(string vector3D, IFormatProvider formatProvider)
            : this(CreateFromString(vector3D, formatProvider))
        {
        }

        // --- Conversion from/to other representaion forms ---------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Creates a new instance from the specified polar form.
        /// </summary>
        /// <remarks>
        /// The polar form is specified by the length and the direction cosine X, Y and Z.
        /// <pre>
        /// DirectionCosine.X = cos( angle between vector and x axis ) = vector x component / vector length 
        /// DirectionCosine.Y = cos( angle between vector and y axis ) = vector y component / vector length 
        /// DirectionCosine.Z = cos( angle between vector and z axis ) = vector z component / vector length 
        /// </pre>
        /// An exception will be thrown if the resultant vector IsNil according to the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The resultant vector IsNil.</exception>
        /// <param name="vectorLength">The length of the vector.</param>
        /// <param name="directionCosinus">The direction cosine values x, y  and z of the vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        public static Vector3D CreateFromPolar(double vectorLength, Vector3D directionCosinus, double tolerance)
        {
            Vector3D result = directionCosinus.Multiply(vectorLength);
            // throw an exception if isNil
            if (result.IsNil(tolerance))
            {
                throw new System.ArithmeticException("The vector IsNil.");
            }
            return result;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Creates a new instance from the specified polar form.
        /// </summary>
        /// <remarks>
        /// The polar form is specified by the length and the angles phi and theta in radians.
        /// The angle phi is the angle between the vector's projection onto the xy-plane 
        /// and the positive x axis (0 'less or equal' phi 'less than' 360 degrees). 
        /// The angle theta is the angle between the vector 
        /// and the positive z axis (0 'less or equal' theta 'less than' 180 degree).
        /// 
        /// An exception will be thrown if the resultant vector IsNil according to the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The resultant vector IsNil.</exception>
        /// <param name="vectorLength">The length of vector.</param>
        /// <param name="anglePhi">The angle phi.</param>
        /// <param name="angleTheta">The angle theta.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        public static Vector3D CreateFromPolar(double vectorLength, double anglePhi, double angleTheta, double tolerance)
        {
            double theSinTheta = System.Math.Sin(angleTheta);

            var result = new Vector3D(
                vectorLength * theSinTheta * System.Math.Cos(anglePhi),
                vectorLength * theSinTheta * System.Math.Sin(anglePhi),
                vectorLength * System.Math.Cos(angleTheta));

            // throw an exception if isNil
            if (result.IsNil(tolerance))
            {
                throw new System.ArithmeticException("The vector IsNil.");
            }
            return result;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Sets this Vector3D object's components x, y and z from the specified string.        
        /// </summary>
        /// <remarks>
        /// The specified string must look like:
        /// <pre>
        /// [-1.0E+000 2.0E+000 -3.0E+000]
        /// </pre>
        /// i.e. Exx format,
        /// and was usually generated before by the ToString(Format, FormatProvider) method.
        /// The specified format provider is used to parse the string.
        /// </remarks>
        /// <param name="vector3D">The string which specifies this Vector3D object.</param>
        /// <param name="formatProvider">The format provider for example System.Globalization.CultureInfo("de-DE").</param>
        public static Vector3D CreateFromString(string vector3D, IFormatProvider formatProvider)
        {
            // String looks like:   [-1.0E+000 2.0E+000 3.0E+000]
            //                      01234567890123456789012345678
            int pos0 = vector3D.IndexOf('[');                    // e.g.  0
            int pos1 = vector3D.IndexOf(' ', pos0 + 7);          // e.g. 10  ( 7 = 1.E+000 )
            int minlength = pos1 - pos0 - 1;                     // length - 1 * <sign>
            int pos2 = vector3D.IndexOf(' ', pos1 + minlength);  // e.g. 19
            int pos3 = vector3D.IndexOf(']', pos2 + minlength);  // e.g. 28

            if (formatProvider == null)
            {
                return new Vector3D(
                    Double.Parse(vector3D.Substring(pos0 + 1, pos1 - pos0 - 1)),
                    Double.Parse(vector3D.Substring(pos1 + 1, pos2 - pos1 - 1)),
                    Double.Parse(vector3D.Substring(pos2 + 1, pos3 - pos2 - 1)));
            }
            return new Vector3D(
                Double.Parse(vector3D.Substring(pos0 + 1, pos1 - pos0 - 1), formatProvider),
                Double.Parse(vector3D.Substring(pos1 + 1, pos2 - pos1 - 1), formatProvider),
                Double.Parse(vector3D.Substring(pos2 + 1, pos3 - pos2 - 1), formatProvider));
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Dumps this Vector3D object to a string.        
        /// </summary>
        /// <remarks>
        /// The string looks like:
        /// <pre>
        /// [componentX componentY componentZ] 
        /// </pre>
        /// The specified format and format provider are used. 
        /// </remarks>
        /// <param name="format">The requested format specifier is for example "E10".</param>
        /// <param name="formatProvider">The format provider is for example System.Globalization.CultureInfo("de-DE")</param>
        /// <returns>This Vector3D object as a string.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return "[" + m_Xval.ToString(format, formatProvider) + " "
                   + m_Yval.ToString(format, formatProvider) + " "
                   + m_Zval.ToString(format, formatProvider) + "]";
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Dumps this Vector3D object to a string.        
        /// </summary>
        /// <remarks>
        /// The string looks like:
        /// <pre>
        /// [componentX componentY componentZ]
        /// </pre>
        /// The specified format is used.
        /// </remarks>
        /// <param name="format">The requested format specifier is for example "E10".</param>
        /// <returns>This Vector3D object as a dumped string.</returns>
        public string ToString(string format)
        {
            return ToString(format, NumberFormatInfo.CurrentInfo);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Dumps this Vector3D object to a string.        
        /// </summary>
        /// <remarks>
        /// The string looks like:
        /// <pre>
        /// [componentX componentY componentZ]
        /// </pre>
        /// The format of each component is "E7".
        /// </remarks>
        /// <returns>This Vector3D object as a dumped string.</returns>
        public override string ToString()
        {
            return ToString(null);
        }


        // --- Vector math (as static functions) ---------------------------------------------------------------------------

        /// <summary>Returns the result of vector 1 + vector 2.</summary>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 + vector 2.</returns>
        private static Vector3D Add(Vector3D v1, Vector3D v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return new Vector3D(
                v1.m_Xval + v2.m_Xval,
                v1.m_Yval + v2.m_Yval,
                v1.m_Zval + v2.m_Zval);
        }

        /// <summary>Returns the result of  vector1 - vector2.</summary>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 - vector 2.</returns>
        private static Vector3D Subtract(Vector3D v1, Vector3D v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return new Vector3D(
                v1.m_Xval - v2.m_Xval,
                v1.m_Yval - v2.m_Yval,
                v1.m_Zval - v2.m_Zval);
        }

        /// <summary>Returns the result of  vector * scalar.</summary>
        /// <param name="v1">The specified vector.</param>
        /// <param name="mul">The specified scalar.</param>
        /// <returns>The result of vector * scalar.</returns>
        private static Vector3D Multiply(Vector3D v1, double mul)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (double.IsNaN(mul))
            {
                throw new ArgumentOutOfRangeException(nameof(mul), "multiplier is NaN");
            }

            return new Vector3D(
                v1.m_Xval * mul,
                v1.m_Yval * mul,
                v1.m_Zval * mul);
        }

        /// <summary>Returns the result of vector / scalar divisor.</summary>
        /// <remarks>
        /// If the scalar divisor IsNaN or equal to zero an exception will be thrown.
        /// </remarks>
        /// <param name="v1">The specified vector.</param>
        /// <param name="div">The specified scalar divisor.</param>
        /// <returns>The result of vector / scalar divisor.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Invalid divisor NaN or zero.</exception>
        private static Vector3D Divide(Vector3D v1, double div)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (double.IsNaN(div))
            {
                throw new ArgumentOutOfRangeException(nameof(div), "divisor is NaN");
            }
            if (div == 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(div), "divisor is zero");
            }

            return new Vector3D(
                v1.m_Xval / div,
                v1.m_Yval / div,
                v1.m_Zval / div);
        }

        /// <summary>Returns the inverted Vector3D of this Vector3D object as a new vector.</summary>
        /// <param name="v1">The specified vector.</param>
        /// <returns>The inverted vector.</returns>
        private static Vector3D GetInverted(Vector3D v1)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }

            return new Vector3D(
                -v1.m_Xval,
                -v1.m_Yval,
                -v1.m_Zval);
        }

        /// <summary>Gets the length of this vector object.</summary>
        /// <param name="v1">The specified vector.</param>
        /// <returns>The length of this vector object.</returns>
        private static double GetLength(Vector3D v1)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }

            return System.Math.Sqrt((v1.m_Xval * v1.m_Xval) +
                             (v1.m_Yval * v1.m_Yval) +
                             (v1.m_Zval * v1.m_Zval));
        }

        /// <summary>Returns given vecor in Normalized length.</summary>
        /// <remarks>
        /// An exception will be thrown if this vector IsNil according to the 
        /// common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">Input vector to normalize</param>
        /// <param name="tolerance">A common tolerance value is Vector3D.DEFAULT_TOLERANCE</param>
        /// <exception cref="System.ArithmeticException">This vector IsNil.</exception>
        /// <returns>This vector object as a normalized vector.</returns>
        private static Vector3D GetNormalized(Vector3D v1, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            double length = GetLength(v1);
            if (length < tolerance)
            {
                throw new System.ArithmeticException("The vector IsNil: Its length is: " + length + " .");
            }
            if (System.Math.Abs(length - 1.0) < tolerance)
            {
                // v1 is already normalized
                return v1;
            }
            return new Vector3D(
                v1.m_Xval / length,
                v1.m_Yval / length,
                v1.m_Zval / length);
        }

        /// <summary>Returns true if this Vector3D object is normalized otherwise false.</summary>
        /// <remarks>
        /// The vector is normalized if
        /// <pre>
        /// (1 - tolerance) is less than  |vector length| is less than (1 + tolerance).
        /// </pre>
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">Input vector to test</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if the vector is normalized otherwise false.</returns>
        private static bool IsNormalized(Vector3D v1, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            double length2 = ((v1.m_Xval * v1.m_Xval) +
                              (v1.m_Yval * v1.m_Yval) +
                              (v1.m_Zval * v1.m_Zval));

            if ((length2 < ((1.0 + tolerance) * (1.0 + tolerance))) &&
                (length2 > ((1.0 - tolerance) * (1.0 - tolerance))))
            {
                return true;
            }
            return false;
        }

        /// <summary>Returns true if this Vector3D object isNil.
        /// </summary>
        /// <remarks>
        /// The vector isNil if |vector length| is less than tolerance.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">Input vector to test</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if the vector isNil otherwise false.</returns>
        private static bool IsNil(Vector3D v1, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            double length2 = ((v1.m_Xval * v1.m_Xval) +
                              (v1.m_Yval * v1.m_Yval) +
                              (v1.m_Zval * v1.m_Zval));

            if (length2 < (tolerance * tolerance))
            {
                return true;
            }
            return false;
        }

        /// <summary>Returns true if this Vector3D object and the specified vector object are almost equal.</summary>
        /// <remarks>
        /// They are almost equal if the length of the difference vector
        /// of this vector and specified vector is less than the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if both vectors are almost equal otherwise false.</returns>
        private static bool IsAlmostEqual(Vector3D v1, Vector3D v2, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            return (((v1.m_Xval - v2.m_Xval) * (v1.m_Xval - v2.m_Xval) +
                     (v1.m_Yval - v2.m_Yval) * (v1.m_Yval - v2.m_Yval) +
                     (v1.m_Zval - v2.m_Zval) * (v1.m_Zval - v2.m_Zval)) <
                    (tolerance * tolerance));
        }

        /// <summary>Returns true if the squared length of this Vector2d object is greater than those of the specified vector object.</summary>
        /// <remarks>
        /// This vector is longer if the difference of the squared length 
        /// of this vector and the specified vector 
        /// is greater than the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if this vector v1 is longer than v2, otherwise false.</returns>
        public bool IsLonger(Vector3D v1, Vector3D v2, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            double l1 =
                (v1.m_Xval * v1.m_Xval) +
                (v1.m_Yval * v1.m_Yval) +
                (v1.m_Zval * v1.m_Zval);

            double l2 =
                (v2.m_Xval * v2.m_Xval) +
                (v2.m_Yval * v2.m_Yval) +
                (v2.m_Zval * v2.m_Zval);

            if ((l1 - l2) > tolerance)
            {
                return true;
            }
            return false;
        }

        /// <summary>Returns true if this Vector3D object and the specified vector object are almost parallel.</summary>
        /// <remarks>
        /// Vectors of inverse or same directions are parallel but oblique vectors are not parallel.
        /// Both are almost parallel if the absolute value of the normalized x, y and z component 
        /// of both vectors are the same at the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if both vectors are almost parallel otherwise false.</returns>
        private static bool IsAlmostParallel(Vector3D v1, Vector3D v2, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            Vector3D nv1 = GetNormalized(v1, tolerance);
            Vector3D nv2 = GetNormalized(v2, tolerance);

            return (((System.Math.Abs(nv1.m_Xval - nv2.m_Xval) < tolerance) &&
                     (System.Math.Abs(nv1.m_Yval - nv2.m_Yval) < tolerance) &&
                     (System.Math.Abs(nv1.m_Zval - nv2.m_Zval) < tolerance)) 
                 || ((System.Math.Abs(nv1.m_Xval + nv2.m_Xval) < tolerance) &&
                     (System.Math.Abs(nv1.m_Yval + nv2.m_Yval) < tolerance) &&
                     (System.Math.Abs(nv1.m_Zval + nv2.m_Zval) < tolerance)));
        }

        /// <summary>Calculates the inner product (scalar product A.B) of this vector A and the specified vector B.</summary>
        /// <param name="v1">The specified first vector (A).</param>
        /// <param name="v2">The specified second vector (B).</param>
        /// <returns>The calculated inner product value.</returns>
        private static double GetInnerProduct(Vector3D v1, Vector3D v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return ((v1.m_Xval * v2.m_Xval) +
                    (v1.m_Yval * v2.m_Yval) +
                    (v1.m_Zval * v2.m_Zval));
        }

        /// <summary>Calculates the outer product of given vectors A and B.</summary>
        /// <remarks>
        /// The outer product is also called vector product or cross product.
        /// The resultant vector = A X B.
        /// </remarks>
        /// <param name="v1">The specified first vector (A).</param>
        /// <param name="v2">The specified second vector (B).</param>
        /// <returns>The resultant vector</returns>
        private static Vector3D GetOuterProduct(Vector3D v1, Vector3D v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }

            return new Vector3D(
                v1.m_Yval * v2.m_Zval - v1.m_Zval * v2.m_Yval,
                v1.m_Zval * v2.m_Xval - v1.m_Xval * v2.m_Zval,
                v1.m_Xval * v2.m_Yval - v1.m_Yval * v2.m_Xval);
        }

        /// <summary>Calculates the angle between two vectors.</summary>
        /// <remarks>
        /// The first vector is the actual (this) vector object.
        /// The second vector is the specified vector.
        /// The calculated angle in radians is the smaller one of 
        /// the two possible angles. For example:
        /// the angle = 0 if both vectors are parallel.
        /// the angle = PI if both vectors are anti parallel.
        /// the angle = PI/2 if both vectors are perpendicular.
        /// An Exception will be thrown if the length of a vector is
        /// less than the specified tolerance.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The vector length is too small</exception>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>The angle in radians between the two vectors.</returns>
        private static double GetAngle(Vector3D v1, Vector3D v2, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            double len1 = v1.GetLength();
            double len2 = v2.GetLength();
            if (len1 < tolerance)
            {
                throw new ArithmeticException("The specified vector is too small. The length is: " + len1 + " .");
            }
            if (len2 < tolerance)
            {
                throw new ArithmeticException("The actual vector is too small. The length is: " + len2 + " .");
            }
            
            // (inner product)/(len1 * len2) should be within the range -1 <= 0 <= 1 
            double cos = GetInnerProduct(v1,v2) / (len1 * len2);
            if ((-1.0 <= cos) && (1.0 >= cos))
            {
                return System.Math.Acos(cos);
            }
            else if (-1.0 > cos)
            {
                return System.Math.PI; // -1 = cos( 180 deg )
            }
            else
            {
                return 0.0; //  1 = cos( 0 deg )
            }
        }

        /// <summary>Returns an orientation enum which describes the direction of given vector A in comparison to vector B.
        /// </summary>
        /// <remarks>
        /// This method was introduced to allow allocation free programming with vectors where all temp variables
        /// are provided from the outside.
        /// Both vectors may be orthogonal, parallel same direction, parallel opposite direction,
        /// pointing to the same direction or point to the opposite direction. The orientation enum
        /// is calculated using the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v1">The given vector A.</param>
        /// <param name="v2">The given vector B.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>The resultant orientation enum.</returns>
        private static Orientation CompareOrientation(Vector3D v1, Vector3D v2, double tolerance)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            if (v2 == null)
            {
                throw new ArgumentNullException(nameof(v2));
            }
            if (double.IsNaN(tolerance))
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is NaN");
            }
            if (tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), "tolerance is less than zero");
            }

            Vector3D v1Normalized = GetNormalized(v1, tolerance);
            Vector3D v2Normalized = GetNormalized(v2, tolerance);

            double iP = GetInnerProduct(v1Normalized, v2Normalized);
            // iP = 0 if angle = 90 degrees
            // iP > 0 if v_st and line are same direction
            // iP < 0 if v_st and line are opposite direction

            // is orthogonal
            if (System.Math.Abs(iP) <= tolerance)
            {
                return Orientation.Orthogonal;
            }
            else if (0.0 < iP)
            {
                // IP > 0
                // same direction
                if (System.Math.Abs(1.0 - iP) < tolerance)
                {
                    // parallel
                    return Orientation.Parallel;
                }
                else
                {
                    // only same direction
                    return Orientation.SameDirection;
                }
            }
            else
            {
                // IP < 0
                // opposite direction
                if (System.Math.Abs(1.0 + iP) < tolerance)
                {
                    // anti parallel
                    return Orientation.AntiParallel;
                }
                else
                {
                    // only opposite direction
                    return Orientation.OppositeDirection;
                }
            }
        }

        // --- Basic operations --------------------------------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Adds to the current vector another vector v2. This operation modifies the current instance.
        /// </summary>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 + vector 2.</returns>
        public Vector3D Add(Vector3D v2)
        {
            return Add(this, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Subtracts from the current instance vector2. This operation modifies the current instance.
        /// </summary>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 - vector 2.</returns>
        public Vector3D Subtract(Vector3D v2)
        {
            return Subtract(this, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculate vector * scalar. This operation modifies the current vector instance.
        /// </summary>
        /// <param name="mul">The specified scalar.</param>
        /// <returns>The result of vector * scalar.</returns>
        public Vector3D Multiply(double mul)
        {
            return Multiply(this, mul);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Divide all components of this vector by divisor. This operation modifies the current vector instance.
        /// </summary>
        /// <remarks>
        /// If the scalar divisor IsNaN or equal to zero an exception will be thrown.
        /// </remarks>
        /// <param name="div">The specified scalar divisor.</param>
        /// <returns>The result of vector / scalar divisor.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Invalid divisor NaN or zero.</exception>
        public Vector3D Divide(double div)
        {
            return Divide(this, div);
        }

        // --- Basic operations (operator overloads) ----------------------------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The overloaded + operator.
        /// </summary>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 + vector 2.</returns>
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return Add(v1, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The overloaded - operator.
        /// </summary>
        /// <param name="v1">The specified vector 1.</param>
        /// <param name="v2">The specified vector 2.</param>
        /// <returns>The result of vector 1 - vector 2.</returns>
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return Subtract(v1, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The overloaded * operator calculates vector * scalar.
        /// </summary>
        /// <param name="v1">The specified vector.</param>
        /// <param name="mul">The specified scalar.</param>
        /// <returns>The result of vector * scalar.</returns>
        public static Vector3D operator *(Vector3D v1, double mul)
        {
            return Multiply(v1, mul);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The overloaded * operator calculates scalar * vector.
        /// </summary>
        /// <param name="mul">The specified scalar.</param>
        /// <param name="v1">The specified vector.</param>
        /// <returns>The result of scalar * vector.</returns>
        public static Vector3D operator *(double mul, Vector3D v1)
        {
            return Multiply(v1, mul);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// The overloaded / operator.
        /// </summary>
        /// <remarks>
        /// If the scalar divisor IsNaN or equal to zero an exception will be thrown.
        /// </remarks>
        /// <param name="v1">The specified vector.</param>
        /// <param name="div">The specified scalar divisor.</param>
        /// <returns>The result of vector / scalar divisor.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Invalid divisor NaN or zero.</exception>
        public static Vector3D operator /(Vector3D v1, double div)
        {
            return Divide(v1, div);
        }

        // --- More operations ---------------------------------------------------------------------------

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns the inverted Vector3D of this Vector3D object as a new vector. 
        /// </summary>
        /// <returns>The inverted vector.</returns>
        public Vector3D GetInverted()
        {
            return GetInverted(this);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Gets the length of this vector object.
        /// </summary>
        /// <returns>The length of this vector object.</returns>
        public double GetLength()
        {
            return GetLength(this);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns this Vector3D object as a new and normalized vector. 
        /// </summary>
        /// <remarks>
        /// An exception will be thrown if this vector IsNil according to the 
        /// common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">This vector IsNil.</exception>
        /// <returns>This vector object as a normalized vector.</returns>
        public Vector3D GetNormalized()
        {
            return GetNormalized(this, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns this Vector3D object as a new and normalized vector. 
        /// </summary>
        /// <remarks>
        /// An exception will be thrown if this vector IsNil according to the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">This vector IsNil.</exception>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>This vector object as a normalized vector.</returns>
        public Vector3D GetNormalized(double tolerance)
        {
            return GetNormalized(this, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object is normalized otherwise false.
        /// </summary>
        /// <remarks>
        /// The vector is normalized if
        /// <pre>
        /// (1 - tolerance) is less than  |vector length| is less than (1 + tolerance).
        /// </pre>
        /// The tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <returns>True if the vector is normalized otherwise false.</returns>
        public bool IsNormalized()
        {
            return IsNormalized(this, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object is normalized otherwise false.</summary>
        /// <remarks>
        /// The vector is normalized if
        /// <pre>
        /// (1 - tolerance) is less than  |vector length| is less than (1 + tolerance).
        /// </pre>
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if the vector is normalized otherwise false.</returns>
        public bool IsNormalized(double tolerance)
        {
            return IsNormalized(this, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object isNil.
        /// </summary>
        /// <remarks>
        /// The vector isNil if |vector length| is less than the
        /// common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <returns>True if the vector isNil otherwise false.</returns>
        public bool IsNil()
        {
            return IsNil(this, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object isNil.
        /// </summary>
        /// <remarks>
        /// The vector isNil if |vector length| is less than tolerance.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if the vector isNil otherwise false.</returns>
        public bool IsNil(double tolerance)
        {
            return IsNil(this, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object and the specified vector object are almost equal.
        /// </summary>
        /// <remarks>
        /// They are almost equal if the length of the difference vector
        /// of this vector and specified vector is less than the 
        /// common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>True if both vectors are almost equal otherwise false.</returns>
        public bool IsAlmostEqual(Vector3D v2)
        {
            return IsAlmostEqual(this, v2, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object and the specified vector object are almost equal.
        /// </summary>
        /// <remarks>
        /// They are almost equal if the length of the difference vector
        /// of this vector and specified vector is less than the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if both vectors are almost equal otherwise false.</returns>
        public bool IsAlmostEqual(Vector3D v2, double tolerance)
        {
            return IsAlmostEqual(this, v2, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if the squared length of this Vector2d object is greater than those of the specified vector object.
        /// </summary>
        /// <remarks>
        /// This vector is longer if the difference of the squared length 
        /// of this vector and the specified vector 
        /// is greater than the common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>True if this vector is longer than the specified one, otherwise false.</returns>
        public bool IsLonger(Vector3D v2)
        {
            return IsLonger(this, v2, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if the squared length of this Vector2d object is greater than those of the specified vector object.
        /// </summary>
        /// <remarks>
        /// This vector is longer if the difference of the squared length 
        /// of this vector and the specified vector 
        /// is greater than the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if this vector is longer than the specified one, otherwise false.</returns>
        public bool IsLonger(Vector3D v2, double tolerance)
        {
            return IsLonger(this, v2, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object and the specified vector object are almost parallel.
        /// </summary>
        /// <remarks>
        /// Vectors of inverse or same directions are parallel but oblique vectors are not parallel.
        /// Both are almost parallel if the absolute value of the normalized x, y and z component 
        /// of both vectors are the same at the common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// In this sense vectors of inverse directions are also parallel.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>True if both vectors are almost parallel otherwise false.</returns>
        public bool IsAlmostParallel(Vector3D v2)
        {
            return IsAlmostParallel(this, v2, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns true if this Vector3D object and the specified vector object are almost parallel.
        /// </summary>
        /// <remarks>
        /// Vectors of inverse or same directions are parallel but oblique vectors are not parallel.
        /// Both are almost parallel if the absolute value of the normalized x, y and z component 
        /// of both vectors are the same at the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>True if both vectors are almost parallel otherwise false.</returns>
        public bool IsAlmostParallel(Vector3D v2, double tolerance)
        {
            return IsAlmostParallel(this, v2, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the inner product (scalar product A.B) of this vector A and the specified vector B.
        /// </summary>
        /// <param name="v2">The specified second vector (B).</param>
        /// <returns>The calculated inner product value.</returns>
        public double GetInnerProduct(Vector3D v2)
        {
            return GetInnerProduct(this, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the outer product of this vector and the specified vector.
        /// </summary>
        /// <remarks>
        /// The outer product is also called vector product or cross product.
        /// The resultant vector = this vector X the specified vector.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>The resultant vector</returns>
        public Vector3D GetOuterProduct(Vector3D v2)
        {
            return GetOuterProduct(this, v2);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <remarks>
        /// The first vector is the actual (this) vector object.
        /// The second vector is the specified vector.
        /// The calculated angle in radians is the smaller one of 
        /// the two possible angles. For example:
        /// the angle = 0 if both vectors are parallel.
        /// the angle = PI if both vectors are anti parallel.
        /// the angle = PI/2 if both vectors are perpendicular.
        /// An Exception will be thrown if the length of a vector is
        /// less than the common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The vector length is too small</exception>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>The angle in radians between the two vectors.</returns>
        public double GetAngle(Vector3D v2)
        {
            return GetAngle(this, v2, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <remarks>
        /// The first vector is the actual (this) vector object.
        /// The second vector is the specified vector.
        /// The calculated angle in radians is the smaller one of 
        /// the two possible angles. For example:
        /// the angle = 0 if both vectors are parallel.
        /// the angle = PI if both vectors are anti parallel.
        /// the angle = PI/2 if both vectors are perpendicular.
        /// An Exception will be thrown if the length of a vector is
        /// less than the specified tolerance.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The vector length is too small</exception>
        /// <param name="v2">The specified second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>The angle in radians between the two vectors.</returns>
        public double GetAngle(Vector3D v2, double tolerance)
        {
            return GetAngle(this, v2, tolerance);
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns an orientation enum which describes the direction of this vector regarding the specified vector.
        /// </summary>
        /// <remarks>
        /// Both vectors may be orthogonal, parallel same direction, parallel opposite direction,
        /// pointing to the same direction or point to the opposite direction. The orientation enum
        /// is calculated using the common tolerance value Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <returns>The resultant orientation enum.</returns>
        public Orientation CompareOrientation(Vector3D v2)
        {
            return CompareOrientation(this, v2, Vector3D.DEFAULT_TOLERANCE);
        }
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns an orientation enum which describes the direction of this vector regarding the specified vector.
        /// </summary>
        /// <remarks>
        /// Both vectors may be orthogonal, parallel same direction, parallel opposite direction,
        /// pointing to the same direction or point to the opposite direction. The orientation enum
        /// is calculated using the specified tolerance value.
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="v2">The specified second vector.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <returns>The resultant orientation enum.</returns>
        public Orientation CompareOrientation(Vector3D v2, double tolerance)
        {
            return CompareOrientation(this, v2, tolerance);
        }


        // --- Advanced calculations --------------------------------------------------------------------------------

        /*
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Does a 2D rotation of a list of points about the specified rotation point by a specified angle.
        /// </summary>
        /// <remarks>
        /// There is a given list of position vectors of points.
        /// Only the x and y components of the 3D vectors will be considered. 
        /// The z component will remain unchanged.
        /// Each point will be rotated by the specified angle about a rotation point.
        /// This rotation point is defined by the actual (this) vector object.
        /// The result will be a rotated point on that x/y plane which is shifted by the 
        /// z component of the actual vector list entry.
        /// The rotation is done within a right handed system.
        /// The rotation axis is parallel to the z coordinate
        /// A counterclockwise rotation regarding the positive rotation axis is positive.
        /// The results will be stored within the vector list again.
        /// </remarks>
        /// <param name="rotation_angle">The rotation angle in radians.</param>
        /// <param name="vector_list">The array of points which will be rotated.</param>
        private static Vector3D[] GetVectorsRotatedInXYPlane(Vector3D rotationPoint, double rotation_angle, Vector3D[] vector_list)
        {
            double r_sin    = Math.Sin( -rotation_angle );
            double r_cos    = Math.Cos( -rotation_angle );
            double x_rot    = 0.0;
            double y_rot    = 0.0;
            double x_scal   = 0.0;
            double y_scal   = 0.0;

            Vector3D[] results = new Vector3D[vector_list.Length];
            int idx = 0;

            //rotate about origin if rotation_point = (0.0, 0.0)
            if ((rotationPoint.X == 0.0) && (rotationPoint.Y == 0.0))
            {
                // x_rot =   x_orig * cos(-angle) + y_orig * sin(-angle)
                // y_rot = - x_orig * sin(-angle) + y_orig * cos(-angle)
                foreach (Vector3D vec in vector_list)
                {
                    x_rot =  vec.m_Xval * r_cos + vec.m_Yval * r_sin;
                    y_rot = -vec.m_Xval * r_sin + vec.m_Yval * r_cos;

                    results[idx] = new Vector3D(x_rot, y_rot, vec.Z);
                    idx++;
                }
                return results;
            }

            // if rotation point is not the origin
            // shift to origin - rotate - and shift back
            foreach (Vector3D vec in vector_list)
            {
                x_scal = vec.m_Xval - rotationPoint.X;
                y_scal = vec.m_Yval - rotationPoint.Y;
                x_rot =  x_scal * r_cos + y_scal * r_sin;
                y_rot = -x_scal * r_sin + y_scal * r_cos;

                results[idx] = new Vector3D(x_rot + rotationPoint.X, y_rot + rotationPoint.Y, vec.Z);
                idx++;
            }
            return results;
        }

        /// <apiflag>No</apiflag>
        /// <summary>API:NO         
        /// Calculates the distance to an orthogonal ellipse within the xy-plane.
        /// </summary>
        /// <remarks>
        /// Only the x and y components of this vector object will be considered.
        /// The distance of this 2D point to an ellipse will be calculated.
        /// The ellipse is centered at the origin of the coordinate system
        /// and its major axis is placed upon the x axis 
        /// and its minor axis is placed upon the y axis
        /// of the XY-plane.
        /// The ellipse is defined by (x/a)*(x/a) + (y*b)*(y*b) = 1.
        /// The major axis must be greater than the minor axis 
        /// and both must be greater than the specified tolerance.
        /// The point must not be upon the major axis.
        /// 
        /// This method is only used internally by the public method GetDistanceToEllipse.
        /// 
        /// The specified tolerance is the value when Newton's method iteration is stopped.
        /// It is the difference between two iterations, i.e. the tolerance of the result.
        /// Maximum iteration count is defined to be 10000.
        /// If the tolerance is too small ( e.g. 0.0 ) the Newton's method will not
        /// converge till the iteration count of 1000 and an exception will be thrown.
        /// 
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// The count of needed iterations will be provided.
        /// The nearest point onto the ellipse will be provided.
        /// </remarks>
        /// <exception cref="System.ArithmeticException">The Newton's method does not converge till maximum iteration cycle 1000</exception>
        /// <param name="width">The width which is the half of the major axis = a.</param>
        /// <param name="height">The height which is the half of the minor axis = b.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <param name="nearestX">The x coordinate of nearest point upon ellipse.</param>
        /// <param name="nearestY">The y coordinate of nearest point upon ellipse.</param>
        /// <param name="iterationCount">The count of needed iterations.</param>
        /// <returns>The distance of this point to the ellipse.</returns>
        private double GetDistanceToOrthogonalEllipse (double width, double height, double tolerance, out double nearestX, out double nearestY, out int iterationCount )
        {
            int max_cycle = 10000; // max count of iterations
            int cycle = 0;

            double XDivWidth  = 0.0;
            double YDivHeight = 0.0;

            // first try for Newton's method
            double Try = height *(m_Yval - height);
            
            // solve via Newton’s method maximum 1000 iterations
            for (cycle = 0; cycle < max_cycle+1; cycle++)
            {
                double TpWidthSqr  = Try + width * width;
                double TpHeigthSqr = Try + height* height;

                double InvTpWidthSqr  = 1.0/TpWidthSqr;
                double InvTpHeigthSqr = 1.0/TpHeigthSqr; 

                XDivWidth  = width * m_Xval * InvTpWidthSqr;
                YDivHeight = height* m_Yval * InvTpHeigthSqr;

                double XDivWidthSqr   = XDivWidth  * XDivWidth;
                double YDivHeighthSqr = YDivHeight * YDivHeight;

                double Fn = XDivWidthSqr + YDivHeighthSqr - 1.0;
                if (Fn < tolerance )
                {
                    // Fn is close enough to zero i.e. distance to ellipse is less than tolerance 
                    // -> terminate the iteration
                    break;
                }

                // Newton's try_n+1 = try_n - Fn / Fnp1
                double Fnp1 = 2.0*(XDivWidthSqr * InvTpWidthSqr + YDivHeighthSqr * InvTpHeigthSqr);

                // Ratio = Fb/fnp1 = try_n - try_n+1
                double Ratio = Fn / Fnp1;
                if ( Ratio < tolerance )
                {
                    // try_n - try_n+1 is close enough to zero -> terminate the iteration
                    break;
                }
                Try += Ratio;
            }

            if ( cycle >= max_cycle )
            {
                // method failed to converge -> exception
                throw new System.ArithmeticException("The Newton's method does not converge till iteration cycle: " + cycle+" .");
            }

            // return of iteration count
            iterationCount = cycle;

            // return nearest x/y on ellipse
            nearestX = XDivWidth  * width;
            nearestY = YDivHeight * height;

            // return distance = length of distance vector of this point to nearest point on ellipse
            double dX = nearestX - m_Xval; 
            double dY = nearestY - m_Yval;
            return Math.Sqrt( dX*dX + dY*dY );
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the distance to an ellipse which is parallel to the xy-plane.
        /// </summary>
        /// <remarks>
        /// Calculates the distance of this Vector3D object to the ellipse 
        /// which is specified by the center, width and height vectors.
        /// Each vector is a 3D vector but the z component of each vector will be ignored.
        /// Therefore the calculation works on the xy-plane.
        /// The distance is positive if the point is outside of the ellipse.
        /// Both axis must be orthogonal.
        /// 
        /// The specified tolerance is the value when Newton's method iteration is stopped.
        /// It is the difference between two iterations, i.e. the tolerance of the result.
        /// Maximum iteration count is defined to be 10000.
        /// If the tolerance is too small ( e.g. 0.0 ) the Newton's method will not
        /// converge till the iteration count of 1000 and an exception will be thrown.
        /// 
        /// A common tolerance value is Vector3D.DEFAULT_TOLERANCE.
        /// The count of needed iterations will be provided.
        /// The nearest point onto the ellipse will be provided.
        /// The z component of the nearest point will be the same as the ellipse's center z component.
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">The width and height vectors are not orthogonal.</exception>
        /// <exception cref="System.ArithmeticException">The Newton's method does not converge till maximum iteration cycle 1000.</exception>
        /// <param name="width">The vector form the center to the ellipse into the first axis direction.</param>
        /// <param name="height">The vector form the center to the ellipse into the second axis direction.</param>
        /// <param name="center">The vector to the center of the ellipse.</param>
        /// <param name="tolerance">The specified tolerance.</param>
        /// <param name="nearest">The vector to the nearest point upon the ellipse.</param>
        /// <param name="iterationCount">The count of needed iterations.</param>
        /// <returns>The distance of this point to ellipse.</returns>
        public double GetDistanceToEllipse (Vector3D width, Vector3D height, Vector3D center, double tolerance, Vector3D nearest, out int iterationCount )
        {
            // distance point ellipse (positive if point is inside the ellipse)
            double distance = 0.0; 

            // setup origin of coordinate system = 0/0/0
            Vector2d origin2D      = new Vector2d();
            // setup ellipse's width, height and center 2D vector (skip z-coordinate)
            Vector2d theWidth      = new Vector2d(width.m_Xval,  width.m_Yval );
            Vector2d theHeight     = new Vector2d(height.m_Xval, height.m_Yval);
            Vector2d center2D      = new Vector2d(center.m_Xval, center.m_Yval);
            // setup 2D vector to the point of interest 
            Vector2d orig_point2D  = new Vector2d( m_Xval, m_Yval);

            // transform original point coordinates to ellipse's system
            // point in ellipse's system = original point2D - center_of_ellipse
            Vector2d point2D       = new Vector2d(orig_point2D - center2D );
                        
            // get the length of width vector 2D   a = half of ellipse axis
            double a = Math.Sqrt(theWidth.m_Xval * theWidth.m_Xval + theWidth.m_Yval * theWidth.m_Yval);

            // get the length of height vector 2D  b = half of the other ellipse axis
            double b = Math.Sqrt(theHeight.m_Xval * theHeight.m_Xval + theHeight.m_Yval * theHeight.m_Yval);

            // if it is a circle
            if ( Math.Abs(a-b) < tolerance )
            {
                if (a > 0 && b > 0)     // if length of both width and height vector (ellipse axis) is greater than 0, 
                                        // then check if width and height vector are orthogonal.
                                        // if the length of one axis is 0, the InnerProduct will always be 0.
                {
                    // exception if width and height vector are not orthogonal
                    Vector2d width2DNormalized = theWidth.GetNormalized(Vector3D.DEFAULT_TOLERANCE);
                    Vector2d height2DNormalized = theHeight.GetNormalized(Vector3D.DEFAULT_TOLERANCE);
                    if (!MathsGlobal.IsEqual(0.0, width2DNormalized.GetInnerProduct(height2DNormalized), Vector3D.DEFAULT_TOLERANCE))
                    {
                        throw new System.ArgumentOutOfRangeException("width", "Circle: The width and the height vector are not orthogonal.");
                    }
                }

                // get length of point regarding center of circle
                double pointLen = Math.Sqrt(point2D.m_Xval * point2D.m_Xval + point2D.m_Yval * point2D.m_Yval);
                // calculate nearest x/y on circle
                if ( pointLen < Vector3D.DEFAULT_TOLERANCE ) // if no point direction
                {
                    // point of interest is at the center of the circle -> nearest x / y = 0.0 / a
                    nearest.m_Xval = 0.0 + center2D.m_Xval;
                    nearest.m_Yval = a   + center2D.m_Yval;
                    nearest.m_Zval = center.m_Zval;                 
                }
                else
                {
                    // adjust points length according radius of the circle
                    point2D.Normalize(Vector3D.DEFAULT_TOLERANCE);
                    point2D *= a ;
                    // and calculate coordinates of nearest point
                    nearest.m_Xval = point2D.m_Xval + center2D.m_Xval;
                    nearest.m_Yval = point2D.m_Yval + center2D.m_Yval;
                    nearest.m_Zval = center.m_Zval;                 
                }

                // no iteration
                iterationCount = 0;
                // calculate distance point of interest to circle
                distance = a - pointLen ;

                // finished
                return distance; 
            }

            // no circle -> ellipse or line

            // calculate rotation angle of the ellipse
            //
            // It is that angle about the bigger ellipse axis has to be rotated
            // so that it is mapped onto the x-axis
            double angle = 0.0;
            if( a > b ) // take bigger axis for angle calculation
            { 
                angle = Math.Abs(Math.Asin(width.m_Yval / a));
                if (     width.m_Yval>=0 && width.m_Xval>=0) // 1. Q
                {
                }
                else if (width.m_Yval>=0 && width.m_Xval<0)  // 2. Q
                {
                    angle = Math.PI - angle;
                }
                else if (width.m_Yval<0  && width.m_Xval<0)  // 3. Q
                {
                    angle = angle - Math.PI;
                }
                else if (width.m_Yval<0  && width.m_Xval>=0) // 4. Q
                {
                    angle = -angle;
                }
            }
            else
            {
                angle = Math.Abs(Math.Asin(height.m_Yval / b));
                if (     height.m_Yval>=0 && height.m_Xval>=0)
                {
                }
                else if (height.m_Yval>=0 && height.m_Xval<0)
                {
                    angle = Math.PI - angle;
                }
                else if (height.m_Yval<0  && height.m_Xval<0)
                {
                    angle = angle - Math.PI;
                }
                else if (height.m_Yval<0  && height.m_Xval>=0)
                if (height.m_Yval<0  && height.m_Xval>=0)
                {
                    angle = -angle;
                }
            }

            // ajust (rotate) point according ellipses rotation angle
            Vector2d[] vec_array = new Vector2d[1];
            vec_array[0] = new Vector2d( point2D );
            origin2D.Rotate(-angle, ref vec_array);
            point2D.X = vec_array[0].m_Xval;
            point2D.Y = vec_array[0].m_Yval;
                
            // correct points components to put point into first quatrant of ellipse
            // and clamp to zero if necessary to assure newton's method will converge

            bool pXreflect = false;
            if ( point2D.m_Xval > tolerance )
            {   // nothing to do
            }
            else if ( point2D.m_Xval < -tolerance )
            {   // reflect
                pXreflect = true;
                point2D.m_Xval = -point2D.m_Xval;
            }
            else
            {   // clamp to zero
                point2D.m_Xval = 0.0;
            }
            bool pYreflect = false;
            if ( point2D.m_Yval > tolerance )
            {   // nothing to do
            }
            else if ( point2D.m_Yval < -tolerance )
            {   // reflect
                pYreflect = true;
                point2D.m_Yval = -point2D.m_Yval;
            }
            else
            {   // clamp to zero
                point2D.m_Yval = 0.0;
            }

            // transpose ellipse's major and minor axes if necessary
            if( a < b )
            {
                double save = a;
                a = b;
                b = save;
            }

            // if it is only a line
            if ( b < tolerance )
            {
                // set line
                Vector2d start = new Vector2d();
                Vector2d end   = new Vector2d();

                // calculate nearest point upon the line
                end.m_Xval = a ;
                if ( point2D.m_Xval > a )
                {
                    nearest.m_Xval = a;
                }
                else
                {
                    nearest.m_Xval = point2D.m_Xval;
                }
                nearest.m_Yval = 0.0;
                nearest.m_Zval = center.m_Zval;                 

                // no iteration
                iterationCount = 0;

                // and calculate distance point to line
                distance = -point2D.GetDistanceToLine(start, end); 

            }

            //  it is an ellipse

            else if ( point2D.m_Xval !=0.0 )   // if the point is not upon the minor axis of the ellipse
            {
                if (a > 0 && b > 0)     // if length of both width and height vector (ellipse axis) is greater than 0, 
                                        // then check if width and height vector are orthogonal.
                {
                    // exception if width and height vector are not orthogonal
                    Vector2d width2DNormalized = theWidth.GetNormalized(Vector3D.DEFAULT_TOLERANCE);
                    Vector2d height2DNormalized = theHeight.GetNormalized(Vector3D.DEFAULT_TOLERANCE);
                    if (!MathsGlobal.IsEqual(0.0, width2DNormalized.GetInnerProduct(height2DNormalized), Vector3D.DEFAULT_TOLERANCE))
                    {
                        throw new System.ArgumentOutOfRangeException("width", "Ellipse: The width and the height vector are not orthogonal.");
                    }
                }

                double nearestX = 0.0;
                double nearestY = 0.0;

                if ( point2D.m_Yval != 0.0 )  // if the point is not upon the major axis of the ellipse
                { 
                    // calculate distance and nearest point upon ellise and iteration count
                    Vector3D point3D = new Vector3D(point2D.m_Xval, point2D.m_Yval, 0.0);
                    distance = point3D.GetDistanceToOrthogonalEllipse (a, b, tolerance, out nearestX, out nearestY, out iterationCount );
                    nearest.m_Xval = nearestX;
                    nearest.m_Yval = nearestY;
                    nearest.m_Zval = center.m_Zval;

                    // invert distance if point is outside the ellipse
                    double inout = (point2D.m_Xval*point2D.m_Xval)/(a*a) + (point2D.m_Yval*point2D.m_Yval)/(b*b);
                    if ( inout > 1.0)
                    { 
                        distance = -distance;
                    }
                }
                else
                {   
                    // point is upon major axis !
                    
                    // is point near center ? x-component <= a - b*b/a !
                    double bSqr = b*b;
                    if( point2D.m_Xval < ( a - bSqr/a ))
                    {   
                        // point is upon minor axis and near center
                        double aSqr = a*a;
                        nearestX = aSqr * point2D.m_Xval / (aSqr - bSqr);

                        double xDIVa = nearestX / a;
                        nearestY = b * Math.Sqrt(Math.Abs(1.0-xDIVa*xDIVa));
                        double xDif = nearestX - point2D.m_Xval;

                        distance = Math.Sqrt( xDif * xDif + nearestY * nearestY ) ;

                        nearest.m_Xval = nearestX;
                        nearest.m_Yval = nearestY;
                        nearest.m_Zval = center.m_Zval;

                        // no iteration
                        iterationCount = 0;
                    }
                    else
                    {
                        // point is upon major axis and far of center i.e. x-component > a - b*b/a
                        // -> nearest point on ellipse is end of major axis ! 
                        distance = a - point2D.m_Xval ;
                        
                        nearest.m_Xval = a;
                        nearest.m_Yval = 0.0;
                        nearest.m_Zval = center.m_Zval;

                        // no iteration
                        iterationCount = 0;
                    }
                }
            }
            else
            {   
                // point is upon the minor axis  -> nearest point upon ellipse is end of minor axis
                distance = b - point2D.m_Yval ;

                nearest.m_Xval = 0.0;
                nearest.m_Yval = b;
                nearest.m_Zval = center.m_Zval;

                // no iteration
                iterationCount = 0;
            }

            // set correct sign of nearest x/y
            if (pXreflect)
            {
                nearest.m_Xval = - nearest.m_Xval;
            }
            if (pYreflect)
            {
                nearest.m_Yval = - nearest.m_Yval;
            }

            // transform nearest point coordinates from ellipse's to original system  means
            // rotate nearest point according rotation of ellipse
            // original point2D = point in ellipses system + center_of_ellipse
            vec_array[0] = new Vector2d( nearest.m_Xval, nearest.m_Yval );
            origin2D.Rotate(angle, ref vec_array);
            nearest.m_Xval = vec_array[0].m_Xval + center2D.m_Xval;;
            nearest.m_Yval = vec_array[0].m_Yval + center2D.m_Yval;
            nearest.m_Zval = center.m_Zval;

            return distance;
        }*/
    }
}
