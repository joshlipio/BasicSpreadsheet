// Skeleton implementation written by Joe Zachary for CS 3500, January 2017.

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// This graph will be represented by a Dictionary with string keys and DependencyNode values.
        /// In this way, strings can be directly mapped to their dependencies and dependees in a way which is O(c) to access and change.
        /// When a dependency is added, the graph checks that there are DependencyNodes for each string and creates them if there are not.
        /// Strings representing the new dependency and dependee are then added to the dependents and dependees HashSets for the DependencyNodes, respectively.
        /// When a DependencyNode no longer has any dependents or dependees, it is removed from the graph.
        /// </summary>
        private Dictionary<string, DependencyNode> graph;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            graph = new Dictionary<string, DependencyNode>();
        }

        /// <summary>
        /// Creates a new DependencyGraph which is a clone of the argument graph
        /// </summary>
        public DependencyGraph(DependencyGraph dg)
        {
            if (dg == null)
                throw new ArgumentNullException("The argument DependencyGraph cannot be null");

            graph = new Dictionary<string, DependencyNode>();

            foreach (KeyValuePair<string, DependencyNode> pair in dg.graph)
            {
                graph.Add(pair.Key, pair.Value.CloneNode());
            }
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get {
                
                int toReturn = 0;
                foreach (KeyValuePair<string, DependencyNode> pair in graph)
                {
                    toReturn += pair.Value.dependents.Count;
                }
                return toReturn;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// Throws an ArgumentNullException if s is null
        /// </summary>
        public bool HasDependents(string s)
        {
            if (s == null)
                throw new ArgumentNullException();
            if (!graph.ContainsKey(s))
                return false;
            return graph[s].dependents.Count > 0;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// Throws an ArgumentNullException if s == null
        /// </summary>
        public bool HasDependees(string s)
        {
            if (s == null)
                throw new ArgumentNullException();
            if (!graph.ContainsKey(s))
                return false;
            return graph[s].dependees.Count > 0;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// Throws an ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
                throw new ArgumentNullException();

            if (!graph.ContainsKey(s))
                yield break;

            foreach (string dependent in graph[s].dependents)
                yield return dependent;
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// Throws an ArgumentNullException if s == null.
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
                throw new ArgumentNullException();

            if (!graph.ContainsKey(s))
                yield break;

            foreach (string dependee in graph[s].dependees)
                yield return dependee;
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// Throws an ArgumentNullException if s == null or t == null
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
                throw new ArgumentNullException();
            
            if (!graph.ContainsKey(s))
            {
                DependencyNode tempS = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                graph.Add(s, tempS);
            }
            if (!graph.ContainsKey(t))
            {
                DependencyNode tempT = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                graph.Add(t, tempT);
            }

            graph[s].dependents.Add(t);
            graph[t].dependees.Add(s);
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// Throws an ArgumentNullException if s == null or t == null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
                throw new ArgumentNullException();

            if (!graph.ContainsKey(s) || !graph.ContainsKey(t)) // Do nothing if either key is not in the graph
                return;

            graph[s].dependents.Remove(t);
            graph[t].dependees.Remove(s);

            // Remove dependent-less and dependee-less keys from the graph
            if (graph[s].dependents.Count == 0 && graph[s].dependees.Count == 0)
                graph.Remove(s);
            if (graph.ContainsKey(t) && graph[t].dependents.Count == 0 && graph[t].dependees.Count == 0)
                graph.Remove(t);
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Throws an ArgumentNullException if s == null or any dependent in newDependents is null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null)
                throw new ArgumentNullException();

            if (!graph.ContainsKey(s))
            {
                DependencyNode tempS = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                graph.Add(s, tempS);
            }

            // Remove all of S's dependents, and take s off those dependents' dependee list
            foreach (string t in graph[s].dependents)
            {
                graph[t].dependees.Remove(s);
                if (graph[t].dependents.Count == 0 && graph[t].dependees.Count == 0)
                    graph.Remove(t);
            }
            graph[s].dependents.Clear();

            foreach (string newDependent in newDependents)
            {
                if (newDependent == null)
                    throw new ArgumentNullException();

                if (!graph.ContainsKey(newDependent))
                {
                    DependencyNode tempT = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                    graph.Add(newDependent, tempT);
                }
                graph[s].dependents.Add(newDependent);
                graph[newDependent].dependees.Add(s);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Throws an ArgumentNullException if t == null or any dependee in newDependees is null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null)
                throw new ArgumentNullException();

            if (!graph.ContainsKey(t))
            {
                DependencyNode tempT = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                graph.Add(t, tempT);
            }

            // Remove all of T's dependees, and take t off those dependees' dependents list
            foreach (string s in graph[t].dependees)
            {
                graph[s].dependents.Remove(t);
                if (graph[s].dependents.Count == 0 && graph[s].dependees.Count == 0)
                    graph.Remove(s);
            }
            graph[t].dependees.Clear();

            foreach (string newDependee in newDependees)
            {
                if (newDependee == null)
                    throw new ArgumentNullException();

                if (!graph.ContainsKey(newDependee))
                {
                    DependencyNode tempS = new DependencyNode(new HashSet<string>(), new HashSet<string>());
                    graph.Add(newDependee, tempS);
                }
                graph[t].dependees.Add(newDependee);
                graph[newDependee].dependents.Add(t);
            }
        }

        /// <summary>
        /// Struct representing a specific string in a dependency graph.
        /// Keeps track of the string's dependents and dependees and provides methods to access them.
        /// </summary>
        private struct DependencyNode
        {
            // The node's dependents and dependees are both stored in HashSets to make accessing and changing them as fast as possible
            public HashSet<string> dependents;
            public HashSet<string> dependees;

            /// <summary>
            /// Creates a new DependencyNode with the specified set of dependents and dependees
            /// </summary>
            public DependencyNode(HashSet<string> dependentsSet, HashSet<string> dependeesSet)
            {
                dependents = dependentsSet;
                dependees = dependeesSet;
            }

            /// <summary>
            /// Returns an identical but distinct clone of this DependencyNode
            /// </summary>
            public DependencyNode CloneNode()
            {
                HashSet<string> newDependents = new HashSet<string>(dependents);
                HashSet<string> newDependees = new HashSet<string>(dependees);
                return new DependencyNode(newDependents, newDependees);
            }
        }
    }
}
