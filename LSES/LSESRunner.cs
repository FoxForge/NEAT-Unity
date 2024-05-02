//--------------------------------------------------------------------------------------------------------->
//
//	File: "LSESRunner.cs"
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
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Forge.Base.Extensions;

	/// <summary>
	/// Internal simulation running class which is responsible for environment cycling and specie management.
	/// </summary>
	internal abstract class LSESRunner : ILSESRunner, ILSESRunFinishedCallback
	{
		/// <summary>
		/// Lock object for finshing tests and getting generation properties.
		/// </summary>
		private readonly object _finishedlock;

		/// <summary>
		/// Table of activated agents within the enviornment by their indicies within the species.
		/// </summary>
		private readonly ConcurrentDictionary<INEATAgent, AgentOriginInfo> _agentsToOriginNetworks;

		/// <summary>
		/// The specie Id function given for the simulation to distinctify the species.
		/// </summary>
		private readonly Func<string> _specieIdFunction;

		/// <summary>
		/// Get the generations remaining.
		/// </summary>
		private int _generationsRemaining;

		/// <summary>
		/// Get the current generation number.
		/// </summary>
		private int _generationNumber;

		/// <summary>
		/// Test counter - the number of simulations or generations which have run.
		/// </summary>
		protected int _testCounter;

		/// <summary>
		/// The current population size of the simulation.
		/// </summary>
		protected int _currentTotalPopulationSize;

		/// <summary>
		/// Best network obtained from the most recent simulation.
		/// </summary>
		private NEATNetwork _bestNework;

		/// <summary>
		/// Token source for when a run has finished.
		/// </summary>
		private CancellationTokenSource _finishedTokenSource;

		/// <summary>
		/// Stopwatch for keeping elapsed time of generation runs.
		/// </summary>
		private Stopwatch _generationStopwatch;

		/// <summary>
		/// Get the overall species for this simulation.
		/// </summary>
		protected Species Species { get; private set; }

		/// <summary>
		/// Get the parameters for LSES.
		/// </summary>
		protected ILSESParameters Parameters { get; private set; }

		/// <summary>
		/// Get the generation callbacks for the environment.
		/// </summary>
		protected ILSESGeneration Environment { get; private set; }

		/// <inheritdoc/>
		public int GenerationsRemaining 
		{
			get
			{
				lock(this._finishedlock)
				{
					return this._generationsRemaining;
				}
			}
			private set
			{
				lock(this._finishedlock)
				{
					this._generationsRemaining = value;
				}
			}
		}

		/// <inheritdoc/>
		public int GenerationNumber 
		{
			get
			{
				lock(this._finishedlock)
				{
					return this._generationNumber;
				}
			} 
			private set
			{
				lock(this._finishedlock)
				{
					this._generationNumber = value;
				}
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parameters">
		///		Parameters.
		/// </param>
		/// <param name="environment">
		///		Callbacks.
		/// </param>
		/// <param name="specieIdFunction">
		///		Specie Id function.
		/// </param>
		protected LSESRunner( ILSESParameters parameters, ILSESGeneration environment, Func<string> specieIdFunction )
		{
			this.Parameters = parameters;
			this.Environment = environment;
			this._finishedlock = new object();
			this._specieIdFunction = specieIdFunction ?? ( () => Guid.NewGuid().ToString() );
			this._agentsToOriginNetworks = new ConcurrentDictionary<INEATAgent, AgentOriginInfo>();
		}

		/// <summary>
		/// Create a new intial network for the environment based on the type of simulation runner instance.
		/// </summary>
		/// <returns>
		///		New initial NEAT Network for the simulation environment.
		/// </returns>
		protected abstract NEATNetwork CreateInitialNetwork();

		/// <summary>
		/// Get a simplified set of specie data for this simulation.
		/// </summary>
		/// <returns></returns>
		public SpeciesInfo GetSpeciesInfo()
		{
			return new SpeciesInfo( this.Species );
		}

		/// <inheritdoc/>
		public void Reset()
		{
			this.GenerationNumber = 0;
			this.Species = new Species( this.Parameters, this._specieIdFunction );
			this.GenerateInitialSpeciesNetworks();
		}

		/// <inheritdoc/>
		public IReadOnlyNEATNetwork GetBestNetwork()
		{
			return this._bestNework;
		}

		/// <inheritdoc/>
		public bool ActionGeneration( int generations )
		{
			if( this.GenerationsRemaining > 0 )
			{
				return false;
			}

			// Number of generation to run
			// Generate population with neural networks
			this.GenerationsRemaining = generations;
			this.GeneratePopulation();
			return true;
		}

		/// <inheritdoc/>
		public void OnFinished( INEATAgent agent )
		{
			this.OnFinishedImpl( agent, invokeFinishCallback: false );
		}

		/// <summary>
		/// <see cref="OnFinished(INEATAgent)"/> implementation with the option to invoke the callback registered externally.
		/// </summary>
		/// <param name="agent">
		///		Agent.
		/// </param>
		/// <param name="invokeFinishCallback">
		///		True if we should invoke callback if we are removing the agent instance; otherwise false.
		/// </param>
		private void OnFinishedImpl( INEATAgent agent, bool invokeFinishCallback )
		{
			// lock since sharing resources
			lock(this._finishedlock)
			{
				// Set the fitness value of the network upon calculation on finish.
				if( this._agentsToOriginNetworks.TryGetValue( agent, out AgentOriginInfo info ) )
				{
					this.Species.GetSpecies()[ info.SpecieIndex ].
						GetPopulation()[ info.NetworkIndex ].
						SetNetFitness( agent.CalculateFitness() );

					// Only invoke if we are being used by OnFinish from our task, leave it up to the agents to clean their environments up otherwise.
					if( invokeFinishCallback )
					{
						agent.OnFinished();
					}

					this._agentsToOriginNetworks.Remove( agent );
				}

				if( this._agentsToOriginNetworks.None() )
				{
					this.TestFinished();
				}
			}
		}

		/// <summary>
		/// Calculate next size of species population using explicit fitness sharing distribution and breed previous generation asexually (elite) and sexually.
		/// </summary>
		private void TestFinished()
		{
			// Dispose so we don't call on finished again if we are waiting to call it.
			this._finishedTokenSource.Dispose();

			// Save the best network for when we want to serialize it out.
			List<NEATNetwork> top = this.Species.GetTopBrains( 1 );
			if( top != null && top.Count > 0 )
			{
				this._bestNework = new NEATNetwork( top[ 0 ] );
				this._bestNework.SetNetFitness( top[ 0 ].GetNetworkFitness() );
			}

			// Do we still need to do more generations?
			if( this.GenerationsRemaining > 0 )
			{
				// Generate population with neural networks
				this.GenerationNumber++;
				this.Species.GenerateNewGeneration( this.Parameters.PopulationSize );
				this.GenerationsRemaining--;
				this.GeneratePopulation();
			}

			// Generation complete callback.
			this.Environment.OnGenerationComplete();
		}

		/// <summary>
		/// Create an equivalent 2D array of indicies for every index of specie count and population count within the species.
		/// </summary>
		/// <returns>
		///		List of all specie and population index pairs.
		/// </returns>
		private IList<AgentOriginInfo> GetSpecieToNetworkInfo()
		{
			List<AgentOriginInfo> originInfo = new List<AgentOriginInfo>();
			List<Population> species = this.Species.GetSpecies();
			for( int i = 0; i < species.Count; i++ )
			{
				int populationCount = species[ i ].GetPopulation().Count;
				for( int j = 0; j < populationCount; j++ )
				{
					originInfo.Add( new AgentOriginInfo( i, j ) );
				}
			}

			return originInfo;
		}

		/// <summary>
		/// Generate tester gameobjects given the species list
		/// </summary>
		private void GeneratePopulation()
		{
			// Perform pre/before generation events.
			this.Environment.BeforeGeneration();

			// 2D id
			this._agentsToOriginNetworks.Clear();

			// Save species count
			List<Population> species = this.Species.GetSpecies();
			IList<AgentOriginInfo> originInfo = this.GetSpecieToNetworkInfo();

			originInfo.RemoveRandomUntilEmtptyWithIndex( ( info, spawnIndex ) =>
			{
				// Get the specie id encoding.
				string specieId = species[ info.SpecieIndex ].GetId();

				// Get the network.
				NEATNetwork network = species[ info.SpecieIndex ].GetPopulation()[ info.NetworkIndex ];

				// Ensure the network has access to get it's time lived for this generation. This one is internal.
				network.SetTimeLivedCallback( () => this._generationStopwatch.Elapsed );

				// We dont need the actual agent to contain the specie id. Therefore keep this seperated out into the contruction of the object.
				// As it will be in other system.
				INEATAgent agent = this.Environment.CreateAgent( specieId, spawnIndex );

				// We don't want to copy the network, as later we may want to mutate the item in the species list.
				// However we can copy the wrapping object.
				this._agentsToOriginNetworks.TryAdd( agent, info );

				/// Do NOT put these into the <see cref="ILSESGeneration.CreateAgent(string, int)"/>
				/// The construction of the object should be kept seperated from the activation to help user environment relationships with their network variables.
				/// They will have to implement something to take the network anyway so we'll help them with the interface.
				agent.Activate( specieId, this, network );
			} );

			// Start the stopwatch for global network life time.
			this._generationStopwatch.Start();

			// Invoke the callback.
			this.Environment.AfterGeneration();

			// Call the Onfinished after the test Time for all agents.
			this._finishedTokenSource = new CancellationTokenSource( this.Parameters.GenerationTestTime );
			this._finishedTokenSource.Token.Register( () =>
			{
				// We have to evaluate the list as OnFinished removes from it.
				IEnumerable<INEATAgent> agents = this._agentsToOriginNetworks.Keys.ToList();
				foreach( INEATAgent agent in agents )
				{
					this.OnFinishedImpl( agent, invokeFinishCallback: true );
				}
			} );
		}

		/// <summary>
		/// Create a new intial network for all of the population size.
		/// </summary>
		private void GenerateInitialSpeciesNetworks()
		{
			// Run through population size
			for( int i = 0; i < this.Parameters.PopulationSize; i++ )
			{
				// Create net with consultor, perceptron information and test time
				NEATNetwork network = this.CreateInitialNetwork();

				// Add to existing species or create a new species and add to it.
				( this.Species.ClosestSpecies( network ) ?? this.Species.CreateNewSpecie() ).Add( network );
			}
		}
	}
}
