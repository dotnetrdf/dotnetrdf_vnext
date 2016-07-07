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
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace VDS.Common.Collections
{
    public abstract class AbstractCollectionContractTests
    {
        /// <summary>
        /// Method to be implemented by deriving classes to provide the instance to test
        /// </summary>
        /// <returns>Instance to test</returns>
        protected abstract ICollection<String> GetInstance();

        /// <summary>
        /// Method to be implemented by deriving classes to provide the instance to test
        /// </summary>
        /// <param name="contents">Contents for the collection</param>
        /// <returns>Instance to test</returns>
        protected abstract ICollection<String> GetInstance(IEnumerable<String> contents);

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.Throws<ArgumentNullException>(() =>
            {
                c.CopyTo(null, 0);
            });
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo2()
        {
            ICollection<String> c = this.GetInstance();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                c.CopyTo(new String[10], -1);
            });
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo3()
        {
            ICollection<String> c = this.GetInstance();

            Assert.Throws<ArgumentException>(() =>
            {
                c.CopyTo(new String[1], 2);
            });
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo4()
        {
            ICollection<String> c = this.GetInstance();

            c.CopyTo(new String[1], 0);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo5()
        {
            ICollection<String> c = this.GetInstance(new String[] { "test" });

            String[] dest = new String[1];
            c.CopyTo(dest, 0);
            Assert.Equal("test", dest[0]);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCopyTo6()
        {
            String[] data = new String[] { "a", "a", "b", "c" };
            ICollection<String> c = this.GetInstance(data);

            String[] dest = new String[data.Length];
            c.CopyTo(dest, 0);
            Assert.Equal(data, dest);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractEnumerate1()
        {
            ICollection<String> c = this.GetInstance();

            using (IEnumerator<String> enumerator = c.GetEnumerator())
            {
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractEnumerate2()
        {
            ICollection<String> c = this.GetInstance(new String[] { "test" });

            using (IEnumerator<String> enumerator = c.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal("test", enumerator.Current);
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractEnumerate3()
        {
            ICollection<String> c = this.GetInstance(new String[] { "a", "b" });

            using (IEnumerator<String> enumerator = c.GetEnumerator())
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal("a", enumerator.Current);
                Assert.True(enumerator.MoveNext());
                Assert.Equal("b", enumerator.Current);
                Assert.False(enumerator.MoveNext());
            }
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractEnumerate4()
        {
            String[] values = new String[] { "a", "a", "b", "c", "d", "e", "e" };
            ICollection<String> c = this.GetInstance(values);

            using (IEnumerator<String> enumerator = c.GetEnumerator())
            {
                int index = 0;
                while (index < values.Length)
                {
                    Assert.True(enumerator.MoveNext(), "Failed to move next at Index " + index);
                    Assert.Equal(values[index], enumerator.Current);
                    index++;
                }
            }
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCount1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.Equal(0, c.Count);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractCount2()
        {
            ICollection<String> c = this.GetInstance(new String[] { "test" });
            Assert.Equal(1, c.Count);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractContains1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.False(c.Contains("test"));
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractContains2()
        {
            ICollection<String> c = this.GetInstance(new String[] { "test" });
            Assert.True(c.Contains("test"));
        }
    }

    public abstract class AbstractImmutableCollectionContractTests
        : AbstractCollectionContractTests
    {
        [Fact, Trait("Category", "Collections")]
        public void CollectionContractAdd1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.Throws<NotSupportedException>(() =>
            {
                c.Add("test");
            });
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractRemove1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.Throws<NotSupportedException>(() =>
            {
                c.Remove("test");
            });
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractClear1()
        {
            ICollection<String> c = this.GetInstance();
            Assert.Throws<NotSupportedException>(() =>
            {
                c.Clear();
            });
        }
    }

    public abstract class AbstractMutableCollectionContractTests
        : AbstractCollectionContractTests
    {
        [Fact, Trait("Category", "Collections")]
        public void CollectionContractAdd1()
        {
            ICollection<String> c = this.GetInstance();

            c.Add("test");
            Assert.True(c.Contains("test"));
            Assert.Equal(1, c.Count);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractAdd2()
        {
            ICollection<String> c = this.GetInstance();

            for (int i = 0; i < 100; i++)
            {
                c.Add("test" + i);
                Assert.True(c.Contains("test" + i));
                Assert.Equal(i + 1, c.Count);
            }
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractRemove1()
        {
            ICollection<String> c = this.GetInstance();

            c.Add("test");
            Assert.True(c.Contains("test"));
            c.Remove("test");
            Assert.False(c.Contains("test"));
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractRemove2()
        {
            ICollection<String> c = this.GetInstance();

            c.Add("test");
            c.Add("test");
            Assert.True(c.Contains("test"));
            c.Remove("test");

            //True because only one instance should get removed
            Assert.True(c.Contains("test"));
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractClear1()
        {
            ICollection<String> c = this.GetInstance();

            Assert.Equal(0, c.Count);
            c.Clear();
            Assert.Equal(0, c.Count);
        }

        [Fact, Trait("Category", "Collections")]
        public void CollectionContractClear2()
        {
            ICollection<String> c = this.GetInstance();

            c.Add("test");
            Assert.True(c.Contains("test"));
            Assert.Equal(1, c.Count);
            c.Clear();
            Assert.False(c.Contains("test"));
            Assert.Equal(0, c.Count);
        }
    }

    public class ListContractTests
        : AbstractMutableCollectionContractTests
    {
        protected override ICollection<string> GetInstance()
        {
            return new List<String>();
        }

        protected override ICollection<string> GetInstance(IEnumerable<string> contents)
        {
            return new List<String>(contents);
        }
    }

    public class ImmutableViewContractTests
        : AbstractImmutableCollectionContractTests
    {
        protected override ICollection<string> GetInstance()
        {
            return new ImmutableView<String>();
        }

        protected override ICollection<string> GetInstance(IEnumerable<string> contents)
        {
            return new ImmutableView<String>(contents);
        }
    }

    public class MaterializedImmutableViewContractTests
        : AbstractImmutableCollectionContractTests
    {
        protected override ICollection<string> GetInstance()
        {
            return new MaterializedImmutableView<String>();
        }

        protected override ICollection<string> GetInstance(IEnumerable<string> contents)
        {
            return new MaterializedImmutableView<String>(contents);
        }
    }

    public class DuplicateSortedListContractTests
        : AbstractMutableCollectionContractTests
    {
        protected override ICollection<string> GetInstance()
        {
            return new DuplicateSortedList<string>();
        }

        protected override ICollection<string> GetInstance(IEnumerable<string> contents)
        {
            return new DuplicateSortedList<string>(contents);
        }
    }
}
