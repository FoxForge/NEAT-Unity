//--------------------------------------------------------------------------------------------------------->
//
//	File: "Species.cs"
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
	using Forge.Base.Extensions;

	/// <summary>
	/// Keeps track of a list of species and judges species based on performance.
	/// </summary>
	public class Species
	{
		/// <summary>
		/// Interal species list.
		/// </summary>
		private List<Population> _species;

		/// <summary>
		/// Funciton to generate specie Ids.
		/// </summary>
		private readonly Func<string> _specieIdGenFunction;

		/// <summary>
		/// LSES parameters.
		/// </summary>
		private readonly ILSESParameters _parameters;

		/// <summary>
		/// Create a species
		/// </summary>
		/// <param name="parameters">
		///		Parameters.
		///	</param>
		/// <param name="specieIdGeneration">
		///		Specie Id Generation Function.
		///	</param>
		public Species( ILSESParameters parameters, Func<string> specieIdGeneration )
		{
			this._species = new List<Population>();
			this._parameters = parameters;
			this._specieIdGenFunction = specieIdGeneration;
		}

		/// <summary>
		/// Create an empty specie with only id function called
		/// </summary>
		/// <returns>
		///		New empty population.
		///	</returns>
		public Population CreateNewSpecie()
		{
			Population newPop = new Population( this._specieIdGenFunction.Invoke() );
			this._species.Add( newPop );
			return newPop;
		}

		/// <summary>
		/// Creating a new specie
		/// </summary>
		/// <param name="species">
		///		List of population in a new specie.
		///	</param>
		/// <param name="specieIdFunction">
		///		Identification for the population. Often given as a html hex colour.
		///	</param>
		/// <returns>
		///		New populaiton that contains this new specie.
		///	</returns>
		public static Population CreateNewSpecie( List<Population> species, Func<string> specieIdFunction )
		{
			Population newPop = new Population( specieIdFunction.Invoke() );
			species.Add( newPop );
			return newPop;
		}

		/// <summary>
		/// Find closest species that match this brain
		/// </summary>
		/// <param name="brain">
		///		Brain to match
		/// </param>
		/// <returns>Species that matches else null</returns>
		public Population ClosestSpecies( NEATNetwork brain )
		{
			return ClosestSpecies( this._species, brain );
		}

		/// <summary>
		/// Get the closest species that match a given brain
		/// </summary>
		/// <param name="species">
		///		Species to match
		///	</param>
		/// <param name="brain">
		///		Brain to match to species
		///	</param>
		/// <returns>A population if match otherwise null</returns>
		public static Population ClosestSpecies( List<Population> species, NEATNetwork brain )
		{
			foreach( Population population in species )
			{
				// Doesn't matter which network we get within the population, so grab one at random.
				NEATNetwork random = population.GetRandom();
				if( random == null || NEATNetwork.IsSameSpecies( random, brain ) )
				{
					return population;
				}
			}

			return null;
		}

		/// <summary>
		/// Get all species
		/// </summary>
		/// <returns>All species</returns>
		public List<Population> GetSpecies()
		{
			return this._species;
		}

		/// <summary>
		/// Get top 'N' brains
		/// </summary>
		/// <param name="n">
		///		The number of brains to get.
		///	</param>
		/// <returns>
		///		Top N brains.
		///	</returns>
		public List<NEATNetwork> GetTopBrains( int n )
		{
			// Get all of the networks within the populations of the species.
			List<NEATNetwork> population = new List<NEATNetwork>( this._species.SelectMany( x => x.GetPopulation() ) );

			// Sort - default implemented by fitness.
			population.Sort();
			if( population.Count < n )
			{
				return null;
			}

			return population.GetRange( population.Count - n, n );
		}

		/// <summary>
		/// Get the network distribution for a maxomim capacity on this species.
		/// </summary>
		/// <param name="maxCap">
		///		Maximum population capacity.
		/// </param>
		/// <returns>
		///		Network distribution.
		/// </returns>
		private List<int> GetNextworkDistribution( int maxCap )
		{
			List<float> fitnessDistributions = new List<float>();
			foreach( Population population in this._species )
			{
				fitnessDistributions.Add( population.GetDistribution( this._parameters.Beta ) );
			}

			// First sum. Total shared fitness of the whole population 
			float totalSharedFitness = fitnessDistributions.Sum();

			// Convert and add network distribution.
			return new List<int>( ( totalSharedFitness <= 0.0f ?
				Enumerable.Repeat<int>( 0, fitnessDistributions.Count ) : fitnessDistributions.Select( x => ( int )( ( x / totalSharedFitness ) * maxCap ) ) ) );
		}

		/// <summary>
		/// Get a new network copy or create a crossover network from the given distribution for a population.
		/// </summary>
		/// <param name="amount">
		///		Amount to tell whether we should crossover from the distribution.
		/// </param>
		/// <param name="distribution">
		///		Distribution value.
		/// </param>
		/// <param name="population">
		///		Population.
		/// </param>
		/// <returns>
		///		New Network as a copy or a crossover.
		/// </returns>
		private NEATNetwork GetDistributionNetwork( int amount, int distribution, Population population )
		{
			// Since we're ordered by fitness. Only crossover the amount above our elitsim modifier; otherwise take the last (best) network the population has to offer.
			if( amount <= distribution * this._parameters.Elite )
			{
				// Create a copy of the best network in the population.
				// return new NEATNetwork( population.GetBestBrain() );
				// Last is the best since we are sorting in order to fitness.
				return new NEATNetwork( population.GetLast() );
			}

			// Selection
			NEATNetwork organism1;
			NEATNetwork organism2;
			switch( this._parameters.SelectionMode )
			{
				// logarithmic ranked pick to make sure highest fitness networks have a greater chance of being chosen than the less fit
				case PopulationSelectionMode.LogarithmicRankedPick:
				{
					NEATNetwork[] bestNetworks = population.GetPopulation().ToArray();
					double random = RandomExtensions.StartupSeed.Next( 1, 100 );
					double powerNeeded = Math.Log( bestNetworks.Length - 1, 100 );;
					double logIndex = Math.Abs( ( ( bestNetworks.Length - 1 ) - Math.Pow( random, powerNeeded ) ) );

					organism1 = bestNetworks[ RandomExtensions.StartupSeed.Next( 0, bestNetworks.Length ) ];  // Pick randomly from best networks
					organism2 = bestNetworks[ ( int )logIndex ]; // Use logarithmicly chosen random index from best network
					break;
				}

				// random selection
				case PopulationSelectionMode.Random:
				default:
				{
					organism1 = population.GetRandom();
					organism2 = population.GetRandom();
					break;
				}
			}

			// Perform crossover.
			NEATNetwork network = NEATNetwork.Crossover( organism1, organism2 );
			network.Mutate();
			return network;
		}

		/// <summary>
		/// Create new generation of networks for this species.
		/// </summary>
		/// <param name="maxCap">
		///		Maximum capacity for networks.
		/// </param>
		public void GenerateNewGeneration( int maxCap )
		{
			// Convert and add network distribution.
			List<int> networkDistributions = this.GetNextworkDistribution( maxCap );

			// Total number of organisms (used for distribution)
			int totalNetworks = networkDistributions.Sum();

			// Remove worst after distribution has been gathered.
			foreach( Population population in this._species )
			{
				population.RemoveWorst( this._parameters.RemoveWorst );
			}

			// Equalise the maxCapacity with total number of networks in our distribution.
			int delta = Math.Abs( maxCap - totalNetworks );
			if( delta != 0 )
			{
				// For all of the delta.
				// Add one, to an index in our upper range.
				// | Or |
				// Remove one, from a random index with network count greater than zero (0)
				if( maxCap > totalNetworks )
				{
					for( int i = 0; i < delta; i++ )
					{
						int index = RandomExtensions.StartupSeed.Next( this._species.Count / 2, this._species.Count );
						networkDistributions[ index ]++;
					}
				}
				else
				{
					for( int i = 0; i < delta; i++ )
					{
						// Whilst iterating through delta, the distribution gets decremented.
						// We reevaluate as we set new values.
						int index = networkDistributions.IndexWhereSelect( x => x > 0 ).GetRandom();
						networkDistributions[ index ]--;
					}
				}
			}

			// Index compatibility will exist as the distribution is made in a shared manner.
			// Going backwards over the species, remove if we have gone below 0 distribution or population count.
			for( int i = this._species.Count - 1; i >= 0; i-- )
			{
				if( networkDistributions[ i ] <= 0 || this._species[ i ].GetPopulation().Count == 0 )
				{
					networkDistributions.RemoveAt( i );
					this._species.RemoveAt( i );
				}
			}

			// Create a new species from the current one with the new distribution.
			List<Population> newSpecies = new List<Population>();
			for( int i = 0; i < networkDistributions.Count; i++ )
			{
				int newDistribution = networkDistributions[ i ];
				Population populationOldGen = this._species[ i ];
				newSpecies.Add( new Population( this._species[ i ].GetId() ) );
				for( int j = 0; j < newDistribution; j++ )
				{
					// Get the new network for our distibution. 
					NEATNetwork net = NEATNetwork.Initialize(
						this.GetDistributionNetwork( j, newDistribution, populationOldGen ),
						false );

					// If we can't add another of matching from this population
					if( !newSpecies[ i ].AddIfMatch( net ) )
					{
						// We try to match from any of the populations in this species.
						// Or if no match, we must create a new specie!
						// New specie pointer to list has already been cached in memory, just add to it!
						( Species.ClosestSpecies( newSpecies, net ) ?? Species.CreateNewSpecie( newSpecies, this._specieIdGenFunction ) ).Add( net );
					}
				}
			}

			this._species = newSpecies;
		}

		/// <summary>
		/// The best brain that currently exists
		/// </summary>
		/// <returns>Brain</returns>
		public NEATNetwork GetBestBrain()
		{
			return this._species.Select( x => x.GetBestBrain() ).MaxBy( x => x.GetNetworkFitness() );
		}
	}
}
