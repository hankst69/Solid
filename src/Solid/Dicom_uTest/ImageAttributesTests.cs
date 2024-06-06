//----------------------------------------------------------------------------------
// File: "ImageAttributesTests.cs"
// Author: Steffen Hanke
// Date: 2019-2022
//----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Solid.Dicom;
using Solid.Dicom.ImageInfo.Impl;
using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.Math;
using NUnit.Framework;

namespace Solid.Dicom_uTest
{
    internal class ImageAttributesTests
    {
        private Mock<ITracer> m_ITracerMock;
        private Mock<IMrDicomAccess> m_MrDicomAccessMock;
        private Mock<IDicomFrameDataSet> m_IDataSetMock;
        private ImageAttributes m_Target;

        [SetUp]
        public void Setup()
        {
            m_ITracerMock = new Mock<ITracer>();
            m_MrDicomAccessMock = new Mock<IMrDicomAccess>();
            m_IDataSetMock = new Mock<IDicomFrameDataSet>();
            m_Target = new ImageAttributes(m_MrDicomAccessMock.Object, m_IDataSetMock.Object);
        }

        [Test]
        public void Ctor_ShouldThrow_WhenMrDicomAccessIsNull()
        {
            // Arrange
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ImageAttributes(null, m_IDataSetMock.Object);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Ctor_ShouldThrow_WhenDataSetIsNull()
        {
            // Arrange
            // Act
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ImageAttributes(m_MrDicomAccessMock.Object, null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void AccessInterfaceProperties_ShouldDelegateToMatchingMrDicomAccessMembers()
        {
            // Arrange
            var target = new ImageAttributes(m_MrDicomAccessMock.Object, m_IDataSetMock.Object);
            var imageAttributesType = typeof(Solid.Dicom.ImageInfo.IImageAttributes);
            var imageAttributesProperties = imageAttributesType.GetProperties();
            var mrDicomAccessType = typeof(Solid.Dicom.IMrDicomAccess);
            var mrDicomAccessTypeMethods = mrDicomAccessType.GetMethods();

            foreach (var propertyInfo in imageAttributesProperties)
            {
                var methodInfo = mrDicomAccessTypeMethods.FirstOrDefault(x => x.Name.Contains(propertyInfo.Name));
                if (methodInfo == null)
                {
                    Console.WriteLine("for property '" + propertyInfo.Name + "' no matching member could be found in IMrDicomAccess");
                    var isSpecialProperty = (propertyInfo.Name.Equals("FrameNumber") || propertyInfo.Name.Equals("DataSet") || (propertyInfo.Name.StartsWith("Image") && propertyInfo.Name.EndsWith("Info")));
                    isSpecialProperty.Should().BeTrue();
                    continue;
                }

                // Act
                Console.WriteLine("testing property '" + propertyInfo.Name + "' to call 'IMrDicomAccess." + methodInfo.Name + "'");
                var value = propertyInfo.GetValue(target, null);

                // proof of concept: Mock.Verify used with explicitely created expression:
                //m_MrDicomAccessMock.Verify(x => x.GetSopClassUid(m_IDataSetMock.Object), Times.Once);
                //Expression<Func<IMrDicomAccess, string>> expr1 = x => x.GetSopClassUid(m_IDataSetMock.Object);
                //m_MrDicomAccessMock.Verify(expr1, Times.Once);

                // Assert
                // setup method call expression tree from given methodInfo:
                // remark:
                // * since Mock.Verify is not interested in the return value type and since the return type 
                //   varies between objects (string, Vector3D) and different types (int, bool, double, DateTime)
                //   we just setup a Lambda expression that represents an action instead of a func
                // * otherwise we would need different expressions for every used return type (int, bool, double, DateTime)
                var methodInstanceParamExpression = Expression.Parameter(typeof(IMrDicomAccess), "target");
                var methodCallExpression =
                    Expression.Lambda<Action<IMrDicomAccess>>( //<Func<IMrDicomAccess,object>>(
                        Expression.Call(
                            methodInstanceParamExpression,
                            methodInfo,
                            Expression.Constant(m_IDataSetMock.Object)
                        ),
                        methodInstanceParamExpression
                    );
                m_MrDicomAccessMock.Verify(methodCallExpression, Times.Once);
            }
        }

        [Test]
        public void TestExpressionTreeCreation()
        {
            // Arrange
            // setup expression tree via anonymous lambda function: 
            Expression<Func<double, double, double>> distanceCalc =
                (x, y) => Math.Sqrt(x * x + y * y);

            // setup same expression tree step by step:
            var xParameter = Expression.Parameter(typeof(double), "x");
            var yParameter = Expression.Parameter(typeof(double), "y");
            var xSquared = Expression.Multiply(xParameter, xParameter);
            var ySquared = Expression.Multiply(yParameter, yParameter);
            var sum = Expression.Add(xSquared, ySquared);
            var sqrtMethod = typeof(Math).GetMethod("Sqrt", new[] {typeof(double)});
            var distance = Expression.Call(sqrtMethod, sum);
            var distanceLambda = Expression.Lambda(
                    distance,
                    xParameter,
                    yParameter)
                as Expression<Func<double, double, double>>;

            // Act
            // compile expression into delegate and execute:
            var c1 = distanceCalc.Compile()(3, 4);
            // compile expression into delegate and execute:
            var c2 = distanceLambda.Compile()(3, 4);

            // Assert
            c1.Should().Be(c2);
        }

        [TestCase("", true)]
        [TestCase("ROW", true)]
        [TestCase("COLUMN", false)]
        public void IsInplanePhaseInRowDirection_ShouldReturnExpectedValue(string direction, bool expectedValue)
        {
            // Arrange
            m_MrDicomAccessMock.Setup(x => x.GetInPlanePhaseEncodingDirection(m_IDataSetMock.Object)).Returns(direction);

            // Act
            var readValue = m_Target.ImageScanInfo.IsInplanePhaseInRowDirection;

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [TestCase("", true)]
        [TestCase("1", true)]
        [TestCase("0", false)]
        public void IsPhaseEncodingDirectionPositive_ShouldReturnExpectedValue(string isPositive, bool expectedValue)
        {
            // Arrange
            m_MrDicomAccessMock.Setup(x => x.GetPhaseEncodingDirectionPositive(m_IDataSetMock.Object)).Returns(isPositive);

            // Act
            var readValue = m_Target.ImageScanInfo.IsPhaseEncodingDirectionPositive;

            // Assert
            readValue.Should().Be(expectedValue);
        }

        [TestCase("allValid", true)]
        [TestCase("posNull rowNull colNull", false)]
        [TestCase("posNil rowNil colNil", false)]
        [TestCase("posNil", false)]
        [TestCase("rowNil", false)]
        [TestCase("colNil", false)]
        [TestCase("posNull", false)]
        [TestCase("rowNull", false)]
        [TestCase("colNull", false)]
        public void ContainsImagePlane_ShouldReturnExpectedValue(string condition, bool hasImagePlane)
        {
            // Arrange
            Vector3D vec3dNil = new Vector3D();
            Vector3D vec3dValid = new Vector3D(1, 2, 3);

            Vector3D pos = vec3dValid;
            Vector3D row = vec3dValid;
            Vector3D col = vec3dValid;
            if (condition.Contains("posNil")) pos = vec3dNil;
            if (condition.Contains("rowNil")) row = vec3dNil;
            if (condition.Contains("colNil")) col = vec3dNil;
            if (condition.Contains("posNull")) pos = null;
            if (condition.Contains("rowNull")) row = null;
            if (condition.Contains("colNull")) col = null;

            if (pos != null) m_MrDicomAccessMock.Setup(x => x.GetImagePosition(m_IDataSetMock.Object)).Returns(pos);
            if (row != null) m_MrDicomAccessMock.Setup(x => x.GetOrientationRow(m_IDataSetMock.Object)).Returns(row);
            if (col != null) m_MrDicomAccessMock.Setup(x => x.GetOrientationCol(m_IDataSetMock.Object)).Returns(col);

            //var builder = hasImagePlane ? new MrImageBuilder().WithImagePlane(new Vector3D(1, 2, 3), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)) : new MrImageBuilder();
            //var frame = builder.ToDicomFrameDataSetMock().Object;

            // Act
            var readValue = m_Target.ImagePlaneInfo.ContainsImagePlane;

            // Assert
            readValue.Should().Be(hasImagePlane);
        }
    }
}
