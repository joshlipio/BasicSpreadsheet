// Written by Elliot Lee to get a good grade in CS 3500, January 2017

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    [TestClass]
    public class UnitTest1
    {
        Lookup lookup = (string a) => 0d;

        public static double ThrowException(string message)
        {
            if (true)
                throw new UndefinedVariableException("'" + message + "' is not defined");
            return 0d;
        }
        Lookup emptyLookup = ThrowException;

        [TestMethod]
        public void TestEvaluate1()
        {
            Formula f = new Formula("1 + 1");
            Assert.AreEqual(2d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate2()
        {
            Formula f = new Formula("1 - 1");
            Assert.AreEqual(0d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate3()
        {
            Formula f = new Formula("4 / 2");
            Assert.AreEqual(2d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate4()
        {
            Formula f = new Formula("4 * 2");
            Assert.AreEqual(8d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate5()
        {
            Formula f = new Formula("(4 + 2) / 3");
            Assert.AreEqual(2d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate6()
        {
            Formula f = new Formula("(((5 + 1) * 3) / 2) - 1");
            Assert.AreEqual(8d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate7()
        {
            Formula f = new Formula("duck / 2");
            Assert.AreEqual(0d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate8()
        {
            Formula f = new Formula("((5 - 1) * 2) / 4");
            Assert.AreEqual(2d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate9()
        {
            Formula f = new Formula("8 / (2 * (1 + 1))");
            Assert.AreEqual(2d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate10()
        {
            Formula f = new Formula("(5)");
            Assert.AreEqual(5d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestEvaluate11()
        {
            Formula f = new Formula("1.0 + 2.0");
            Assert.AreEqual(3d, f.Evaluate(lookup));
        }

        [TestMethod]
        public void TestDivideByZero()
        {
            try
            {
                Formula f = new Formula("1 / 0");
                f.Evaluate(lookup);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error: Divide by 0", e.Message);
            }
        }

        [TestMethod]
        public void TestUndefinedVariable()
        {
            try
            {
                Formula f = new Formula("duck + 2");
                f.Evaluate(emptyLookup);
            }
            catch (Exception e)
            {
                Assert.AreEqual("'duck' is undefined", e.Message);
            }
        }
    }
}
