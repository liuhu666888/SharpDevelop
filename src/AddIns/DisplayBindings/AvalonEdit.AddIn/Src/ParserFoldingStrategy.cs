﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Uses SharpDevelop.Dom to create parsing information.
	/// </summary>
	public class ParserFoldingStrategy
	{
		readonly FoldingManager foldingManager;
		TextArea textArea;
		FoldingMargin margin;
		FoldingElementGenerator generator;
		
		public ParserFoldingStrategy(TextArea textArea)
		{
			this.textArea = textArea;
			foldingManager = new FoldingManager(textArea.TextView, textArea.Document);
			margin = new FoldingMargin() { FoldingManager = foldingManager, TextView = textArea.TextView };
			generator = new FoldingElementGenerator() { FoldingManager = foldingManager };
		}
		
		public bool IsAttached {
			get { return textArea.TextView.ElementGenerators.Contains(generator); }
		}
		
		public void Attach()
		{
			if (!IsAttached) {
				textArea.LeftMargins.Add(margin);
				textArea.TextView.ElementGenerators.Add(generator);
			}
		}
		
		public void Detach()
		{
			textArea.LeftMargins.Remove(margin);
			textArea.TextView.ElementGenerators.Remove(generator);
		}
		
		public void UpdateFoldings(ParseInformation parseInfo)
		{
			var oldFoldings = foldingManager.AllFoldings.ToArray();
			IEnumerable<NewFolding> newFoldings = GetNewFoldings(parseInfo);
			int oldFoldingIndex = 0;
			// merge new foldings into old foldings so that sections keep being collapsed
			// both oldFoldings and newFoldings are sorted by start offset
			foreach (NewFolding newFolding in newFoldings) {
				// remove old foldings that were skipped
				while (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset > oldFoldings[oldFoldingIndex].StartOffset) {
					foldingManager.RemoveFolding(oldFoldings[oldFoldingIndex++]);
				}
				FoldingSection section;
				// reuse current folding if its matching:
				if (oldFoldingIndex < oldFoldings.Length && newFolding.StartOffset == oldFoldings[oldFoldingIndex].StartOffset) {
					section = oldFoldings[oldFoldingIndex++];
					section.Length = newFolding.EndOffset - newFolding.StartOffset;
				} else {
					// no matching current folding; create a new one:
					section = foldingManager.CreateFolding(newFolding.StartOffset, newFolding.EndOffset);
				}
			}
			// remove all outstanding old foldings:
			while (oldFoldingIndex < oldFoldings.Length) {
				foldingManager.RemoveFolding(oldFoldings[oldFoldingIndex++]);
			}
		}
		
		IEnumerable<NewFolding> GetNewFoldings(ParseInformation parseInfo)
		{
			List<NewFolding> newFoldMarkers = new List<NewFolding>();
			if (parseInfo != null) {
				foreach (IClass c in parseInfo.CompilationUnit.Classes) {
					AddClassMembers(c, newFoldMarkers);
				}
				foreach (FoldingRegion foldingRegion in parseInfo.CompilationUnit.FoldingRegions) {
					newFoldMarkers.Add(new NewFolding(textArea.Document.GetOffset(foldingRegion.Region.BeginLine, foldingRegion.Region.BeginColumn),
					                                  textArea.Document.GetOffset(foldingRegion.Region.EndLine, foldingRegion.Region.EndColumn)));
				}
			}
			return newFoldMarkers.Where(f => f.EndOffset > f.StartOffset).OrderBy(f=>f.StartOffset);
		}
		
		void AddClassMembers(IClass c, List<NewFolding> newFoldMarkers)
		{
			if (c.ClassType == ClassType.Delegate) {
				return;
			}
			DomRegion cRegion = c.BodyRegion;
			if (cRegion.IsEmpty)
				cRegion = c.Region;
			if (cRegion.BeginLine < cRegion.EndLine) {
				newFoldMarkers.Add(new NewFolding(textArea.Document.GetOffset(cRegion.BeginLine, cRegion.BeginColumn),
				                                  textArea.Document.GetOffset(cRegion.EndLine, cRegion.EndColumn)));
			}
			foreach (IClass innerClass in c.InnerClasses) {
				AddClassMembers(innerClass, newFoldMarkers);
			}
			
			foreach (IMember m in c.AllMembers) {
				if (m.Region.EndLine < m.BodyRegion.EndLine) {
					newFoldMarkers.Add(new NewFolding(textArea.Document.GetOffset(m.Region.EndLine, m.Region.EndColumn),
					                                  textArea.Document.GetOffset(m.BodyRegion.EndLine, m.BodyRegion.EndColumn)));
				}
			}
		}
		
		struct NewFolding
		{
			public readonly int StartOffset, EndOffset;
			
			public NewFolding(int start, int end)
			{
				Debug.Assert(start < end);
				this.StartOffset = start;
				this.EndOffset = end;
			}
		}
	}
}
