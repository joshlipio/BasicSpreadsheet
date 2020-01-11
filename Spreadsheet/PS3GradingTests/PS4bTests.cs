using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace GradingTests
{
    [TestClass]
    public class PS4bTests
    {

        /// <summary>
        ///This is a test class for DependencyGraphTest and is intended
        ///to contain all DependencyGraphTest Unit Tests
        ///</summary>
        [TestClass()]
        public class DependencyGraphTest
        {
            // ************************** TESTS ON EMPTY DGs ************************* //

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest1()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.AreEqual(0, t.Size);
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest2()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.IsFalse(t.HasDependees("x"));
                Assert.IsFalse(t.HasDependents("x"));
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest3()
            {
                DependencyGraph t = new DependencyGraph();
                Assert.IsFalse(t.GetDependees("x").GetEnumerator().MoveNext());
                Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
            }

            /// <summary>
            ///Removing from an empty DG shouldn't fail
            ///</summary>
            [TestMethod()]
            public void EmptyTest5()
            {
                DependencyGraph t = new DependencyGraph();
                t.RemoveDependency("x", "y");
            }

            /// <summary>
            ///Replace on an empty DG shouldn't fail
            ///</summary>
            [TestMethod()]
            public void EmptyTest6()
            {
                DependencyGraph t = new DependencyGraph();
                t.ReplaceDependents("x", new HashSet<string>());
                t.ReplaceDependees("y", new HashSet<string>());
            }


            // ************************ MORE TESTS ON EMPTY DGs *********************** //

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest7()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(1, t.Size);
                t.RemoveDependency("x", "y");
                Assert.AreEqual(0, t.Size);
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest8()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.IsTrue(t.HasDependees("y"));
                Assert.IsTrue(t.HasDependents("x"));
                t.RemoveDependency("x", "y");
                Assert.IsFalse(t.HasDependees("y"));
                Assert.IsFalse(t.HasDependents("x"));
            }

            /// <summary>
            ///Empty graph should contain nothing
            ///</summary>
            [TestMethod()]
            public void EmptyTest9()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
                Assert.IsTrue(e1.MoveNext());
                Assert.AreEqual("x", e1.Current);
                IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
                Assert.IsTrue(e2.MoveNext());
                Assert.AreEqual("y", e2.Current);
                t.RemoveDependency("x", "y");
                Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
                Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
            }

            /// <summary>
            ///Removing from an empty DG shouldn't fail
            ///</summary>
            [TestMethod()]
            public void EmptyTest11()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(t.Size, 1);
                t.RemoveDependency("x", "y");
                t.RemoveDependency("x", "y");
            }

            /// <summary>
            ///Replace on an empty DG shouldn't fail
            ///</summary>
            [TestMethod()]
            public void EmptyTest12()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                Assert.AreEqual(t.Size, 1);
                t.RemoveDependency("x", "y");
                t.ReplaceDependents("x", new HashSet<string>());
                t.ReplaceDependees("y", new HashSet<string>());
            }


            // ********************** Making Sure that Static Variables Weren't Used ****************** //
            ///<summary>
            ///It should be possibe to have more than one DG at a time.  This test is
            ///repeated because I want it to be worth more than 1 point.
            ///</summary>
            [TestMethod()]
            public void StaticTest1()
            {
                DependencyGraph t1 = new DependencyGraph();
                DependencyGraph t2 = new DependencyGraph();
                t1.AddDependency("x", "y");
                Assert.AreEqual(1, t1.Size);
                Assert.AreEqual(0, t2.Size);
            }

            [TestMethod()]
            public void StaticTest2()
            {
                StaticTest1();
            }

            [TestMethod()]
            public void StaticTest3()
            {
                StaticTest1();
            }

            [TestMethod()]
            public void StaticTest4()
            {
                StaticTest1();
            }

            [TestMethod()]
            public void StaticTest5()
            {
                StaticTest1();
            }

            /**************************** SIMPLE NON-EMPTY TESTS ****************************/

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest1()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest3()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest4()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");

                IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

                e = t.GetDependees("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("a", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("d").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest5()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest6()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest8()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest9()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");

                IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

                e = t.GetDependees("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("a", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("d").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest10()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "b");
                t.AddDependency("c", "b");
                t.AddDependency("b", "d");
                t.AddDependency("c", "b");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest11()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest13()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest14()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");

                IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

                e = t.GetDependees("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("a", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("d").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest15()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "y");
                t.AddDependency("a", "b");
                t.AddDependency("a", "c");
                t.AddDependency("a", "d");
                t.AddDependency("c", "b");
                t.RemoveDependency("a", "d");
                t.AddDependency("e", "b");
                t.AddDependency("b", "d");
                t.RemoveDependency("e", "b");
                t.RemoveDependency("x", "y");

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest16()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "b");
                t.AddDependency("a", "z");
                t.ReplaceDependents("b", new HashSet<string>());
                t.AddDependency("y", "b");
                t.ReplaceDependents("a", new HashSet<string>() { "c" });
                t.AddDependency("w", "d");
                t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
                t.ReplaceDependees("d", new HashSet<string>() { "b" });
                Assert.AreEqual(4, t.Size);
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest18()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "b");
                t.AddDependency("a", "z");
                t.ReplaceDependents("b", new HashSet<string>());
                t.AddDependency("y", "b");
                t.ReplaceDependents("a", new HashSet<string>() { "c" });
                t.AddDependency("w", "d");
                t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
                t.ReplaceDependees("d", new HashSet<string>() { "b" });
                Assert.IsTrue(t.HasDependents("a"));
                Assert.IsFalse(t.HasDependees("a"));
                Assert.IsTrue(t.HasDependents("b"));
                Assert.IsTrue(t.HasDependees("b"));
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest19()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "b");
                t.AddDependency("a", "z");
                t.ReplaceDependents("b", new HashSet<string>());
                t.AddDependency("y", "b");
                t.ReplaceDependents("a", new HashSet<string>() { "c" });
                t.AddDependency("w", "d");
                t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
                t.ReplaceDependees("d", new HashSet<string>() { "b" });

                IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

                e = t.GetDependees("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("a", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependees("d").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());
            }

            /// <summary>
            ///Non-empty graph contains something
            ///</summary>
            [TestMethod()]
            public void NonEmptyTest20()
            {
                DependencyGraph t = new DependencyGraph();
                t.AddDependency("x", "b");
                t.AddDependency("a", "z");
                t.ReplaceDependents("b", new HashSet<string>());
                t.AddDependency("y", "b");
                t.ReplaceDependents("a", new HashSet<string>() { "c" });
                t.AddDependency("w", "d");
                t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
                t.ReplaceDependees("d", new HashSet<string>() { "b" });

                IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                String s1 = e.Current;
                Assert.IsTrue(e.MoveNext());
                String s2 = e.Current;
                Assert.IsFalse(e.MoveNext());
                Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

                e = t.GetDependents("b").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("d", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("c").GetEnumerator();
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual("b", e.Current);
                Assert.IsFalse(e.MoveNext());

                e = t.GetDependents("d").GetEnumerator();
                Assert.IsFalse(e.MoveNext());
            }

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

            // Check that the one argument constructor throws an exception when dg == null
            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void TestOneArgumentConstructorException()
            {
                DependencyGraph g = new DependencyGraph(null);
            }

            // Check that the one argument constructor works as specified
            [TestMethod]
            public void TestOneArgumentConstructor()
            {
                DependencyGraph g = new DependencyGraph();
                g.AddDependency("s", "t");
                DependencyGraph h = new DependencyGraph(g);
                foreach (string s in h.GetDependents("s"))
                    Assert.AreEqual("t", s);
                foreach (string s in h.GetDependees("t"))
                    Assert.AreEqual("s", s);
                h.AddDependency("a", "b");
                foreach (string s in g.GetDependents("a"))
                    Assert.Fail();
                Assert.AreEqual(true, h.HasDependents("s"));
                Assert.AreEqual(false, g.HasDependents("t"));
                h.AddDependency("t", "q");
                Assert.AreEqual(false, g.HasDependents("t"));
            }
        }
    }
}

