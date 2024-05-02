//--------------------------------------------------------------------------------------------------------->
//
//	File: "Population.cs"
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
	using System.Linq;
	using System;
	using Forge.Base.Extensions;

	/// <summary>
	/// Keeps track of each individual species and it's population
	/// </summary>
	public class Population
	{
		/// <summary>
		/// List of net's in a species
		/// </summary>
		private readonly List<NEATNetwork> _population;

		/// <summary>
		/// Identification for the population. Often given as a html hex colour.
		/// </summary>
		private readonly string _id;

		/// <summary>
		/// Set the color of the species and create list of net's
		/// </summary>
		/// <param name="id">
		///     Identification for the population. Often given as a html hex colour.
		/// </param>
		public Population( string id )
		{
			this._id = id;
			this._population = new List<NEATNetwork>();
		}

		/// <summary>
		/// Add a member to this population.
		/// </summary>
		/// <param name="brain">
		///		New member brain.
		///	</param>
		public void Add( NEATNetwork brain )
		{
			this._population.Add( brain );
		}

		/// <summary>
		/// Sort this poplation based on fitness
		/// </summary>
		public void Sort()
		{
			this._population.Sort();
		}

		/// <summary>
		/// Get a random member
		/// </summary>
		/// <returns>Random net from the population</returns>
		public NEATNetwork GetRandom()
		{
			return this._population.Count == 0 ? null : this._population.GetRandom();
		}

		/// <summary>
		/// Get the entire population.
		/// </summary>
		/// <returns>
		///		List of all networks in the population.
		/// </returns>
		public List<NEATNetwork> GetPopulation()
		{
			return this._population;
		}

		/// <summary>
		/// Get the id for the population (often encoded as the color).
		/// </summary>
		/// <returns>
		///		The Id.
		/// </returns>
		public string GetId()
		{
			return this._id;
		}

		/// <summary>
		/// Get shared cumulative fitness of this population.
		/// </summary>
		/// <returns>
		///		Fitness distribution.
		///	</returns>
		public float GetDistribution( float beta )
		{
			double distribution = 0;
			foreach( NEATNetwork network in this._population )
			{
				int sharedSpecieCount = Math.Max(
					1,
					this._population.
						Except( network ).
						Where( x => NEATNetwork.IsSameSpecies( network, x ) ).
						Count() );

				float fitness = Math.Max( 0.0f, network.GetNetworkFitness() );
				distribution += Math.Pow( fitness, beta ) / sharedSpecieCount;
			}

			return Math.Max( 0.0f, ( float )distribution );
		}

		/// <summary>
		/// Remote worst percet of the population.
		/// Note: it's not really percent, but rather percent/100
		/// </summary>
		/// <param name="percent">
		///		Percet of the population to remove.
		///	</param>
		public void RemoveWorst( float percent )
		{
			if( this._population.Count <= 1 )
			{
				return;
			}

			this._population.Sort();
			if( this._population.Count == 2 && percent > 0f )
			{
				this._population.RemoveAt( 0 );
			}
			else
			{
				int amount = this._population.Count - ( int )( this._population.Count * percent );
				this._population.RemoveFirst( amount );
			}
		}

		/// <summary>
		/// Get last network if it exists.
		/// </summary>
		/// <returns>
		///		Last network default sort order.
		///	</returns>
		public NEATNetwork GetLast()
		{
			if( this._population.Count == 0 )
			{
				return null;
			}

			return this._population.Last();
		}

		/// <summary>
		/// Get last networks if they exist.
		/// </summary>
		/// <returns>
		///		Last network default sort order.
		///	</returns>
		public IEnumerable<NEATNetwork> GetLast(int n)
		{
			if( this._population.Count == 0 )
			{
				return null;
			}

			return this._population.TakeLast(n);
		}

		/// <summary>
		/// Add brain to this population if it matches
		/// </summary>
		/// <param name="brain">
		///		The brain to match.
		///	</param>
		/// <returns>
		///		True if added, false if not.
		///	</returns>
		public bool AddIfMatch( NEATNetwork brain )
		{
			if( this._population.Count == 0 )
			{
				this._population.Add( brain );
				return true;
			}
			else
			{
				if( NEATNetwork.IsSameSpecies( this.GetRandom(), brain ) )
				{
					this._population.Add( brain );
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Return brain with highest fitness
		/// </summary>
		/// <returns>
		///		NEAT Network.
		/// </returns>
		public NEATNetwork GetBestBrain()
		{
			return this._population.MaxBy( x => x.GetNetworkFitness() );
		}
	}
}
