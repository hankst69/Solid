//----------------------------------------------------------------------------------
// File: "Vector3DPointLineCalculationsExtensions.cs"
// Author: Steffen Hanke
// Date: 2022
//----------------------------------------------------------------------------------

namespace Solid.Infrastructure.Math
{
    public static class Vector3DPointLineCalculationsExtensions
    {
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the distance of a point of interest to a finite line.
        /// </summary>
        /// <remarks>
        /// This vector object is treated as the point of interest.
        /// The finite line is defined by the specified line start and line end point.
        /// All points are given by the position vector from the origin to the point in the 3D space.
        ///  
        /// If the intersection of the line and the perpendicular line 
        /// through the point of interest is outside the range of the line, 
        /// the distance will be taken from the line's start point respectively 
        /// from the line's end point to the point of interest.
        /// The result is distance to the start point of the line if the length of the line is less 
        /// than Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="line_start">The line's starting point.</param>
        /// <param name="line_end">The line's end point.</param>
        /// <returns>The distance of the point defined by this vector to the specified finite line.</returns>
        public static double GetDistanceToLine(this Vector3D point, Vector3D line_start, Vector3D line_end)
        {
            // Get the nearest point upon the line
            Vector3D nearestPointOnLine = GetNearestPointUponLine(point, line_start, line_end);

            // Calculate the distance
            Vector3D dist_v = point - nearestPointOnLine;

            return dist_v.GetLength();
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the point nearest to given point onto a finite line.
        /// </summary>
        /// <remarks>
        /// This vector object is treated as the point of interest.
        /// The finite line is defined by the specified line start and line end point.
        /// All points are given by the position vector from the origin to the point in the 3D space.
        ///
        /// The nearest point is at the intersection of the line and the perpendicular line 
        /// through the point of interest.
        /// If this intersection is outside the range of the line, 
        /// the nearest point will be the line's start point respectively 
        /// the line's end point.
        /// The result is the start point of the line if the length of the line is less 
        /// than Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="line_start">The line's starting point.</param>
        /// <param name="line_end">The line's end point.</param>
        /// <returns>The nearest point onto the specified finite line.</returns>
        public static Vector3D GetNearestPointUponLine(this Vector3D point, Vector3D line_start, Vector3D line_end)
        {
            // calculate the line vector
            Vector3D line = line_end - line_start;

            // calculate the length of the line
            double lineLength = line.GetLength();

            // find the nearest point on the line to the given point
            Vector3D nearestPointOnLine = null;

            if (Vector3D.DEFAULT_TOLERANCE > lineLength)
            {
                // line is degenerated to a point
                // so the nearest point is either start or end
                nearestPointOnLine = new Vector3D(line_start);
            }
            else
            {
                // v_st is a vector from the beginning of the line to the 
                // point in question
                Vector3D v_st = point - line_start;

                // normalize line
                line /= lineLength;

                // calculate inner product u = v_st * line     i.e.
                // u = |v_st| * |1| * cos (angle)
                // angle is the angle between v_st and line
                // u = 0 if angle = 90 degrees
                // u > 0 if v_st and line are      parallel (when v_st is rotated about angle)
                // u < 0 if v_st and line are anti parallel (when v_st is rotated about angle)
                double u = v_st.GetInnerProduct(line);  // scalar product

                // explicitly calculate the nearest point on line
                if (u < 0.0)             // befor start of line
                {
                    nearestPointOnLine = new Vector3D(line_start);
                }
                else if (u > lineLength) // after end of line
                {
                    nearestPointOnLine = new Vector3D(line_end);
                }
                else                      // on line
                {
                    nearestPointOnLine = (line_start + (line * u));
                }
            }
            return nearestPointOnLine;
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the distance of a point of interest to an infinite line.
        /// </summary>
        /// <remarks>
        /// This vector object is treated as the point of interest.
        /// The infinite line is defined by two specified points upon the line.
        /// All points are given by the position vector from the origin to the point in the 3D space.
        /// The result is the distance to the first point upon the line if the length of the line is less 
        /// than Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="line_P1">The first specified point upon the line.</param>
        /// <param name="line_P2">The second specified point upon the line.</param>
        /// <returns>The distance of the point defined by this vector to the specified infinite line.</returns>
        public static double GetDistanceToInfiniteLine(this Vector3D point, Vector3D line_P1, Vector3D line_P2)
        {
            // find the nearest point on the line to the given point
            Vector3D nearestPointOnLine = GetNearestPointUponInfiniteLine(point, line_P1, line_P2);

            // calculate the distance
            Vector3D dist_v = point - nearestPointOnLine;

            return dist_v.GetLength();
        }

        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Calculates the point nearest to given point onto an infinite line.
        /// </summary>
        /// <remarks>
        /// This vector object is treated as the point of interest.
        /// The infinite line is defined by two points upon the line.
        /// All points are given by the position vector from the origin to the point in the 3D space.
        ///
        /// The nearest point is at the intersection of the line and the perpendicular line 
        /// through the point of interest.
        /// The result is the first point upon the line if the length of the line is less 
        /// than Vector3D.DEFAULT_TOLERANCE.
        /// </remarks>
        /// <param name="line_P1">The first specified point upon the line.</param>
        /// <param name="line_P2">The second specified point upon the line.</param>
        /// <returns>The nearest point onto the specified infinite line.</returns>
        public static Vector3D GetNearestPointUponInfiniteLine(this Vector3D point, Vector3D line_P1, Vector3D line_P2)
        {
            // calculate the line vector
            Vector3D line = line_P2 - line_P1;

            // calculate the length of the line
            double lineLength = line.GetLength();

            // find the nearest point on the line to the given point
            Vector3D nearestPointOnLine = null;

            if (Vector3D.DEFAULT_TOLERANCE > lineLength)
            {
                // line is degenerated to a point
                // so the nearest point is either P1 or P2
                nearestPointOnLine = new Vector3D(line_P1);
            }
            else
            {
                // v_st is a vector from P1 of the line to the 
                // point in question
                Vector3D v_st = point - line_P1;

                // normalize line
                line /= lineLength;

                // calculate inner product u = v_st * line     i.e.
                // u = |v_st| * |1| * cos (angle)
                // angle is the angle between v_st and line
                // u = 0 if angle = 90 degrees
                // u > 0 if v_st and line are      parallel (when v_st is rotated about angle)
                // u < 0 if v_st and line are anti parallel (when v_st is rotated about angle)
                double u = v_st.GetInnerProduct(line);  // scalar product

                // calculate the nearest point on the line
                nearestPointOnLine = (line_P1 + (line * u));

            }
            return nearestPointOnLine;
        }

    }
}
