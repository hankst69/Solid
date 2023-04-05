using NUnit.Framework;

using Solid.Infrastructure.Environment.Impl;
using FluentAssertions;
using System.Threading;
using System;

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
        public void ExecuteDelayed()
        {
            // ARRANGE
            var sleep = 250;
            var executed = false;
            int executeThreadId = 0;
            DateTime executeTime = DateTime.MinValue;
            var action = () => 
            {
                executed = true;
                executeTime = DateTime.Now;
                executeThreadId = Thread.CurrentThread.ManagedThreadId;
            };

            var startThreadId = Thread.CurrentThread.ManagedThreadId;
            var startTime = DateTime.Now;

            // ACT
            _target.ExecuteDelayed(action, sleep);

            // ASSERT
            executed.Should().BeFalse();
            Thread.Sleep(sleep+sleep);

            executed.Should().BeTrue();
            (executeTime - startTime).TotalMilliseconds.Should().BeApproximately(sleep, precision: sleep/10);
            //executeThreadId.Should().Be(startThreadId);
        }
    }
}
