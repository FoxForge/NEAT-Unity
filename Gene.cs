//--------------------------------------------------------------------------------------------------------->
//
//	File: "Gene.cs"
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
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Describes gene information of in-comming and out-going nodes
	/// A gene can be turned on or off rendering it to be surpressed or expresses
	/// </summary>
	public class Gene : IEquatable<Gene>
	{
		/// <summary>
		/// 4 pieces of information that make up a gene.
		/// </summary>
		private const int GeneInformationSize = 4;

		/// <summary>
		/// Innovation number of this gene
		/// </summary>
		private readonly int _innovation;

		/// <summary>
		/// The "in" id of this gene.
		/// </summary>
		private readonly int _inId;

		/// <summary>
		/// The "out" id of this gene.
		/// </summary>
		private readonly int _outId;

		/// <summary>
		/// The weight of this gene.
		/// </summary>
		private float _weight;

		/// <summary>
		/// Is this gene active?
		/// </summary>
		private bool _active;

		/// <summary>
		/// Using Linq libary and delimiters, parse and spilt string genome from neat packet into float array
		/// </summary>
		/// <param name="genome">
		///     Genome string.
		/// </param>
		/// <returns>
		///     Gene information.
		/// </returns>
		private static float[] ParseGeneInformation( string genome )
		{
			return genome.Split( '_' ).Select( x => float.Parse( x ) ).ToArray();
		}

		/// <summary>
		/// Get genes from genome information.
		/// </summary>
		/// <param name="genome">
		///		Genome.
		/// </param>
		/// <param name="innovationFunction">
		///		Innovation function.
		/// </param>
		/// <param name="overrideWeight">
		///		Override weight.
		/// </param>
		/// <param name="overrideActivation">
		///		Override activation flag.
		/// </param>
		/// <returns>
		///		Genes.
		/// </returns>
		public static IEnumerable<Gene> CreateGenesFromInfo( string genome, Func<int, int, int> innovationFunction, float? overrideWeight = null, bool? overrideActivation = null )
		{
			float[] geneInformation = ParseGeneInformation( genome );

			for( int i = 0; i < geneInformation.Length; i += GeneInformationSize )
			{
				int innovation = innovationFunction.Invoke( ( int )geneInformation[ i ], ( int )geneInformation[ i + 1 ] );
				Gene gene = new Gene(
					innovation,
					( int )geneInformation[ i ],
					( int )geneInformation[ i + 1 ],
					overrideWeight ?? geneInformation[ i + 2 ],
					overrideActivation ?? geneInformation[ i + 3 ] == 1.0 );

				yield return gene;
			}
		}

		/// <summary>
		/// Deep copy a given gene
		/// </summary>
		/// <param name="copy">
		///		Gene to copy.
		///	</param>
		public Gene( Gene copy )
		{
			this._innovation = copy._innovation;
			this._inId = copy._inId;
			this._outId = copy._outId;
			this._weight = copy._weight;
			this._active = copy._active;
		}

		/// <summary>
		/// Create a new gene with the given parameters
		/// </summary>
		/// <param name="inno">
		///		Innovation number of this gene.
		///	</param>
		/// <param name="inID">
		///		Input node.
		///	</param>
		/// <param name="outID">
		///		Output node.
		/// </param>
		/// <param name="weight">
		///		Weight of this connection.
		///	</param>
		/// <param name="on">
		///		State of this gene, true or false.
		///	</param>
		public Gene( int inno, int inID, int outID, float weight, bool on )
		{
			this._innovation = inno;
			this._inId = inID;
			this._outId = outID;
			this._weight = weight;
			this._active = on;
		}

		/// <summary>
		/// Get gene in node id
		/// </summary>
		/// <returns>
		///		Return in node id.
		///	</returns>
		public int GetInID()
		{
			return this._inId;
		}

		/// <summary>
		/// Get gene out node id
		/// </summary>
		/// <returns>
		///		Return out node id.
		///	</returns>
		public int GetOutID()
		{
			return this._outId;
		}

		/// <summary>
		/// Get gene innovation number
		/// </summary>
		/// <returns>
		///		Return innovation number.
		///	</returns>
		public int GetInnovation()
		{
			return this._innovation;
		}

		/// <summary>
		/// Get gene weight
		/// </summary>
		/// <returns>
		///		Return weight.
		///	</returns>
		public float GetWeight()
		{
			return this._weight;
		}

		/// <summary>
		/// Get gene state
		/// </summary>
		/// <returns>
		///		Return state.
		///	</returns>
		public bool GetGeneState()
		{
			return this._active;
		}

		/// <summary>
		/// Set the gene state.
		/// </summary>
		/// <param name="on">
		///		Turn the gene on or off?
		/// </param>
		public void SetGeneState( bool on )
		{
			this._active = on;
		}

		/// <summary>
		/// Set weight of this gene
		/// </summary>
		/// <param name="weight">
		///		Weight value to set.
		///	</param>
		public void SetWeight( float weight )
		{
			this._weight = weight;
		}

		/// <summary>
		/// Convert this gene into a string
		/// </summary>
		/// <returns>
		///		String version of the gene.
		///	</returns>
		public string GetGeneString()
		{
			// Encode the active status as a 1 (active) or 0 (inactive)
			// example: 2_5_1.2345235897_0
			return string.Join( "_", this._inId, this._outId, this._weight, this._active ? 1 : 0 );
		}

		/// <summary>
		/// Check if two genes are the same.
		/// Two genes are the same if they share the same in and out nodes.
		/// </summary>
		/// <param name="other">
		///		Gene to compare with this gene.
		///	</param>
		/// <returns>
		///		True if genes are the same else false.
		///	</returns>
		public bool Equals( Gene other )
		{
			if( other == null )
			{
				return false;
			}

			return this._inId == other._inId && this._outId == other._outId;
		}
	}
}
