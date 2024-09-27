//----------------------------------------------------------------------------------
// File: "BootStrapperTests.cs"
// Author: Steffen Hanke
// Date: 2017-2022
//----------------------------------------------------------------------------------
using System;
using FluentAssertions;
using Moq;
using Solid.Infrastructure.BootStrapper;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.RuntimeTypeExtensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Solid.Infrastructure_uTest.BootStrapper
{
    class BootStrapperTests
    {
        private Infrastructure.BootStrapper.Impl.BootStrapper m_Target;
        private Mock<IDiContainer> m_DiContainerMock;
        private Mock<IBootable> m_BootableMock;
        private IDiRegistrar[] m_Registrars;

        [SetUp]
        public void SetUp()
        {
            var registrarMock = new Mock<IDiRegistrar>();
            m_Registrars = new IDiRegistrar[] { registrarMock.Object };

            m_BootableMock = new Mock<IBootable>();
            m_DiContainerMock = new Mock<IDiContainer>();

            m_DiContainerMock.Setup(x => x.ResolveAllImplementing<IBootable>()).Returns(new List<IBootable>() {m_BootableMock.Object});

            m_Target = new Infrastructure.BootStrapper.Impl.BootStrapper(m_DiContainerMock.Object);
        }

        [Test]
        public void Ctor_ShouldThrow_WhenDiContainerIsNull()
        {
            // Arrange
            // Act
            Action action = () => new Infrastructure.BootStrapper.Impl.BootStrapper(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Startup_ShouldThrow_WhenRegistrarsIsNull()
        {
            // Arrange
            // Act
            Action action = () => m_Target.Startup(null);

            // Assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Startup_ShouldCallRegister_OnAllRegistrars()
        {
            // Arrange
            // Act
            m_Target.Startup(m_Registrars);

            // Assert
            m_Registrars.ForEach(r => Mock.Get(r).Verify(x => x.Register(m_DiContainerMock.Object), Times.Once));
        }

        [Test]
        public void Startup_ShouldResolveAllImplementing_IBootable()
        {
            // Arrange
            // Act
            m_Target.Startup(m_Registrars);

            // Assert
            m_DiContainerMock.Verify(x => x.ResolveAllImplementing<IBootable>(), Times.Once);
        }

        [Test]
        public void Shutdown_ShouldCallFini_OnAllBootables()
        {
            // Arrange
            m_Target.Startup(m_Registrars);

            // Act
            m_Target.Shutdown();

            // Assert
            m_BootableMock.Verify(x => x.Fini(), Times.Once);
        }

        //[Test]
        //public void Shutdown_ShouldResolveAndDisposeAllImplementingIBootable()
        //{
        //    // Arrange
        //    // Act
        //    m_Target.Shutdown();
        //    // Assert
        //    m_DiContainerMock.Verify(x => x.ResolveAllImplementing<IBootable>(), Times.Once);
        //    m_BootableMoc.Verify(x => x.Dispose(), Times.Once);
        //}
        //[Test]
        //public void Shutdown_ShouldResolveAllImplementingIDsposable()
        //{
        //    // Arrange
        //    // Act
        //    m_Target.Shutdown();
        //    // Assert
        //    m_DiContainerMock.Verify(x => x.ResolveAllImplementing<IDisposable>(), Times.Once);
        //}
    }
}
