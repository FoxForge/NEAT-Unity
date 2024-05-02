//--------------------------------------------------------------------------------------------------------->
//
//	File: "GeneHashInfo.cs"
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
	using System.Linq;
	using System.Collections.Generic;

	/// <summary>
	/// Gene hash information for obtaining the gene statistics.
	/// </summary>
	internal class GeneHashInfo
	{
		/// <summary>
		/// Number of excess genes (genes that do match and are outside the innovation number of the other network)
		/// </summary>
		public int ExcessGenes { get; }

		/// <summary>
		/// Number of disjoint genes (genes that do not match in the two networks)
		/// </summary>
		public int DisjointGenes { get; }

		/// <summary>
		/// Number of genes both neural network have
		/// </summary>
		public int EqualGenes { get; }

		/// <summary>
		/// average weight difference of the two network's equal genes
		/// </summary>
		public float AverageWeightDifference { get; }

		/// <summary>
		/// Default constructor. Create the hash information breakdown from the genehashtable.
		/// </summary>
		public GeneHashInfo( GeneHashtable geneHash )
		{
			// Flags for logical processing of excess genes.
			bool foundAllExcess = false;

			// Order by descending gene index.
			IOrderedEnumerable<KeyValuePair<int, Gene[]>> sortedGeneHash = geneHash
				.OrderByDescending( kvp => kvp.Key );

			// Get index 1 of geneArray at first hash. Are we an excess gene?
			bool isFirstGeneExcess = sortedGeneHash.FirstOrDefault().Value[ 1 ] == null;

			// Now that we're sorted by gene index, gather the distribution info.
			foreach( KeyValuePair<int, Gene[]> kvp in sortedGeneHash )
			{
				int geneIndex = kvp.Key;

				Gene gene0 = kvp.Value[ 0 ];
				Gene gene1 = kvp.Value[ 1 ];

				// If all excess genes have not been found
				if( !foundAllExcess )
				{
					// If excess gene exist in net 1 and there is no gene in second location of the value, or
					// If excess gene exist in net 12 and there is no gene in first location of the value
					if( isFirstGeneExcess && gene1 == null || !isFirstGeneExcess && gene0 == null )
					{
						// This is an excess gene and increment excess gene
						this.ExcessGenes++;
					}
					else
					{
						// All excess genes are found
						foundAllExcess = true;
					}
				}

				// If all excess genes are found
				if( foundAllExcess )
				{
					// Both gene location exist.
					if( gene0 != null && gene1 != null )
					{
						// Increment equal genes
						this.EqualGenes++;

						// Add absolute difference between 2 weight
						this.AverageWeightDifference += Math.Abs( gene0.GetWeight() - gene1.GetWeight() );
					}
					else
					{
						// This is disjoint gene, so increment
						this.DisjointGenes++;
					}
				}
			}

			// Get average weight difference of equal genes
			this.AverageWeightDifference /= this.EqualGenes;
		}
	}
}
