﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public partial class EditAvailableConfigurationsDialog
	{
		readonly IConfigurable configurable;
		readonly bool editPlatforms;
		readonly IConfigurationOrPlatformNameCollection editedCollection;
		
		public EditAvailableConfigurationsDialog(IConfigurable configurable, bool editPlatforms)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			foreach (Control ctl in this.Controls) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
			
			if (editPlatforms) {
				this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditSolutionPlatforms}");
				this.editedCollection = configurable.ConfigurationNames;
			} else {
				this.Text = StringParser.Parse("${res:Dialog.EditAvailableConfigurationsDialog.EditSolutionConfigurations}");
				this.editedCollection = configurable.PlatformNames;
			}
			InitList();
		}
		
		void InitList()
		{
			if (editPlatforms) {
				ShowEntries(configurable.PlatformNames, configurable.ActiveConfiguration.Platform);
			} else {
				ShowEntries(configurable.ConfigurationNames, configurable.ActiveConfiguration.Configuration);
			}
		}
		
		void ShowEntries(IEnumerable<string> list, string activeItem)
		{
			string[] array = list.ToArray();
			listBox.Items.Clear();
			listBox.Items.AddRange(array);
			if (listBox.Items.Count == 0) {
				throw new Exception("There must be at least one configuration/platform");
			}
			listBox.SelectedIndex = Math.Max(Array.IndexOf(array, activeItem), 0);
		}
		
		void RemoveButtonClick(object sender, EventArgs e)
		{
			if (listBox.Items.Count == 1) {
				MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.CannotDeleteAllConfigurationsOrPlatforms}");
				return;
			}
			string name = listBox.SelectedItem.ToString();
			if (MessageService.AskQuestion(StringParser.Format(
				"${res:Dialog.EditAvailableConfigurationsDialog.ConfirmRemoveConfigurationOrPlatform}", name)))
			{
				editedCollection.Remove(name);
				InitList();
			}
		}
		
		void RenameButtonClick(object sender, EventArgs e)
		{
			string oldName = listBox.SelectedItem.ToString();
			string newName = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}",
			                                             "${res:Dialog.EditAvailableConfigurationsDialog.EnterNewName}", oldName);
			if (string.IsNullOrEmpty(newName) || newName == oldName)
				return;
			if (!EnsureCorrectName(ref newName))
				return;
			editedCollection.Rename(oldName, newName);
			ISolution solution = configurable as ISolution;
			if (solution != null) {
				// Solution platform name => project platform name
				foreach (IProject p in solution.Projects) {
					if (editPlatforms) {
						p.PlatformNames.Rename(oldName, newName);
					} else {
						p.ConfigurationNames.Rename(oldName, newName);
					}
				}
			}
			InitList();
		}
		
		bool EnsureCorrectName(ref string newName)
		{
			newName = editedCollection.ValidateName(newName);
			if (newName == null) {
				MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.InvalidName}");
				return false;
			}
			foreach (string item in listBox.Items) {
				if (string.Equals(item, newName, StringComparison.OrdinalIgnoreCase)) {
					MessageService.ShowMessage("${res:Dialog.EditAvailableConfigurationsDialog.DuplicateName}");
					return false;
				}
			}
			return true;
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{
			using (AddNewConfigurationDialog dlg = new AddNewConfigurationDialog
			       (configurable is ISolution, editPlatforms,
			        editedCollection,
			        delegate (string name) { return EnsureCorrectName(ref name); }
			       ))
			{
				if (dlg.ShowDialog(this) == DialogResult.OK) {
					editedCollection.Add(dlg.NewName, dlg.CopyFrom);
					if (dlg.CreateInAllProjects) {
						#warning
						throw new NotImplementedException();
					}
					InitList();
				}
			}
		}
	}
}
