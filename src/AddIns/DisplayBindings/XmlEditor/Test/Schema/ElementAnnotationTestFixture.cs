// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the completion data retrieves the annotation documentation
	/// that an element may have.
	/// </summary>
	[TestFixture]
	public class ElementAnnotationTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] fooChildElementCompletionData;
		ICompletionData[] rootElementCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			rootElementCompletionData = schemaCompletionData.GetElementCompletionData();
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("foo", "http://foo.com"));
			
			fooChildElementCompletionData = schemaCompletionData.GetChildElementCompletionData(path);
		}
				
		[Test]
		public void RootElementDocumentation()
		{
			Assert.AreEqual("Documentation for foo element.", rootElementCompletionData[0].Description);
		}
		
		[Test]
		public void FooChildElementDocumentation()
		{
			Assert.AreEqual("Documentation for bar element.", fooChildElementCompletionData[0].Description);
		}
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://foo.com\" xmlns=\"http://foo.com\" elementFormDefault=\"qualified\">\r\n" +
				"\t<xs:element name=\"foo\">\r\n" +
				"\t\t<xs:annotation>\r\n" +
				"\t\t\t<xs:documentation>Documentation for foo element.</xs:documentation>\r\n" +
				"\t\t</xs:annotation>\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:sequence>\t\r\n" +
				"\t\t\t\t<xs:element name=\"bar\" type=\"bar\">\r\n" +
				"\t\t\t\t\t<xs:annotation>\r\n" +
				"\t\t\t\t\t\t<xs:documentation>Documentation for bar element.</xs:documentation>\r\n" +
				"\t\t\t\t</xs:annotation>\t\r\n" +
				"\t\t\t</xs:element>\r\n" +
				"\t\t\t</xs:sequence>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"\t<xs:complexType name=\"bar\">\r\n" +
				"\t\t<xs:annotation>\r\n" +
				"\t\t\t<xs:documentation>Documentation for bar element.</xs:documentation>\r\n" +
				"\t\t</xs:annotation>\t\r\n" +
				"\t\t<xs:attribute name=\"id\"/>\r\n" +
				"\t</xs:complexType>\r\n" +
				"</xs:schema>";
		}		
	}
}
