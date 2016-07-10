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
using FluentAssertions;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;

namespace VDS.RDF.Parsing.Suites
{
   
    public class TriX
        : BaseDatasetParserSuite
    {
        public TriX()
            : base(new TriXParser(), new NQuadsParser(), "trix\\")
        {
            this.CheckResults = false;
        }

#if NO_XSL
        private readonly string[] _trixFilesRequiringStylesheet = new string[]
            {
                "resources\\trix\\curies.xml",
                "resources\\trix\\datatypes.xml",
                "resources\\trix\\multiple-stylesheets.xml"
            };
#endif

        [Fact]
        public void ParsingSuiteTriX()
        {
#if NO_XSL
            //Run manifests
            this.RunDirectory(f => Path.GetExtension(f).Equals(".xml") && !f.Contains("bad") && !_trixFilesRequiringStylesheet.Contains(f), true);
            this.RunDirectory(f => Path.GetExtension(f).Equals(".xml") && f.Contains("bad") && !_trixFilesRequiringStylesheet.Contains(f), false);
#else
            //Run manifests
            this.RunDirectory(f => Path.GetExtension(f).Equals(".xml") && !f.Contains("bad"), true);
            this.RunDirectory(f => Path.GetExtension(f).Equals(".xml") && f.Contains("bad"), false);
#endif

            this.Count.Should().NotBe(0, "No tests found");

            Console.WriteLine(this.Count + " Tests - " + this.Passed + " Passed - " + this.Failed + " Failed");
            Console.WriteLine((((double)this.Passed / (double)this.Count) * 100) + "% Passed");

            this.Failed.Should().Be(0, this.Failed + " Tests failed");
            this.Indeterminate.Should().Be(0, this.Indeterminate + " Tests are indeterminate");
        }
    }
}
