// Written by Joe Zachary for CS 3500, January 2017.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {

        [TestMethod]
        public void Construct1()
        {
            try
            {
                Formula f = new Formula("_");
            }
            catch (Exception e)
            {
                Assert.AreEqual("'_' is not a valid token", e.Message);
            }
        }

        [TestMethod]
        public void Construct2()
        {
            try
            {
                Formula f = new Formula("");
            }
            catch (Exception e)
            {
                Assert.AreEqual("There must be at least one token in the formula", e.Message);
            }
        }

        [TestMethod]
        public void Construct3()
        {
            try
            {
                Formula f = new Formula(" ");
            }
            catch (Exception e)
            {
                Assert.AreEqual("There must be at least one token in the formula", e.Message);
            }
        }

        [TestMethod]
        public void Construct4()
        {
            try
            {
                Formula f = new Formula("(3 + 4) )");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Unexpected closing parenthesis: ')'", e.Message);
            }
        }

        [TestMethod]
        public void Construct5()
        {
            try
            {
                Formula f = new Formula("((3 + 4)");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Missing closing parenthesis: ')'", e.Message);
            }
        }

        [TestMethod]
        public void Construct6()
        {
            try
            {
                Formula f = new Formula("-3 + 4)");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The first token of a formula must be a number, variable, or opening parenthesis", e.Message);
            }
        }

        [TestMethod]
        public void Construct7()
        {
            try
            {
                Formula f = new Formula("+3 + 4)");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The first token of a formula must be a number, variable, or opening parenthesis", e.Message);
            }
        }

        [TestMethod]
        public void Construct8()
        {
            try
            {
                Formula f = new Formula("duck + 4/");
            }
            catch (Exception e)
            {
                Assert.AreEqual("'/' is an invalid final token", e.Message);
            }
        }

        [TestMethod]
        public void Construct9()
        {
            try
            {
                Formula f = new Formula("d123 + 4/");
            }
            catch (Exception e)
            {
                Assert.AreEqual("'/' is an invalid final token", e.Message);
            }
        }

        /// <summary>
        /// This tests that a syntactically incorrect parameter to Formula results
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct10()
        {
            Formula f = new Formula("_");
        }

        /// <summary>
        /// This is another syntax error
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct11()
        {
            Formula f = new Formula("2++3");
        }

        /// <summary>
        /// Another syntax error.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void Construct12()
        {
            Formula f = new Formula("2 3");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            Assert.AreEqual(f.Evaluate(Lookup4), 20.0, 1e-6);
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
