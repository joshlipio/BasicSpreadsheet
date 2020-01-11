using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    /// <summary>
    /// Class for testing the Spreadsheet class
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        // Make sure constructing a new spreadsheet doesn't throw any exceptions
        [TestMethod]
        public void TestConstructor1()
        {
            Spreadsheet sheet = new Spreadsheet();
        }

        // Check that SetCellContents(number) throws an exception when name is null
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameDoubleNull1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, 0);
        }

        // Check that SetCellContents(number) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameDoubleInvalid1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("z", 0);
        }

        // Check that SetCellContents(number) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameInvalid1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("X07", 0);
        }

        // Check that SetCellContents(text) throws an exception when name is null
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameTextNull1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, "s");
        }

        // Check that SetCellContents(text) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameTextInvalid1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("z", "s");
        }

        // Check that SetCellContents(text) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameTextInvalid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("X07", "s");
        }

        // Check that SetCellContents(text) throws an excpetion when text is null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNameTextNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", null);
        }

        // Check that SetCellContents(formula) throws an exception when name is null
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameFormulaNull1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents(null, new Formula());
        }

        // Check that SetCellContents(formula) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameFormulaInvalid1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("z", new Formula());
        }

        // Check that SetCellContents(formula) throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameformulaInvalid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("X07", new Formula());
        }

        // Check that SetCellContents(formula) throws an exception when a variable in the formula is not a valid cell name
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNameformulaInvalid3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("X1", new Formula("a1 + 2.0 + a2 + horse"));
        }

        // Check that SetCellContents(formula) throws an exception when the new formula creates a circular reference
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetNameformulaCircularError()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", new Formula("a2 + 2.0 + a3"));
            s.SetCellContents("a3", new Formula("a1"));
        }

        // Check that GetCellContents throws an exception when name is null
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetContentsNullException()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        // Check that GetCellContents throws an exception when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetContentsNullInvalid()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("x08");
        }

        // Make sure GetCellContents works as it should
        [TestMethod]
        public void TestGetContents()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("a1"));
            s.SetCellContents("a1", 1d);
            Assert.AreEqual(1d, s.GetCellContents("a1"));
            s.SetCellContents("s17", "potato");
            Assert.AreEqual("potato", s.GetCellContents("s17"));
            Formula f = new Formula("1 + 2 + a1");
            s.SetCellContents("s17", f);
            Assert.AreEqual(f, s.GetCellContents("s17"));
            s.SetCellContents("s17", "doggie");
            Assert.AreEqual("doggie", s.GetCellContents("s17"));
        }

        // Check that GetNamesOfAllNonemptyCells works as it should
        [TestMethod]
        public void TestGetNonempties()
        {
            Spreadsheet s = new Spreadsheet();
            List<string> l = new List<string>();
            foreach (string str in s.GetNamesOfAllNonemptyCells())
                l.Add(str);
            Assert.AreEqual(0, l.Count);
            s.SetCellContents("a1", 1);
            s.SetCellContents("s7", "butt");
            s.SetCellContents("xxi45", new Formula());
            foreach (string str in s.GetNamesOfAllNonemptyCells())
                l.Add(str);
            Assert.AreEqual(3, l.Count);
            l.Clear();
            s.SetCellContents("a1", "");
            s.SetCellContents("s7", "");
            foreach (string str in s.GetNamesOfAllNonemptyCells())
                l.Add(str);
            Assert.AreEqual(1, l.Count);
        }

        // Test that dependencies are updating properly
        [TestMethod]
        public void TestCircular2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", new Formula("a2 + a3"));
            s.SetCellContents("a1", new Formula("a4"));
            s.SetCellContents("a2", new Formula("a1 + a3"));
            s.SetCellContents("a3", new Formula("a1"));
        }

        // Check that the SetCellContents methods return the proper sets
        [TestMethod]
        public void TestSetCellContents()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetCellContents("a1", new Formula("a2"));
            s.SetCellContents("a3", new Formula("a2"));
            HashSet<string> set = new HashSet<string>();
            set.Add("A1");
            set.Add("A3");
            set.Add("A2");
            int iterator = 0;
            foreach (string str in s.SetCellContents("a2", 1))
            {
                Assert.AreEqual(true, set.Contains(str));
                iterator++;
            }
            Assert.AreEqual(3, iterator);
            iterator = 0;
            s.SetCellContents("a1", 1);
            set.Remove("A1");
            foreach (string str in s.SetCellContents("a2", "potato"))
            {
                Assert.AreEqual(true, set.Contains(str));
                iterator++;
            }
            Assert.AreEqual(2, iterator);
            iterator = 0;
            s.SetCellContents("a4", new Formula("a2"));
            set.Add("A4");
            foreach (string str in s.SetCellContents("a2", new Formula("A5")))
            {
                Assert.AreEqual(true, set.Contains(str));
                iterator++;
            }
            Assert.AreEqual(3, iterator);
        }
    }
}
