using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Formulas;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace GradingTests
{
    /// <summary>
    /// These are tests for PS6
    ///</summary>
    [TestClass()]
    public class SpreadsheetTest
    {
        /// <summary>
        /// Used to make assertions about set equality.  Everything is converted first to
        /// upper case.
        /// </summary>
        public static void AssertSetEqualsIgnoreCase(IEnumerable<string> s1, IEnumerable<string> s2)
        {
            var set1 = new HashSet<String>();
            foreach (string s in s1)
            {
                set1.Add(s.ToUpper());
            }

            var set2 = new HashSet<String>();
            foreach (string s in s2)
            {
                set2.Add(s.ToUpper());
            }

            Assert.IsTrue(new HashSet<string>(set1).SetEquals(set2));
        }

        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("AA");
        }

        [TestMethod()]
        public void Test3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1A", "1.5");
        }

        // Make sure exception is thrown if content is null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test5a()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        [TestMethod()]
        public void Test6()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test11()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("AZ", "= 2");
        }

        [TestMethod()]
        public void Test13()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "= 3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(3, f.Evaluate(x => 0), 1e-6);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test14()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7", "=A1+A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                object o = s.GetCellContents("A2");
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test17()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test18()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            List<string> list = new List<string>();
            foreach (string str in s.GetNamesOfAllNonemptyCells())
                list.Add(str);
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("C2", "hello");
            s.SetContentsOfCell("C2", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test21()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test22()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test23()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            AssertSetEqualsIgnoreCase(s.GetNamesOfAllNonemptyCells(), new string[] { "A1", "B1", "C1" });
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("A1", "17.2"), new string[] { "A1" });
        }

        [TestMethod()]
        public void Test25()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("B1", "hello"), new string[] { "B1" });
        }

        [TestMethod()]
        public void Test26()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            AssertSetEqualsIgnoreCase(s.SetContentsOfCell("C1", "=5"), new string[] { "C1" });
        }

        [TestMethod()]
        public void Test27()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            HashSet<string> result = new HashSet<string>(s.SetContentsOfCell("A5", "82.5"));
            AssertSetEqualsIgnoreCase(result, new string[] { "A5", "A4", "A3", "A1" });
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void Test30()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(23, ((Formula)s.GetCellContents("A1")).Evaluate(x => 0));
        }

        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            AssertSetEqualsIgnoreCase(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }, cells);
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                AssertSetEqualsIgnoreCase(cells, s.SetContentsOfCell("A" + i, "=A" + (i + 1)));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, "=A" + (i + 1));
            }
            try
            {
                s.SetContentsOfCell("A150", "=A50");
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        [TestMethod()]
        public void Test43()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            for (int i = 0; i < 500; i++)
            {
                s.SetContentsOfCell("A1" + i, "=A1" + (i + 1));
            }

            ISet<string> sss = s.SetContentsOfCell("A1499", "25.0");
            Assert.AreEqual(500, sss.Count);
            for (int i = 0; i < 500; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }

            sss = s.SetContentsOfCell("A1249", "25.0");
            Assert.AreEqual(250, sss.Count);
            for (int i = 0; i < 250; i++)
            {
                Assert.IsTrue(sss.Contains("A1" + i) || sss.Contains("a1" + i));
            }


        }

        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }

        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        // Test that GetCellValue throws an exception when name is null
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test51()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue(null);
        }

        // Test that GetCellValue throws an excetion when name is invalid
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test52()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue("A1B");
        }

        // Test GetCellValue
        [TestMethod]
        public void Test53()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "3.14");
            Assert.AreEqual(3.14, s.GetCellValue("a1"));
        }

        [TestMethod]
        public void Test54()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "dog");
            Assert.AreEqual("dog", s.GetCellValue("a1"));
        }

        [TestMethod]
        public void Test55()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=15");
            Assert.AreEqual(15d, s.GetCellValue("a1"));
        }

        [TestMethod]
        public void Test56()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            Assert.AreEqual(new FormulaError().GetType(), s.GetCellValue("a1").GetType());
            s.SetContentsOfCell("A2", "12");
            Assert.AreEqual(12d, s.GetCellValue("A1"));
            s.SetContentsOfCell("A2", "=A3");
            Assert.AreEqual(new FormulaError().GetType(), s.GetCellValue("a1").GetType());
            s.SetContentsOfCell("A3", "=A4 + A5");
            s.SetContentsOfCell("A4", "1");
            s.SetContentsOfCell("A5", "2");
            Assert.AreEqual(s.GetCellValue("A3"), s.GetCellValue("A1"));
            Assert.AreEqual(3d, s.GetCellValue("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Test57()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=");
        }

        // Test that cells with formulas are updating properly
        [TestMethod]
        public void Test58()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "1");
            s.SetContentsOfCell("A2", "=A1");
            s.SetContentsOfCell("A3", "=A2");
            Assert.AreEqual(1d, s.GetCellValue("A3"));
            s.SetContentsOfCell("A1", "");
            Assert.AreEqual(new FormulaError().GetType(), s.GetCellValue("A3").GetType());
            s.SetContentsOfCell("A1", "555");
            Assert.AreEqual(555d, s.GetCellValue("A3"));
            s.SetContentsOfCell("A3", "= A1 + A2");
            Assert.AreEqual(1110d, s.GetCellValue("A3"));
        }

        // Test that two arg constructor throws an exception when .xml is an invalid format
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test59()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidFormat1.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws an exception when .xml is an invalid format
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test60()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidFormat1.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws an exception when .xml contains an invalid formula
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test61()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidFormula.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws an exception when .xml contains an invalid regex
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test62()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidRegex.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws an exception when .xml contains repeated cells
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test63()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../RepeatCells.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws an exception when .xml contains a circular formula
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test64()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../CircularFormula.xml"), new Regex("^.*$"));
        }

        // Test that two arg constructor throws a VersionException
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetVersionException))]
        public void Test65()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../SampleSavedSpreadsheet.xml"), new Regex("^[A-Y][1-9]\\d*$"));
        }

        // Test that two arg constructor throws a VersionException when a formula cell name does not match new IsValid
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetVersionException))]
        public void Test66()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidFormulaVersion.xml"), new Regex("^[A-Y][1-9]\\d*$"));
        }

        // Test two arg constructor
        [TestMethod]
        public void Test67()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../SampleSavedSpreadsheet.xml"), new Regex("^.*$"));
            Assert.AreEqual(1.5d, s.GetCellContents("A1"));
            Assert.AreEqual(8.0d, s.GetCellContents("A2"));
            Assert.AreEqual(35d, s.GetCellValue("A3"));
            Assert.AreEqual("Hello", s.GetCellValue("Z2"));
        }

        // Test save
        [TestMethod]
        public void Test68()
        {
            Spreadsheet a = new Spreadsheet();
            a.SetContentsOfCell("A1", "hello");
            a.SetContentsOfCell("A2", "15");
            a.SetContentsOfCell("A3", "=Z15+B4");
            a.SetContentsOfCell("B4", "20");
            a.SetContentsOfCell("Z15", "10");
            StreamWriter sa = new StreamWriter("../../TesterFile.xml");
            a.Save(sa);
            sa.Close();
            StreamReader sb = new StreamReader("../../TesterFile.xml");
            Spreadsheet b = new Spreadsheet(sb, new Regex("^.*$"));
            sb.Close();
            Assert.AreEqual("hello", b.GetCellValue("A1"));
            Assert.AreEqual(15d, b.GetCellValue("A2"));
            Assert.AreEqual(30d, b.GetCellValue("A3"));
            int count = 0;
            List<string> list = new List<string>();
            foreach (string str in b.GetNamesOfAllNonemptyCells())
            {
                list.Add(str);
                count++;
            }
            Assert.AreEqual(5, list.Count);
            foreach (string str in a.GetNamesOfAllNonemptyCells())
            {
                Assert.AreEqual(true, list.Contains(str));
            }
        }

        // Test Changed
        [TestMethod]
        public void Test69()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(false, s.Changed);
            s.SetContentsOfCell("B1", "15");
            Assert.AreEqual(true, s.Changed);
            StreamWriter sa = new StreamWriter("../../TesterFile.xml");
            s.Save(sa);
            sa.Close();
            Assert.AreEqual(false, s.Changed);

            StreamReader sb = new StreamReader("../../TesterFile.xml");
            Spreadsheet b = new Spreadsheet(sb, new Regex("^.*$"));
            sb.Close();
            Assert.AreEqual(false, b.Changed);

            Spreadsheet c = new Spreadsheet(new Regex("^.*$"));
            Assert.AreEqual(false, c.Changed);
        }

        // Make sure cells which do not pass the old IsValid throw an exception
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Test70()
        {
            Spreadsheet s = new Spreadsheet(new StreamReader("../../InvalidOldName.xml"), new Regex("^.*$"));
        }


        public void RunRandomizedTest(int seed, int size)
        {
            AbstractSpreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }

    }
}
