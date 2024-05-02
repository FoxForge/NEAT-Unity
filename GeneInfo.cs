//--------------------------------------------------------------------------------------------------------->
//
//	File: "GeneInfo.cs"
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
	/// Container for the information about a <see cref="Gene"/> to draw connections in external applications.
	/// </summary>
	public class GeneInfo
	{
		/// <summary>
		/// Gets the in-ID of the gene
		/// </summary>
		public int GeneInId { get; }

		/// <summary>
		/// Gets the out-ID of the gene.
		/// </summary>
		public int GeneOutId { get;  }

		/// <summary>
		/// Gets the weight of the gene.
		/// </summary>
		public float Weight { get; }

		/// <summary>
		/// Default constructor. 
		/// </summary>
		/// <param name="gene">
		///		Gene.
		/// </param>
		public GeneInfo( Gene gene )
		{
			this.GeneInId = gene.GetInID();
			this.GeneOutId = gene.GetOutID();
			this.Weight = gene.GetGeneState() ? gene.GetWeight() : 0.0f;
		}
	}
}
