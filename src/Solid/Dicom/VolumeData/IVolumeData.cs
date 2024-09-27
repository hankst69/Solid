//----------------------------------------------------------------------------------
// File: "IVolumeData.cs"
// Author: Steffen Hanke
// Date: 2019
//----------------------------------------------------------------------------------
using System.Collections.Generic;
using Solid.Dicom.ImageData;
using Solid.Infrastructure.Math;

namespace Solid.Dicom.VolumeData
{
    /// <summary>
    /// IVolumeData
    /// </summary>
    public interface IVolumeData
    {
        uint DimensionSizeX { get; }    //number of voxels in dimension x
        uint DimensionSizeY { get; }    //number of voxels in dimension y
        uint DimensionSizeZ { get; }    //number of voxels in dimension z

        double VoxelSizeX { get; }      //size of voxels in x direction (mm)
        double VoxelSizeY { get; }      //size of voxels in y direction (mm)
        double VoxelSizeZ { get; }      //size of voxels in z direction (mm)

        Vector3D OrientationX { get; }  //direction of x dimension within PatientCoordianteSystem (typically the patient orientation row value of the images that build the voume)
        Vector3D OrientationY { get; }  //direction of y dimension within PatientCoordianteSystem (typically the patient orientation col value of the images that build the voume)
        Vector3D OrientationZ { get; }  //direction of z dimension within PatientCoordianteSystem (typically the outer product of the previous 2 unles the image positions are shifted) -- not used by the algorithm

        Vector3D Position { get; }      //position of center of first voxel in patient coords

        ushort[] Voxels { get; }        //voxels of all input images in serial order
        bool IsSigned { get; }          //voxel data is signed / unsigned short

        IEnumerable<IImageData> SourceImageDatas { get; }
    }
}
