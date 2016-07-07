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

namespace VDS.Common.Tries
{
    public abstract class AbstractTrieContractTests
    {
        protected abstract ITrie<String, char, String> GetInstance();

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractAdd1()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");

            Assert.Equal("a", trie["test"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractAdd2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.Equal("a", trie["test"]);
            Assert.Equal("b", trie["testing"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractClear1()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");

            Assert.Equal("a", trie["test"]);

            trie.Clear();

            Assert.False(trie.ContainsKey("test"));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractClear2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.Equal("a", trie["test"]);
            Assert.Equal("b", trie["testing"]);

            trie.Clear();

            Assert.False(trie.ContainsKey("test"));
            Assert.False(trie.ContainsKey("testing"));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractContains1()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.True(trie.ContainsKey("test"));
            Assert.True(trie.ContainsKey("testing"));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractContains2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            String key = "test";
            trie.Add(key, "a");

            Assert.True(trie.ContainsKey(key));
            for (int i = 1; i < key.Length; i++)
            {
                Assert.False(trie.ContainsKey(key.Substring(0, i)));
                Assert.True(trie.ContainsKey(key.Substring(0, i), false));
            }
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractRemove1()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.Equal("a", trie["test"]);
            Assert.Equal("b", trie["testing"]);

            trie.Remove("test");

            Assert.Equal("b", trie["testing"]);
            Assert.NotNull(trie.Find("test"));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractRemove2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.Equal("a", trie["test"]);
            Assert.Equal("b", trie["testing"]);

            trie.Remove("testing");

            Assert.Equal("a", trie["test"]);
            Assert.Null(trie.Find("testing"));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractTryGetValue1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            String value;
            Assert.False(trie.TryGetValue("test", out value));
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractTryGetValue2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            String value;
            Assert.True(trie.TryGetValue("test", out value));
            Assert.Equal("a", value);
            Assert.True(trie.TryGetValue("testing", out value));
            Assert.Equal("b", value);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractTryGetValue3()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            String key = "test";
            trie.Add(key, "a");

            String value;
            Assert.True(trie.TryGetValue(key, out value));
            Assert.Equal("a", value);
            for (int i = 1; i < key.Length; i++)
            {
                Assert.False(trie.TryGetValue(key.Substring(0, i), out value));
                Assert.Null(value);
            }
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractItemGet1()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");

            Assert.Equal("a", trie["test"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractItemGet2()
        {
            ITrie<String, char, String> trie = this.GetInstance();
            trie.Add("test", "a");
            trie.Add("testing", "b");

            Assert.Equal("a", trie["test"]);
            Assert.Equal("b", trie["testing"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractItemGet3()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                ITrie<String, char, String> trie = this.GetInstance();

                Assert.Equal(null, trie["test"]);
            });
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractItemSet1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.ContainsKey("test"));

            trie["test"] = "a";

            Assert.True(trie.ContainsKey("test"));
            Assert.Equal("a", trie["test"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractItemSet2()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.ContainsKey("test"));

            trie["test"] = "a";

            Assert.True(trie.ContainsKey("test"));
            Assert.Equal("a", trie["test"]);

            trie["test"] = "b";

            Assert.Equal("b", trie["test"]);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractValues1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Values.Any());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractValues2()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Values.Any());

            trie.Add("test", "a");

            Assert.True(trie.Values.Any());
            Assert.Equal(1, trie.Values.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractValues3()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            IEnumerable<String> values = trie.Values;

            Assert.False(values.Any());

            trie.Add("test", "a");

            Assert.True(values.Any());
            Assert.Equal(1, values.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractValues4()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Values.Any());

            trie.Add("test", "a");

            Assert.True(trie.Values.Any());
            Assert.Equal(1, trie.Values.Count());

            trie.Add("testing", "b");

            Assert.True(trie.Values.Any());
            Assert.Equal(2, trie.Values.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractValues5()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            IEnumerable<String> values = trie.Values;

            Assert.False(values.Any());

            trie.Add("test", "a");

            Assert.True(values.Any());
            Assert.Equal(1, values.Count());

            trie.Add("testing", "b");

            Assert.True(values.Any());
            Assert.Equal(2, values.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractDescendants1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Root.Descendants.Any());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractDescendants2()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Root.Descendants.Any());

            trie.Add("test", "a");

            Assert.True(trie.Root.Descendants.Any());
            Assert.Equal(4, trie.Root.Descendants.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractDescendants3()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            IEnumerable<ITrieNode<char, String>> descendants = trie.Root.Descendants;

            Assert.False(descendants.Any());

            trie.Add("test", "a");

            Assert.True(descendants.Any());
            Assert.Equal(4, descendants.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractDescendants4()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            Assert.False(trie.Root.Descendants.Any());

            trie.Add("test", "a");

            Assert.True(trie.Root.Descendants.Any());
            Assert.Equal(4, trie.Root.Descendants.Count());

            trie.Add("testing", "b");

            Assert.True(trie.Root.Descendants.Any());
            Assert.Equal(7, trie.Root.Descendants.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractDescendants5()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            IEnumerable<ITrieNode<char, String>> descendants = trie.Root.Descendants;

            Assert.False(descendants.Any());

            trie.Add("test", "a");

            Assert.True(descendants.Any());
            Assert.Equal(4, descendants.Count());

            trie.Add("testing", "b");

            Assert.True(descendants.Any());
            Assert.Equal(7, descendants.Count());
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractMoveToNode1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.MoveToNode("test");
            Assert.NotNull(node);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractMoveToNode2()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.MoveToNode("test");
            Assert.NotNull(node);
            Assert.Equal(4, trie.Count);

            node = trie.MoveToNode("test");
            Assert.NotNull(node);
            Assert.Equal(4, trie.Count);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFind1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.Find("test");
            Assert.Null(node);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFind2()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.Find("test");
            Assert.Null(node);

            trie.Add("test", "a");

            node = trie.Find("test");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFind3()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.Find("test");
            Assert.Null(node);

            trie.Add("test", "a");

            node = trie.Find("testing");
            Assert.Null(node);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFind4()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.Find("test");
            Assert.Null(node);

            trie.Add("test", "a");

            //Find with a custom key mapper
            node = trie.Find("testing", (s => s.Substring(0, 4).ToCharArray()));
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFindPredecessor1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.FindPredecessor("test");
            Assert.Null(node);

            trie.Add("test", "a");
            trie.Add("testing", "b");
            trie.Add("blah", "c");

            node = trie.FindPredecessor("test");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);

            node = trie.FindPredecessor("testi");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);

            node = trie.FindPredecessor("testit");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);

            node = trie.FindPredecessor("testing");
            Assert.NotNull(node);
            Assert.Equal("b", node.Value);

            node = trie.FindPredecessor("testingtest");
            Assert.NotNull(node);
            Assert.Equal("b", node.Value);

            node = trie.FindPredecessor("b");
            Assert.Null(node);

            node = trie.FindPredecessor("blahblah");
            Assert.NotNull(node);
            Assert.Equal("c", node.Value);
        }

        [Trait("Category", "Tries")]
        [Fact]
        public void TrieContractFindSuccessor1()
        {
            ITrie<String, char, String> trie = this.GetInstance();

            ITrieNode<char, String> node = trie.FindSuccessor("test");
            Assert.Null(node);

            trie.Add("test", "a");
            trie.Add("testing", "b");
            trie.Add("blah", "c");
            trie.Add("testinh", "d");
            trie.Add("testings", "e");

            node = trie.FindSuccessor("test");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);

            node = trie.FindSuccessor("testi");
            Assert.NotNull(node);
            Assert.Equal("b", node.Value);

            node = trie.FindSuccessor("testit");
            Assert.Null(node);

            node = trie.FindSuccessor("testing");
            Assert.NotNull(node);
            Assert.Equal("b", node.Value);

            node = trie.FindSuccessor("t");
            Assert.NotNull(node);
            Assert.Equal("a", node.Value);

            node = trie.FindSuccessor("b");
            Assert.NotNull(node);
            Assert.Equal("c", node.Value);

            node = trie.FindSuccessor("testinga");
            Assert.Null(node);
        }
    }

    public class TrieContractTests
        : AbstractTrieContractTests
    {
        protected override ITrie<string, char, String> GetInstance()
        {
            return new StringTrie<String>();
        }
    }

    public class TrieContractTests2
        : AbstractTrieContractTests
    {
        protected override ITrie<string, char, String> GetInstance()
        {
            return new Trie<String, char, String>(StringTrie<String>.KeyMapper);
        }
    }

    public class SparseTrieContractTests
        : AbstractTrieContractTests
    {
        protected override ITrie<string, char, string> GetInstance()
        {
            return new SparseStringTrie<String>();
        }
    }

    public class SparseTrieContractTests2
        : AbstractTrieContractTests
    {
        protected override ITrie<string, char, string> GetInstance()
        {
            return new SparseCharacterTrie<String, String>(StringTrie<String>.KeyMapper);
        }
    }

    public class SparseTrieContractTests3
        : AbstractTrieContractTests
    {
        protected override ITrie<string, char, string> GetInstance()
        {
            return new SparseValueTrie<String, char, String>(StringTrie<String>.KeyMapper);
        }
    }
}
