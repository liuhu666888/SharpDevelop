// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;

namespace XmlEditor.Tests.Paths
{
	[TestFixture]
	public class SingleElementPathTestFixture
	{
		XmlElementPath path;
		QualifiedName qualifiedName;
		
		[SetUp]
		public void Init()
		{
			path = new XmlElementPath();
			qualifiedName = new QualifiedName("foo", "http://foo");
			path.Elements.Add(qualifiedName);
		}
		
		[Test]
		public void HasOneItem()
		{
			Assert.AreEqual(1, path.Elements.Count, 
			                "Should have 1 element.");
		}
		
		[Test]
		public void RemoveLastItem()
		{
			path.Elements.RemoveLast();
			Assert.AreEqual(0, path.Elements.Count, "Should have no items.");
		}
		
		[Test]
		public void Equality()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.Elements.Add(new QualifiedName("foo", "http://foo"));
			
			Assert.IsTrue(newPath.Equals(path), "Should be equal.");
		}
		
		[Test]
		public void NotEqual()
		{
			XmlElementPath newPath = new XmlElementPath();
			newPath.Elements.Add(new QualifiedName("Foo", "bar"));
			
			Assert.IsFalse(newPath.Equals(path), "Should not be equal.");
		}	
		
		[Test]
		public void Compact()
		{
			path.Compact();
			Equality();
		}
	}
}
