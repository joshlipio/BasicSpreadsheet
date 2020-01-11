// Skeleton written by Joe Zachary for CS 3500, January 2017
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules. Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        private List<string> allTokens;

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// Throws an ArgumentNullException if the formula string is null.
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a new Formula from the given formula string using the specified Normalizer delegate to convert variables into a canonical form
        /// and the specified Validator delegate to impose extra restrictions on teh validity of a variable beyond the basic requirements of the Formula object.
        /// Throws an ArgumentNullException if any argument is null.
        /// </summary>
        public Formula(String formula, Normalizer N, Validator V)
        {
            if (formula == null)
            {
                throw new ArgumentNullException("The formula string cannot be null");
            }
            if (N == null)
            {
                throw new ArgumentNullException("The Normalizer delegate cannot be null");
            }
            if (V == null)
            {
                throw new ArgumentNullException("The Validator deligate cannot be null");
            }

            allTokens = new List<string>();
            int oldCount = 0;
            int openingParens = 0;
            int closingParens = 0;
            bool validClosingToken = false;
            foreach (string token in GetTokens(formula))
            {
                validClosingToken = false;
                double num;
                if (Double.TryParse(token, out num)) // Token is a double
                {
                    if (num >= 0)
                    {
                        allTokens.Add(token);
                        validClosingToken = true;
                    }
                    else
                    {
                        throw new FormulaFormatException(token + " is less than zero");
                    }
                }
                else if (IsLetter(token[0])) // Token is a variable
                {
                    string temp = N(token);
                    if (!IsLetter(temp[0]))
                        throw new FormulaFormatException("The Normalizer delegate returned an illegal variable name");
                    if (!V(temp))
                        throw new FormulaFormatException("'" + token + "' is not a valid name");
                    allTokens.Add(temp);
                    validClosingToken = true;
                }
                else // Token is an operator
                {
                    switch (token)
                    {
                        case "+":
                            allTokens.Add(token);
                            break;
                        case "-":
                            allTokens.Add(token);
                            break;
                        case "*":
                            allTokens.Add(token);
                            break;
                        case "/":
                            allTokens.Add(token);
                            break;
                        case "(":
                            openingParens++;
                            allTokens.Add(token);
                            break;
                        case ")":
                            closingParens++;
                            if (closingParens > openingParens)
                                throw new FormulaFormatException("Unexpected closing parenthesis: ')'");
                            allTokens.Add(token);
                            validClosingToken = true;
                            break;
                    }
                }
                if (allTokens.Count < 1 || allTokens.Count == oldCount) // Invalid token
                    throw new FormulaFormatException("'" + token + "' is not a valid token");
                oldCount = allTokens.Count;

                if (allTokens.Count > 1)
                {
                    string current = allTokens[allTokens.Count - 1];
                    string prev = allTokens[allTokens.Count - 2];
                    if (prev == "(" || prev == "+" || prev == "-" || prev == "*" || prev == "/")
                    {
                        if (!IsLetter(current[0]) && !IsNumber(current) && current != "(")
                            throw new FormulaFormatException(current + " cannot follow a " + prev + " token");
                    }
                    else if (IsLetter(prev[0]) || IsNumber(prev) || prev == ")")
                    {
                        if (current != ")" && current != "+" && current != "-" && current != "*" && current != "/")
                            throw new FormulaFormatException(current + " cannot follow a " + prev + " token");
                    }
                }
                else if (allTokens.Count == 1)
                {
                    if (!IsNumber(allTokens[0]) && !IsLetter(allTokens[0][0]) && allTokens[0] != "(")
                        throw new FormulaFormatException("The first token of a formula must be a number, variable, or opening parenthesis");
                }
            }

            if (allTokens.Count == 0)
                throw new FormulaFormatException("There must be at least one token in the formula");
            if (openingParens > closingParens)
                throw new FormulaFormatException("Missing closing parenthesis: ')'");
            if (!validClosingToken)
                throw new FormulaFormatException("'" + allTokens[allTokens.Count - 1] + "' is an invalid final token");
        }

        /// <summary>
        /// Returns a set of strings that contains each distinct variable (in normalized form) that appears in the formula
        /// </summary>
        public ISet<string> GetVariables()
        {
            HashSet<string> toReturn = new HashSet<string>();

            if (allTokens == null)
                return toReturn;

            foreach (string token in allTokens)
            {
                if (IsLetter(token[0]))
                    toReturn.Add(token);
            }
            return toReturn;
        }

        /// <summary>
        /// Returns a string representation of the Formula (in normalized form), formatted to be readable
        /// </summary>
        public override string ToString()
        {
            if (allTokens == null)
                return "=0";

            string toReturn = "=";
            for (int i = 0; i < allTokens.Count; i++)
            {
                if (allTokens[i] == "(")
                {
                    toReturn += "(";
                }
                else if (IsNumber(allTokens[i]) || IsLetter(allTokens[i][0]) || allTokens[i] == ")")
                {
                    if (CheckNext(i) == ")")
                        toReturn += allTokens[i];
                    else
                        toReturn += allTokens[i] + " ";
                }
                else if (allTokens[i] == "+" || allTokens[i] == "-" || allTokens[i] == "*" || allTokens[i] == "/")
                {
                    toReturn += allTokens[i] + " ";
                }
            }
            toReturn = toReturn.Trim(new char[] { ' ' });

            return toReturn;
        }

        /// <summary>
        /// Helper method for the ToString method. Returns the string in allTokens after "index."
        /// If "index" is the last item in the List, returns null.
        /// </summary>
        /// <returns></returns>
        private string CheckNext(int index)
        {
            try
            {
                return allTokens[index + 1];
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// 
        /// Throws an ArgumentNullException if the lookup argument is null.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            if (lookup == null)
                throw new ArgumentNullException("Lookup delegate cannot be null");
            if (allTokens == null)
                return 0d;

            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            foreach (string token in allTokens)
            {
                if (IsNumber(token)) // Next token is a number
                {
                    ProcessValue(Double.Parse(token), values, operators);
                }
                else if (IsLetter(token[0])) // Next token is a variable
                {
                    try
                    {
                        ProcessValue(lookup(token), values, operators);
                    }
                    catch (UndefinedVariableException e)
                    {
                        throw new FormulaEvaluationException(token);
                    }
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == "+" || token == "-")
                {
                    ProcessAddOrSub(token, values, operators);
                }
                else if (token == "*" || token == "/")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    ProcessClosedParen(token, values, operators);
                }
            }

            if (operators.Count == 0)
            {
                return values.Pop();
            }
            else
            {
                ProcessAddOrSub("+", values, operators);
                return values.Pop();
            }
        }

        /// <summary>
        /// Takes a double and decides what to do with it based on the state of the operator stack
        /// </summary>
        private void ProcessValue(double val, Stack<double> values, Stack<string> operators)
        {
            double result = val;
            if (operators.Count > 0)
            {
                if (operators.Peek() == "*")
                {
                    operators.Pop();
                    result = val * values.Pop();
                }
                else if (operators.Peek() == "/")
                {
                    if (val == 0)
                        throw new FormulaEvaluationException("Error: Divide by 0");
                    operators.Pop();
                    result = values.Pop() / val;
                }
            }
            values.Push(result);
        }

        /// <summary>
        /// Takes an addition or subtraction operator and decides what to do with it based on the operator stack
        /// </summary>
        private void ProcessAddOrSub(string op, Stack<double> values, Stack<string> operators)
        {
            if (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
            {
                double a = values.Pop();
                double b = values.Pop();
                if (operators.Pop() == "+")
                    values.Push(a + b);
                else
                    values.Push(b - a);
            }
            operators.Push(op);
        }

        /// <summary>
        /// Decides what to do when the Evaluate method runs into a closed parenthesis based on the state of the operator stack
        /// </summary>
        private void ProcessClosedParen(string op, Stack<double> values, Stack<string> operators)
        {
            if (operators.Count > 0 && (operators.Peek() == "+" || operators.Peek() == "-"))
            {
                double a = values.Pop();
                double b = values.Pop();
                if (operators.Pop() == "+")
                    values.Push(a + b);
                else
                    values.Push(b - a);
            }
            operators.Pop();
            if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
            {
                double a = values.Pop();
                double b = values.Pop();
                if (operators.Pop() == "*")
                {
                    values.Push(a * b);
                }
                else
                {
                    if (a == 0)
                        throw new FormulaEvaluationException("Error: Divide by 0");
                    values.Push(b / a);
                }
            }
        }

        /// <summary>
        /// Returns true if the supplied character is a letter
        /// </summary>
        private static bool IsLetter(char c)
        {
            char upper = Char.ToUpper(c);
            char lower = Char.ToLower(c);
            return upper != lower;
        }

        /// <summary>
        /// Returns true if the supplied character is a number
        /// </summary>
        private static bool IsNumber(string s)
        {
            double temp;
            return Double.TryParse(s, out temp);
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// A Normalizer method is one that converts variables into a canonical form.
    /// What that canonical form is is up to the implementation of the method.
    /// </summary>
    public delegate string Normalizer(string s);

    /// <summary>
    /// A Validator method is one which imposes extra restrictions on the validity of a variable
    /// beyond the ones already built into the Formula definition. Returns false if the supplied
    /// variable does not meet the Validator's restrictions.
    /// </summary>
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
