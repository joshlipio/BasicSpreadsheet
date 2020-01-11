using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTester
{
    /// <summary>
    /// A class which implements the ISpreadsheetView interface, used for testing.
    /// Contains a public variable "lastMethodCalled" which contains the name of the last method called
    /// to ensure Controller is doing the right things.
    /// </summary>
    class SpreadsheetStubView : ISpreadsheetView
    {
        public event Action<string, string> CellContentsSetEvent;
        public event Action<bool> CloseSpreadsheetEvent;
        public event Action NewSpreadsheetEvent;
        public event Action OpenHelpEvent;
        public event Action<string> OpenSpreadsheetEvent;
        public event Action<string> SaveSpreadsheetEvent;
        public event Action<string> SelectedCellChangedEvent;

        // These variables track what the Controller has done for later validation.
        public string lastMethodCalled = "";
        public string currentCell = "A1";
        public object currentValue = "";

        public void FireCellContentsSetEvent(string name, string val)
        {
            CellContentsSetEvent(name, val);
        }

        public void FireSelectedCellChangedEvent(string name)
        {
            SelectedCellChangedEvent(name);
        }

        public void FireCloseSpreadsheetEvent(bool validated)
        {
            CloseSpreadsheetEvent(validated);
        }

        public void FireNewSpreadsheetEvent()
        {
            NewSpreadsheetEvent();
        }

        public void FireOpenHelpEvent()
        {
            OpenHelpEvent();
        }

        public void FireOpenSpreadsheetEvent()
        {
            OpenSpreadsheetEvent("Test.ss");
        }

        public void FireCancelOpeningSpreadsheetEvent()
        {
            OpenSpreadsheetEvent("");
        }

        public void FireSaveSpreadsheetEvent()
        {
            SaveSpreadsheetEvent("Test.ss");
        }

        public void DisplayContents(string contents)
        {
            lastMethodCalled = "DisplayContents";
        }

        public void DisplayValue(string name, string val)
        {
            lastMethodCalled = "DisplayValue";
            currentValue = val;
        }

        public void DoClose()
        {
            lastMethodCalled = "DoClose";
        }

        public void OpenNew()
        {
            lastMethodCalled = "OpenNew";
        }

        public void ShowCircularExceptionBox()
        {
            lastMethodCalled = "ShowCircularExceptionBox";
        }

        public void ShowFormulaExceptionBox(string badVariable)
        {
            lastMethodCalled = "ShowFormulaExceptionBox";
        }

        public void WarnClose()
        {
            lastMethodCalled = "WarnClose";
        }

        public void OpenSaved(string fileName)
        {
            lastMethodCalled = "OpenSaved";
        }

        public void DisplayHelpMenu()
        {
            lastMethodCalled = "DisplayHelpMenu";
        }
    }
}
