//--------------------------------------------------------------------------------------------------------->
//
//	File: "IMutationParameters.cs"
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
	using System.Collections.Generic;

	/// <summary>
	/// Mutation parameter interface for LSES environments.
	/// </summary>
	public interface IMutationParameters
	{
		/// <summary>
		/// Chance of a topology mutation.
		/// </summary>
		double TopologyMutateChance { get; }

		/// <summary>
		/// Chance of gene mutation.
		/// </summary>
		double GeneMutateChance { get; }

		/// <summary>
		/// Flags to determine how the genes are allowed to mutate.
		/// </summary>
		GeneMutationFlags GeneMutateFlags { get; }

		/// <summary>
		/// Chance that parents will cross genes in selection.
		/// </summary>
		double ParentGeneCrossChanceDefault { get; }

		/// <summary>
		/// Provides an override lookup for genecomparison cross chance.
		/// </summary>
		IReadOnlyDictionary<ParentGeneComparison, double> ParentGeneCrossChanceLookup { get; }

	}
}
