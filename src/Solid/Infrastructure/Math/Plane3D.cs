//----------------------------------------------------------------------------------
// File: "Plane3D.cs"
// Author: Steffen Hanke
// Date: 2021
// partly based on: syngo.Services.ImageProcessing/Maths/MathsSlice.cs
//----------------------------------------------------------------------------------
using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.Math
{
    public class Plane3D
    {
        /// <apiflag>Yes</apiflag>
        /// <summary>API:YES 
        /// Returns the projection of a specified 3D point onto the specified image plane.
        /// </summary>
        /// <remarks>
        /// The image plane is defined by the position, row and column vector.
        /// </remarks>
        /// <param name="point">The specified 3D point which will be projected onto the image plane.</param>
        /// <param name="planePos">The image plane position (one point residing on the plane).</param>
        /// <param name="planeRowDir">The image plane row direction.</param>
        /// <param name="planeColDir">The image plane column direction.</param>
        /// <returns>3D projected point</returns>
        public static Vector3D ProjectPointOntoPlane(Vector3D point, Vector3D planePos, Vector3D planeRowDir, Vector3D planeColDir)
        {
            ConsistencyCheck.EnsureArgument(point).IsNotNull();
            ConsistencyCheck.EnsureArgument(planePos).IsNotNull();
            ConsistencyCheck.EnsureArgument(planeRowDir).IsNotNull();
            ConsistencyCheck.EnsureArgument(planeColDir).IsNotNull();

            // Compute the normal vector of the plane
            Vector3D normal = planeRowDir.GetOuterProduct(planeColDir).GetNormalized();

            // Compute the vector 'diff' from the image position to the 3D point
            Vector3D diff = point - planePos;

            // Compute the distance from 3D point to plane (part of 'diff' in planes 'normal' direction)
            // -> using inner product of 'diff' and 'normal'
            // -> sign of inner product is positive if normal and projection of diff onto normal are of the same direction.
            double distance = normal.GetInnerProduct(diff);

            // if input data was invalid (e.g. point contained a coordinate component of value NaN), then distance might be NaN
            // we either have to ensure valid arguments or we have to handle that here to avoid exception in Vector3D scalar multiplication
            if (double.IsNaN(distance))
            {
                return new Vector3D();
                //throw ApplicationException("distance is NaN");
                //return null;
                //return point;
            }

            // if the direction of the normal vector is pointing towards the point 
            // the distance is positive and the direction normal*distance is pointing away from the plane
            // -> result = point - (normal*distance)
            // if the direction of the normal vector is pointing away from the point 
            // the distance is negative and the direction normal*distance is also pointing away from the plane
            // because the negative distance inverses the direction of the normal vector. And again
            // -> result = point - (normal*distance)

            // Compute the projected 3D point
            return point - normal * distance;
        }
    }
}
