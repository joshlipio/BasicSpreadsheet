using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SS
{
    public class Controller
    {
        // The view being used
        private ISpreadsheetView window;

        // The model being used.
        private Spreadsheet spreadsheet;

        // The selected, highlited cell. Content changes go into this cell, and its content and value are displayed.
        private string selectedCell;

        /// <summary>
        /// Begins controlling the supplied window.
        /// </summary>
        public Controller(ISpreadsheetView viewWindow)
        {
            window = viewWindow;
            spreadsheet = new Spreadsheet(new Regex("^[a-z|A-Z][1-9][0-9]$"));
            selectedCell = "A1";
        }

        /// <summary>
        /// Handles a request to change the selected cell.
        /// The view should handle highliting the new cell, itself.
        /// </summary>
        private void HandleSelectedCellChanged(string name)
        {
            selectedCell = name;
            window.DisplayContents(spreadsheet.GetCellContents(name).ToString());
            window.DisplayValue(spreadsheet.GetCellValue(name));
        }

        /// <summary>
        /// Handles a request to change the contents of the selected cell.
        /// </summary>
        private void HandleCellContentsSet(string newContents)
        {
            spreadsheet.SetContentsOfCell(selectedCell, newContents);
            window.DisplayValue(spreadsheet.GetCellValue(selectedCell));
        }

        /// <summary>
        /// Handles a request to open a new spreadsheet.
        /// </summary>
        private void HandleNewSpreadsheet()
        {

        }

        /// <summary>
        /// Handles a request to save the spreadsheet.
        /// </summary>
        private void HandleSaveSpreadsheet()
        {

        }

        /// <summary>
        /// Handles a request to open a spreadsheet from file.
        /// </summary>
        private void HandleOpenSpreadsheet()
        {

        }

        /// <summary>
        /// Handles a request to close the spreadsheet.
        /// If the spreadsheet has been changed since it was last saved, opens a dialogue box before closing.
        /// </summary>
        private void HandleCloseSpreadsheet()
        {

        }

        /// <summary>
        /// Handles a request to open a help menu.
        /// </summary>
        private void HandleOpenHelp()
        {

        }
    }
}
