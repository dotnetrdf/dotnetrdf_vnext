# Known Issues

## String normalization not supported

Due to a limitation of the .NET Core APIs; string and IRI values are not tested for or normalized to
Unicode Normal Form C (NFC). As a result a small number of tests from the RDF/XML parser test-suite fail
(where they test for the parser rejecting IRIs that are not in NFC). This issue will also show itself 
in testing for string equality when the strings have different encodings that would map to the same 
encoding under NFC.

For more information / status please see https://github.com/dotnetrdf/dotnetrdf2/issues/1

## XSLT transforms not supported in the TriX parser

The current .NET Core APIs provide no support for XSLT processing. This means that the TriX parser
cannot support the optional XSLT transform processing part of the processing pipeline. Attempting
to parse a TriX file that contains XSLT stylesheet processing instructions will result in an
RdfParseException being raised.

For more information / status please see https://github.com/dotnetrdf/dotnetrdf2/issues/3

## No DTD entity support when parsing XML formats

The current .NET Core APIs disallow any DTD parsing when processing XML documents. To avoid 
unecessary exceptions the XML parsers (RDf/XML and TriX parsers) are set to ignore the 
XML DTD declaration (the only other alternative in .NET Core is the default behaviour
which is to raise an exception when a DTD declaration is encountered). However, this
behaviour means that any entity declarations contained in the DTD declaration will not
be processed. This will lead to XML parser errors being raised if the XML content contains
references to entities declared in the XML DTD declaration.

For more information / status please see https://github.com/dotnetrdf/dotnetrdf2/issues/5