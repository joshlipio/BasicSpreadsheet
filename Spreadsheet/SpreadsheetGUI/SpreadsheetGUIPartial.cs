using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;

namespace SpreadsheetGUI
{
    partial class Form1 : Form, ISpreadsheetView
    {
        public Form1()
        {
            InitializeComponent();
            AcceptButton = enterButton;
            enterButton.Click += HandleCurrentContentsChanged;
            spreadsheetPanel1.SelectionChanged += ViewHandleSelectionChanged;
            newToolStripMenuItem.Click += ViewHandleNew;
            exitToolStripMenuItem.Click += ViewHandleClose;
            openToolStripMenuItem.Click += ViewHandleOpen;
            saveToolStripMenuItem.Click += ViewHandleSave;
            helpToolStripMenuItem.Click += ViewHandleHelp;
        }

        public event Action<string, string> CellContentsSetEvent;
        public event Action<bool> CloseSpreadsheetEvent;
        public event Action OpenHelpEvent;
        public event Action NewSpreadsheetEvent;
        public event Action<string> OpenSpreadsheetEvent;
        public event Action<string> SaveSpreadsheetEvent;
        public event Action<string> SelectedCellChangedEvent;

        /// <summary>
        /// Handle a request to change the selected cell
        /// </summary>
        private void ViewHandleSelectionChanged(SpreadsheetPanel ss)
        {
            int col;
            int row;
            ss.GetSelection(out col, out row);
            row++;
            char letterCol = ColToLetter(col);
            if (SelectedCellChangedEvent != null)
                SelectedCellChangedEvent(letterCol + "" + row);
            cellNameBox.Text = letterCol + "" + row;
        }

        /// <summary>
        /// Handles a request to change the contents of the selected cell.
        /// </summary>
        private void HandleCurrentContentsChanged(object sender, EventArgs e)
        {
            if (CellContentsSetEvent != null)
            {
                int col;
                int row;
                spreadsheetPanel1.GetSelection(out col, out row);
                CellContentsSetEvent(ColToLetter(col) + "" + (row + 1), contentsBox.Text);
            }
        }

        /// <summary>
        /// Handles a request to open a new spreadsheet.
        /// </summary>
        private void ViewHandleNew(Object sender, EventArgs e)
        {
            if (NewSpreadsheetEvent != null)
            {
                NewSpreadsheetEvent();
            }
        }

        /// <summary>
        /// Handles a request to open a new spreadsheet from a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewHandleOpen(object sender, EventArgs e)
        {
            if (OpenSpreadsheetEvent != null)
            {
                OpenFileDialog result = new OpenFileDialog();
                result.Filter = "Spreadsheet Files (*.ss)|*.ss|All Files (*.*)|*.*";
                result.DefaultExt = ".ss";
                result.ShowDialog();
                OpenSpreadsheetEvent(result.FileName);
            }
        }

        /// <summary>
        /// Handles a request to save a spreadsheet.
        /// </summary>
        private void ViewHandleSave(object sender, EventArgs e)
        {
            if (SaveSpreadsheetEvent != null)
            {
                SaveFileDialog result = new SaveFileDialog();
                result.Filter = "Spreadsheet Files (*.ss)|*.ss|All Files (*.*)|*.*";
                result.DefaultExt = ".ss";
                result.ShowDialog();
                SaveSpreadsheetEvent(result.FileName);
            }
        }


        /// <summary>
        /// Handles a request to exit the spreadsheet from the File menu.
        /// </summary>
        private void ViewHandleClose(object sender, EventArgs e)
        {
            if (CloseSpreadsheetEvent != null)
            {
                CloseSpreadsheetEvent(false);
            }
        }

        /// <summary>
        /// Handles a request to open the help menu by firing a 
        /// </summary>
        private void ViewHandleHelp(object sender, EventArgs e)
        {
            if (OpenHelpEvent != null)
            {
                OpenHelpEvent();
            }
        }

        /// <summary>
        /// Converts a letter into the corresponding number. i.e. A will be 0, Z will be 25
        /// </summary>
        private char ColToLetter(int col)
        {
            int num = (int)'A';
            int res = col + num;
            char toReturn = (char)res;
            return toReturn;
        }

        /// <summary>
        /// Converts a number into the corresponding letter. i.e. 0 will be A and Z will be 25
        /// </summary>
        private int LetterToCol(char letter)
        {
            return (int)letter - (int)'A';
        }

        /// <summary>
        /// Sets the contents box's text value.
        /// </summary>
        public void DisplayContents(string contents)
        {
            contentsBox.Text = contents;
        }

        /// <summary>
        /// Updates the value of the specified cell in the grid and in the upper bar.
        /// </summary>
        public void DisplayValue(string name, string val)
        {
            valueBox.Text = val;
            int col = LetterToCol(name[0]);
            int row = Int32.Parse(name.Substring(1)) - 1;
            spreadsheetPanel1.SetValue(col, row, val);
        }

        /// <summary>
        /// Displays the help menu.
        /// </summary>
        public void DisplayHelpMenu()
        {
            string message = "Welcome to the FratStars spreadsheet project!\n\nYou can access cells simply by clicking on them.\nTo change the contents of a cell, click in the formula bar (next to the fancy 'f'), type in the contents, and press 'Enter'.\nTo create a formula, type '=' into the box, followed by the desired formula. If a formula references a cell without a number value, the value of the cell will be a FormErr.\nIn the 'File' menu, you can click 'New' to open a new, empty spreadsheet, 'Open' to open an existing spreadsheet, 'Save' to save the current spreadsheet, and 'Exit' to close the current window.";
            string caption = "Help";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
        }

        /// <summary>
        /// Opens a new spreadsheet window.
        /// </summary>
        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        public void OpenSaved(string fileName)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(fileName);
        }

        /// <summary>
        /// Called when the user enters a bad formula.
        /// </summary>
        public void ShowFormulaExceptionBox(string explanation)
        {
            string message = "Error: " + explanation;
            string caption = "";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
        }

        /// <summary>
        /// Called when the user enters a formula which results in a circular exception.
        /// </summary>
        public void ShowCircularExceptionBox()
        {
            string message = "Error: That formula would create a circular dependency";
            string caption = "";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result = MessageBox.Show(message, caption, buttons);
        }

        /// <summary>
        /// Called when the user is trying to exit the program, but has not saved.
        /// Brings up a dialogue box to make sure they want to exit.
        /// </summary>
        public void WarnClose()
        {
            string message = "The spreadsheet has not been saved!\nAre you sure you want to exit?";
            string caption = "";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.Yes)
                if (CloseSpreadsheetEvent != null)
                    CloseSpreadsheetEvent(true);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void DoClose()
        {
            Close();
        }

        void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseSpreadsheetEvent != null)
            {
                CloseSpreadsheetEvent(false);
            }
        }
    }
}
