﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpLearning.Containers;
using SharpLearning.Containers.Matrices;
using SharpLearning.InputOutput.Csv;
using SharpLearning.Metrics.Regression;
using SharpLearning.RandomForest.Learners;
using SharpLearning.RandomForest.Test.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpLearning.RandomForest.Test.Learners
{
    [TestClass]
    public class RegressionExtremelyRandomizedTreesLearnerTest
    {
        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Aptitude_Trees_1()
        {
            var error = RegressionExtremelyRandomizedTreesLearner_Learn_Aptitude(1);
            Assert.AreEqual(0.1278846153846154, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Aptitude_Trees_5()
        {
            var error = RegressionExtremelyRandomizedTreesLearner_Learn_Aptitude(5);
            Assert.AreEqual(0.088792307692307712, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Aptitude_Trees_100()
        {
            var error = RegressionExtremelyRandomizedTreesLearner_Learn_Aptitude(100);
            Assert.AreEqual(0.079629411481205281, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass_1()
        {
            var error = RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass(1);
            Assert.AreEqual(1.1195386995283647, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass_5()
        {
            var error = RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass(5);
            Assert.AreEqual(0.50272659119354779, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass_100()
        {
            var error = RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass(100);
            Assert.AreEqual(0.3513123937886431, error, 0.0000001);
        }

        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass_100_Indices()
        {
            var parser = new CsvParser(() => new StringReader(Resources.Glass));
            var observations = parser.EnumerateRows(v => v != "Target").ToF64Matrix();
            var targets = parser.EnumerateRows("Target").ToF64Vector();
            var rows = targets.Length;

            var sut = new RegressionExtremelyRandomizedTreesLearner(100, 1, 100, 1, 0.0001, 42, 1);
            
            var indices = Enumerable.Range(0, targets.Length).ToArray();
            indices.Shuffle(new Random(42));
            indices = indices.Take((int)(targets.Length * 0.7))
                .ToArray();

            var model = sut.Learn(observations, targets, indices);

            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var error = evaluator.Error(targets, predictions);

            Assert.AreEqual(0.63876128131525645, error, 0.0000001);
        }

        double RegressionExtremelyRandomizedTreesLearnerTest_Learn_Glass(int trees)
        {
            var parser = new CsvParser(() => new StringReader(Resources.Glass));
            var observations = parser.EnumerateRows(v => v != "Target").ToF64Matrix();
            var targets = parser.EnumerateRows("Target").ToF64Vector();
            var rows = targets.Length;

            var sut = new RegressionExtremelyRandomizedTreesLearner(trees, 1, 100, 1, 0.0001, 42, 1);
            var model = sut.Learn(observations, targets);

            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var error = evaluator.Error(targets, predictions);
            return error;
        }

        double RegressionExtremelyRandomizedTreesLearner_Learn_Aptitude(int trees)
        {
            var parser = new CsvParser(() => new StringReader(Resources.AptitudeData));
            var observations = parser.EnumerateRows(v => v != "Pass").ToF64Matrix();
            var targets = parser.EnumerateRows("Pass").ToF64Vector();
            var rows = targets.Length;

            var sut = new RegressionExtremelyRandomizedTreesLearner(trees, 5, 100, 1, 0.0001, 42, 1);
            var model = sut.Learn(observations, targets);

            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var error = evaluator.Error(targets, predictions);
            return error;
        }

        [Ignore]
        [TestMethod]
        public void RegressionExtremelyRandomizedTreesLearner_Learn_Timing()
        {
            var rows = 4000;
            var cols = 10;

            var random = new Random(42);
            var observations = new F64Matrix(rows, cols);
            var targets = new double[rows];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = random.Next(5);
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    observations.SetItemAt(i, j, random.NextDouble());
                }
            }

            var sut = new RegressionExtremelyRandomizedTreesLearner();
            var timer = new Stopwatch();
            timer.Start();
            var model = sut.Learn(observations, targets);
            timer.Stop();

            var predictions = model.Predict(observations);

            var evaluator = new MeanSquaredErrorRegressionMetric();
            var error = evaluator.Error(targets, predictions);

            Trace.WriteLine("Error: " + error);
            Trace.WriteLine("Time: " + timer.ElapsedMilliseconds);
        }
    }
}
