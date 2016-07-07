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
using System.Diagnostics;
using Xunit;

namespace VDS.Common.Collections
{
    [Trait("Category", "Dictionaries")]
    public class MultiDictionaryTests
    {
        [Fact]
        public void MultiDictionaryInstantiation1()
        {
            MultiDictionary<String, int> dict = new MultiDictionary<string, int>();
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling1()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.Add(null, 1);
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling2()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                int i = dict[null];
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling3()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            int i;
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.TryGetValue(null, out i);
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling4()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict[null] = 1;
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling5()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.Add(new KeyValuePair<Object, int>(null, 1));
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling6()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.Remove(null);
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling7()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.Remove(new KeyValuePair<Object, int>(null, 1));
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling8()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.ContainsKey(null);
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling10()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Contains(new KeyValuePair<Object, int>(null, 1));
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling11()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Add(null, 1);
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling12()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            Assert.Throws<KeyNotFoundException>(() =>
            {
                int i = dict[null];
            });
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling13()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            int i;
            dict.TryGetValue(null, out i);
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling14()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict[null] = 1;
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling15()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Add(new KeyValuePair<Object, int>(null, 1));
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling16()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Remove(null);
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling17()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Remove(new KeyValuePair<Object, int>(null, 1));
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling18()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.ContainsKey(null);
        }

        [Fact]
        public void MultiDictionaryNullKeyHandling19()
        {
            MultiDictionary<Object, int> dict = new MultiDictionary<object, int>(x => (x == null ? 0 : x.GetHashCode()), true);
            dict.Contains(new KeyValuePair<Object, int>(null, 1));
        }

        [Fact]
        public void MultiDictionaryVsDictionaryInsertBasic1()
        {
            Dictionary<TestKey<String>, int> dict = new Dictionary<TestKey<String>, int>();
            MultiDictionary<TestKey<String>, int> mDict = new MultiDictionary<TestKey<String>, int>(new TestKeyComparer<String>());

            TestKey<String> a = new TestKey<String>(1, "a");
            TestKey<String> b = new TestKey<String>(1, "b");

            dict.Add(a, 1);
            Assert.Throws<ArgumentException>(() =>
            {
                dict.Add(b, 2);
            });
            mDict.Add(a, 1);
            mDict.Add(b, 2);

            Assert.Equal(1, dict.Count);
            Assert.Equal(1, dict[a]);
            Assert.Equal(1, dict[b]);
            Assert.Equal(2, mDict.Count);
            Assert.Equal(1, mDict[a]);
            Assert.Equal(2, mDict[b]);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryInsertBasic2()
        {
            Dictionary<TestKey<String>, int> dict = new Dictionary<TestKey<String>, int>(new TestKeyComparer<String>());
            MultiDictionary<TestKey<String>, int> mDict = new MultiDictionary<TestKey<String>, int>(new TestKeyComparer<String>());

            TestKey<String> a = new TestKey<String>(1, "a");
            TestKey<String> b = new TestKey<String>(1, "b");

            dict.Add(a, 1);
            dict.Add(b, 2);
            mDict.Add(a, 1);
            mDict.Add(b, 2);

            Assert.Equal(2, dict.Count);
            Assert.Equal(1, dict[a]);
            Assert.Equal(2, dict[b]);
            Assert.Equal(2, mDict.Count);
            Assert.Equal(1, mDict[a]);
            Assert.Equal(2, mDict[b]);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryLookupPathological1()
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Build dictionaries with 10000 keys in them
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < 10000; i++)
            {
                TestKey<int> key = new TestKey<int>(0, i);
                keys.Add(key);
                dict.Add(key, i);
                mDict.Add(key, i);
            }

            Stopwatch timer = new Stopwatch();

            //Lookup all keys in dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);
            timer.Reset();

            //Lookup all keys in multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MultiDictionary took " + timer.Elapsed);

            Assert.True(mDictTime < dictTime);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryLookupPathological2()
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Build dictionaries with 10000 keys in them
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < 10000; i++)
            {
                TestKey<int> key = new TestKey<int>(0, i);
                keys.Add(key);
                dict.Add(key, i);
                mDict.Add(key, i);
            }

            Stopwatch timer = new Stopwatch();

            //Lookup all keys in multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MultiDictionary took " + timer.Elapsed);

            timer.Reset();

            //Lookup all keys in dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);

            Assert.True(mDictTime < dictTime);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryInsertPathological1()
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Generate 10000 keys
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < 10000; i++)
            {
                TestKey<int> key = new TestKey<int>(0, i);
                keys.Add(key);
            }

            Stopwatch timer = new Stopwatch();

            //Add to dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);

            timer.Reset();

            //Add to multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MutliDictionary took " + timer.Elapsed);

            Assert.True(mDictTime < dictTime);
        }

        [Theory, InlineData(50000), InlineData(100000), InlineData(250000)]
        public void MultiDictionaryVsDictionaryInsertNormal1(int numKeys)
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Generate some number of keys
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < numKeys; i++)
            {
                TestKey<int> key = new TestKey<int>(i, i);
                keys.Add(key);
            }

            Stopwatch timer = new Stopwatch();

            //Add to dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);

            timer.Reset();

            //Add to multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MultiDictionary took " + timer.Elapsed);

            Assert.True(mDictTime > dictTime);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryLookupPool1()
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Build dictionaries with 10000 keys in them
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < 10000; i++)
            {
                TestKey<int> key = new TestKey<int>(i%100, i);
                keys.Add(key);
                dict.Add(key, i);
                mDict.Add(key, i);
            }

            Stopwatch timer = new Stopwatch();

            //Lookup all keys in dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);
            timer.Reset();

            //Lookup all keys in multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MultiDictionary took " + timer.Elapsed);

            Assert.True(mDictTime < dictTime);
        }

        [Fact]
        public void MultiDictionaryVsDictionaryLookupPool2()
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Build dictionaries with 10000 keys in them
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < 10000; i++)
            {
                TestKey<int> key = new TestKey<int>(i%100, i);
                keys.Add(key);
                dict.Add(key, i);
                mDict.Add(key, i);
            }

            Stopwatch timer = new Stopwatch();

            //Lookup all keys in multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MultiDictionary took " + timer.Elapsed);

            timer.Reset();

            //Lookup all keys in dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.ContainsKey(key);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);

            Assert.True(mDictTime < dictTime);
        }

        [Theory, InlineData(1000), InlineData(10000), InlineData(100000), InlineData(250000)]
        public void MultiDictionaryVsDictionaryInsertPool1(int numKeys)
        {
            Dictionary<TestKey<int>, int> dict = new Dictionary<TestKey<int>, int>(new TestKeyComparer<int>());
            MultiDictionary<TestKey<int>, int> mDict = new MultiDictionary<TestKey<int>, int>(new TestKeyComparer<int>());

            //Generate 10000 keys
            List<TestKey<int>> keys = new List<TestKey<int>>();
            for (int i = 0; i < numKeys; i++)
            {
                TestKey<int> key = new TestKey<int>(i%100, i);
                keys.Add(key);
            }

            Stopwatch timer = new Stopwatch();

            //Add to dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                dict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan dictTime = timer.Elapsed;
            Console.WriteLine("Dictionary took " + timer.Elapsed);

            timer.Reset();

            //Add to multi-dictionary
            timer.Start();
            foreach (TestKey<int> key in keys)
            {
                mDict.Add(key, key.Value);
            }
            timer.Stop();

            TimeSpan mDictTime = timer.Elapsed;
            Console.WriteLine("MutliDictionary took " + timer.Elapsed);
            Assert.True(mDictTime - dictTime < new TimeSpan(0, 0, 0, 0, 100));
        }
    }
}