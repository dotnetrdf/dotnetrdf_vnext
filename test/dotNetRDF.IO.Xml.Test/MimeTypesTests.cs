using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using Xunit;

namespace VDS.RDF
{
    public class MimeTypesTests
    {
        public MimeTypesTests()
        {
            IOManager.ResetDefinitions();
            IOManager.ScanDefinitions();
        }

        public void Dispose()
        {
            IOManager.ResetDefinitions();
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeRdfXml1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/rdf+xml");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(RdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(RdfXmlWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedRdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedRdfXmlWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByTypeRdfXml2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("text/xml");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(RdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(RdfXmlWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedRdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedRdfXmlWriter), d.RdfWriterType);
#endif
        }



        [Fact]
        public void MimeTypesGetDefinitionsByTypeRdfXml3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitions("application/xml");
#if PORTABLE
            Assert.Equal(1, defs.Count());
#else
            Assert.Equal(2, defs.Count());
#endif

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(RdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(RdfXmlWriter), d.RdfWriterType);

#if !NO_COMPRESSION
            //Check GZipped definition
            d = defs.Last();
            Assert.Equal(typeof(GZippedRdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedRdfXmlWriter), d.RdfWriterType);
#endif
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtRdfXml1()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".rdf");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(RdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(RdfXmlWriter), d.RdfWriterType);
        }

        [Fact]
        public void MimeTypesGetDefinitionsByExtRdfXml2()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("rdf");
            Assert.Equal(1, defs.Count());

            //Check normal definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(RdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(RdfXmlWriter), d.RdfWriterType);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtRdfXml3()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension(".rdf.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedRdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedRdfXmlWriter), d.RdfWriterType);
        }
#endif

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetDefinitionsByExtRdfXml4()
        {
            IEnumerable<MimeTypeDefinition> defs = IOManager.GetDefinitionsByFileExtension("rdf.gz");
            Assert.Equal(1, defs.Count());

            //Check GZipped definition
            MimeTypeDefinition d = defs.First();
            Assert.Equal(typeof(GZippedRdfXmlParser), d.RdfParserType);
            Assert.Equal(typeof(GZippedRdfXmlWriter), d.RdfWriterType);
        }
#endif

        [Fact]
        public void MimeTypesGetParserByTypeRdfXml1()
        {
            IRdfReader parser = IOManager.GetParser("text/xml");
            Assert.IsType<RdfXmlParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeRdfXml2()
        {
            IRdfReader parser = IOManager.GetParser("application/rdf+xml");
            Assert.IsType<RdfXmlParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByTypeRdfXml3()
        {
            IRdfReader parser = IOManager.GetParser("application/xml");
            Assert.IsType<RdfXmlParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtRdfXml1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".rdf");
            Assert.IsType<RdfXmlParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtRdfXml2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("rdf");
            Assert.IsType<RdfXmlParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtRdfXml3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".rdf.gz");
            Assert.IsType<GZippedRdfXmlParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtRdfXml4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("rdf.gz");
            Assert.IsType<GZippedRdfXmlParser>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetWriterByTypeRdfXml1()
        {
            IRdfWriter writer = IOManager.GetWriter("text/xml");
            Assert.IsType<RdfXmlWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeRdfXml2()
        {
            IRdfWriter writer = IOManager.GetWriter("application/xml");
            Assert.IsType<RdfXmlWriter>(writer);
        }

        [Fact]
        public void MimeTypesGetWriterByTypeRdfXml3()
        {
            IRdfWriter writer = IOManager.GetWriter("application/rdf+xml");
            Assert.IsType<RdfXmlWriter>(writer);
        }


        [Fact]
        public void MimeTypesGetWriterByExtRdfXml1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".rdf");
            Assert.IsType<RdfXmlWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtRdfXml2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("rdf");
            Assert.IsType<RdfXmlWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtRdfXml3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".rdf.gz");
            Assert.IsType<GZippedRdfXmlWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtRdfXml4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("rdf.gz");
            Assert.IsType<GZippedRdfXmlWriter>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetParserByTypeTriX1()
        {
            IRdfReader parser = IOManager.GetParser("application/trix");
            Assert.IsType<TriXParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTriX1()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".xml");
            Assert.IsType<TriXParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTriX2()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("xml");
            Assert.IsType<TriXParser>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetParserByExtTriX3()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension(".xml.gz");
            Assert.IsType<GZippedTriXParser>(parser);
        }

        [Fact]
        public void MimeTypesGetParserByExtTriX4()
        {
            IRdfReader parser = IOManager.GetParserByFileExtension("xml.gz");
            Assert.IsType<GZippedTriXParser>(parser);
        }
#endif

        [Fact]
        public void MimeTypesGetWriterByTypeTriX1()
        {
            IRdfWriter parser = IOManager.GetWriter("application/trix");
            Assert.IsType<TriXWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTriX1()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".xml");
            Assert.IsType<TriXWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTriX2()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("xml");
            Assert.IsType<TriXWriter>(parser);
        }

#if !NO_COMPRESSION
        [Fact]
        public void MimeTypesGetWriterByExtTriX3()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension(".xml.gz");
            Assert.IsType<GZippedTriXWriter>(parser);
        }

        [Fact]
        public void MimeTypesGetWriterByExtTriX4()
        {
            IRdfWriter parser = IOManager.GetWriterByFileExtension("xml.gz");
            Assert.IsType<GZippedTriXWriter>(parser);
        }
#endif

        [Fact]
        public void MimeTypesContentNegotiation2()
        {
            String[] types = new String[] { "application/rdf+xml", "application/turtle", "text/plain" };
            MimeTypeDefinition def = IOManager.GetDefinitions(types).FirstOrDefault();
            Assert.NotNull(def);
            Assert.Equal(typeof(RdfXmlParser), def.RdfParserType);
        }

        [Fact]
        public void MimeTypesContentNegotiation5()
        {
            String[] types = new String[] { "application/turtle; q=0.8", "application/rdf+xml", "text/plain; q=0.9" };
            MimeTypeDefinition def = IOManager.GetDefinitions(types).FirstOrDefault();
            Assert.NotNull(def);
            Assert.Equal(typeof(RdfXmlParser), def.RdfParserType);
        }

    }
}
