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
using System.Linq;
using Xunit;

namespace VDS.Common.Collections
{
    public abstract class AbstractDictionaryContractTests
    {
        /// <summary>
        /// Gets the instance of a dictionary for use in a test
        /// </summary>
        /// <returns></returns>
        protected abstract IDictionary<String, int> GetInstance();

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractAdd1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.Throws<ArgumentException>(() =>
            {
                dict.Add("key", 2);
            });
        }
        
        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractAdd2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(new KeyValuePair<String, int>("key", 1));
            Assert.Throws<ArgumentException>(() =>
            {
                dict.Add(new KeyValuePair<String, int>("key", 2));
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemove1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.Remove("key"));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemove2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.Remove("key"));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemove3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.Remove(new KeyValuePair<String, int>("key", 1)));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemove4()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.False(dict.Remove(new KeyValuePair<String, int>("key", 2)));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContains1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.ContainsKey("key"));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContains2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.ContainsKey("key"));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContains3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.Contains(new KeyValuePair<String, int>("key", 1)));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContains4()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.False(dict.Contains(new KeyValuePair<String, int>("key", 2)));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGet1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<KeyNotFoundException>(() =>
            {
                int value = dict["key"];
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGet2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.Equal(1, dict["key"]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGet3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            dict.Remove("key");
            Assert.Throws<KeyNotFoundException>(() =>
            {
                int value = dict["key"];
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSet1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict["key"] = 1;
            Assert.Equal(1, dict["key"]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSet2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            dict["key"] = 2;
            Assert.Equal(2, dict["key"]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSet3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict["key"] = 1;
            dict.Remove("key");
            dict["key"] = 2;
            Assert.Equal(2, dict["key"]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractTryGetValue1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            int value;
            Assert.False(dict.TryGetValue("key", out value));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractTryGetValue2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            int value;
            dict.Add("key", 1);
            Assert.True(dict.TryGetValue("key", out value));
            Assert.Equal(1, value);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractKeys1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.Keys.Any());
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractKeys2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.Keys.Any());
            Assert.True(dict.Keys.Contains("key"));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractKeys3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<NotSupportedException>(() =>
            {
                Assert.False(dict.Keys.Any());
                dict.Keys.Add("key");
                Assert.False(dict.ContainsKey("key"));
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractKeys4()
        {
            IDictionary<String, int> dict = this.GetInstance();

            ICollection<String> keys = dict.Keys;
            Assert.Equal(0, keys.Count);
            dict.Add("key", 1);
            Assert.Equal(1, keys.Count);
            Assert.Equal(1, dict.Count);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractValues1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.Values.Any());
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractValues2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add("key", 1);
            Assert.True(dict.Values.Any());
            Assert.True(dict.Values.Contains(1));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractValues3()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<NotSupportedException>(() =>
            {
                Assert.False(dict.Values.Any());
                dict.Values.Add(1);
                Assert.False(dict.Values.Contains(1));
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractValues4()
        {
            IDictionary<String, int> dict = this.GetInstance();

            ICollection<int> values = dict.Values;
            Assert.Equal(0, values.Count);
            dict.Add("key", 1);
            Assert.Equal(1, values.Count);
            Assert.Equal(1, dict.Count);
        }
    }

    public abstract class AbstractDictionaryWithNullKeysAllowedContractTests
        : AbstractDictionaryContractTests
    {
        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractAddNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(null, 1);
            Assert.Equal(1, dict[null]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemoveNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.Remove(null));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemoveNullKey2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(null, 1);
            Assert.True(dict.Remove(null));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContainsNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.False(dict.ContainsKey(null));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContainsNullKey2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(null, 1);
            Assert.True(dict.ContainsKey(null));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGetNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<KeyNotFoundException>(() =>
            {
                int value = dict[null];
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGetNullKey2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(null, 1);
            int value = dict[null];
            Assert.Equal(1, value);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSetNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict[null] = 1;
            Assert.Equal(1, dict[null]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSetNullKey2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict[null] = 1;
            Assert.Equal(1, dict[null]);
            dict[null] = 2;
            Assert.Equal(2, dict[null]);
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractTryGetValueNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            int value;
            Assert.False(dict.TryGetValue(null, out value));
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractTryGetValueNullKey2()
        {
            IDictionary<String, int> dict = this.GetInstance();

            dict.Add(null, 1);
            int value;
            Assert.True(dict.TryGetValue(null, out value));
            Assert.Equal(1, value);
        }
    }

    public abstract class AbstractDictionaryWithNullKeysForbiddenContractTests
        : AbstractDictionaryContractTests
    {
        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractAddNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();
            Assert.Throws<ArgumentNullException>(() =>
            {
                dict.Add(null, 1);
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractRemoveNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<ArgumentNullException>(() =>
            {
                Assert.False(dict.Remove(null));
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractContainsNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<ArgumentNullException>(() =>
            {
                Assert.False(dict.ContainsKey(null));
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemGetNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<ArgumentNullException>(() =>
            {
                int value = dict[null];
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractItemSetNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            Assert.Throws<ArgumentNullException>(() =>
            {
                dict[null] = 1;
            });
        }

        [Fact, Trait("Category", "Dictionaries")]
        public void DictionaryContractTryGetValueNullKey1()
        {
            IDictionary<String, int> dict = this.GetInstance();

            int value;
            Assert.Throws<ArgumentNullException>(() =>
            {
                Assert.False(dict.TryGetValue(null, out value));
            });
        }
    }

    public class DictionaryContractTests
        : AbstractDictionaryWithNullKeysForbiddenContractTests
    {
        protected override IDictionary<string, int> GetInstance()
        {
            return new Dictionary<string, int>();
        }
    }

    public class MultiDictionaryContractTests
        : AbstractDictionaryWithNullKeysForbiddenContractTests
    {
        protected override IDictionary<string, int> GetInstance()
        {
            return new MultiDictionary<string, int>();
        }
    }

    public class MultiDictionaryWithNullableKeysContractTests
        : AbstractDictionaryWithNullKeysAllowedContractTests
    {
        protected override IDictionary<string, int> GetInstance()
        {
            return new MultiDictionary<string, int>(s => s != null ? s.GetHashCode() : 0, true);
        }
    }

    public class TreeSortedDictionaryContractTests
        : AbstractDictionaryWithNullKeysAllowedContractTests
    {
        protected override IDictionary<string, int> GetInstance()
        {
            return new TreeSortedDictionary<String, int>();
        }
    }
}
