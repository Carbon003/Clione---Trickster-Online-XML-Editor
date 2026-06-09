using System.CodeDom;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Clione___Trickster_Online_XML_Editor
{
    public partial class MainForm : Form
    {
        private string _currentXmlFolder;
        // store the XML DOM & its corresponding DataTable for the grid
        private XDocument _currentDoc;
        private DataTable _currentTable;
        int currenttable = -1; // Current active table


        public MainForm()
        {
            InitializeComponent();
            dtgrdvwXML.CellValueChanged += DtgrdvwXML_CellValueChanged;
            dtgrdvwXML.CurrentCellDirtyStateChanged += DtgrdvwXML_CurrentCellDirtyStateChanged;
            // disable the OS theme so our header colors stick
            dtgrdvwXML.EnableHeadersVisualStyles = false;

            // overall background behind the rows
            dtgrdvwXML.BackgroundColor = Color.DimGray;
            // the lines between cells
            dtgrdvwXML.GridColor = Color.DarkGray;

            // header styling
            var headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = dtgrdvwXML.ColumnHeadersDefaultCellStyle.Font
            };
            dtgrdvwXML.ColumnHeadersDefaultCellStyle = headerStyle;
            dtgrdvwXML.RowHeadersDefaultCellStyle = headerStyle;

            // cell styling
            dtgrdvwXML.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(64, 64, 64),
                ForeColor = Color.White,
                SelectionBackColor = Color.DarkSlateGray,
                SelectionForeColor = Color.White
            };

            // alternate-row stripe (just a touch lighter)
            dtgrdvwXML.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White
            };
            lstXML.ContextMenuStrip = ctxMenuXMLList;

        }

        private void DeleteFile(string fileName)
        {
            // combine the folder + the filename
            string fullPath = Path.Combine(_currentXmlFolder, fileName);

            // quickly sanity-check that the file really exists there
            if (!File.Exists(fullPath))
            {
                MessageBox.Show(
                    $"File not found:\n{fullPath}",
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var choice = MessageBox.Show(
                $"Are you sure you want to delete \"{fileName}\"?",
                "Confirm Delete",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (choice != DialogResult.OK)
                return;

            try
            {
                setLoadingCue();
                File.Delete(fullPath);
                dtgrdvwXML.ClearSelection();
                dtgrdvwXML.DataSource = null;
                dtgrdvwXML.Columns.Clear();
                dtgrdvwXML.Refresh();
                LoadXmlFiles(_currentXmlFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unable to delete {fullPath}:\n{ex.Message}",
                    "Delete Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (var dlg = new AboutForm())
            {
                dlg.ShowDialog(this);
            }
        }
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1.0)
                Opacity += 0.05;
            else
                fadeTimer.Stop();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void tlstrpOpenFolder_Click(object sender, EventArgs e)
        {
            while (true)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing Trickster's XML files";
                    fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                    fbd.ShowNewFolderButton = false;

                    // If they cancel the folder‐picker, just bail out.
                    if (fbd.ShowDialog(this) != DialogResult.OK)
                        return;
                    var xmlFolder = fbd.SelectedPath;
                    // look for any *.xml in that folder
                    var xmlFiles = Directory.GetFiles(xmlFolder, "*.xml", SearchOption.TopDirectoryOnly);

                    if (xmlFiles.Length == 0)
                    {
                        var retry = MessageBox.Show(
                            "No XML found in selected folder.",
                            "Folder Selection",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Warning
                        );
                        if (retry == DialogResult.Retry)
                            continue;    // show the FolderBrowserDialog again
                        else
                            return;      // user chose Cancel
                    }
                    LoadXmlFiles(xmlFolder);
                    break;
                }
            }
        }

        // at class scope
        private List<string> _allXmlFiles = new List<string>();

        private void LoadXmlFiles(string folder)
        {
            setLoadingCue();
            _currentXmlFolder = folder;

            // grab all .xml files in that directory
            var fullPaths = Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly);

            // build our master list of just the file names
            _allXmlFiles = fullPaths
                .Select(Path.GetFileName)
                .ToList();

            // show them in the ListBox
            RefreshFileList(_allXmlFiles);

            // update count
            tlstrpXMLCount.Text = $"Loaded XML Count: {_allXmlFiles.Count}";
            clearLoadingCue();
        }

        private void RefreshFileList(IEnumerable<string> files)
        {
            lstXML.BeginUpdate();
            lstXML.Items.Clear();
            foreach (var f in files)
                lstXML.Items.Add(f);
            lstXML.EndUpdate();
        }


        private async void lstXML_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstXML.SelectedIndex < 0 || string.IsNullOrEmpty(_currentXmlFolder))
                return;

            txtbxSearchTable.Text = string.Empty;

            string fileName = lstXML.SelectedItem.ToString();
            string path = Path.Combine(_currentXmlFolder, fileName);

            // show loading cue on UI
            setLoadingCue();

            try
            {
                // offload to thread‐pool
                var result = await Task.Run(() => LoadTableFromXml(path));

                // now back on UI‐thread – bind your grid
                _currentDoc = result.Doc;
                _currentTable = result.Table;

                dtgrdvwXML.DataSource = _currentTable;
                dtgrdvwXML.AutoGenerateColumns = true;
                dtgrdvwXML.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
                dtgrdvwXML.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                // 3) (Optional) Make *sure* user‐resize is enabled
                dtgrdvwXML.AllowUserToResizeColumns = true;
                foreach (DataGridViewColumn col in dtgrdvwXML.Columns)
                {
                    col.Resizable = DataGridViewTriState.True;
                    // col.Width already set by the AutoResizeColumns call
                }


                lblCurrentTable.Text = fileName;
                tlstrpRowCount.Text = $"Rows: {_currentTable.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load '{fileName}':\n{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                clearLoadingCue();
            }
        }

        /// <summary>
        /// Parses the XML from disk into a DataTable (and keeps the XDocument alive)
        /// All of this runs off the UI thread.
        /// </summary>
        private (XDocument Doc, DataTable Table) LoadTableFromXml(string path)
        {
            var doc = XDocument.Load(path);
            var tableElem = doc.Element("TABLE")
                              ?? throw new InvalidDataException("Root <TABLE> element not found.");

            // grab field names
            var fieldNames = tableElem
                .Elements("FIELDINFO")
                .Select(fi => (string)fi.Attribute("Name"))
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // build DataTable
            var dt = new DataTable(Path.GetFileNameWithoutExtension(path));
            foreach (var fn in fieldNames)
                dt.Columns.Add(fn, typeof(string));

            // fill rows
            foreach (var rowElem in tableElem.Elements("ROW"))
            {
                var dr = dt.NewRow();
                foreach (var fn in fieldNames)
                    dr[fn] = rowElem.Element(fn)?.Value ?? "";
                dt.Rows.Add(dr);
            }

            return (doc, dt);
        }



        private void DtgrdvwXML_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SaveCurrentXml();
        }

        private void SaveCurrentXml()
        {
            if (_currentDoc == null || _currentTable == null || lstXML.SelectedItem == null)
                return;

            // 1) Clear out existing <ROW> elements
            var tableElem = _currentDoc.Element("TABLE");
            tableElem.Elements("ROW").Remove();

            // 2) Rebuild from your DataTable
            foreach (DataRow dr in _currentTable.Rows)
            {
                var rowEl = new XElement("ROW");
                foreach (DataColumn col in _currentTable.Columns)
                    rowEl.Add(new XElement(col.ColumnName, dr[col]?.ToString() ?? ""));
                tableElem.Add(rowEl);
            }

            // 3) Save back to disk
            var fileName = lstXML.SelectedItem.ToString();
            var path = Path.Combine(_currentXmlFolder, fileName);
            _currentDoc.Save(path);

            // 4) Update status strip
            tlstrpStatus.Text = $"Saved: {fileName}";
        }


        private void DtgrdvwXML_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dtgrdvwXML.IsCurrentCellDirty)
                dtgrdvwXML.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }


        private void ctxMenuXMLList_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstXML.SelectedIndex < 0) return;

            tlstrpmnTableName.Text = $"File : {lstXML.SelectedItem.ToString()}";
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstXML.SelectedItem == null) return;

            string oldName = lstXML.SelectedItem.ToString();
            using (var dlg = new RenameForm(oldName, _currentXmlFolder))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // Now use dlg.NewName instead of accessing txtbxNewName
                    string newName = dlg.NewName;

                    // Reload and re-select
                    LoadXmlFiles(_currentXmlFolder);
                    lstXML.SelectedItem = newName;
                }
            }
        }

        private void lstXML_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // figure out which item (if any) is under the mouse
                int idx = lstXML.IndexFromPoint(e.Location);
                if (idx != ListBox.NoMatches)
                {
                    lstXML.SelectedIndex = idx;
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstXML.SelectedItem == null) return;
            DeleteFile(lstXML.SelectedItem.ToString());
        }

        private void deleteTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstXML.SelectedItem == null) return;
            DeleteFile(lstXML.SelectedItem.ToString());
        }


        private void setLoadingCue()
        {
            Cursor = Cursors.WaitCursor;
            tlstrpStatus.Text = "Loading... ";
        }

        private void clearLoadingCue()
        {
            Cursor = Cursors.Default;
            tlstrpStatus.Text = "Ready.";
        }

        private void showInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "/select, \"" + _currentXmlFolder + "\\" + lstXML.SelectedItem.ToString() + "\"");
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature not implemented in this version.");
            return;
            using (var dlg = new LogForm())
            {
                dlg.ShowDialog(this);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Saving is automatic. Saving parameters not implemented in this version");
            return;
        }

        private void editColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void cloneRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dtgrdvwXML.SelectedRows.Count > 0)
            {
                // Get the index of the selected row in the DataGridView
                int rowIndex = dtgrdvwXML.SelectedRows[0].Index;

                // Get the data source of the DataGridView
                DataTable dataTable = (DataTable)dtgrdvwXML.DataSource;

                // Ensure the data source is valid
                if (dataTable != null)
                {
                    // Get the DataRow index based on the DataGridView row index
                    DataRowView rowView = (DataRowView)dtgrdvwXML.SelectedRows[0].DataBoundItem;
                    int dataTableRowIndex = dataTable.Rows.IndexOf(rowView.Row);

                    // Get the selected row from the data source
                    DataRow selectedRow = dataTable.Rows[dataTableRowIndex];

                    // Create a new row with the same values
                    DataRow clonedRow = dataTable.NewRow();
                    clonedRow.ItemArray = selectedRow.ItemArray.Clone() as object[];

                    // Add the new row to the data source
                    dataTable.Rows.Add(clonedRow);

                    // Re-sort the DataGridView to reflect the addition
                    try
                    {
                        dtgrdvwXML.Sort(dtgrdvwXML.SortedColumn, dtgrdvwXML.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
                    }
                    catch
                    {
                        // Handle exception if sorting is not possible
                    }

                    // Find the new row in the DataGridView
                    foreach (DataGridViewRow row in dtgrdvwXML.Rows)
                    {
                        DataRowView currentRowView = row.DataBoundItem as DataRowView;
                        if (currentRowView != null && currentRowView.Row == clonedRow)
                        {
                            // Clear any existing selections
                            dtgrdvwXML.ClearSelection();

                            // Select the new row
                            row.Selected = true;
                            dtgrdvwXML.CurrentCell = row.Cells[0]; // Set focus to the first cell of the new row

                            // Set the background color of the new row to YellowGreen
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                cell.Style.BackColor = Color.YellowGreen;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to clone.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void createTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature not implemented in this version.");
            return;
        }

        private void txtbxSearchFiles_TextChanged(object sender, EventArgs e)
        {
            var filter = txtbxSearchFiles.Text.Trim();
            if (string.IsNullOrEmpty(filter))
            {
                RefreshFileList(_allXmlFiles);
            }
            else
            {
                // hide (i.e. don’t show) anything that doesn’t contain the filter
                var matches = _allXmlFiles
                    .Where(name => name
                        .IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
                RefreshFileList(matches);
            }

            // optional: clear selection if it’s no longer visible
            if (lstXML.SelectedItem != null && !lstXML.Items.Contains(lstXML.SelectedItem))
                lstXML.ClearSelected();
        }

        private void txtbxSearchTable_Enter(object sender, EventArgs e)
        {
            tlstrplblSearch.Text = "Press \"Enter\" to search. Press again for the next entry.";
        }

        private void txtbxSearchTable_Leave(object sender, EventArgs e)
        {
            tlstrplblSearch.Text = string.Empty;
        }

        private object oldCellValue = null;

        // Keep track of the last matching result
        private int lastMatchIndex = -1;

        // Number of match when serching
        private int matchCount = 0;

        // last searched value. if changed, update this value and update count
        private string lastSearchedString = "";

        private List<int> matchRowIndices = new List<int>();

        private int currentMatchIndex = 0;

        private void txtbxSearchTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true; // Suppress AutoComplete default behavior
                e.SuppressKeyPress = true; // Prevent the TextBox from processing Enter
                string searchValue = this.txtbxSearchTable.Text;

                // Check if the search value has changed
                if (!searchValue.Equals(lastSearchedString, StringComparison.OrdinalIgnoreCase))
                {
                    // Update the last searched string
                    lastSearchedString = searchValue;

                    // Count the total matches and store the row indices
                    matchCount = 0;
                    matchRowIndices.Clear();
                    foreach (DataGridViewRow row in dtgrdvwXML.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchValue.ToLower()))
                            {
                                if (!matchRowIndices.Contains(row.Index))
                                {
                                    matchCount++;
                                    matchRowIndices.Add(row.Index);
                                }
                            }
                        }
                    }

                    // Reset the match index
                    lastMatchIndex = -1;
                    currentMatchIndex = 0;

                    // Update the ToolStripStatusLabel with the total count of matches
                }


                if (!string.IsNullOrEmpty(searchValue))
                {
                    dtgrdvwXML.ClearSelection();
                    bool valueFound = false;

                    for (int i = 0; i < matchRowIndices.Count; i++)
                    {
                        int rowIndex = (lastMatchIndex + 1 + i) % matchRowIndices.Count;
                        try {
                            DataGridViewRow row = dtgrdvwXML.Rows[matchRowIndices[rowIndex]];

                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchValue.ToLower()))
                                {
                                    cell.Selected = true;
                                    dtgrdvwXML.FirstDisplayedScrollingRowIndex = row.Index;
                                    lastMatchIndex = rowIndex;
                                    currentMatchIndex = rowIndex + 1;
                                    valueFound = true;
                                    break;
                                }
                            }
                        }
                        catch
                        {

                        }
                        

                        

                        if (valueFound)
                        {
                            break;
                        }
                    }

                    // If no match is found, reset the index
                    if (!valueFound)
                    {
                        lastMatchIndex = -1;
                    }

                    // Update the ToolStripStatusLabel with the current match index
                    if (matchCount > 0)
                    {
                        tlstrplblSearch.Text = "Match " + currentMatchIndex.ToString() + " / " + matchCount.ToString() + " match(es).";
                    }
                }
                else
                {
                    // Clear the status label if the search box is empty
                    tlstrplblSearch.Text = string.Empty;
                    lastMatchIndex = -1;
                    matchCount = 0;
                    lastSearchedString = "";
                    matchRowIndices.Clear();
                }
            }
        }
    }
}
