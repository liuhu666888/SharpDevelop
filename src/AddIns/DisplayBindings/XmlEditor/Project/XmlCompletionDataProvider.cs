// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using System;
using System.Collections;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Provides the autocomplete (intellisense) data for an
	/// xml document that specifies a known schema.
	/// </summary>
	public class XmlCompletionDataProvider : ICompletionDataProvider
	{
		XmlSchemaCompletionDataCollection schemaCompletionDataItems;
		XmlSchemaCompletionData defaultSchemaCompletionData;
		string defaultNamespacePrefix = String.Empty;
		string preSelection = null;
		
		public XmlCompletionDataProvider(XmlSchemaCompletionDataCollection schemaCompletionDataItems, XmlSchemaCompletionData defaultSchemaCompletionData, string defaultNamespacePrefix)
		{
			this.schemaCompletionDataItems = schemaCompletionDataItems;
			this.defaultSchemaCompletionData = defaultSchemaCompletionData;
			this.defaultNamespacePrefix = defaultNamespacePrefix;
		}
		
		public ImageList ImageList {
			get
			{
				return XmlCompletionDataImageList.GetImageList();
			}
		}
		
		/// <summary>
		/// Gets the preselected text.
		/// </summary>
		public string PreSelection {
			get
			{
				return preSelection;
			}
		}
		
		public int DefaultIndex {
			get {
				return -1;
			}
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			preSelection = null;
			string text = String.Concat(textArea.Document.GetText(0, textArea.Caret.Offset), charTyped);
			
			switch (charTyped) {
				case '=':
					// Namespace intellisense.
					if (XmlParser.IsNamespaceDeclaration(text, text.Length)) {
						return schemaCompletionDataItems.GetNamespaceCompletionData();;
					}
					break;
				case '<':
					// Child element intellisense.
					XmlElementPath parentPath = XmlParser.GetParentElementPath(text, text.Length);
					if (parentPath.Elements.Count > 0) {
						return GetChildElementCompletionData(parentPath);
					} else if (defaultSchemaCompletionData != null) {
						return defaultSchemaCompletionData.GetElementCompletionData(defaultNamespacePrefix);
					}
					break;
					
				case ' ':
					// Attribute intellisense.
					XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
					if (path.Elements.Count > 0) {
						return GetAttributeCompletionData(path);
					}
					break;
					
				default:
					
					// Attribute value intellisense.
					if (XmlParser.IsAttributeValueChar(charTyped)) {
						string attributeName = XmlParser.GetAttributeName(text, text.Length);
						if (attributeName.Length > 0) {
							XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
							if (elementPath.Elements.Count > 0) {
								preSelection = charTyped.ToString();
								return GetAttributeValueCompletionData(elementPath, attributeName);
							}
						}
					}
					break;
			}
			
			return null;
		}
		
		ICompletionData[] GetChildElementCompletionData(XmlElementPath path)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetChildElementCompletionData(path);
			}
			
			return completionData;
		}
		
		ICompletionData[] GetAttributeCompletionData(XmlElementPath path)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetAttributeCompletionData(path);
			}
			
			return completionData;
		}
		
		ICompletionData[] GetAttributeValueCompletionData(XmlElementPath path, string name)
		{
			ICompletionData[] completionData = null;
			
			XmlSchemaCompletionData schema = FindSchema(path);
			if (schema != null) {
				completionData = schema.GetAttributeValueCompletionData(path, name);
			}
			
			return completionData;
		}
		
		XmlSchemaCompletionData FindSchema(XmlElementPath path)
		{
			XmlSchemaCompletionData schemaData = null;
			
			if (path.Elements.Count > 0) {
				string namespaceUri = path.Elements[0].Namespace;
				if (namespaceUri.Length > 0) {
					schemaData = schemaCompletionDataItems[namespaceUri];
				} else if (defaultSchemaCompletionData != null) {
					
					// Use the default schema namespace if none
					// specified in a xml element path, otherwise
					// we will not find any attribute or element matches
					// later.
					foreach (QualifiedName name in path.Elements) {
						if (name.Namespace.Length == 0) {
							name.Namespace = defaultSchemaCompletionData.NamespaceUri;
						}
					}
					schemaData = defaultSchemaCompletionData;
				}
			}
			
			return schemaData;
		}
		
//		string GetTextFromStartToEndOfCurrentLine(TextArea textArea)
//		{
//			LineSegment line = textArea.Document.GetLineSegment(textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset));
//			if (line != null) {
//				return textArea.Document.GetText(0, line.Offset + line.Length);
//			}
//
//			return String.Empty;//textArea.Document.GetText(0, textArea.Caret.Offset);
//		}
	}
}
