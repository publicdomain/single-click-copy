// <copyright file="MainForm.cs" company="PublicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace SingleClickCopy
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using PublicDomain;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The assembly version.
        /// </summary>
        private Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The semantic version.
        /// </summary>
        private string semanticVersion = string.Empty;

        /// <summary>
        /// The associated icon.
        /// </summary>
        private Icon associatedIcon = null;

        /// <summary>
        /// The friendly name of the program.
        /// </summary>
        private string friendlyName = "Single Click Copy";

        /// <summary>
        /// The copy count.
        /// </summary>
        private int copyCount = 0;

        /// <summary>
        /// The item list file path.
        /// </summary>
        private string itemListFilePath = "SingleClickCopyList.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SingleClickCopy.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Set semantic version
            this.semanticVersion = this.assemblyVersion.Major + "." + this.assemblyVersion.Minor + "." + this.assemblyVersion.Build;

            // Focus text box
            this.ActiveControl = this.itemTextBox;

            // Check for a previously-saved list
            if (File.Exists(this.itemListFilePath))
            {
                // Load from disk
                this.LoadItemList(this.itemListFilePath);
            }
        }

        /// <summary>
        /// Handles the add button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            // Check for some text length
            if (this.itemTextBox.Text.Length == 0)
            {
                // Advise user
                MessageBox.Show("Please add item text", "No item text", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Check for a duplicate
            if (this.clipboardCopyListBox.Items.Contains(this.itemTextBox.Text))
            {
                // Advise user
                MessageBox.Show("Item already exists", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Halt flow
                goto focusExit;
            }

            // Add item
            this.clipboardCopyListBox.Items.Add(this.itemTextBox.Text);

            // Clear text box
            this.itemTextBox.Clear();

            // Update item count in status label
            this.UpdateStatus();

        // Focus text box and exit function
        focusExit:

            // Focus text box
            this.ActiveControl = this.itemTextBox;
        }

        /// <summary>
        /// Saves the items list.
        /// </summary>
        /// <param name="filePath">File path.</param>
        private void SaveItemList(string filePath)
        {
            // Use stream writer
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                // Iterate items
                foreach (var item in this.clipboardCopyListBox.Items)
                {
                    // Write current item as line
                    streamWriter.WriteLine(item.ToString());
                }
            }
        }

        /// <summary>
        /// Loads the item list.
        /// </summary>
        /// <param name="filePath">File path.</param>
        private void LoadItemList(string filePath)
        {
            // Use stream reader
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                // Declare line variable
                string line;

                // Iterate file
                while ((line = streamReader.ReadLine()) != null)
                {
                    // Trim line
                    line = line.Trim();

                    // Only add lines with text
                    if (line.Length > 0)
                    {
                        // Add to list box
                        this.clipboardCopyListBox.Items.Add(line);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the status label with item and copy count.
        /// </summary>
        private void UpdateStatus()
        {
            // Update list count
            this.toolStripStatusLabel.Text = $"Items in list: {this.clipboardCopyListBox.Items.Count} / Copy count: {this.copyCount}";
        }

        /// <summary>
        /// Handles the edit button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnEditButtonClick(object sender, EventArgs e)
        {
            // Check for some text length
            if (this.itemTextBox.Text.Length == 0)
            {
                // Advise user
                MessageBox.Show("Please add item text to edit", "No item text", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Check for a selected item
            if (this.clipboardCopyListBox.SelectedIndex == -1)
            {
                // Advise user
                MessageBox.Show("Please select an item to edit", "Select item", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Edit item
            this.clipboardCopyListBox.Items[this.clipboardCopyListBox.SelectedIndex] = this.itemTextBox.Text;

        // Focus text box and exit function
        focusExit:

            // Focus text box
            this.ActiveControl = this.itemTextBox;
        }

        /// <summary>
        /// Handles the clipboard copy list box selected index changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnClipboardCopyListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO Check for selected item with text length [Verify if truly needed on runtimes such as Mono]
            if (this.clipboardCopyListBox.SelectedIndex > -1 && this.clipboardCopyListBox.SelectedItem.ToString().Length > 0)
            {
                // Set item text box
                this.itemTextBox.Text = this.clipboardCopyListBox.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Handles the clipboard copy list box mouse click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnClipboardCopyListBoxMouseClick(object sender, MouseEventArgs e)
        {
            // Check for a selected item
            if (this.clipboardCopyListBox.Items.Count == 0)
            {
                // Advise user
                MessageBox.Show("Please add items to copy", "No items", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Check for a selected item
            if (this.clipboardCopyListBox.SelectedIndex == -1)
            {
                // Advise user
                MessageBox.Show("Please select an item to copy", "Select item", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Copy selected item to clipboard
            Clipboard.SetText(this.clipboardCopyListBox.SelectedItem.ToString());

            // Increase copy count
            this.copyCount++;

            // Update copy count in status label
            this.UpdateStatus();

        // Focus text box and exit function
        focusExit:

            // Focus text box
            this.ActiveControl = this.itemTextBox;
        }

        /// <summary>
        /// Hanfles the delete button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            // Check for a selected item
            if (this.clipboardCopyListBox.SelectedIndex == -1)
            {
                // Advise user
                MessageBox.Show("Please select an item to delete", "Select item", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Delete item
            this.clipboardCopyListBox.Items.RemoveAt(this.clipboardCopyListBox.SelectedIndex);

            // Update item count in status label
            this.UpdateStatus();

        // Focus text box and exit function
        focusExit:

            // Focus text box
            this.ActiveControl = this.itemTextBox;
        }

        /// <summary>
        /// Handles the clear all button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnClearAllButtonClick(object sender, EventArgs e)
        {
            // Check for items
            if (this.clipboardCopyListBox.Items.Count == 0)
            {
                // Advise user
                MessageBox.Show("No items to clear", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Halt flow
                goto focusExit;
            }

            // Ask user
            if (MessageBox.Show($"Would you like to clear {this.clipboardCopyListBox.Items.Count} item{(this.clipboardCopyListBox.Items.Count > 1 ? "s" : string.Empty)}?", "Clear all", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                // Halt flow
                goto focusExit;
            }

            // Clear all items
            this.clipboardCopyListBox.Items.Clear();

            // Update item count
            this.UpdateStatus();

        // Focus text box and exit function
        focusExit:

            // Focus text box
            this.ActiveControl = this.itemTextBox;
        }

        /// <summary>
        /// Handles the new tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Remove all items in list
            this.clipboardCopyListBox.Items.Clear();

            // Update item count display
            this.UpdateStatus();
        }

        /// <summary>
        /// Handles the open tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Show open file dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load lines into list
                    this.LoadItemList(this.openFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when opening \"{Path.GetFileName(this.saveFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Open file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles the save as tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open save file dialog
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK && this.saveFileDialog.FileName.Length > 0)
            {
                try
                {
                    // Write list items to file
                    this.SaveItemList(this.saveFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    // Inform user
                    MessageBox.Show($"Error when saving to \"{Path.GetFileName(this.saveFileDialog.FileName)}\":{Environment.NewLine}{exception.Message}", "Save file error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Inform user
                MessageBox.Show($"Saved {this.clipboardCopyListBox.Items.Count} items to \"{Path.GetFileName(this.saveFileDialog.FileName)}\"", "Items saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Close program
            this.Close();
        }

        /// <summary>
        /// Handles the headquarters patreon.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHeadquartersPatreoncomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open Patreon headquarters
            Process.Start("https://www.patreon.com/publicdomain");
        }

        /// <summary>
        /// Handles the source code github.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open GitHub
            Process.Start("https://github.com/publicdomain");
        }

        /// <summary>
        /// Handles the original thread reddit.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadRedditcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original thread @ Reddit
            Process.Start("https://www.reddit.com/r/software/comments/ewoi4z/copypaste_text_with_a_single_click_without/");
        }

        /// <summary>
        /// Handles the main form form closing event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            // Check for items
            if (this.clipboardCopyListBox.Items.Count > 0)
            {
                // Save item list to disk
                this.SaveItemList(this.itemListFilePath);
            }
            else
            {
                // Remove the file from disk
                File.Delete(this.itemListFilePath);
            }
        }

        /// <summary>
        /// Handles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Set license text [Insert program icon info]
            var licenseText = $"CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication{Environment.NewLine}" +
                $"https://creativecommons.org/publicdomain/zero/1.0/legalcode{Environment.NewLine}{Environment.NewLine}" +
                $"Libraries and icons have separate licenses.{Environment.NewLine}{Environment.NewLine}" + // Program icon info below
                $"Patreon icon used according to published brand guidelines{Environment.NewLine}" +
                $"https://www.patreon.com/brand{Environment.NewLine}{Environment.NewLine}" +
                $"GitHub mark icon used according to published logos and usage guidelines{Environment.NewLine}" +
                $"https://github.com/logos{Environment.NewLine}{Environment.NewLine}" +
                $"Reddit icon used according to published brand guidelines{Environment.NewLine}" +
                $"https://www.reddit.com/wiki/licensing#wiki_using_the_reddit_brand{Environment.NewLine}{Environment.NewLine}" +
                $"PublicDomain icon is based on the following source images:{Environment.NewLine}{Environment.NewLine}" +
                $"Bitcoin by GDJ - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/vectors/bitcoin-digital-currency-4130319/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter P by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/p-glamour-gold-lights-2790632/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter D by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/d-glamour-gold-lights-2790573/{Environment.NewLine}{Environment.NewLine}";

            // Set about form
            var aboutForm = new AboutForm(
                $"About {this.friendlyName}",
                $"{this.friendlyName} {this.semanticVersion}",
                $"Made for: u/Cracker_Z{Environment.NewLine}Reddit.com{Environment.NewLine}Week #06 @ February 2020",
                licenseText,
                this.Icon.ToBitmap());

            // Check for an associated icon
            if (this.associatedIcon == null)
            {
                // Set associated icon from exe file, once
                this.associatedIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            }

            // Set about form icon
            aboutForm.Icon = this.associatedIcon;

            // Match topmost
            aboutForm.TopMost = this.TopMost;

            // Show about form
            aboutForm.ShowDialog();
        }
    }
}
