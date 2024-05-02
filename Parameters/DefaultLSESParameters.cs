//--------------------------------------------------------------------------------------------------------->
//
//	File: "DefaultLSESParameters.cs"
//
//	MIT License
//	
//	This file is part of the NEAT-Unity distribution (https://github.com/FoxForge/NEAT-Unity)
//	Copyright © 2024 Danny
//	
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//	
//	The above copyright notice and this permission notice shall be included in all
//	copies or substantial portions of the Software.
//	
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//	SOFTWARE.
//
//--------------------------------------------------------------------------------------------------------->

namespace Forge.AI.NEAT.Parameters
{
	using System;
	using Forge.AI.NEAT;

	/// <summary>
	/// Default structure for implementation of <see cref="ILSESParameters"/>
	/// </summary>
	public class DefaultLSESParameters : ILSESParameters
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="selectionMode">
		///		Population selection mode for new generations.
		/// </param>
		/// <param name="populationSize">
		///		Population size.
		/// </param>
		/// <param name="generationTestTime">
		///		Generation test time.
		/// </param>
		/// <param name="numberOfInputPerceptrons">
		///		Number of inputs perceptrons for the neural networks.
		/// </param>
		/// <param name="numberOfOutputPerceptrons">
		///		Number of output perceptrons for the neural networks.
		/// </param>
		/// <param name="elite">
		///		Elitism percentage for the population 0f-1.0f
		/// </param>
		/// <param name="beta">
		///		Beta boost modifier for a creatures fitness.
		/// </param>
		/// <param name="removeWorst">
		///		Percentage to remove worst from a population on generation.
		/// </param>
		/// <param name="deltaThreshold">
		///		2 different organisms with delta less than this threshold are considered to be in the same species.
		/// </param>
		/// <param name="disjoinCoefficient">
		///		How much affect disjoint genes have on delta calculation (disjoint means genes that go not line up) 
		/// </param>
		/// <param name="excessCoefficient">
		///		How much affect excess genes have on delta calculation (like excess, but outside the other species innovation number) 
		/// </param>
		/// <param name="averageWeightDifferenceCoefficient">
		///		How much affect the absolute average weight differences have on delta calculation
		/// </param>
		public DefaultLSESParameters(
			PopulationSelectionMode selectionMode,
			int populationSize,
			TimeSpan generationTestTime,
			int numberOfInputPerceptrons,
			int numberOfOutputPerceptrons,
			float elite,
			float beta,
			float removeWorst,
			float deltaThreshold,
			float disjoinCoefficient,
			float excessCoefficient,
			float averageWeightDifferenceCoefficient )
		{
			this.SelectionMode = selectionMode;
			this.PopulationSize = populationSize;
			this.GenerationTestTime = generationTestTime;
			this.NumberOfInputPerceptrons = numberOfInputPerceptrons;
			this.NumberOfOutputPerceptrons = numberOfOutputPerceptrons;
			this.Elite = elite;
			this.Beta = beta;
			this.RemoveWorst = removeWorst;
			this.DeltaThreshold = deltaThreshold;
			this.DisjointCoefficient = disjoinCoefficient;
			this.ExcessCoefficient = excessCoefficient;
			this.AverageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;
		}

		/// <summary>
		/// Construct default parameters for a very basic generation environment setup.
		/// </summary>
		/// <param name="populationSize">
		///		Population size.
		/// </param>
		/// <param name="generationTestTime">
		///		Generation test time.
		/// </param>
		/// <param name="numberOfInputPerceptrons">
		///		Number of inputs perceptrons for the neural networks.
		/// </param>
		/// <param name="numberOfOutputPerceptrons">
		///		Number of output perceptrons for the neural networks.
		/// </param>
		/// <remarks>
		/// Other construciton values were formulated on testings of this library with a very simple environment of 8 <paramref name="numberOfInputPerceptrons"/> 
		/// and 2 <paramref name="numberOfOutputPerceptrons"/> with <paramref name="populationSize"/> of 100 and <paramref name="generationTestTime"/> of 25
		/// </remarks>
		public DefaultLSESParameters(
			int populationSize,
			TimeSpan generationTestTime,
			int numberOfInputPerceptrons,
			int numberOfOutputPerceptrons) : 
			this(
				selectionMode: PopulationSelectionMode.Random,
				populationSize: populationSize,
				generationTestTime: generationTestTime,
				numberOfInputPerceptrons: numberOfInputPerceptrons,
				numberOfOutputPerceptrons: numberOfOutputPerceptrons,
				elite: 0.1f,
				beta: 1.5f,
				removeWorst: 0.5f,
				deltaThreshold: 0.2f,
				disjoinCoefficient: 1f,
				excessCoefficient: 2f,
				averageWeightDifferenceCoefficient: 2f) 
		{ 
		}

		/// <inheritdoc/>
		public PopulationSelectionMode SelectionMode { get; }

		/// <inheritdoc/>
		public int PopulationSize { get; }

		/// <inheritdoc/>
		public TimeSpan GenerationTestTime { get; }

		/// <inheritdoc/>
		public int NumberOfInputPerceptrons { get; }

		/// <inheritdoc/>
		public int NumberOfOutputPerceptrons { get; }

		/// <inheritdoc/>
		public float Elite { get; }

		/// <inheritdoc/>
		public float Beta { get; }

		/// <inheritdoc/>
		public float RemoveWorst { get; }

		/// <inheritdoc/>
		public float DeltaThreshold { get; }

		/// <inheritdoc/>
		public float DisjointCoefficient { get; }

		/// <inheritdoc/>
		public float ExcessCoefficient { get; }

		/// <inheritdoc/>
		public float AverageWeightDifferenceCoefficient { get; }
	}
}
