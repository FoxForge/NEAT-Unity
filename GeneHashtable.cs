//--------------------------------------------------------------------------------------------------------->
//
//	File: "GeneHashtable.cs"
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
	/// Gene hashtable for comparing and performing gene operations between multiple <see cref="NEATNetwork"/> networks.
	/// </summary>
	internal class GeneHashtable : Dictionary<int, Gene[]>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="networks">
		///		Networks.
		/// </param>
		public GeneHashtable( params NEATNetwork[] networks )
			: base()
		{
			int length = networks.Length;

			// Add all genes from the parents in order, using our special hashtable to union the values into an array.
			for( int i = 0; i < networks.Length; i++ )
			{
				foreach( Gene gene in networks[ i ].GetGenes() )
				{
					this.AddToManyAtIndexWithLength( gene.GetInnovation(), gene, i, length );
				}
			}
		}

		/// <summary>
		/// Inserts the specified value to the multi array value dictionary. If a key doesn't exist, it will create a new array of constructor length and set our value at our index.
		/// Otherwise, union happens with existing array which matches our key.
		/// </summary>
		/// <param name="key">
		///		The key of the element to add.
		///	</param>
		/// <param name="value">
		///		The value of the element to add. The value can be null for reference types.
		///	</param>
		///	<param name="index">
		///		Index of the gene array to add to.
		///	</param>
		///	<param name="length">
		///		Length to initialize the gene arrays with.
		///	</param>
		private void AddToManyAtIndexWithLength( int key, Gene value, int index, int length )
		{
			//if the dictionary doesn't contain the key, make a new list under the key
			if( !this.ContainsKey( key ) )
			{
				this.Add( key, new Gene[ length ] );
			}

			//add the value to the list at the key index
			this[ key ][ index ] = ( value );
		}
	}
}
