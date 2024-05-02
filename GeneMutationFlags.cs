//--------------------------------------------------------------------------------------------------------->
//
//	File: "GeneMutationFlags.cs"
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
	/// Types when performing mutations on genes.
	/// </summary>
	[Flags]
	public enum GeneMutationFlags
	{
		/// <summary>
		/// Mutation type for flipping the weight of a gene.
		/// </summary>
		FlipWeight,

		/// <summary>
		/// Mutation type for flipping the activated state of a gene.
		/// </summary>
		FlipGeneState,

		/// <summary>
		/// Mutation type for setting the weight of a gene.
		/// </summary>
		RandomSetWeight,

		/// <summary>
		/// Mutation type for randomly increasing the weight of a gene.
		/// </summary>
		RandomIncreaseWeight,

		/// <summary>
		/// Mutation type for randomly decreasing the weight of a gene.
		/// </summary>
		RandomDecreaseWeight,

		/// <summary>
		/// Default mutation flags
		/// </summary>
		Default = FlipWeight | FlipGeneState | RandomSetWeight | RandomIncreaseWeight | RandomDecreaseWeight
	}
}
