// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Parser
{
	/// <summary>
	/// Tests that we can detect the attribute's name.
	/// </summary>
	[TestFixture]
	public class AttributeNameTestFixture
	{		
		[Test]
		public void SuccessTest1()
		{
			string text = " foo='a";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}

		[Test]
		public void SuccessTest2()
		{
			string text = " foo='";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void SuccessTest3()
		{
			string text = " foo=";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest4()
		{
			string text = " foo=\"";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void SuccessTest5()
		{
			string text = " foo = \"";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest6()
		{
			string text = " foo = '#";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void FailureTest1()
		{
			string text = "foo=";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void FailureTest2()
		{
			string text = "foo=<";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void FailureTest3()
		{
			string text = "a";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void FailureTest4()
		{
			string text = " a";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
	}
}
