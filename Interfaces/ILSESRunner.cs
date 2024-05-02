//--------------------------------------------------------------------------------------------------------->
//
//	File: "ILSESRunner.cs"
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
	/// <summary>
	/// Interface for controlling the runners which capture the simulations.
	/// </summary>
	public interface ILSESRunner
	{
		/// <summary>
		/// Resets a LSES to it's original state. Use carefully as any trained/evolved networks will loose progress to their original form. All Species are reformed.
		/// </summary>
		void Reset();

		/// <summary>
		/// Queue the creation of a number of generations within the simulation environment.
		/// </summary>
		/// <param name="generations">
		///		Number of generations.
		/// </param>
		/// <returns>
		///		True if the generations were queue; otherwise false. (false when a simulation is already currently running).
		/// </returns>
		bool ActionGeneration( int generations );

		/// <summary>
		/// Get the best network obtained by the simulation.
		/// </summary>
		/// <returns>
		///		Best network.
		/// </returns>
		IReadOnlyNEATNetwork GetBestNetwork();

		/// <summary>
		/// Get the number of generations remaining in the simulation.
		/// </summary>
		int GenerationsRemaining { get; }

		/// <summary>
		/// Get the number of the current generation running in the simulation.
		/// </summary>
		int GenerationNumber { get; }

		/// <summary>
		/// Get the brief of species information of the current agents in the simulaiton.
		/// </summary>
		/// <returns>
		///		Species Info.
		/// </returns>
		SpeciesInfo GetSpeciesInfo();
	}
}
