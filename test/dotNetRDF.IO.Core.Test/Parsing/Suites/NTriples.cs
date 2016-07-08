/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2015 dotNetRDF Project (dotnetrdf-develop@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;
using FluentAssertions;

namespace VDS.RDF.Parsing.Suites
{
   
    public class NTriples
        : BaseRdfParserSuite
    {
        public NTriples()
            : base(new NTriplesParser(), new NTriplesParser(), "ntriples\\")
        {
            this.CheckResults = false;
        }

        [Fact]
        public void ParsingSuiteNTriples()
        {
            //Run manifests
            this.RunDirectory(f => Path.GetExtension(f).Equals(".nt"), true);

            this.Count.Should().NotBe(0, "No tests found");

            Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed");
            Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

            this.Failed.Should().Be(0, this.Failed + " Tests failed");
            this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate");
        }

        [Fact]
        public void ParsingNTriplesUnicodeEscapes1()
        {
            Graph g = new Graph();
            g.LoadFromFile(@"resources\\turtle11\localName_with_assigned_nfc_bmp_PN_CHARS_BASE_character_boundaries.nt");
            Assert.False(g.IsEmpty);
            Assert.Equal(1, g.Count);
        }
    }
}
