/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

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
using System.IO;
using VDS.RDF.Parsing.Contexts;

namespace VDS.RDF.Parsing
{
    /// <summary>
    /// Parser for SPARQL Boolean results as Plain Text
    /// </summary>
    public class SparqlBooleanParser : ISparqlResultsReader
    {
        public void Load(ISparqlResultsHandler handler, TextReader input, IParserProfile profile)
        {
            this.Load(handler, input);
        }

        /// <summary>
        /// Loads a Result Set from an Input using a Results Handler
        /// </summary>
        /// <param name="handler">Results Handler to use</param>
        /// <param name="input">Input to read from</param>
        public void Load(ISparqlResultsHandler handler, TextReader input)
        {
            if (handler == null) throw new RdfParseException("Cannot read SPARQL Results using a null Results Handler");
            if (input == null) throw new RdfParseException("Cannot read SPARQL Results from a null Input");
            this.Parse(new BaseResultsParserContext(handler), input);
        }

        private void Parse(IResultsParserContext context, TextReader input)
        {
            this.Parse(context, input.ReadToEnd());
        }

        private void Parse(IResultsParserContext context, String data)
        {
            try
            {
                context.Handler.StartResults();

                bool result;
                if (Boolean.TryParse(data.Trim(), out result))
                {
                    context.Handler.HandleBooleanResult(result);
                }
                else
                {
                    throw new RdfParseException("The input was not a single boolean value as a String");
                }

                context.Handler.EndResults(true);
            }
            catch (RdfParsingTerminatedException)
            {
                context.Handler.EndResults(true);
            }
            catch
            {
                context.Handler.EndResults(false);
                throw;
            }
        }

        /// <summary>
        /// Helper Method which raises the Warning event when a non-fatal issue with the SPARQL Results being parsed is detected
        /// </summary>
        /// <param name="message">Warning Message</param>
        private void RaiseWarning(String message)
        {
            this.Warning?.Invoke(message);
        }

        /// <summary>
        /// Event raised when a non-fatal issue with the SPARQL Results being parsed is detected
        /// </summary>
        public event SparqlWarning Warning;

        /// <summary>
        /// Gets the String representation of the Parser which is a description of the syntax it parses
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SPARQL Boolean Result";
        }

    }
}
