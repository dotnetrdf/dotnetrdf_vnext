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
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace VDS.Common.Filters
{
    public abstract class AbstractBloomFilterContractTests
    {
        /// <summary>
        /// Creates an instance of a bloom filter for testing
        /// </summary>
        /// <param name="numBits">Number of bits</param>
        /// <param name="hashFunctions">Hash Functions</param>
        /// <returns>Bloom Filter</returns>
        protected abstract IBloomFilter<String> CreateInstance(int numBits, IEnumerable<Func<String, int>> hashFunctions);

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void BloomFilterInstantiation1(int numBits)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CreateInstance(numBits, Enumerable.Empty<Func<String, int>>());
            });
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterInstantiation2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CreateInstance(2, Enumerable.Empty<Func<String, int>>());
            });
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterInstantiation3()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CreateInstance(2, Enumerable.Repeat((Func<String, int>)(s => s.GetHashCode()), 1));
            });
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterInstantiation4()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CreateInstance(2, Enumerable.Repeat((Func<String, int>)(s => s.GetHashCode()), 2));
            });
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterInstantiation5()
        {
            CreateInstance(3, Enumerable.Repeat((Func<String, int>)(s => s.GetHashCode()), 2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain1()
        {
            const string item1 = "test";

            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>) (x => x.GetHashCode())), 2));
            Assert.False(filter.MayContain(item1));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain2()
        {
            const string item1 = "test";
            const string item2 = "other";

            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>)(x => x.GetHashCode())), 2));
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain3()
        {
            const string item1 = "test";
            const string item2 = "other";

            // In this test use has functions that will map all items to the same hash values
            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>)(_ => 0)), 2));
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            // Due to dud hash functions should get a false positive here
            Assert.True(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain4()
        {
            const string item1 = "test";
            const string item2 = "other";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain5()
        {
            const string item1 = "test";
            const string item2 = "time";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            // Only one hash should be equivalent so should be negative
            Assert.False(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterMayContain6()
        {
            const string item1 = "test";
            const string item2 = "tat";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            // Both hash functions give equal values so should be a false positive
            Assert.True(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd1()
        {
            const string item1 = "test";

            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>)(x => x.GetHashCode())), 2));
            Assert.False(filter.MayContain(item1));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));

            // Adding the item again should report false since it is already in the filter
            Assert.False(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd2()
        {
            const string item1 = "test";
            const string item2 = "other";

            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>)(x => x.GetHashCode())), 2));
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));

            // Adding the second item should report true because it is considered different
            Assert.True(filter.Add(item2));
            Assert.True(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd3()
        {
            const string item1 = "test";
            const string item2 = "other";

            // In this test use has functions that will map all items to the same hash values
            IBloomFilter<String> filter = this.CreateInstance(100, Enumerable.Repeat(((Func<String, int>)(_ => 0)), 2));
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));

            // Due to dud hash functions should get false here since the items are considered equivalent
            // This is a false positive
            Assert.False(filter.Add(item2));
            Assert.True(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd4()
        {
            const string item1 = "test";
            const string item2 = "other";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            // Should be possible to add the second item
            Assert.True(filter.Add(item2));
            Assert.True(filter.MayContain(item2));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd5()
        {
            const string item1 = "test";
            const string item2 = "time";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            // Only one hash should be equivalent so should be possible to add the additional item
            Assert.True(filter.Add(item2));
            Assert.True(filter.MayContain(item1));
        }

        [Fact, Trait("Category", "Filters")]
        public void BloomFilterAdd6()
        {
            const string item1 = "test";
            const string item2 = "tat";

            // In this test use hash functions that will map the first and last letters to their character values
            IBloomFilter<String> filter = this.CreateInstance(100, new Func<String, int>[] { x => x.ToCharArray()[0], x => x.ToCharArray()[x.Length - 1] });
            Assert.False(filter.MayContain(item1));
            Assert.False(filter.MayContain(item2));

            Assert.True(filter.Add(item1));
            Assert.True(filter.MayContain(item1));

            // Both hash functions give equal values so should return false since it is already considered to be in the filter
            // This is a false positive
            Assert.False(filter.Add(item2));
            Assert.True(filter.MayContain(item2));
        }
    }

    public class NaiveBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            return new NaiveBloomFilter<string>(numBits, hashFunctions);
        }
    }

    public class SparseNaiveBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            return new SparseNaiveBloomFilter<string>(numBits, hashFunctions);
        }
    }

    public class FastBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            IList<Func<String, int>> funcs = hashFunctions as IList<Func<string, int>> ?? hashFunctions.ToList();
            Func<String, int> h1 = funcs.FirstOrDefault();
            Func<String, int> h2 = funcs.Skip(1).FirstOrDefault();

            return new FastBloomFilter<string>(new BloomFilterParameters(numBits, funcs.Count), h1, h2);
        }
    }

    public class SparseFastBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            IList<Func<String, int>> funcs = hashFunctions as IList<Func<string, int>> ?? hashFunctions.ToList();
            Func<String, int> h1 = funcs.FirstOrDefault();
            Func<String, int> h2 = funcs.Skip(1).FirstOrDefault();

            return new SparseFastBloomFilter<string>(new BloomFilterParameters(numBits, funcs.Count), h1, h2);
        }
    }

    public class HybridBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            IList<Func<String,int>> functions = hashFunctions as IList<Func<string, int>> ?? hashFunctions.ToList();
            return new HybridBloomFilter<string>(new BloomFilterParameters(numBits, functions.Count()), functions);
        }
    }

    public class SparseHybridBloomFilterContractTests
        : AbstractBloomFilterContractTests
    {
        protected override IBloomFilter<string> CreateInstance(int numBits, IEnumerable<Func<string, int>> hashFunctions)
        {
            IList<Func<String, int>> functions = hashFunctions as IList<Func<string, int>> ?? hashFunctions.ToList();
            return new SparseHybridBloomFilter<string>(new BloomFilterParameters(numBits, functions.Count), functions);
        }
    }
}