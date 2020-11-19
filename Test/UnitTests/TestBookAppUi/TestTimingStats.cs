// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Infrastructure.LoggingServices;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestTimingThisUrl
    {
        [Theory]
        [InlineData(new double[] {})]
        [InlineData(new double[]{1.0, 2.0})]
        [InlineData(new double[] { 1.0, 2.0, 3.0})]
        [InlineData(new double[] { 1.0, 2.0, 3.0, 4.0 })]
        [InlineData(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 })]
        [InlineData(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 })]
        public void TestCreateTimingAllTimings(double[] timings)
        {
            //SETUP
            var expectedMin = timings.Any() ? timings.Min(x => x) : 0;
            var expectedMax = timings.Any() ? timings.Max(x => x) : 0;
            var expectedAve = timings.Any() ? timings.Average(x => x) : 0;

            //ATTEMPT
            var stats = new TimingThisUrl("test", timings.ToList(), DateTime.Now, 5);

            //VERIFY
            stats.AllTimings.MinMillisecond.ShouldEqual(expectedMin);
            stats.AllTimings.MaxMillisecond.ShouldEqual(expectedMax);
            stats.AllTimings.AverageMilliseconds.ShouldEqual(expectedAve);
        }
    }
}