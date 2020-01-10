﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SharpLearning.Optimization.Test.ObjectiveUtilities;

namespace SharpLearning.Optimization.Test
{
    [TestClass]
    public class ParticleSwarmOptimizerTest
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(0)]
        [DataRow(null)]
        public async Task ParticleSwarmOptimizer_OptimizeBest(int? maxDegreeOfParallelism)
        {
            var parameters = new MinMaxParameterSpec[]
            {
                new MinMaxParameterSpec(-10.0, 10.0, Transform.Linear),
                new MinMaxParameterSpec(-10.0, 10.0, Transform.Linear),
                new MinMaxParameterSpec(-10.0, 10.0, Transform.Linear),
            };

            var sut = maxDegreeOfParallelism.HasValue ? 
                new ParticleSwarmOptimizer(parameters, 100, maxDegreeOfParallelism: maxDegreeOfParallelism.Value) : 
                new ParticleSwarmOptimizer(parameters, 100);

            var actual = await sut.OptimizeBest(Minimize);

            Assert.AreEqual(-0.64324321766401094, actual.Error, Delta);
            Assert.AreEqual(3, actual.ParameterSet.Length);

            Assert.AreEqual(-4.92494268653156, actual.ParameterSet[0], Delta);
            Assert.AreEqual(10, actual.ParameterSet[1], Delta);
            Assert.AreEqual(-0.27508308116943514, actual.ParameterSet[2], Delta);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(0)]
        [DataRow(null)]
        public async Task ParticleSwarmOptimizer_Optimize(int? maxDegreeOfParallelism)
        {
            var parameters = new MinMaxParameterSpec[]
            {
                new MinMaxParameterSpec(0.0, 100.0, Transform.Linear)
            };

            var sut = maxDegreeOfParallelism.HasValue ? 
                new ParticleSwarmOptimizer(parameters, 100, maxDegreeOfParallelism: maxDegreeOfParallelism.Value) : 
                new ParticleSwarmOptimizer(parameters, 100);

            var results = await sut.Optimize(MinimizeWeightFromHeight);

            var actual = new OptimizerResult[] { results.First(), results.Last() };

            var expected = new OptimizerResult[]
            {
                new OptimizerResult(new double[] { 38.1151505704492 }, 115.978346548015),
                new OptimizerResult(new double[] { 37.2514904205637 }, 118.093289672808),
            };

            Assert.AreEqual(expected.First().Error, actual.First().Error, Delta);
            Assert.AreEqual(expected.First().ParameterSet.First(), 
                actual.First().ParameterSet.First(), Delta);

            Assert.AreEqual(expected.Last().Error, actual.Last().Error, Delta);
            Assert.AreEqual(expected.Last().ParameterSet.First(), 
                actual.Last().ParameterSet.First(), Delta);
        }
    }
}
