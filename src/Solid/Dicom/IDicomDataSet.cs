//----------------------------------------------------------------------------------
// File: "IDicomDataSet.cs"
// Author: Steffen Hanke
// Date: 2019-2020
//----------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Solid.Dicom
{
    /// <summary>
    /// API:YES
    /// IDicomDataSet
    /// </summary>
    public interface IDicomDataSet
    {
        /// <summary>API:YES
        /// Gets the location of the IDicomDataSet.
        /// </summary>
        /// <remarks>
        /// this could be either a file location or a SyngoUid
        /// syntax:
        /// "SyngoUid~~string.representation.of.syngouid"
        /// "File~~string.representation.of.file.location"
        /// </remarks>
        string DataSetLocationUid { get; }

        /// <summary>API:YES
        /// Gets the unique class identifier representing the type of this IDicomDataSet.
        /// </summary>
        string DataSetSopClassUid { get; }

        /// <summary>API:YES
        /// Gets the unique identifier representing this IDicomDataSet.
        /// </summary>
        string DataSetSopInstanceUid { get; }


        /// <summary>API:YES
        /// Check whether the IDicomDataSet is empty. IDicomDataSet is considered not empty when it contains
        /// at least one element which was not marked to remove.
        /// </summary>
        /// <returns>Bool (true/false).</returns>
        bool IsEmpty();

        /// <summary>API:YES
        /// Returns the total number of root level attributes.
        /// Elements marked to remove will be considered as well.
        /// </summary>
        int GetNumberOfElements();

        /// <summary>API:YES
        /// Returns the tagas of all elements contained in the dataset as collection
        /// Elements marked to remove will be considered as well.
        /// </summary>
        IEnumerable<long> GetElements();


        /// <summary>API:YES
        /// Check for the presence of an attribute within dataset.
        /// An element marked to remove is considered non-existent.
        /// </summary>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>Bool (true/false).</returns>
        bool Contains(long tag);

        /// <summary>API:YES
        /// Checks whether any value is set for the attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>Bool (true/false).</returns>
        bool IsElementEmpty(long tag);

        /// <summary>API:YES
        /// Get the number of values set for an attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>Count as Int32 </returns>
        int GetNumberOfValues(long tag);

        ///// <summary>API:YES
        ///// Gets the data type / Value representation of an element.
        ///// </summary>
        ///// <remarks>
        ///// The data type is returned only if the attribute is present within the data set.
        ///// </remarks>
        ///// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        ///// <param name="tag">Attribute used as key for the value</param>
        ///// <returns>Data type / Value Representation</returns>
        //short GetDataType(long tag);


        /// <summary>API:YES
        /// Checks whether any value is set for the attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>Bool (true/false).</returns>
        bool ContainsValue(long tag);

        /// <summary>API:YES
        /// Checks whether any value is set for the attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <param name="index">index of value</param>
        /// <returns>Bool (true/false).</returns>
        bool ContainsValueAt(long tag, int index);

        /// <summary>API:YES
        /// Checks for the emptiness of zeroth index of an attribute.
        /// </summary>
        /// <remarks>
        /// The following values are considered to be empty values:
        ///  - string.Empty
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If there was no value inserted, or all values have been removed. Use <see cref="M:Solid.Dicom.IDicomDataSet.IsElementEmpty(System.Int64)" /> to avoid this exception.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>Bool (true/false)</returns>
        bool IsValueEmpty(long tag);

        /// <summary>API:YES
        /// Checks for the emptiness at any index of an attribute
        /// </summary>
        /// <remarks>
        /// The following values are considered to be empty values:
        ///  - string.Empty
        /// </remarks>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If the given index is out of bounds. Use <see cref="M:Solid.Dicom.IDicomDataSet.IsElementEmpty(System.Int64)" /> and/or <see cref="M:Solid.Dicom.IDicomDataSet.GetNumberOfValues(System.Int64)" /> to avoid this exception.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <param name="index">index of value</param>
        /// <returns>Bool (true/false)</returns>
        bool IsValueEmptyAt(long tag, int index);



        /// <summary>API:YES
        /// Get value at zeroth index as object from an attribute.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the element is a sequence (use GetItem instead).</exception>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If there is no value at the element.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>value as Object.</returns>
        object GetValue(long tag);

        /// <summary>API:YES
        /// Get value at the given index as object from an attribute.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">If the element is a sequence (use GetItemAt instead).</exception>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If parameter index is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If there is no value at the specified position.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <param name="index">index of value</param>
        /// <returns>value as Object.</returns>
        object GetValueAt(long tag, int index);


        /// <summary>API:YES
        /// Get all the values of the element as Byte array.
        /// Padding is done within the IDicomDataSet implementation in case the stream set by the user is not even length
        /// or in case the conversion of existing values to bytestream resulted in an odd length byte stream. The padding
        /// is done based on VR of the tag, the character with which the byte stream is padded depends on the VR.
        /// for e.g. for VR - OB the padding is done with zero byte.
        /// </summary>
        /// <remarks>
        /// Returned stream is in dicom standadard, ex: For AT vr type stream representation is 16bit pairs
        /// </remarks>
        /// <exception cref="T:System.InvalidOperationException">If the element is a sequence (use GetItem instead).</exception>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>value as Byte array.</returns>
        byte[] GetValueAsByteStream(long tag);

        ///// <summary>API:YES
        ///// Get all the values of the element as Byte array. For SCS type VRs (LO, LT, PN, SH, ST, UT) when a character of value
        ///// can't be encoded with the applied SCS value, this character will be replaced with '?' in ASCII and encoded.
        ///// Padding is done within the IDicomDataSet implementation in case the stream set by the user is not even length
        ///// or in case the conversion of existing values to byte stream resulted in an odd length byte stream. The padding
        ///// is done based on VR of the tag, the character with which the byte stream is padded depends on the VR.
        ///// e.g. for VR - OB the padding is done with zero byte.
        ///// </summary>
        ///// <remarks>
        ///// Returned stream is in dicom standadard, ex: For AT vr type stream representation is 16bit pairs.
        ///// </remarks>
        ///// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        ///// <exception cref="T:syngo.Services.CorruptElementException">If the element is corrupt.</exception>
        ///// <param name="tag">Attribute used as key for the value.</param>
        ///// <param name="failedChars_out">Count of characters which were replaced because of conversion error.</param>
        ///// <returns>Value as byte array.</returns>
        //byte[] ForceGetValueAsByteStream(long tag, out int failedChars_out);


        /// <summary>API:YES
        /// Gets an IDicomDataSet (sequence item) at zeroth index from an attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.InvalidOperationException">If the value representation of the element is not sequence (SQ).</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <returns>IDicomDataSet</returns>
        IDicomDataSet GetItem(long tag);

        /// <summary>API:YES
        /// Gets an IDicomDataSet (sequence item) at an index from an attribute.
        /// </summary>
        /// <exception cref="T:System.ArgumentException">If the element does not exist or it is marked to remove.</exception>
        /// <exception cref="T:System.InvalidOperationException">If the value representation of the element is not sequence (SQ).</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">If index is negative or is greater than the highest available index.</exception>
        /// <param name="tag">Attribute used as key for the value</param>
        /// <param name="index">index from which the sequence/item has to be given out</param>
        /// <returns>IDicomDataSet</returns>
        IDicomDataSet GetItemAt(long tag, int index);
    }
}