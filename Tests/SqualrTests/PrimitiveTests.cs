namespace SqualrTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Squalr.Engine.Common.Extensions;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class PrimitiveTests
    {
        [TestMethod]
        public void TestFloatEquivalency()
        {
            List<KeyValuePair<Single, Single>> positiveCases = new List<KeyValuePair<Single, Single>>()
            {
                new KeyValuePair<Single, Single>(0.0f, 0.0f),
                new KeyValuePair<Single, Single>(1.0f, 1.0f),
                new KeyValuePair<Single, Single>(-1.0f, -1.0f),
                new KeyValuePair<Single, Single>(1000.0f, 1000.01f),
                new KeyValuePair<Single, Single>(50000.0f, 50000.5f),
                new KeyValuePair<Single, Single>(0.000001f, 0.000001f),
            };

            foreach (KeyValuePair<Single, Single> pair in positiveCases)
            {
                Assert.IsTrue((pair.Key).AlmostEquals(pair.Value), pair.Key.ToString() + " - " + pair.Value.ToString());
            }
        }

        [TestMethod]
        public void TestFloatInequivalency()
        {
            List<KeyValuePair<Single, Single>> negativeCases = new List<KeyValuePair<Single, Single>>()
            {
                new KeyValuePair<Single, Single>(1000.0f, 1500.0f),
                new KeyValuePair<Single, Single>(0.01f, 0.10f),
            };

            foreach (KeyValuePair<Single, Single> pair in negativeCases)
            {
                Assert.IsFalse((pair.Key).AlmostEquals(pair.Value), pair.Key.ToString() + " - " + pair.Value.ToString());
            }
        }

        [TestMethod]
        public void TestDoubleEquivalency()
        {
            List<KeyValuePair<Double, Double>> positiveCases = new List<KeyValuePair<Double, Double>>()
            {
                new KeyValuePair<Double, Double>(0.0, 0.0),
                new KeyValuePair<Double, Double>(1.0, 1.0),
                new KeyValuePair<Double, Double>(-1.0, -1.0),
                new KeyValuePair<Double, Double>(1000.0, 1000.0001),
                new KeyValuePair<Double, Double>(50000.0, 50000.01),
                new KeyValuePair<Double, Double>(0.000001, 0.000001),
            };

            foreach (KeyValuePair<Double, Double> pair in positiveCases)
            {
                Assert.IsTrue((pair.Key).AlmostEquals(pair.Value), pair.Key.ToString() + " - " + pair.Value.ToString());
            }
        }

        [TestMethod]
        public void TestDoubleInequivalency()
        {
            List<KeyValuePair<Double, Double>> negativeCases = new List<KeyValuePair<Double, Double>>()
            {
                new KeyValuePair<Double, Double>(1000.0, 1001.0),
                new KeyValuePair<Double, Double>(0.01, 0.10),
            };

            foreach (KeyValuePair<Double, Double> pair in negativeCases)
            {
                Assert.IsFalse((pair.Key).AlmostEquals(pair.Value), pair.Key.ToString() + " - " + pair.Value.ToString());
            }
        }
    }
    //// End class
}
//// End namespace