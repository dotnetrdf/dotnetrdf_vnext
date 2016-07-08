using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VDS.RDF.Attributes;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using VDS.RDF;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("dotNetRDF.IO.Xml")]
[assembly: AssemblyTrademark("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0c455829-cecf-4a45-96f5-71b3975d0765")]

// RDF/XML support
[assembly: RdfIO(SyntaxName = "RDF/XML", Encoding = "utf-8", 
    CanonicalMimeType = "application/rdf+xml", 
    MimeTypes = new string[] { "text/xml", "application/xml" }, 
    FormatUri = IOManager.W3CFormatsNamespace + "RDF_XML",
    Quality = 0.3,
    CanonicalFileExtension = ".rdf", 
    FileExtensions = new string[] {".owl"},
    ParserType = typeof(RdfXmlParser), 
    WriterType = typeof(RdfXmlWriter))]
[assembly: RdfIO(SyntaxName = "GZipped RDF/XML", Encoding = "utf-8", 
    CanonicalMimeType = "application/rdf+xml", 
    MimeTypes = new string[] { "text/xml", "application/xml" }, 
    Quality = 0.2,
    CanonicalFileExtension = ".rdf.gz", 
    FileExtensions =new string[] {".owl.gz"},
    ParserType = typeof(GZippedRdfXmlParser),
    WriterType = typeof(GZippedRdfXmlWriter))]

// TriX support
[assembly: RdfIO(SyntaxName = "TriX", Encoding = "utf-8", 
    CanonicalMimeType = "application/trix",
    CanonicalFileExtension = ".xml",
    ParserType = typeof(TriXParser),
    WriterType = typeof(TriXWriter),
    Quality = 0.3)]
[assembly: RdfIO(SyntaxName = "GZipped TriX", Encoding = "utf-8", 
    CanonicalMimeType = "application/trix",
    CanonicalFileExtension = ".xml.gz",
    ParserType = typeof(GZippedTriXParser), 
    WriterType = typeof(GZippedTriXWriter),
    Quality = 0.2)]
