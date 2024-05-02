//--------------------------------------------------------------------------------------------------------->
//
//	File: "DefaultMutationParameters.cs"
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
	using System.Collections.Generic;

	/// <summary>
	/// Default implementation for a structure of <see cref="IMutationParameters"/>
	/// </summary>
	public class DefaultMutationParameters : IMutationParameters
	{
		/// <summary>
		/// Default construtor.
		/// </summary>
		/// <param name="topologyMutateChance">
		///		Chance of a topology mutation.
		/// </param>
		/// <param name="geneMutateChance">
		///		Chance of gene mutation.
		/// </param>
		/// <param name="geneMutationFlags">
		///		Flags to determine how the genes are allowed to mutate.
		/// </param>
		/// <param name="parentGeneCrossChanceDefault">
		///		Chance that parents will cross genes in selection.
		/// </param>
		/// <param name="parentGeneCrossChanceLookup">
		///		Provides an override lookup for genecomparison cross chance.
		/// </param>
		public DefaultMutationParameters(
			double topologyMutateChance,
			double geneMutateChance,
			GeneMutationFlags geneMutationFlags,
			double parentGeneCrossChanceDefault,
			IReadOnlyDictionary<ParentGeneComparison, double> parentGeneCrossChanceLookup )
		{
			this.TopologyMutateChance = topologyMutateChance;
			this.GeneMutateChance = geneMutateChance;
			this.GeneMutateFlags = geneMutationFlags;
			this.ParentGeneCrossChanceDefault = parentGeneCrossChanceDefault;
			this.ParentGeneCrossChanceLookup = parentGeneCrossChanceLookup;
		}

		/// <summary>
		/// Default constructor with default values implemented.
		/// </summary>
		public DefaultMutationParameters() :
			this(
				topologyMutateChance: 0.25,
				geneMutateChance: 0.05,
				geneMutationFlags: GeneMutationFlags.Default,
				parentGeneCrossChanceDefault: 0.02,
				parentGeneCrossChanceLookup: new Dictionary<ParentGeneComparison, double>
				{
					{ ParentGeneComparison.Inversed, 0.05 },
				} )
		{
		}

		/// <inheritdoc/>
		public double TopologyMutateChance { get; }

		/// <inheritdoc/>
		public double GeneMutateChance { get; }

		/// <inheritdoc/>
		public GeneMutationFlags GeneMutateFlags { get; }

		/// <inheritdoc/>
		public double ParentGeneCrossChanceDefault { get; }

		/// <inheritdoc/>
		public IReadOnlyDictionary<ParentGeneComparison, double> ParentGeneCrossChanceLookup { get; }

	}
}
