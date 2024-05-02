﻿//--------------------------------------------------------------------------------------------------------->
//
//	File: "INEATAgent.cs"
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
	/// Interfaces for agents to interact with LSES runners.
	/// </summary>
	public interface INEATAgent
	{
		/// <summary>
		/// Final fitness calculation at the end of a generation. This is called externally by a LSES runner.
		/// </summary>
		/// <returns>
		///     Fitness.
		/// </returns>
		float CalculateFitness();

		/// <summary>
		/// Activate NEATNet of agent with the controller for local events.
		/// </summary>
		/// <param name="runnerCallback">
		///     Runner finished Callback.
		/// </param>
		/// <param name="net">
		///     Network.
		/// </param>
		void Activate( string specieId, ILSESRunFinishedCallback runnerCallback, INEATNetwork net );

		/// <summary>
		/// Call for the agent to perform any cleanup operations when finished testing.
		/// </summary>
		void OnFinished();
	}
}