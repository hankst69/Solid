using NUnit.Framework;

using Solid.Infrastructure.Environment.Impl;
using Solid.Infrastructure.Environment;
using FluentAssertions;
using System.Threading.Tasks;
using System.Threading;

namespace Solid.Infrastructure_uTest.Environment
{
    public class MultiThreadingHelperTests
    {
        private MultiThreadingHelper _target;

        [SetUp]
        public void SetUp()
        {
            _target = new MultiThreadingHelper();
        }

        [Test]
        public void ExecuteDelayedInCurrentThreadTest()
        {
            // ARRANGE
            var value = 1;
            var sleep = 250;
            var action = () => 
            { 
                value++;
            };
            
            // ACT
            _target.ExecuteDelayedInCurrentThread(action, sleep);

            // ASSERT
            value.Should().Be(1);
            Thread.Sleep(sleep+sleep);
            value.Should().Be(2);
        }
    }
}
