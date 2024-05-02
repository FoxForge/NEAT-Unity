//--------------------------------------------------------------------------------------------------------->
//
//	File: "LSESFactory.cs"
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
	/// Large Scale Environment Simualtion (LSES) Factory class to create simulation runners.
	/// </summary>
	public static class LSESFactory
	{
		/// <summary>
		/// Create a LSES Runner from a <see cref="NEATFileInfo"/> instance.
		/// </summary>
		/// <param name="envParams">
		///		Parameters.
		/// </param>
		/// <param name="environment">
		///		Callbacks.
		/// </param>
		/// <param name="fileInfo">
		///		NEAT File info.
		/// </param>
		/// <param name="specieIdFunction">
		///		Specie Id function.
		/// </param>
		/// <returns>
		///		LSES Runner.
		/// </returns>
		public static ILSESRunner Load( 
			ILSESParameters envParams,
			IMutationParameters mutationParams,
			ILSESGeneration environment,
			NEATFileInfo fileInfo,
			Func<string> specieIdFunction = null )
		{
			return new LSESRunnerLoaded( envParams, mutationParams, environment, fileInfo, specieIdFunction );
		}

		/// <summary>
		/// Create a new LSES Runner.
		/// </summary>
		/// <param name="envParams">
		///		Paramenters.
		/// </param>
		/// <param name="environment">
		///		Callbacks.
		/// </param>
		/// <param name="specieIdFunction">
		///		Specie Id function.
		/// </param>
		/// <returns>
		///		LSES Runner.
		/// </returns>
		public static ILSESRunner Create( 
			ILSESParameters envParams,
			IMutationParameters mutationParams,
			ILSESGeneration environment,
			Func<string> specieIdFunction = null )
		{
			return new LSESRunnerGenerated( envParams, mutationParams, environment, specieIdFunction );
		}
	}
}
