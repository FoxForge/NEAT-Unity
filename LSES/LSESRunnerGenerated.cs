///--------------------------------------------------------------------------------------------------------->
//
//	File: "LSESRunnerGenerated.cs"
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
	/// <see cref="LSESRunner"/> for new generated NEAT Networks.
	/// </summary>
	internal class LSESRunnerGenerated : LSESRunner
	{
		/// <summary>
		/// NEAT consultor
		/// </summary>
		private readonly NEATConsultor _consultor;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="envParams">
		///		Parameters.
		/// </param>
		/// <param name="environment">
		///		Environment callbacks.
		/// </param>
		/// <param name="specieIdFunction">
		///		Specie Id function.
		/// </param>
		internal LSESRunnerGenerated( 
			ILSESParameters envParams,
			IMutationParameters mutationParams,
			ILSESGeneration environment,
			Func<string> specieIdFunction = null ) : 
				base( envParams, environment, specieIdFunction )
		{
			// Create a consultor with NEAT packet retrieved from database and coefficients
			this._consultor = new NEATConsultor(
				mutationParams,
				envParams.NumberOfInputPerceptrons,
				envParams.NumberOfOutputPerceptrons,
				envParams.DeltaThreshold,
				envParams.DisjointCoefficient,
				envParams.ExcessCoefficient,
				envParams.AverageWeightDifferenceCoefficient );

			this.Reset();
		}

		/// <inheritdoc/>
		protected override NEATNetwork CreateInitialNetwork()
		{
			// Create new network with consultor, perceptron information and test time
			NEATNetwork network = new NEATNetwork(
				this._consultor,
				this.Parameters.NumberOfInputPerceptrons,
				this.Parameters.NumberOfOutputPerceptrons,
				this.Parameters.GenerationTestTime );

			// Mutate once for diversity (must make sure SJW's aren't triggered)
			network.Mutate();

			// Setup network for LSES evnironment.
			network.GenerateNeuralNetworkFromGenome();

			return network;
		}
	}
}
