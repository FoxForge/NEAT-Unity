//--------------------------------------------------------------------------------------------------------->
//
//	File: "Neuron.cs"
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

	/// <summary>
	/// Internal neuron class for managing genes within a neural network.
	/// </summary>
	internal class Neuron
	{
		/// <summary>
		/// The incomming genes for this neuron.
		/// </summary>
		private readonly List<Gene> _incomming;

		/// <summary>
		/// Get or set the value of this neuron.
		/// </summary>
		public float Value { get; set; }

		/// <summary>
		/// Get the id for this neuron.
		/// </summary>
		public int Id { get; private set; }

		/// <summary>
		/// Get the cached/baked incomming genes for this neuron.
		/// </summary>
		public Gene[] IncommingArray { get; private set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="id">
		///		Neuron Id.
		/// </param>
		/// <param name="value">
		///		Neuron Value.
		/// </param>
		public Neuron( int id, float value )
		{
			this._incomming = new List<Gene>();
			this.Id = id;
			this.Value = value;
		}

		/// <summary>
		/// Add an incomming gene.
		/// </summary>
		/// <param name="gene">
		///		Gene.
		/// </param>
		public void AddIncommingGene( Gene gene )
		{
			this._incomming.Add( gene );
		}

		/// <summary>
		/// Sort the incomming genes for this neuron.
		/// </summary>
		/// <param name="comparison">
		///		Comparison.
		/// </param>
		public void SortIncommingGenes( Comparison<Gene> comparison )
		{
			this._incomming.Sort( comparison );
		}

		/// <summary>
		/// Cache or Bake the current <see cref="_incomming"/> genes into the <see cref="IncommingArray"/>
		/// </summary>
		public void CacheIncommingArray()
		{
			this.IncommingArray = this._incomming.ToArray();
		}
	}
}
