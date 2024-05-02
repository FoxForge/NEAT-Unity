//--------------------------------------------------------------------------------------------------------->
//
//	File: "ILSESGeneration.cs"
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
	/// Interface for environment controllers to work with generation of NEAT agents.
	/// </summary>
	public interface ILSESGeneration
	{
		/// <summary>
		/// Create a NEAT agent for the simulation.
		/// </summary>
		/// <param name="specieId">
		///		Specie Id.
		/// </param>
		/// <param name="index">
		///		Index - Count of the agent for the current instantiation starting at 0.
		/// </param>
		/// <returns>
		///		Agent interface.
		/// </returns>
		INEATAgent CreateAgent( string specieId, int index );

		/// <summary>
		/// Callback before generation begins.
		/// </summary>
		void BeforeGeneration();

		/// <summary>
		/// Callback after generation has happened.
		/// </summary>
		void AfterGeneration();

		/// <summary>
		/// Callback for once a simulation has completed 1 generation.
		/// </summary>
		void OnGenerationComplete();
	}
}
