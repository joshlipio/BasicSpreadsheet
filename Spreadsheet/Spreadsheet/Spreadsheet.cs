using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using Dependencies;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace SS
{
    /// <summary>
    /// A class which represents a spreadsheet in a spreadsheet program.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// The spreadsheet will be represented by a Dictionary, where the key is the cell's name and the value is the
        /// underlying cell object. When a cell is "filled," a new cell object will be placed in the dictionary. When
        /// the cell data is removed from the cell, it will be removed from the dictionary.
        /// A dependency graph keeps track of all the dependencies in the spreadsheet.
        /// It is created with regex expression which validates cell names.
        /// A private boolean value keeps track of whether the spreadsheet has been changed since it was created or last saved.
        /// </summary>
        private Dictionary<string, Cell> cells;
        private DependencyGraph dependencies;
        private Regex validator;
        private bool changed;

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet() : this(new Regex(".*?"))
        {
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        /// </summary>
        public Spreadsheet(Regex isValid)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            validator = isValid;
            changed = false;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newIsValid)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            validator = newIsValid;

            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add(null, "Spreadsheet.xsd");
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += delegate (object sender, ValidationEventArgs e) { throw new SpreadsheetReadException("Validation Error: " + e); };

            Regex oldIsValid = new Regex("");
            using (XmlReader reader = XmlReader.Create(source, settings))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                try
                                {
                                    oldIsValid = new Regex(reader["IsValid"]);
                                }
                                catch (Exception e)
                                {
                                    throw new SpreadsheetReadException(e.Message);
                                }
                                break;
                            case "cell":
                                if (cells.ContainsKey(reader["name"].ToUpper()))
                                    throw new SpreadsheetReadException("Duplicate cell name: " + reader["name"].ToUpper());
                                if (!oldIsValid.IsMatch(reader["name"]))
                                    throw new SpreadsheetReadException("According to the old IsValid regex, " + reader["name"] + " is not a valid cell name");
                                if (!validator.IsMatch(reader["name"]))
                                    throw new SpreadsheetVersionException("According to the new IsValid regex, " + reader["name"] + " is not a valid cell name");
                                ValidateVarsUsingOld(reader["contents"], oldIsValid);
                                try
                                {
                                    SetContentsOfCell(reader["name"], reader["contents"]);
                                }
                                catch (CircularException e)
                                {
                                    throw new SpreadsheetReadException(e.Message);
                                }
                                catch (Exception e)
                                {
                                    throw new SpreadsheetVersionException(e.Message);
                                }
                                break;
                        }
                    }
                }
            }
            changed = false;
        }

        /// <summary>
        /// Method used to ensure that all variables in a loading spreadsheet are valid according
        /// to that spreadsheet's supplied IsValid pattern. Throws a SpreadsheetReadException if
        /// any variables are not valid.
        /// </summary>
        private bool ValidateVarsUsingOld(string toValidate, Regex regex)
        {
            if (toValidate.Length < 2)
            {
                return true;
            }
            else if (toValidate[0] == '=')
            {
                Formula formula = new Formula(toValidate.Substring(1));
                foreach (string variable in formula.GetVariables())
                    if (!regex.IsMatch(variable))
                        throw new SpreadsheetReadException(variable + " is not a valid cell name according to the spreadsheet's IsValid");
            }
            return true;
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            using (XmlWriter writer = XmlWriter.Create(dest))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", validator.ToString());

                foreach (KeyValuePair<string, Cell> pair in cells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", pair.Key);
                    string contents = pair.Value.Contents.ToString();
                    contents = (pair.Value.Contents.GetType().Equals(typeof(Formula))) ? contents : contents;
                    writer.WriteAttributeString("contents", contents);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            changed = false;
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed {
            get {
                return changed;
            }

            protected set {
                changed = value;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name == null || !IsValid(name))
                throw new InvalidNameException();

            name = name.ToUpper();
            Cell toReturn;
            if (cells.TryGetValue(name, out toReturn))
                return toReturn.Value;
            else
                return "";
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (name == null || !IsValid(name))
                throw new InvalidNameException();
            if (content == null)
                throw new ArgumentNullException();

            name = name.ToUpper();
            double contentDouble;
            if (Double.TryParse(content, out contentDouble)) //double
            {
                return SetCellContents(name, contentDouble);
            }
            else if (content.Length == 0) //Empty string
            {
                return SetCellContents(name, content);
            }
            else if (content[0] == '=') //Formula
            {
                string formulaString;
                if (content.Length == 1)
                    formulaString = "";
                else
                    formulaString = content.Substring(1);
                Formula formula = new Formula(formulaString, s => s.ToUpper(), IsValid);
                return SetCellContents(name, formula);
            }
            else //Non-empty string
            {
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null || !IsValid(name))
                throw new InvalidNameException();
            name = name.ToUpper();
            if (cells.ContainsKey(name))
                return cells[name].Contents;
            else
                return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (KeyValuePair<string, Cell> cell in cells)
                yield return cell.Key;
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !IsValid(name))
                throw new InvalidNameException();
            foreach (string variable in formula.GetVariables())
            {
                if (!IsValid(variable))
                    throw new InvalidNameException();
            }
            AddCell(name, formula);
            HashSet<string> toReturn = new HashSet<string>();
            foreach (string variable in formula.GetVariables())
                dependencies.AddDependency(variable, name);
            toReturn.Add(name);
            GetAllDependents(toReturn, name);
            return toReturn;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            name = name.ToUpper();
            AddCell(name, text);
            HashSet<string> toReturn = new HashSet<string>();
            toReturn.Add(name);
            GetAllDependents(toReturn, name);
            return toReturn;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            name = name.ToUpper();
            AddCell(name, number);
            HashSet<string> toReturn = new HashSet<string>();
            toReturn.Add(name);
            GetAllDependents(toReturn, name);
            return toReturn;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
                throw new ArgumentNullException();
            if (!IsValid(name))
                throw new InvalidNameException();

            return dependencies.GetDependents(name.ToUpper());
        }

        /// <summary>
        /// Recursive function which searches down the dependency graph, returning all the
        /// cells which are dependent on cellName.
        /// </summary>
        private void GetAllDependents(HashSet<string> list, string cellName)
        {
            foreach (string str in dependencies.GetDependents(cellName))
            {
                list.Add(str);
                GetAllDependents(list, str);
            }
        }

        /// <summary>
        /// Called by the SetCellContents functions.
        /// Decides if a new cell needs to be made, adds it to the dictionary, then updates its contents.
        /// If the cell's contents are an empty string, removes it from the dictionary.
        /// </summary>
        private void AddCell(string name, object filler)
        {
            Cell tempCell;
            if (cells.TryGetValue(name, out tempCell))
            {
                if (filler.Equals(""))
                {
                    cells.Remove(name);
                }
                else
                {
                    tempCell.Contents = filler;
                }
                // Remove all old dependencies associated with this cell
                List<string> dependees = new List<string>();
                foreach (string dependee in dependencies.GetDependees(name))
                    dependees.Add(dependee);
                foreach (string dependee in dependees)
                    dependencies.RemoveDependency(dependee, name);
            }
            else if (!filler.Equals(""))
            {
                cells.Add(name, new Cell(filler));
            }
            if (filler.GetType().Equals(typeof(Formula)))
            {
                Formula formula = (Formula)filler;
                foreach (string str in formula.GetVariables())
                {
                    string dependee = str.ToUpper();
                    dependencies.AddDependency(dependee, name);
                }
            }

            // Assign values
            object oldValue = GetCellValue(name);
            DetermineCellValue(name);
            try
            {
                foreach (string toRecalc in GetCellsToRecalculate(name))
                    DetermineCellValue(toRecalc);
            }
            catch (CircularException e) // Reset to old value if the new formula results in a CircularException
            {
                AddCell(name, oldValue);
                throw e;
            }
            changed = true;
        }

        /// <summary>
        /// Sets the specified cell's value based on its contents
        /// </summary>
        private void DetermineCellValue(string name)
        {
            Cell cell;
            if (!cells.TryGetValue(name, out cell))
                return;

            if (cell.Contents.GetType().Equals(typeof(Formula)))
            {
                Formula formula = (Formula)cell.Contents;
                try
                {
                    cell.Value = formula.Evaluate(LookupCellValue);
                }
                catch (FormulaEvaluationException e)
                {
                    cell.Value = new FormulaError("The value of cell " + e + " is not a double");
                }
            }
            else
            {
                cell.Value = cell.Contents;
            }
        }

        /// <summary>
        /// Lookup delegate for this class.
        /// Throws an UndefinedVariableException if the cell with the given name's value
        /// is not a double.
        /// </summary>
        private double LookupCellValue(string name)
        {
            object value = GetCellValue(name);
            if (value.GetType().Equals(typeof(double)))
            {
                return (double)value;
            }
            else
            {
                throw new UndefinedVariableException(name);
            }
        }

        /// <summary>
        /// Returns true if "name" is a valid name for a spreadsheet cell, or false otherwise.
        /// </summary>
        private bool IsValid(string name)
        {
            Regex pattern = new Regex("\\A[a-zA-Z]+[1-9]\\d*$");
            return pattern.IsMatch(name) && validator.IsMatch(name.ToUpper());
        }
    }

    /// <summary>
    /// Represents a cell in the spreadsheet. Keeps track of that cell's name, contents, and value.
    /// Also holds a reference to the dictionary so it can calculate its value if it is a formula.
    /// </summary>
    class Cell
    {
        // Stores the cell's contents
        public object _contents;

        // Stores the cell's value
        public object _value;

        /// <summary>
        /// Creates the cell with the specified contents.
        /// Defaults the value of this cell to an empty string.
        /// </summary>
        public Cell(object contents)
        {
            _contents = contents;
            _value = "";
        }

        /// <summary>
        /// The contents of this cell.
        /// </summary>
        public object Contents {
            get {
                return _contents;
            }
            set {
                _contents = value;
            }
        }

        /// <summary>
        /// The value of this cell.
        /// </summary>
        public object Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
    }
}
