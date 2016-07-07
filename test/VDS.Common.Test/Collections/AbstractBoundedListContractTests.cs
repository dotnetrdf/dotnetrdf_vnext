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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace VDS.Common.Collections
{
    public abstract class AbstractBoundedListContractTests
        : AbstractMutableCollectionContractTests
    {
        protected abstract IBoundedList<string> GetInstance(int capacity);

        protected abstract IBoundedList<string> GetInstance(int capacity, IEnumerable<string> contents);

        protected override ICollection<string> GetInstance()
        {
            return GetInstance(100);
        }

        protected override ICollection<string> GetInstance(IEnumerable<string> contents)
        {
            var enumerable = contents as IList<string> ?? contents.ToList();
            return GetInstance(100, enumerable);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractAdd1()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractAddError1()
        {
            IBoundedList<String> list = this.GetInstance(1);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Adding an additional item should exceed capacity and result in an error
            Assert.Throws<InvalidOperationException>(() =>
            {
                list.Add("b");
            });
        }

        [Theory,
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractAddError2(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0; i < iterations; i++)
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Add(newItem);

                // Try to add to list, should error once capacity is exceeded
                try
                {
                    list.Add(newItem);
                    Assert.True(list.Contains(newItem));

                    // Should never exceed list capacity
                    Assert.False(list.Count > list.MaxCapacity);
                }
                catch (InvalidOperationException)
                {
                    // If this error occurs then we expect the list to be full
                    Assert.Equal(list.MaxCapacity, list.Count);
                }

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                    Assert.Equal(items[index], list[index]);
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractAddDiscard1()
        {
            IBoundedList<string> list = this.GetInstance(2);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);

            // Third item should be discarded
            list.Add("c");
            Assert.False(list.Contains("c"));
            Assert.Equal(2, list.Count);
        }

        [Theory, 
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractAddDiscard2(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0; i < iterations; i++)
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Add(newItem);
                list.Add(newItem);

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                    Assert.Equal(items[index], list[index]);
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractRemove1()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);

            // Now remove item a
            Assert.True(list.Remove("a"));
            Assert.False(list.Contains("a"));
            Assert.Equal("b", list[0]);
            Assert.Equal(1, list.Count);

            // Now remove item b
            Assert.True(list.Remove("b"));
            Assert.False(list.Contains("b"));
            Assert.Equal(0, list.Count);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractRemove2()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);

            // Now remove item b
            Assert.True(list.Remove("b"));
            Assert.False(list.Contains("b"));
            Assert.Equal("a", list[0]);
            Assert.Equal(1, list.Count);

            // Now remove item a
            Assert.True(list.Remove("a"));
            Assert.False(list.Contains("a"));
            Assert.Equal(0, list.Count);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractRemove3()
        {
            IBoundedList<string> list = this.GetInstance(2);

            // Can't remove non-existent items
            Assert.False(list.Remove("a"));
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractRemove4()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));

            // Can't remove non-existent items
            Assert.False(list.Remove("bhg"));
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAt1()
        {
            IBoundedList<string> list = this.GetInstance(2);

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));

            list.RemoveAt(0);
            Assert.Equal(0, list.Count);
            Assert.False(list.Contains("a"));
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAt2()
        {
            IBoundedList<string> list = this.GetInstance(2);

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            list.RemoveAt(0);
            Assert.Equal(1, list.Count);
            Assert.False(list.Contains("a"));
            Assert.True(list.Contains("b"));
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAt3()
        {
            IBoundedList<string> list = this.GetInstance(2);

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            list.Add("b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));

            list.RemoveAt(1);
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.False(list.Contains("b"));
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAtOutOfRange1()
        {
            IBoundedList<string> list = this.GetInstance(2);

            // Remove out of range due to empty list
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list.RemoveAt(0);
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAtOutOfRange2()
        {
            IBoundedList<string> list = this.GetInstance(2);

            // Remove out of range
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list.RemoveAt(-1);
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListRemoveAtOutOfRange3()
        {
            IBoundedList<string> list = this.GetInstance(2, new string[] {"a", "b"});

            // Remove out of range due to being >= current size of list
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list.RemoveAt(2);
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsert1()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Insert before
            list.Insert(0, "b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));
            Assert.Equal("b", list[0]);
            Assert.Equal("a", list[1]);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsert2()
        {
            IBoundedList<string> list = this.GetInstance(2);
            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Insert after
            list.Insert(1, "b");
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains("b"));
            Assert.Equal("a", list[0]);
            Assert.Equal("b", list[1]);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertOutOfRange1()
        {
            IBoundedList<string> list = this.GetInstance(2);

            // Insert out of range
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list.Insert(2, "b");
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertOutOfRange2()
        {
            IBoundedList<string> list = this.GetInstance(2);

            // Insert out of range
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list.Insert(-1, "b");
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertError1()
        {
            IBoundedList<String> list = this.GetInstance(1);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Inserting an additional item should exceed capacity and result in an error
            Assert.Throws<InvalidOperationException>(() =>
            {
                list.Insert(0, "b");
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertError2()
        {
            IBoundedList<String> list = this.GetInstance(1);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                // Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Inserting an additional item should exceed capacity and result in an error
            Assert.Throws<InvalidOperationException>(() =>
            {
                list.Insert(1, "b");
            });
        }

        [Theory,
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractInsertError3(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0; i < iterations; i++)
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Add(newItem);

                // Try to insert to list, should error once capacity is exceeded
                try
                {
                    list.Insert(0, newItem);
                    Assert.True(list.Contains(newItem));

                    // Should never exceed list capacity
                    Assert.False(list.Count > list.MaxCapacity);
                }
                catch (InvalidOperationException)
                {
                    // If this error occurs then we expect the list to be full
                    Assert.Equal(list.MaxCapacity, list.Count);
                }

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                    Assert.Equal(items[Math.Min(items.Count, list.MaxCapacity) - 1 - index], list[index]);
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Theory,
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractInsertError4(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Error)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Error");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0, insert = 0; i < iterations; i++, insert = (insert + 1)%(capacity + 1))
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Add(newItem);

                // Try to insert to list, should error once capacity is exceeded
                try
                {
                    // Insert at cycled position
                    list.Insert(insert, newItem);
                    Assert.True(list.Contains(newItem));

                    // Should never exceed list capacity
                    Assert.False(list.Count > list.MaxCapacity);
                }
                catch (InvalidOperationException)
                {
                    // If this error occurs then we expect the list to be full
                    Assert.Equal(list.MaxCapacity, list.Count);
                }

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertDiscard1()
        {
            IBoundedList<String> list = this.GetInstance(1);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Inserting an additional item should simply cause the excess items to be discarded
            list.Insert(0, "b");
            Assert.Equal(1, list.Count);
            Assert.False(list.Contains("a"));
            Assert.True(list.Contains("b"));
            Assert.Equal("b", list[0]);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractInsertDiscard2()
        {
            IBoundedList<String> list = this.GetInstance(1);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            // Inserting an additional item at end should simply discard it
            list.Insert(1, "b");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.False(list.Contains("b"));
            Assert.Equal("a", list[0]);
        }

        [Theory,
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractInsertDiscard3(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0; i < iterations; i++)
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Insert(0, newItem);

                // Try to insert to list, once capacity is exceeded excess items are discarded
                list.Insert(0, newItem);
                Assert.True(list.Contains(newItem));

                // Should never exceed list capacity
                Assert.False(list.Count > list.MaxCapacity);

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                    Assert.Equal(items[index], list[index]);
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Theory,
            InlineData(10, 100),
         InlineData(10, 1000),
         InlineData(1, 100),
         InlineData(100, 10),
         InlineData(100, 1000),
         InlineData(2, 100),
         InlineData(2, 1000)]
        public void BoundedListContractInsertDiscard4(int capacity, int iterations)
        {
            IBoundedList<String> list = this.GetInstance(capacity);
            if (list.OverflowPolicy != BoundedListOverflowPolicy.Discard)
            {
                //Assert.Ignore("Test is only applicable to implementations with an OverflowPolicy of Discard");
                return;
            }

            List<String> items = new List<string>();
            for (int i = 0, insert = 0; i < iterations; i++, insert = (insert + 1)%(capacity + 1))
            {
                String newItem = i.ToString(CultureInfo.InvariantCulture);
                Assert.False(list.Contains(newItem));
                items.Insert(insert, newItem);

                // Insert at cycled position
                list.Insert(insert, newItem);
                if (insert < capacity || list.Count < list.MaxCapacity)
                {
                    Assert.True(list.Contains(newItem));
                }
                else
                {
                    Assert.False(list.Contains(newItem));
                }

                // Should never exceed list capacity
                Assert.False(list.Count > list.MaxCapacity);

                // Check expected items are in list
                for (int index = 0; index < Math.Min(items.Count, list.MaxCapacity); index++)
                {
                    Assert.True(list.Contains(items[index]));
                    Assert.Equal(items[index], list[index]);
                }
                // Check additional items are not in list
                if (items.Count <= list.MaxCapacity) continue;
                for (int index = list.MaxCapacity; index < items.Count; index++)
                {
                    Assert.False(list.Contains(items[index]));
                }
            }
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractGet1()
        {
            IBoundedList<string> list = this.GetInstance(2);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                String x = list[0];
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractGet2()
        {
            IBoundedList<string> list = this.GetInstance(2);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                // ReSharper disable once UnusedVariable
                String x = list[-1];
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractGet3()
        {
            IBoundedList<string> list = this.GetInstance(2, new String[] {"a", "b"});

            // ReSharper disable once UnusedVariable
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                String x = list[2];
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractGet4()
        {
            IBoundedList<string> list = this.GetInstance(2);

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractSet1()
        {
            IBoundedList<string> list = this.GetInstance(2);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list[0] = "a";
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractSet2()
        {
            IBoundedList<string> list = this.GetInstance(2);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list[-1] = "a";
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractSet3()
        {
            IBoundedList<string> list = this.GetInstance(2, new String[] {"a", "b"});

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                list[2] = "a";
            });
        }

        [Fact, Trait("Category", "Lists")]
        public void BoundedListContractSet4()
        {
            IBoundedList<string> list = this.GetInstance(2);

            list.Add("a");
            Assert.Equal(1, list.Count);
            Assert.True(list.Contains("a"));
            Assert.Equal("a", list[0]);

            list[0] = "b";
            Assert.Equal(1, list.Count);
            Assert.False(list.Contains("a"));
            Assert.True(list.Contains("b"));
            Assert.Equal("b", list[0]);
        }
    }

    public class CappedBoundedListTests
        : AbstractBoundedListContractTests
    {
        protected override IBoundedList<string> GetInstance(int capacity)
        {
            return new CappedBoundedList<string>(capacity);
        }

        protected override IBoundedList<string> GetInstance(int capacity, IEnumerable<string> contents)
        {
            return new CappedBoundedList<string>(capacity, contents);
        }
    }

    public class DiscardingBoundedListTests
        : AbstractBoundedListContractTests
    {
        protected override IBoundedList<string> GetInstance(int capacity)
        {
            return new DiscardingBoundedList<string>(capacity);
        }

        protected override IBoundedList<string> GetInstance(int capacity, IEnumerable<string> contents)
        {
            return new DiscardingBoundedList<string>(capacity, contents);
        }
    }
}