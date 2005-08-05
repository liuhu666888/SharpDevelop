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

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Element that uses an attribute group ref.
	/// </summary>
	[TestFixture]
	public class AttributeGroupRefTestFixture : SchemaTestFixtureBase
	{
		XmlSchemaCompletionData schemaCompletionData;
		ICompletionData[] attributeCompletionData;
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			StringReader reader = new StringReader(GetSchema());
			schemaCompletionData = new XmlSchemaCompletionData(reader);
			
			XmlElementPath path = new XmlElementPath();
			path.Elements.Add(new QualifiedName("note", "http://www.w3schools.com"));
			attributeCompletionData = schemaCompletionData.GetAttributeCompletionData(path);
		}
		
		[Test]
		public void AttributeCount()
		{
			Assert.AreEqual(4, attributeCompletionData.Length, "Should be 4 attributes.");
		}
		
		[Test]
		public void NameAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "name"), 
			              "Attribute name does not exist.");
		}		
		
		[Test]
		public void IdAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "id"), 
			              "Attribute id does not exist.");
		}		
		
		[Test]
		public void StyleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "style"), 
			              "Attribute style does not exist.");
		}	
		
		[Test]
		public void TitleAttribute()
		{
			Assert.IsTrue(base.Contains(attributeCompletionData, "title"), 
			              "Attribute title does not exist.");
		}		
		
		string GetSchema()
		{
			return "<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"http://www.w3schools.com\" xmlns=\"http://www.w3schools.com\" elementFormDefault=\"qualified\">\r\n" +
				"<xs:attributeGroup name=\"coreattrs\">" +
				"\t<xs:attribute name=\"id\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"style\" type=\"xs:string\"/>" +
				"\t<xs:attribute name=\"title\" type=\"xs:string\"/>" +
				"</xs:attributeGroup>" +
				"\t<xs:element name=\"note\">\r\n" +
				"\t\t<xs:complexType>\r\n" +
				"\t\t\t<xs:attributeGroup ref=\"coreattrs\"/>" +
				"\t\t\t<xs:attribute name=\"name\" type=\"xs:string\"/>\r\n" +
				"\t\t</xs:complexType>\r\n" +
				"\t</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
