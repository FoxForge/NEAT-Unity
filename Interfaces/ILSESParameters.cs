//--------------------------------------------------------------------------------------------------------->
//
//	File: "ILSESParameters.cs"
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

namespace Forge.AI.NEAT
{
	using System;

	/// <summary>
	/// Interface for Large Scale Environment Simulation (LSES) Parameters.
	/// </summary>
	public interface ILSESParameters
	{
		/// <summary>
		/// Selection mode when meeting the base criteria for selection.
		/// </summary>
		PopulationSelectionMode SelectionMode { get; }

		/// <summary>
		/// Population traning size
		/// </summary>
		int PopulationSize { get; }

		/// <summary>
		/// Time to test each generation
		/// </summary>
		TimeSpan GenerationTestTime { get; }

		/// <summary>
		/// Number of input perceptrons for neural networks (including bias)
		/// </summary>
		int NumberOfInputPerceptrons { get; }

		/// <summary>
		/// Number of output perceptrons for neural networks.
		/// </summary>
		int NumberOfOutputPerceptrons { get; }

		/// <summary>
		/// Elite from each species
		/// </summary>
		float Elite { get; }

		/// <summary>
		/// Beta boost for a creature's fitness
		/// </summary>
		float Beta { get; }

		/// <summary>
		/// Remove % worst per generation.
		/// </summary>
		float RemoveWorst { get; }

		/// <summary>
		/// 2 different organisms with delta less than this threshold are considered to be in the same species
		/// </summary>
		float DeltaThreshold { get; }

		/// <summary>
		/// How much affect disjoint genes have on delta calculation (disjoint means genes that go not line up) 
		/// </summary>
		float DisjointCoefficient { get; }

		/// <summary>
		/// How much affect excess genes have on delta calculation (like excess, but outside the other species innovation number) 
		/// </summary>
		float ExcessCoefficient { get; }

		/// <summary>
		/// How much affect the absolute average weight differences have on delta calculation
		/// </summary>
		float AverageWeightDifferenceCoefficient { get; }
	}
}
