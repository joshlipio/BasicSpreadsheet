// Unit tests written by Elliot Lee to get a good grade in CS 3500, February 2017

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    /// <summary>
    /// Class for testing the DependencyGraph object
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        // Large graph for stress tests
        public DependencyGraph giant = new DependencyGraph();

        // Make sure the constructor doesn't throw any errors
        [TestMethod]
        public void TestConstructorBlank()
        {
            DependencyGraph g = new DependencyGraph();
        }

        // Check that AddDependency fails when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDependencyFail1()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency(null, "t");
        }

        // Check that AddDependency fails when t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDependencyFail2()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", null);
        }

        // Check that AddDependency fails when s and t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDependencyFail3()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency(null, null);
        }

        // Check that AddDependency works as specified
        [TestMethod]
        public void TestAddDependency()
        {
            DependencyGraph g = new DependencyGraph();
            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < 100000; i++)
            {
                g.AddDependency("s", i + "");
                set.Add(i + "");
            }
            foreach (string t in g.GetDependents("s"))
                Assert.AreEqual(true, set.Contains(t));
        }

        // Check that RemoveDependency works when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveDependencyFail1()
        {
            DependencyGraph g = new DependencyGraph();
            g.RemoveDependency(null, "t");
        }

        // Check that RemoveDependency works when t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveDependencyFail2()
        {
            DependencyGraph g = new DependencyGraph();
            g.RemoveDependency("s", null);
        }

        // Check that RemoveDependency works when s and t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveDependencyFail3()
        {
            DependencyGraph g = new DependencyGraph();
            g.RemoveDependency(null, null);
        }

        // Check that RemoveDependency doesn't throw an exception when s doesn't exist in the graph
        [TestMethod]
        public void TestRemoveDependencyDNES()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            g.RemoveDependency("a", "t");
        }

        // Check that RemoveDependency doesn't throw an exception when t doesn't exist in the graph
        [TestMethod]
        public void TestRemoveDependencyDNET()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            g.RemoveDependency("s", "b");
        }

        // Make sure RemoveDependency works on a string of dependencies
        [TestMethod]
        public void TestRemoveDependencyString()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("a", "b");
            g.AddDependency("b", "c");
            g.AddDependency("c", "d");
            g.RemoveDependency("b", "c");
            Assert.AreEqual(true, g.HasDependents("a"));
            Assert.AreEqual(false, g.HasDependees("c"));
            Assert.AreEqual(true, g.HasDependees("b"));
            Assert.AreEqual(false, g.HasDependents("b"));
            g.RemoveDependency("c", "d");
        }

        // Check that HasDependents throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependentsNullFail()
        {
            DependencyGraph g = new DependencyGraph();
            g.HasDependents(null);
        }

        // Check that HasDependents returns false when there are no dependents
        [TestMethod]
        public void TestHasDependentsZero()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            Assert.AreEqual(false, g.HasDependents("t"));
        }

        // Check that HasDependents returns true when there is a dependent
        [TestMethod]
        public void TestHasDependentsOne()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            Assert.AreEqual(true, g.HasDependents("s"));
        }

        // Check that HasDependents returns false after the only dependency has been removed
        [TestMethod]
        public void TestHasDependentsRemoved()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            g.RemoveDependency("s", "t");
            Assert.AreEqual(false, g.HasDependents("s"));
        }

        // Make sure Size works as it should
        [TestMethod]
        public void TestSize1()
        {
            DependencyGraph g = new DependencyGraph();
            Assert.AreEqual(0, g.Size);
            g.AddDependency("s", "1");
            g.AddDependency("a", "1");
            g.AddDependency("q", "1");
            g.AddDependency("f", "1");
            g.AddDependency("d", "1");
            g.AddDependency("butter", "1");
            g.AddDependency("flapjack", "1");
            g.AddDependency("austria", "1");
            Assert.AreEqual(8, g.Size);
            g.RemoveDependency("s", "1");
            Assert.AreEqual(7, g.Size);
            g.RemoveDependency("butter", "7");
            Assert.AreEqual(7, g.Size);
            g.AddDependency("s", "1");
            Assert.AreEqual(8, g.Size);
        }

        // Stress test on AddDependency and RemoveDependency. Should run relatively quickly
        [TestMethod]
        public void TestAddAndRemoveStress()
        {
            DependencyGraph g = new DependencyGraph();
            for (int i = 1; i < 100000; i++)
            {
                g.AddDependency(i + "", (i + i) + "");
            }
            for (int i = 1; i < 100000; i++)
            {
                g.RemoveDependency(i + "", (i + i) + "");
            }
        }

        // Check that HasDependees throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHasDependeesNullFail()
        {
            DependencyGraph g = new DependencyGraph();
            g.HasDependees(null);
        }

        // Check that HasDependees returns false when there are no dependees
        [TestMethod]
        public void TestHasDependeesFalse()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            Assert.AreEqual(false, g.HasDependees("s"));
        }

        // Check that HasDependees returns true when there are dependees
        [TestMethod]
        public void TestHasDependeesTrue()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            Assert.AreEqual(true, g.HasDependees("t"));
        }

        // Check that HasDependees returns false when the dependency has been removed
        [TestMethod]
        public void TestHasDependeesRemoved()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "t");
            g.RemoveDependency("s", "t");
            Assert.AreEqual(false, g.HasDependees("t"));
        }

        // Check that GetDependents throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependents()
        {
            DependencyGraph g = new DependencyGraph();
            foreach (string s in g.GetDependents(null))
                continue;
        }

        // Check that GetDependents doesn't throw an exception when s is not in the graph
        [TestMethod]
        public void TestGetDependentsDNE()
        {
            DependencyGraph g = new DependencyGraph();
            foreach (string s in g.GetDependents("s"))
                Assert.Fail();
        }

        // Check that GetDependees throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDependeesNullFail()
        {
            DependencyGraph g = new DependencyGraph();
            string k;
            foreach (string s in g.GetDependees(null))
                k = s;
        }

        // Check that GetDependees works if s is not in the graph
        [TestMethod]
        public void TestGetDependeesDNE()
        {
            DependencyGraph g = new DependencyGraph();
            g.GetDependees("s");
        }

        // Check that GetDependees works as expected
        [TestMethod]
        public void TestGetDependees()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("a", "b");
            g.AddDependency("c", "b");
            g.AddDependency("d", "b");
            List<string> dependees = new List<string>();
            dependees.Add("a");
            dependees.Add("c");
            dependees.Add("d");
            foreach (string s in g.GetDependees("b"))
                Assert.AreEqual(true, dependees.Contains(s));
            g.RemoveDependency("d", "b");
            dependees.Remove("d");
            foreach (string s in g.GetDependees("b"))
                Assert.AreEqual(true, dependees.Contains(s));
        }

        // Check that ReplaceDependents throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceDependentsSNull()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependents(null, new string[] { "t" });
        }

        // Check that ReplaceDependents throws an exception when a t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceDependentsTNull()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependents("s", new string[] { "t", null, "b" });
        }

        // Check that ReplaceDependents works as specified
        [TestMethod]
        public void TestReplaceDependents()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("s", "a");
            g.AddDependency("s", "b");
            g.AddDependency("g", "a");
            g.ReplaceDependents("s", new string[] { "x", "y", "z" });
            List<string> newDependents = new List<string>();
            foreach (string s in g.GetDependents("s"))
                newDependents.Add(s);
            Assert.AreEqual(false, newDependents.Contains("a"));
            Assert.AreEqual(false, newDependents.Contains("b"));
            Assert.AreEqual(true, newDependents.Contains("x"));
            Assert.AreEqual(true, newDependents.Contains("y"));
            Assert.AreEqual(true, newDependents.Contains("z"));
            foreach (string s in g.GetDependees("a"))
                Assert.AreEqual("g", s);
        }

        // Check that ReplaceDependents works when s is not in the graph
        [TestMethod]
        public void TestReplaceDependentsDNE()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependents("s", new string[] { "t" });
            List<string> list = new List<string>();
            foreach (string s in g.GetDependents("s"))
                list.Add(s);
            Assert.AreEqual("t", list[0]);
        }

        // Check that ReplaceDependees works when s is not in the graph
        [TestMethod]
        public void TestReplaceDependeesDNE()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependees("s", new string[] { "t" });
            List<string> list = new List<string>();
            foreach (string s in g.GetDependees("s"))
                list.Add(s);
            Assert.AreEqual("t", list[0]);
        }

        // Check that ReplaceDependees throws an exception when s == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceDependeesSNull()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependees(null, new string[] { "t" });
        }

        // Check that ReplaceDependees throws an exception when a t == null
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReplaceDependeesTNull()
        {
            DependencyGraph g = new DependencyGraph();
            g.ReplaceDependees("s", new string[] { "t", null, "b" });
        }

        // Check that ReplaceDependees works as specified
        [TestMethod]
        public void TestReplaceDependees()
        {
            DependencyGraph g = new DependencyGraph();
            g.AddDependency("a", "s");
            g.AddDependency("b", "s");
            g.AddDependency("a", "q");
            g.ReplaceDependees("s", new string[] { "x", "y", "z" });
            List<string> newDependees = new List<string>();
            foreach (string s in g.GetDependees("s"))
                newDependees.Add(s);
            Assert.AreEqual(false, newDependees.Contains("a"));
            Assert.AreEqual(false, newDependees.Contains("b"));
            Assert.AreEqual(true, newDependees.Contains("x"));
            Assert.AreEqual(true, newDependees.Contains("y"));
            Assert.AreEqual(true, newDependees.Contains("z"));
            foreach (string s in g.GetDependees("a"))
                Assert.AreEqual("q", s);
        }

        // Initialize the giant graph
        [TestInitialize]
        public void InitBig()
        {
            for (int i = 0; i < 25000; i++)
            {
                giant.AddDependency("a", i + "");
                giant.AddDependency("b", i + "");
                giant.AddDependency("c", (-i) + "");
                giant.AddDependency(i + "", "d");
            }
        }

        // Test size on giant
        [TestMethod]
        public void GiantSize()
        {
            Assert.AreEqual(100000, giant.Size);
        }

        // Test HasDependents on giant
        [TestMethod]
        public void GiantHasDependents()
        {
            Assert.AreEqual(true, giant.HasDependents("2345"));
        }

        // Test HasDependees on giant
        [TestMethod]
        public void GiantHasDependees()
        {
            Assert.AreEqual(true, giant.HasDependees("42"));
        }

        // Test GetDependents on giant
        [TestMethod]
        public void GiantGetDependents()
        {
            int num = 0;
            foreach (string s in giant.GetDependents("a"))
                num++;
            Assert.AreEqual(25000, num);
        }

        // Test GetDependees on giant
        [TestMethod]
        public void GiantGetDependees()
        {
            int num = 0;
            foreach (string s in giant.GetDependees("d"))
                num++;
            Assert.AreEqual(25000, num);
        }

        // Test AddDependency on giant
        [TestMethod]
        public void GiantAddDependency()
        {
            giant.AddDependency("a", "potato");
            Assert.AreEqual(true, giant.HasDependees("potato"));
        }

        // Test RemoveDependency on giant
        [TestMethod]
        public void GiantRemoveDependency()
        {
            giant.RemoveDependency("a", "potato");
            Assert.AreEqual(false, giant.HasDependees("potato"));
        }

        // Test ReplaceDependents on giant
        [TestMethod]
        public void GiantReplaceDependents()
        {
            giant.ReplaceDependents("a", new string[] { "watermelon seeds" });
            string w = "pomegranite";
            foreach (string s in giant.GetDependents("a"))
                w = s;
            Assert.AreEqual("watermelon seeds", w);
        }

        // Test ReplaceDependees on giant
        [TestMethod]
        public void GiantReplaceDependees()
        {
            giant.ReplaceDependees("d", new string[] { "potato" });
            string w = "sasafrass";
            foreach (string s in giant.GetDependees("d"))
                w = s;
            Assert.AreEqual("potato", w);
        }
    }
}
