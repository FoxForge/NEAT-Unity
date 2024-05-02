//--------------------------------------------------------------------------------------------------------->
//
//	File: "LSESRunnerLoaded.cs"
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
	/// <see cref="LSESRunner"/> for loading existing NEAT Networks from a file.
	/// </summary>
	internal class LSESRunnerLoaded : LSESRunner
	{
		/// <summary>
		/// File info for the NEAT network base model.
		/// </summary>
		private readonly NEATFileInfo _fileInfo;

		/// <summary>
		/// NEAT consultor.
		/// </summary>
		private readonly NEATConsultor _consultor;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parameters">
		///		Parameters.
		/// </param>
		/// <param name="environment">
		///		Environment Callbacks.
		/// </param>
		/// <param name="fileInfo">
		///		File info,
		/// </param>
		/// <param name="specieIdFunction">
		///		Specie Id function.
		/// </param>
		internal LSESRunnerLoaded( 
			ILSESParameters envParams, 
			IMutationParameters mutationParams, 
			ILSESGeneration environment, 
			NEATFileInfo fileInfo, 
			Func<string> specieIdFunction = null ) : 
				base( envParams, environment, specieIdFunction )
		{
			this._fileInfo = fileInfo;

			// Create a consultor with NEAT packet retrieved from database and coefficients
			this._consultor = new NEATConsultor(
				fileInfo,
				mutationParams,
				envParams.DeltaThreshold,
				envParams.DisjointCoefficient,
				envParams.ExcessCoefficient,
				envParams.AverageWeightDifferenceCoefficient );

			this.Reset();
		}

		/// <inheritdoc/>
		protected override NEATNetwork CreateInitialNetwork()
		{
			NEATNetwork network = new NEATNetwork( this._fileInfo, this._consultor );
			return NEATNetwork.Initialize( network, true );
		}
	}
}
