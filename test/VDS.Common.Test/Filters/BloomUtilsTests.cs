/*
VDS.Common is licensed under the MIT License

Copyright (c) 2012-2015 Robert Vesse

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using Xunit;

namespace VDS.Common.Filters
{
    public class BloomUtilsTests
    {
        // Test cases are based on values calculated at http://hur.st/bloomfilter

        private void CheckErrorRate(long expectedItems, long expectedErrorRate, IBloomFilterParameters parameters)
        {
            long actualErrorRate = CalculateErrorRate(expectedItems, parameters);
            Console.WriteLine("n = {0}, p = 1 in {1}", expectedItems, actualErrorRate);
            Assert.Equal(expectedErrorRate, actualErrorRate);

            // If we half the number of items we add the error rate should decrease
            // NB - Since we are expressing error rate as 1 in p actual value will increase
            actualErrorRate = CalculateErrorRate(expectedItems/2, parameters);
            Console.WriteLine("n = {0}, p = 1 in {1}", expectedItems / 2, actualErrorRate);
            Assert.True(actualErrorRate > expectedErrorRate);

            // If we double the number of items we add the error rate should increase
            // NB - Since we are expressing error rate as 1 in p actual value will decrease
            actualErrorRate = CalculateErrorRate(expectedItems * 2, parameters);
            Console.WriteLine("n = {0}, p = 1 in {1}", expectedItems * 2, actualErrorRate);
            Assert.True(actualErrorRate < expectedErrorRate);
        }

        private static long CalculateErrorRate(long expectedItems, IBloomFilterParameters parameters)
        {
            double calcErrorRate = BloomUtils.CalculateErrorRate(expectedItems, parameters);
            return Convert.ToInt64(1/calcErrorRate);
        }

        [Theory]
        [InlineData(100000, 100000, 2396265, 17)]
        [InlineData(100000, 10000, 1917012, 13)]
        [InlineData(100000, 1000, 1437759, 10)]
        public void CheckParameterCalculation(long expectedItems, long errorRate, int expectedNumBits, int expectedNumHashFunctions)
        {
            IBloomFilterParameters parameters = BloomUtils.CalculateBloomParameters(expectedItems, errorRate);
            Assert.Equal(expectedNumBits, parameters.NumberOfBits);
            Assert.Equal(expectedNumHashFunctions, parameters.NumberOfHashFunctions);

            CheckErrorRate(expectedItems, errorRate, parameters);
        }
    }
}
