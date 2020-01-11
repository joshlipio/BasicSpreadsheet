using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;
using System.Text.RegularExpressions;

namespace ControllerTester
{
    /// <summary>
    /// Class for testing the Controller class
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        // Test the one arg constructor
        [TestMethod]
        public void TestMethod1()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("B7", "cat");
            Assert.AreEqual("DisplayValue", window.lastMethodCalled);
            Assert.AreEqual("cat", window.currentValue);
            window.FireCellContentsSetEvent("A3", "15");
            Assert.AreEqual("15", window.currentValue);
            window.FireCellContentsSetEvent("A1", "=A1");
            Assert.AreEqual("ShowCircularExceptionBox", window.lastMethodCalled);
            window.FireCellContentsSetEvent("A1", "=A100");
            Assert.AreEqual("ShowFormulaExceptionBox", window.lastMethodCalled);
            window.FireCloseSpreadsheetEvent(false);
            Assert.AreEqual("WarnClose", window.lastMethodCalled);
            window.FireCloseSpreadsheetEvent(true);
            Assert.AreEqual("DoClose", window.lastMethodCalled);
            window.FireNewSpreadsheetEvent();
            Assert.AreEqual("OpenNew", window.lastMethodCalled);
            window.FireSaveSpreadsheetEvent();
            window.FireOpenSpreadsheetEvent();
        }

        // Test the two arg constructor
        [TestMethod]
        public void TestMethod2()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller("Test.ss", window);

            window.FireCellContentsSetEvent("B7", "cat");
            Assert.AreEqual("DisplayValue", window.lastMethodCalled);
            Assert.AreEqual("cat", window.currentValue);
            window.FireCellContentsSetEvent("A3", "15");
            Assert.AreEqual("15", window.currentValue);
            window.FireCellContentsSetEvent("A1", "=A1");
            Assert.AreEqual("ShowCircularExceptionBox", window.lastMethodCalled);
            window.FireCellContentsSetEvent("A1", "=A100");
            Assert.AreEqual("ShowFormulaExceptionBox", window.lastMethodCalled);
            window.FireCloseSpreadsheetEvent(false);
            Assert.AreEqual("WarnClose", window.lastMethodCalled);
            window.FireCloseSpreadsheetEvent(true);
            Assert.AreEqual("DoClose", window.lastMethodCalled);
            window.FireNewSpreadsheetEvent();
            Assert.AreEqual("OpenNew", window.lastMethodCalled);
            window.FireSaveSpreadsheetEvent();
            window.FireOpenSpreadsheetEvent();
        }

        /// <summary>
        /// Tests opening empty file path
        /// </summary>
        [TestMethod]
        public void TestEmptyStart()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller("", window);
        }

        [TestMethod]
        public void TestStart()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);
        }

        [TestMethod]
        public void TestOpenCancel()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCancelOpeningSpreadsheetEvent();
        }
    
        [TestMethod]
        public void TestSetContent()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("B7", "cat");
            Assert.AreEqual("DisplayValue", window.lastMethodCalled);
            Assert.AreEqual("cat", window.currentValue);
        }

        [TestMethod]
        public void TestRewriteContent()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("B7", "cat");
            window.FireCellContentsSetEvent("B7", "dog");
            Assert.AreEqual("dog", window.currentValue);
        }

        [TestMethod]
        public void TestCircularDependency()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("A1", "=A1");
            Assert.AreEqual("ShowCircularExceptionBox", window.lastMethodCalled);
        }
        
        [TestMethod]
        public void TestInvalidFormula()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("A1", "=A100");
            Assert.AreEqual("ShowFormulaExceptionBox", window.lastMethodCalled);
        }

        [TestMethod]
        public void TestCloseSpreadsheet()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCloseSpreadsheetEvent(false);
            Assert.AreEqual("DoClose", window.lastMethodCalled);
        }

        [TestMethod]
        public void TestChangedCloseSpreadsheet()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("A7", "7");
            window.FireCloseSpreadsheetEvent(false);
            Assert.AreEqual("WarnClose", window.lastMethodCalled);
            window.FireCloseSpreadsheetEvent(true);
            Assert.AreEqual("DoClose", window.lastMethodCalled);
        }

        [TestMethod]
        public void TestNewSpreadsheet()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireNewSpreadsheetEvent();
            Assert.AreEqual("OpenNew", window.lastMethodCalled);
        }

        [TestMethod]
        public void TestSaveSpreadsheet()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);

            window.FireCellContentsSetEvent("A5", "dog");
            window.FireCellContentsSetEvent("A1", "=A5");
            window.FireSaveSpreadsheetEvent();           
        }

        [TestMethod]
        public void TestLoadSpreadsheet()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);
            window.FireOpenSpreadsheetEvent();
        }

        [TestMethod]
        public void TestHelpDialog()
        {
            SpreadsheetStubView window = new SpreadsheetStubView();
            Controller controller = new Controller(window);
            window.FireOpenHelpEvent();
            Assert.AreEqual("DisplayHelpMenu", window.lastMethodCalled);

        }
    }
}
