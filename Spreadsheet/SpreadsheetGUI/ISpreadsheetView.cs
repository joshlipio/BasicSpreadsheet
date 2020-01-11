using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Interface which provides stubs for the GUI of a spreadsheet.
    /// </summary>
    public interface ISpreadsheetView
    {
        /// <summary>
        /// A new cell has been selected.
        /// </summary>
        event Action<string> SelectedCellChangedEvent;

        /// <summary>
        /// The contents of a cell have changed.
        /// The first parameter is the name of the cell, the second parameter is the cell's new contents.
        /// </summary>
        event Action<string, string> CellContentsSetEvent;

        /// <summary>
        /// The user is trying to save the spreadsheet.
        /// </summary>
        event Action<string> SaveSpreadsheetEvent;

        /// <summary>
        /// The user is trying to open a new spreadsheet from a file.
        /// </summary>
        event Action<string> OpenSpreadsheetEvent;

        /// <summary>
        /// The user is trying to open a new spreadsheet.
        /// </summary>
        event Action NewSpreadsheetEvent;

        /// <summary>
        /// The user is trying to close the current spreadsheet.
        /// True if the user has already been warned that the spreadsheet has not been saved.
        /// </summary>
        event Action<bool> CloseSpreadsheetEvent;

        /// <summary>
        /// The user is trying to open the help menu.
        /// </summary>
        event Action OpenHelpEvent;

        /// <summary>
        /// Display the specified contents in the formula bar.
        /// </summary>
        void DisplayContents(string contents);

        /// <summary>
        /// Display the specified value in the value bar.
        /// </summary>
        void DisplayValue(string name, string val);

        /// <summary>
        /// Display the help menu.
        /// </summary>
        void DisplayHelpMenu();

        /// <summary>
        /// Called when the user tries to enter a bad variable.
        /// </summary>
        void ShowFormulaExceptionBox(string badVariable);

        /// <summary>
        /// Called when the user enters a formula which results in a circular exception.
        /// </summary>
        void ShowCircularExceptionBox();

        /// <summary>
        /// Opens a new Window.
        /// </summary>
        void OpenNew();

        /// <summary>
        /// Opens a new windows from a file.
        /// </summary>
        void OpenSaved(string fileName);

        /// <summary>
        /// Warns the user that the spreadsheet has not been saved.
        /// </summary>
        void WarnClose();

        /// <summary>
        /// Closes the current spreadsheet window.
        /// </summary>
        void DoClose();
    }
}
