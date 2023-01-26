//----------------------------------------------------------------------------------
// <copyright file="Vector3DComparer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2019. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Infrastructure.Math
{
    public class Vector3DComparer : IComparer<Vector3D>, IEqualityComparer<Vector3D>
    {
        int IComparer<Vector3D>.Compare(Vector3D a, Vector3D b)
        {
            if (a == null && b == null)
            {
                return 0;
            }
            if (a != null && b == null)
            {
                return 1;
            }
            if (a == null)
            {
                return -1;
            }
            // both are nor null:
            return a.GetLength().CompareTo(b.GetLength());
        }

        bool IEqualityComparer<Vector3D>.Equals(Vector3D a, Vector3D b)
        {
            if (a != null && b != null)
            {
                return a.IsAlmostEqual(b);
            }
            return a == b;
        }

        int IEqualityComparer<Vector3D>.GetHashCode(Vector3D point)
        {
            return point.X.GetHashCode() ^ point.Y.GetHashCode() ^ point.Z.GetHashCode();
        }
    }
}