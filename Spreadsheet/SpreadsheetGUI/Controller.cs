using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SS;

namespace SpreadsheetGUI
{
    public class Controller
    {
        // The view being used
        private ISpreadsheetView window;

        // The model being used.
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Begins controlling the supplied window.
        /// </summary>
        public Controller(ISpreadsheetView viewWindow)
        {
            window = viewWindow;
            InitializeWindow();
            spreadsheet = new Spreadsheet(new Regex("^[a-z|A-Z][1-9][0-9]{0,1}$"));
        }

        /// <summary>
        /// Begins controlling the supplied window with an already-created spreadsheet.
        /// </summary>
        public Controller(string fileName, ISpreadsheetView viewWindow)
        {
            if (fileName == "")
                return;
            System.IO.TextReader source = new System.IO.StreamReader(fileName);
            spreadsheet = new Spreadsheet(source, new Regex("^[a-z|A-Z][1-9][0-9]{0,1}$"));
            source.Close();
            window = viewWindow;
            InitializeWindow();

            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                HandleSelectedCellChanged(cell);
            }
        }

        /// <summary>
        /// Add listeners to the window.
        /// </summary>
        private void InitializeWindow()
        {
            window.SelectedCellChangedEvent += HandleSelectedCellChanged;
            window.CellContentsSetEvent += HandleCellContentsSet;
            window.CloseSpreadsheetEvent += HandleCloseSpreadsheet;
            window.NewSpreadsheetEvent += HandleNewSpreadsheet;
            window.SaveSpreadsheetEvent += HandleSaveSpreadsheet;
            window.OpenSpreadsheetEvent += HandleOpenSpreadsheet;
            window.OpenHelpEvent += HandleOpenHelp;
        }

        /// <summary>
        /// Handles a request to change the selected cell.
        /// The view should handle highliting the new cell, itself.
        /// </summary>
        private void HandleSelectedCellChanged(string name)
        {
            window.DisplayContents(spreadsheet.GetCellContents(name).ToString());

            object val = spreadsheet.GetCellValue(name);
            string toDisplay;
            if (val.GetType().Equals(typeof(FormulaError)))
                toDisplay = "FormErr";
            else
                toDisplay = val.ToString();
            window.DisplayValue(name, toDisplay);
        }

        /// <summary>
        /// Handles a request to change the contents of the selected cell.
        /// </summary>
        private void HandleCellContentsSet(string name, string newContents)
        {
            try
            {
                foreach (string cell in spreadsheet.SetContentsOfCell(name, newContents))
                {
                    object val = spreadsheet.GetCellValue(cell);
                    string toDisplay;
                    if (val.GetType().Equals(typeof(FormulaError)))
                        toDisplay = "FormErr";
                    else
                        toDisplay = val.ToString();
                    window.DisplayValue(cell, toDisplay);
                }
            }
            catch (CircularException e)
            {
                window.ShowCircularExceptionBox();
            }
            catch (Formulas.FormulaFormatException e)
            {
                window.ShowFormulaExceptionBox(e.Message);
            }
        }

        /// <summary>
        /// Handles a request to open a new spreadsheet.
        /// </summary>
        private void HandleNewSpreadsheet()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Handles a request to save the spreadsheet.
        /// </summary>
        private void HandleSaveSpreadsheet(string fileName)
        {
            if (fileName == "")
                return;
            System.IO.TextWriter saveName = new System.IO.StreamWriter(fileName);
            spreadsheet.Save(saveName);
            saveName.Close();
        }

        /// <summary>
        /// Handles a request to open a spreadsheet from file.
        /// </summary>
        private void HandleOpenSpreadsheet(string fileName)
        {
            if (fileName == "")
                return;    
            window.OpenSaved(fileName);
        }

        /// <summary>
        /// Handles a request to close the spreadsheet.
        /// If the spreadsheet has been changed since it was last saved, opens a dialogue box before closing.
        /// </summary>
        private void HandleCloseSpreadsheet(bool verified)
        {
            if (spreadsheet.Changed && !verified)
                window.WarnClose();
            else
                window.DoClose();
        }


        /// <summary>
        /// Handles a request to open a help menu.
        /// </summary>
        private void HandleOpenHelp()
        {
            window.DisplayHelpMenu();
        }
    }
}