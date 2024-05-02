//--------------------------------------------------------------------------------------------------------->
//
//	File: "NEATNetwork.cs"
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
	using System.Text;
	using Forge.Base.Extensions;

	/// <summary>
	/// Handles mutation, crossover, specification, feedforward activation and creation of neural network's genotype. 
	/// </summary>
	public class NEATNetwork : IEquatable<NEATNetwork>, IComparable<NEATNetwork>, INEATNetwork, IReadOnlyNEATNetwork
	{
		#region Members
		/// <summary>
		/// Handles consultor genome sequence
		/// </summary>
		private readonly NEATConsultor _consultor;

		/// <summary>
		/// Number of input perceptrons of neural network (including bias)
		/// </summary>
		private readonly int _numberOfInputs;

		/// <summary>
		/// Number of output perceptrons 
		/// </summary>
		private readonly int _numberOfOutputs;

		/// <summary>
		/// The list of neurons in the network.
		/// </summary>
		private List<Neuron> _network;

		/// <summary>
		/// The non-dynamic set of neurons in the network.
		/// </summary>
		private Neuron[] _networkArray;

		/// <summary>
		/// The index for using which hidden neuron layer.
		/// </summary>
		private int _usedHiddenNeuronIndex;

		/// <summary>
		/// List of the genome sequence for this neural network
		/// </summary>
		private List<Gene> _geneList;

		/// <summary>
		/// List of nodes for this neural network
		/// </summary>
		private List<Node> _nodeList;

		/// <summary>
		/// Id of this neural network.
		/// </summary>
		private Guid _networkId;

		/// <summary>
		/// Get the time lived by the network for it's generation.
		/// </summary>
		private Func<TimeSpan> _getTimeLivedCallback;

		/// <summary>
		/// Fitness of this neural network
		/// </summary>
		private float _networkFitness;
		#endregion // Members

		#region Static Methods
		/// <summary>
		/// Initializes this network by setting test time & reseting fitness, timelived, node values. Finally generating the network from genome.
		/// <see cref="SetNetFitness(float)"/>
		/// <see cref="ClearNodeValues"/>
		/// <see cref="GenerateNeuralNetworkFromGenome"/>
		/// </summary>
		/// <param name="network">
		///		Network.
		/// </param>
		/// <param name="testTime">
		///		Test time.
		/// </param>
		/// <param name="copy">
		///		Do we want to perform a copy of the network before intialization?
		/// </param>
		/// <returns>
		///		Initialized network.
		/// </returns>
		public static NEATNetwork Initialize( NEATNetwork network, bool copy )
		{
			NEATNetwork networkToInitialize = copy ? new NEATNetwork( network ) : network;
			networkToInitialize.SetNetFitness( 0f );
			networkToInitialize.ClearNodeValues();
			networkToInitialize.GenerateNeuralNetworkFromGenome();
			return networkToInitialize;
		}

		/// <summary>
		/// Create a mutated deep copy of a given neural network
		/// </summary>
		/// <param name="network">
		///		Neural network copy to mutate.
		///	</param>
		/// <returns>
		///		Mutated deep copy of the given neural network.
		///	</returns>
		internal static NEATNetwork CreateMutateCopy( NEATNetwork network )
		{
			// Create deep copy of network
			NEATNetwork copy = new NEATNetwork( network );

			// Mutate copy
			copy.Mutate();

			// Perform generation for LSES
			copy.GenerateNeuralNetworkFromGenome();

			// Now return mutated copy
			return copy;
		}

		/// <summary>
		/// Check whether two neural networks belong to the same species based on defined coefficient values in the consultor
		/// </summary>
		/// <param name="net1">
		///		Neural network to compare.
		///	</param>
		/// <param name="net2">
		///		Neural network to compare.
		///	</param>
		/// <returns>
		///		True of false whether they belong to the same species.
		/// </returns>
		internal static bool IsSameSpecies( NEATNetwork net1, NEATNetwork net2 )
		{
			// Hash table to be used to compared genes from the two networks
			GeneHashtable geneHash = new GeneHashtable( net1, net2 );

			// Get consultor (consultor is the same for all neural networks)
			NEATConsultor consultor = net1._consultor;

			// Get the gene hash info.
			GeneHashInfo info = new GeneHashInfo( geneHash );

			// Get one that is larger between the 2 network
			// Handy pointers to the networks.
			List<Gene> geneList1 = net1._geneList;
			List<Gene> geneList2 = net2._geneList;
			int largerGenomeSize = geneList1.Count > geneList2.Count ? geneList1.Count : geneList2.Count;

			// Similarity of the two networks 
			float similarity = CalculateNetworkSimilarity( consultor, info, largerGenomeSize );

			// If similairty is <= to threshold then return true, otherwise false
			return similarity <= consultor.GetDeltaThreshold();
		}

		/// <summary>
		/// Corssover between two parents neural networks to create a child neural network.
		/// Crossover method is as described by the NEAT algorithm.
		/// </summary>
		/// <param name="parent1">
		///		First Neural network parent.
		///	</param>
		/// <param name="parent2">
		///		Second Neural network parent.
		///	</param>
		/// <returns>
		///		Child neural network.
		///	</returns>
		internal static NEATNetwork Crossover( NEATNetwork parent1, NEATNetwork parent2 )
		{
			NEATConsultor consultor = parent1.GetConsultor();
			IMutationParameters mutationParams = consultor.GetMutationParameters();

			// Hash table to be used to compared genes from the two parents. Use special multi array dictionary for union inserts.
			GeneHashtable geneHash = new GeneHashtable( parent1, parent2 );

			// New gene child gene list to be created
			List<Gene> childGeneList = new List<Gene>();

			// Sort the gene hash ascending order this time.
			IOrderedEnumerable<KeyValuePair<int, Gene[]>> sortedGeneHash = geneHash
				.OrderBy( kvp => kvp.Key );

			// Perform crossover in sortedGenehash
			foreach( KeyValuePair<int, Gene[]> kvp in sortedGeneHash )
			{
				Gene gene0 = kvp.Value[ 0 ];
				Gene gene1 = kvp.Value[ 1 ];

				Gene selected = GetSelectedGeneForMutation(
					gene0,
					gene1,
					parent1.GetNetworkFitness(),
					parent2.GetNetworkFitness(),
					out ParentGeneComparison comparison );

				// Get the selected gene f it exists, crossover with type, then add to list.
				if( selected != null )
				{
					// Get the cross over chance for the copy operation. It could be adjusted by lookup configs.
					if( !mutationParams.ParentGeneCrossChanceLookup.TryGetValue( comparison, out double crossOverChance ) )
					{
						crossOverChance = mutationParams.ParentGeneCrossChanceDefault;
					}

					childGeneList.Add( CrossoverCopyGene( crossOverChance, selected, comparison ) );
				}
			}

			// Child node list to be obtained from parent with the most nodes.
			List<Node> childNodeList = parent1.GetNodeCount() > parent2.GetNodeCount() ?
				parent1._nodeList : parent2._nodeList;

			// Provide consultor (consultor is the same or all networks).
			// Provide number of inputs & outputs (which is the same for both parents)
			// Return newly created neural network
			return new NEATNetwork(
				consultor,
				parent1.GetNumberOfInputNodes(),
				parent1.GetNumberOfOutputNodes(),
				childNodeList,
				childGeneList );
		}

		/// <summary>
		/// Get the selected gene from the two given for comparison which is also obtained.
		/// </summary>
		/// <param name="gene1">
		///		Gene 1.
		/// </param>
		/// <param name="gene2">
		///		Gene 2.
		/// </param>
		/// <param name="parentFitness1">
		///		Parent network to <paramref name="gene1"/> fitness.
		/// </param>
		/// <param name="parentFitness2">
		///		Parent network to <paramref name="gene2"/> fitness.
		/// </param>
		/// <param name="comparison">
		///		Gene comparison type.
		/// </param>
		/// <returns>
		///		Selected gene.
		/// </returns>
		private static Gene GetSelectedGeneForMutation( Gene gene1, Gene gene2, float parentFitness1, float parentFitness2, out ParentGeneComparison comparison )
		{
			// Do we have both genes?
			if( gene1 != null && gene2 != null )
			{
				comparison = gene1.GetGeneState() && gene2.GetGeneState()
					? ParentGeneComparison.BothActivated
					: !gene1.GetGeneState() && !gene2.GetGeneState()
						? ParentGeneComparison.BothInactive
						: ParentGeneComparison.Inversed;

				// Randomly pick a gene from eaither parent and create deep copy with crossover, then add to list.
				return RandomExtensions.StartupSeed.NextBoolean() ? gene1 : gene2;
			}
			else
			{
				// Get the selected gene based on the fitness values or presence.
				Gene selected = ( parentFitness1 > parentFitness2 )
					? gene1
					: parentFitness1 < parentFitness2
						? gene2 : gene1 ?? gene2;

				// Return out if no gene selection is valid.
				if( selected == null )
				{
					comparison = ParentGeneComparison.None;
					return null;
				}

				comparison = selected.GetGeneState() ?
					   ParentGeneComparison.DominantActive : ParentGeneComparison.DominantInactive;

				return selected;
			}
		}

		/// <summary>
		/// Created a deep copy of a given gene. 
		/// This gene can be muated with a small chance based on the compare value. 
		/// Deactivated genes have a small chance of being activated based on the compare value. 
		/// </summary>
		/// <param name="copyGene">
		///		Gene to deep copy
		///	</param>
		/// <param name="comparison">
		///		Comparison type to use when activating a gene.
		///	</param>
		/// <returns>
		///		Deep copied gene.
		///	</returns>
		private static Gene CrossoverCopyGene( double crossOverChance, Gene copyGene, ParentGeneComparison comparison )
		{
			// Deep copy gene as new.
			Gene gene = new Gene( copyGene );

			switch( comparison )
			{
				// Chance to Deactivate as both parents had deactivated genes.
				case ParentGeneComparison.BothInactive:
				{
					if( RandomExtensions.StartupSeed.GetChance( crossOverChance ) )
					{
						// Deactivate
						gene.SetGeneState( false );
					}

					break;
				}

				// Chance to Reactivate and parents had conflicting activation status.
				case ParentGeneComparison.Inversed:
				{
					if( RandomExtensions.StartupSeed.GetChance( crossOverChance ) )
					{
						gene.SetGeneState( true );
					}

					break;
				}

				// Chance to SwitchActivation as comparison provides mixed pass-on priority for mutation.
				case ParentGeneComparison.BothActivated:
				case ParentGeneComparison.DominantActive:
				case ParentGeneComparison.DominantInactive:
				{
					if( RandomExtensions.StartupSeed.GetChance( crossOverChance ) )
					{
						gene.SetGeneState( !gene.GetGeneState() );
					}

					break;
				}

				default:
					throw new NotImplementedException();
			}

			// Return new gene
			return gene; 
		}

		/// <summary>
		/// Similarity of the two networks which have been compared using <see cref="GeneHashInfo.Get(GeneHashtable)"/>.
		/// Formula -> Sim = (AVG_DIFF * AVG_COFF) + (((DISJ*DISJ_COFF) + (EXSS*EXSS_COFF)) /GENOME_SIZE)
		/// </summary>
		/// <param name="consultor">
		///		Consultor.
		/// </param>
		/// <param name="info">
		///		Gene hash info.
		/// </param>
		/// <param name="genomeSize">
		///		Genome size.
		/// </param>
		/// <returns></returns>
		private static float CalculateNetworkSimilarity( NEATConsultor consultor, GeneHashInfo info, int genomeSize )
		{
			// wW + (dD/g) + (eE/g)
			// Calculate weight difference disparity
			// Calculate disjoint disparity
			// Calculate excess disparity
			return ( info.AverageWeightDifference * consultor.GetAverageWeightDifferenceCoefficient() ) +
					( ( info.DisjointGenes * consultor.GetDisjointCoefficient() ) / genomeSize ) +
					( ( info.ExcessGenes * consultor.GetExcessCoefficient() ) / genomeSize );
		}
		#endregion // Static Methods

		#region Constructor(s)
		/// <summary>
		/// This is a deep copy constructor. 
		/// Creating neural network structure from deep copying another network
		/// </summary>
		/// <param name="copy">
		///		Neural network to deep copy.
		///	</param>
		public NEATNetwork( NEATNetwork copy )
		{
			// Shallow copy consultor
			this._consultor = copy._consultor;

			// Copy number of inputs & outputs
			this._numberOfInputs = copy._numberOfInputs;
			this._numberOfOutputs = copy._numberOfOutputs;

			// Deep copy node & gene list
			this.CopyNodes( copy._nodeList );
			this.CopyGenes( copy._geneList );

			// Reset fitness related values
			this._getTimeLivedCallback = copy._getTimeLivedCallback;
			this._networkFitness = 0f;
		}

		/// <summary>
		/// Creating neural network structure using neat packet from database.
		/// </summary>
		/// <param name="packet">
		///		Neat packet received from database.
		///	</param>
		/// <param name="consultor">
		///		Consultor with master genome and specification information.
		///	</param>
		public NEATNetwork( NEATFileInfo packet, NEATConsultor consultor )
		{
			// Shallow copy consultor
			this._consultor = consultor;

			// Copy number of inputs & outputs
			this._numberOfInputs = packet.NodeInputCount;
			this._numberOfOutputs = packet.NodeOutputCount;

			// Number of nodes & genes in the network from database
			int numberOfNodes = packet.NodeTotalCount;
			int numberOfgenes = packet.GeneTotalCount;

			// Initialize initial nodes
			this.InitilizeNodes();

			// Run through the left over nodes, since (numberOfInputs + numberOfOutputs) where created by initilize node method
			for( int i = this._numberOfInputs + this._numberOfOutputs; i < numberOfNodes; i++ )
			{
				// Create node with index i as id and will be hidden node. Add to list.
				Node node = new Node( i, NodeType.HiddenNode );
				this._nodeList.Add( node );
			}

			// Using Linq libary and delimiters, parse and spilt string genome from neat packet into float array added in gene list.
			this._geneList = new List<Gene>(
				Gene.CreateGenesFromInfo(
					packet.Genome,
					( a, b ) => this._consultor.GetOrAddGeneExistance( a, b ) ) );

			// Initialize network & fitness values.
			this._networkFitness = 0f;
			this._networkId = Guid.NewGuid();
		}

		/// <summary>
		/// Creating a primitive network structure (every input connect to every output) from provided parameters
		/// </summary>
		/// <param name="consultor">
		///		Consultor with master genome and specification information.
		///	</param>
		/// <param name="numberOfInputs">
		///		Number of input perceptrons.
		///	</param>
		/// <param name="numberOfOutputs">
		///		Number of output perceptrons.
		///	</param>
		/// <param name="time">
		///		Time to test the network
		///	</param>
		public NEATNetwork( NEATConsultor consultor, int numberOfInputs, int numberOfOutputs, TimeSpan time )
		{
			// Shallow copy consultor
			this._consultor = consultor;

			// Copy number of inputs & outputs
			this._numberOfInputs = numberOfInputs;
			this._numberOfOutputs = numberOfOutputs;

			// Copy network & fitness values
			this._networkFitness = 0f;

			// Initialize nodes & genes with network.
			this.InitilizeNodes();
			this.InitilizeGenes();
			this._networkId = Guid.NewGuid();
		}

		/// <summary>
		/// Creating an already designed network structure from given node and gene lists
		/// </summary>
		/// <param name="consultor">
		///		Consultor with master genome and specification information.
		///	</param>
		/// <param name="numberOfInputs">
		///		Number of input perceptrons.
		///	</param>
		/// <param name="numberOfOutputs">
		///		Number of output perceptrons.
		///	</param>
		/// <param name="copyNodes">
		///		Node list to deep copy.
		///	</param>
		/// <param name="copyGenes">
		///		Gene list to deep copy.
		///	</param>
		public NEATNetwork( NEATConsultor consultor, int numberOfInputs, int numberOfOutputs, List<Node> copyNodes, List<Gene> copyGenes )
		{
			// Shallow copy consultor
			this._consultor = consultor;

			// Copy number of inputs & outputs
			this._numberOfInputs = numberOfInputs;
			this._numberOfOutputs = numberOfOutputs;

			// Deep copy node & gene list
			this.CopyNodes( copyNodes );
			this.CopyGenes( copyGenes );

			// Reset network and fitness variables
			this._networkFitness = 0f;
			this._networkId = Guid.NewGuid();
		}
		#endregion // Constructor(s)

		#region Private Methods
		/// <summary>
		/// Initilizing initial node list with given number of input perceptrons which includes the bias node
		/// </summary>
		private void InitilizeNodes()
		{
			// Refresh node list.
			this._nodeList = new List<Node>();

			// Run through number of input perceptrons
			for( int i = 0; i < this._numberOfInputs; i++ )
			{
				// If the last input, make sure it's an input bias; otherwise just an input.
				NodeType nodeType = i == ( this._numberOfInputs - 1 ) ?
					NodeType.InputBiasNode : NodeType.InputNode;

				this._nodeList.Add( new Node( i, nodeType ) );
			}

			// Run through number of output perceptrons and add
			for( int i = this._numberOfInputs; i < this._numberOfInputs + this._numberOfOutputs; i++ )
			{
				this._nodeList.Add( new Node( i, NodeType.OutputNode ) );
			}
		}

		/// <summary>
		/// Initilizing initial gene list with given number of input and output perceptrons to create a primitive genome (all inputs connected to all outputs)
		/// </summary>
		private void InitilizeGenes()
		{
			// Refersh gene list.
			this._geneList = new List<Gene>();

			for( int i = 0; i < this._numberOfInputs; i++ )
			{
				for( int j = this._numberOfInputs; j < this._numberOfInputs + this._numberOfOutputs; j++ )
				{
					// Check if gene exists in consultor and get innovation number
					// Then create gene with default weight of 1.0 and and is active 
					int innovation = this._consultor.GetOrAddGeneExistance( i, j );
					Gene gene = new Gene(
						innovation,
						i,
						j,
						RandomExtensions.StartupSeed.RandomRange( -1.0f, 1.0f ),
						true );

					// Insert gene to correct location in gene list
					this.InsertNewGene( gene );
				}
			}
		}

		/// <summary>
		/// Create node list from deep copying a given node list
		/// </summary>
		/// <param name="copyNodes">
		///		Node list to deep copy.
		///	</param>
		private void CopyNodes( List<Node> copyNodes )
		{
			this._nodeList = new List<Node>();
			this._nodeList.AddRange( copyNodes.Select( x => new Node( x ) ) );
		}

		/// <summary>
		/// Create gene list from deep copying a given gene list
		/// </summary>
		/// <param name="copyGenes">
		///		Gene list to deep copy.
		///	</param>
		private void CopyGenes( List<Gene> copyGenes )
		{
			this._geneList = new List<Gene>();
			this._geneList.AddRange( copyGenes.Select( x => new Gene( x ) ) );
		}

		/// <summary>
		/// Adding a connection between 2 previously unconnected nodes (except no inputs shall ever connect to other inputs)  
		/// </summary>
		private void AddConnection()
		{
			// Total attempts allowed to find two unconnected nodes
			int totalAttemptsAllowed = ( int )Math.Pow( this._nodeList.Count, 2 );

			// Used to check if a connection is found
			bool found = false;

			// If connection is found and greater than 0 attempts left
			while( totalAttemptsAllowed > 0 && found == false )
			{
				// Random node ID's
				int randomNodeId1 = RandomExtensions.StartupSeed.Next( 0, this._nodeList.Count );
				int randomNodeId2 = RandomExtensions.StartupSeed.Next( this._numberOfInputs, this._nodeList.Count );

				// If connection does not exist with random node 1 as in node and random node 2 and out node
				if( !this.ConnectionExists( randomNodeId1, randomNodeId2 ) )
				{
					this.CreateGeneDefault( randomNodeId1, randomNodeId2 );
					found = true;
				}
				else
				{
					// If random node 1 isn't input type and connection does not exist with random node 2 as in node and random node 1 and out node
					switch( this._nodeList[ randomNodeId1 ].GetNodeType() )
					{
						case NodeType.HiddenNode:
						case NodeType.OutputNode:
						{
							if( !this.ConnectionExists( randomNodeId2, randomNodeId1 ) )
							{
								this.CreateGeneDefault( randomNodeId2, randomNodeId1 );
								found = true;
							}

							break;
						}

						default:
							break;
					}
				}

				// If both nodes are equal, only one attemp removed becuase only 1 connection can be made.
				// Otherwise nodes are different, two connections can be made.
				totalAttemptsAllowed -= ( randomNodeId1 == randomNodeId2 ? 1 : 2 );
			}

			// If not found and attempts ran out, then add a node.
			if( !found )
			{
				this.AddNode();
			}
		}

		/// <summary>
		/// Creates and inserts a gene with a given in-Id & out-Id with default weight and activated.
		/// </summary>
		/// <param name="inId">
		///		In-Id.
		/// </param>
		/// <param name="outId">
		///		Out-Id.
		/// </param>
		/// <param name="defaultWeight">
		///		Default weight.
		/// </param>
		private void CreateGeneDefault( int inId, int outId, float defaultWeight = 1.0f )
		{
			// Get the new innovation number
			int innovation = this._consultor.GetOrAddGeneExistance( inId, outId );

			// Create gene which is enabled and 1 as default weight
			Gene gene = new Gene( innovation, inId, outId, defaultWeight, true );

			// Add gene to the gene list
			this.InsertNewGene( gene );
		}

		/// <summary>
		/// Adding a new node between an already existing connection. 
		/// Disable the existing connection, add a node which with connection that becomes the out node to the old connections in node, and a connection with in node to the old connection out node. 
		/// The first new connections gets a weight of 1.
		/// The second second new connections gets a weight of the old weight
		/// </summary>
		private void AddNode()
		{
			// Find a random existing/old gene and disable it.
			Gene gene = this._geneList.Where( x => x.GetGeneState() ).GetRandom();
			gene.SetGeneState( false );

			// Get Ids & weight of deactivated gene.
			int idIn = gene.GetInID();
			int idOut = gene.GetOutID();
			float weight = gene.GetWeight();

			// Create a new hidden node & add to list.
			Node newNode = new Node( this._nodeList.Count, NodeType.HiddenNode );
			this._nodeList.Add( newNode );
			int idNew = newNode.GetNodeID();

			// Old Gene In -> New Gene Id
			this.CreateGeneDefault( idIn, idNew );

			// New Gene Id -> Old Gene Out
			// Pass over the old gene weight too.
			this.CreateGeneDefault( idNew, idOut, weight );
		}

		/// <summary>
		/// Run through all genes and randomly apply various muations with a chance of 1% 
		/// </summary>
		private void MutateWeight()
		{
			// Get the avaliable mutation from the config.
			IMutationParameters mutationParams = this._consultor.GetMutationParameters();
			IList<GeneMutationFlags> avaliableMutations = new List<GeneMutationFlags>();
			foreach( GeneMutationFlags value in Enum.GetValues( typeof( GeneMutationFlags ) ) )
			{
				if (mutationParams.GeneMutateFlags.HasFlag(value))
				{
					avaliableMutations.Add(value);
				}
			}

			foreach( Gene gene in this._geneList )
			{
				// Chance the mutation and get a random mutation at that.
				if( RandomExtensions.StartupSeed.GetChance(
					mutationParams.GeneMutateChance ) )
				{
					GeneMutationFlags mutationType = avaliableMutations.GetRandom();
					switch( mutationType )
					{
						// Flip the sign of the weight.
						case GeneMutationFlags.FlipWeight:
						{
							float weight = gene.GetWeight();
							weight *= -1f;
							gene.SetWeight( weight );
							break;
						}

						// Flips the gene state
						case GeneMutationFlags.FlipGeneState:
						{
							bool geneState = gene.GetGeneState();
							gene.SetGeneState( !geneState );
							break;
						}

						// Randomly set the weight between -1 and 1.
						case GeneMutationFlags.RandomSetWeight:
						{
							float weight = RandomExtensions.StartupSeed.RandomRange( -1f, 1f );
							gene.SetWeight( weight );
							break;
						}

						// Randomly increase by 0% to 100%
						case GeneMutationFlags.RandomIncreaseWeight:
						{
							float factor = RandomExtensions.StartupSeed.RandomRange( 0f, 1f ) + 1f;
							float weight = gene.GetWeight() * factor;
							gene.SetWeight( weight );
							break;
						}

						// Randomly decrease by 0% to 100%
						case GeneMutationFlags.RandomDecreaseWeight:
						{
							float factor = RandomExtensions.StartupSeed.RandomRange( 0f, 1f );
							float weight = gene.GetWeight() * factor;
							gene.SetWeight( weight );
							break;
						}

						default:
							break;
					}
				}
			}
		}

		/// <summary>
		/// Check if a connection exists in this gene list
		/// </summary>
		/// <param name="inID">
		///		In node in gene.
		///	</param>
		/// <param name="outID">
		///		Out node in gene.
		///	</param>
		/// <returns>
		///		True or false if connection exists in gene list.
		/// </returns>
		private bool ConnectionExists( int inID, int outID )
		{
			return this._geneList.FirstOrDefault(
				x => x.GetInID() == inID && x.GetOutID() == outID ) != null;
		}

		/// <summary>
		/// Insert new gene into its proper location the gene list.
		/// All genes are orders in asending order based on their innovation number.
		/// </summary>
		/// <param name="gene">
		///		Gene to inset into the gene list.
		///	</param>
		private void InsertNewGene( Gene gene )
		{
			// Get innovation number
			int innovation = gene.GetInnovation();

			// Get insert index
			int insertIndex = this.FindInnovationInsertIndex( innovation );

			// If insert index is equal to the size of the genome then add gene.
			if( insertIndex == this._geneList.Count )
			{
				this._geneList.Add( gene );
			}
			else
			{
				// Otherwise, add gene to the given insert index location
				this._geneList.Insert( insertIndex, gene );
			}
		}

		/// <summary>
		/// Find the correct location to insert a given innovation number.
		/// Using bianry search to find insert location.
		/// </summary>
		/// <param name="innovationNumber">
		///		Innovation to insert.
		///	</param>
		/// <returns>
		///		Location to insert the innovation number.
		///	</returns>
		private int FindInnovationInsertIndex( int innovationNumber )
		{
			int numberOfGenes = this._geneList.Count;
			switch( numberOfGenes )
			{
				case 0:
				{
					// No genes. First index.
					return 0;
				}

				case 1:
				{
					// If innovation is greater than the girst gene's innovation then insert into second index.
					return innovationNumber > this._geneList[ 0 ].GetInnovation() ? 1 : 0;
				}

				default:
				{
					// Gather Indicies
					int startIndex = 0;
					int endIndex = numberOfGenes - 1;

					// Loop while limits are not within 1 delta
					while( endIndex - startIndex != 1 )
					{
						// Find middle index & innovation (middle of start and end)
						int middleIndex = ( endIndex + startIndex ) / 2;
						int middleInnovation = this._geneList[ middleIndex ].GetInnovation();

						// Innovation is greater than middle innovation, midpoint shall be the start of next cycle.
						if( innovationNumber > middleInnovation )
						{
							startIndex = middleIndex;
						}
						else
						{
							// Innovation is less than middle innovation, midpoint shall be the end of next cycle.
							endIndex = middleIndex;
						}
					}

					// Get innovation limits.
					int endInnovation = this._geneList[ endIndex ].GetInnovation();
					int startInnovation = this._geneList[ startIndex ].GetInnovation();

					// Innovation is less than start innovation then get start index
					if( innovationNumber < startInnovation )
					{
						return startIndex;
					}
					else if( innovationNumber > endInnovation )
					{
						// If innovation is greater than end innovation then get end index +1
						return endIndex + 1;
					}
					else
					{
						// Otherwise end index
						return endIndex;
					}
				}
			}
		}
		#endregion // Private Methods

		#region Public Methods
		/// <summary>
		/// Generates the neural network based from the genomes.
		/// </summary>
		public void GenerateNeuralNetworkFromGenome()
		{
			this._network = new List<Neuron>();

			// Get the maximum value of in or out Id as our hidden layer index.
			// Then increase by 1 for new hidden layer.
			this._usedHiddenNeuronIndex = Math.Max(
				this._geneList.Max( x => x.GetInID() ),
				this._geneList.Max( x => x.GetOutID() ) ) + 1;

			// Create new hidden layer.
			for( int i = 0; i < this._usedHiddenNeuronIndex; i++ )
			{
				Neuron neuron = new Neuron( i, 0f );
				this._network.Add( neuron );
			}

			// Sort the network Ids & genes by OutId.
			this._network.Sort( ( x, y ) => x.Id.CompareTo( y.Id ) );
			this._geneList.Sort( ( x, y ) => x.GetOutID().CompareTo( y.GetOutID() ) );

			// Add incoming genes for those which are activated.
			for( int i = 0; i < this._geneList.Count; i++ )
			{
				Gene gene = this._geneList[ i ];
				if( gene.GetGeneState() )
				{
					this._network[ gene.GetOutID() ].AddIncommingGene( gene );
				}
			}

			// Sort the network cache the array by InIds
			this._networkArray = this._network.ToArray();
			for( int i = 0; i < this._networkArray.Length; i++ )
			{
				this._networkArray[ i ].SortIncommingGenes( ( x, y ) => x.GetInID().CompareTo( y.GetInID() ) );
				this._networkArray[ i ].CacheIncommingArray();
			}

			// Reset back to sorted in terms of innovation number
			this._geneList.Sort( ( x, y ) => x.GetInnovation().CompareTo( y.GetInnovation() ) );
		}

		/// <summary>
		/// Gets the genes of this network.
		/// </summary>
		/// <returns>
		///		Genes.
		/// </returns>
		public IEnumerable<Gene> GetGenes()
		{
			return this._geneList;
		}

		/// <summary>
		/// Set ID of the network
		/// </summary>
		/// <param name="id">
		///		Network ID to set
		///	</param>
		public void SetNetID( Guid id )
		{
			this._networkId = id;
		}

		/// <summary>
		/// Set fitness to given a value
		/// </summary>
		/// <param name="value">
		///		Fitness for the network.
		///	</param>
		public void SetNetFitness( float value )
		{
			this._networkFitness = value;
		}

		/// <summary>
		/// Add given fitness to the current fitness for this network.
		/// </summary>
		/// <param name="value">
		///		Fitness value to add.
		///	</param>
		public void AddNetFitness( float value )
		{
			this._networkFitness += value;
		}

		/// <summary>
		/// Return consultor of this network
		/// </summary>
		/// <returns>
		///		Consultor.
		///	</returns>
		public NEATConsultor GetConsultor()
		{
			return this._consultor;
		}

		/// <summary>
		/// Mutating this neural network
		/// </summary>
		public void Mutate()
		{
			// We could add these behind some flags for topology mutation much like the genes
			// But that seems overkill for parameter sake. Lets just chance both of them,
			// we'll hardly want to restructure the topology in preset ways to categorize as flags other than these.
			// We could however flags the operation, either AND chance or OR chance.
			// Or we provide lookup chances to each topological action. Again seems overkill.
			Queue<Action> topologyMutateActions = new Queue<Action>();
			topologyMutateActions.Enqueue( () => this.AddConnection() );
			topologyMutateActions.Enqueue( () => this.AddNode() );

			// 1 chance per topology mutation
			while( topologyMutateActions.Count > 0 )
			{
				Action action = topologyMutateActions.Dequeue();
				if( RandomExtensions.StartupSeed.GetChance( 
					this._consultor.GetMutationParameters().TopologyMutateChance ) )
				{
					action.Invoke();
				}
			}

			// Always mutate the weight.
			this.MutateWeight();
		}

		/// <summary>
		/// Set all node values to 0
		/// </summary>
		public void ClearNodeValues()
		{
			this._nodeList.ForEach( x => x.SetValue( 0f ) );
		}

		/// <inheritdoc/>
		public float[] FireNet( float[] inputs )
		{
			// Set the values for all the nodes on the first layer.
			for( int i = 0; i < inputs.Length; i++ )
			{
				this._networkArray[ i ].Value = inputs[ i ];
			}

			// Create output and tempvalue arrays for calculation.
			float[] output = new float[ this._numberOfOutputs ];
			float[] tempValues = new float[ this._networkArray.Length ];
			for( int i = 0; i < tempValues.Length; i++ )
			{
				tempValues[ i ] = this._networkArray[ i ].Value;
			}

			// Set the last node in the layer value and gather the weights by setting values from their activation function.
			this._networkArray[ this._numberOfInputs - 1 ].Value = 1f;
			for( int i = 0; i < this._networkArray.Length; i++ )
			{
				float value = 0;
				Neuron neuron = this._networkArray[ i ];
				Gene[] incommingArray = neuron.IncommingArray;
				if( incommingArray.Length > 0 )
				{
					for( int j = 0; j < incommingArray.Length; j++ )
					{
						if( incommingArray[ j ].GetGeneState() )
						{
							// Get the value from the weight.
							value += ( incommingArray[ j ].GetWeight() * tempValues[ incommingArray[ j ].GetInID() ] );
						}
					}

					// Set the value from the activation function.
					neuron.Value = ( float )Math.Tanh( value );
				}
			}

			// Obtain the output from the last layer in the network for the respective node.
			for( int i = 0; i < output.Length; i++ )
			{
				output[ i ] = this._networkArray[ i + this._numberOfInputs ].Value;
			}

			return output;
		}

		/// <summary>
		/// Sets the time lived callback for the runner to change as the tests are conducted.
		/// </summary>
		/// <param name="getTimeLived">
		///		Get time lived callback.
		/// </param>
		internal void SetTimeLivedCallback(Func<TimeSpan> getTimeLived)
		{
			this._getTimeLivedCallback = getTimeLived;
		}

		/// <inheritdoc/>
		public float GetNetworkFitness()
		{
			return this._networkFitness; 
		}

		/// <inheritdoc/>
		public TimeSpan GetTimeLived()
		{
			return this._getTimeLivedCallback == null ? TimeSpan.Zero : this._getTimeLivedCallback();
		}

		/// <inheritdoc/>
		public Guid GetNetId()
		{
			return this._networkId;
		}

		/// <inheritdoc/>
		public int GetNodeCount()
		{
			return this._nodeList.Count;
		}

		/// <inheritdoc/>
		public int GetGeneCount()
		{
			return this._geneList.Count;
		}

		/// <inheritdoc/>
		public int GetNumberOfInputNodes()
		{
			return this._numberOfInputs;
		}

		/// <inheritdoc/>
		public int GetNumberOfOutputNodes()
		{
			return this._numberOfOutputs;
		}

		/// <inheritdoc/>
		public IReadOnlyNEATConsultor GetConsultorReadOnly()
		{
			return this._consultor;
		}

		/// <inheritdoc/>
		public IEnumerable<GeneInfo> GetGeneInfo()
		{
			return this._geneList.Select( x => new GeneInfo( x ) );
		}

		/// <inheritdoc/>
		public string GetGenomeString()
		{
			return string.Join( "_", this._geneList.Select( x => x.GetGeneString() ) );
		}

		/// <inheritdoc/>
		public string PrintDetails()
		{
			StringBuilder builder = new StringBuilder();
			this._nodeList.ForEach( x =>
				builder.AppendLine(
					string.Format( "Node Network: {0} Node: {1} Type: {2}",
					this.GetNetId(),
					x.GetNodeID(),
					x.GetNodeType() ) ) );

			this._geneList.ForEach( x =>
				builder.AppendLine(
					string.Format( "Gene Network: {0} Innovation: {1} In: {2} Out: {3} State: {4} Weight {5}",
					this.GetNetId(),
					x.GetInnovation(), x.GetInID(), x.GetOutID(), x.GetGeneState(), x.GetWeight() ) ) );

			return builder.ToString();
		}

		/// <inheritdoc/>
		public bool Equals( NEATNetwork other )
		{
			return other == null ? false : other._networkId == this._networkId;
		}

		/// <inheritdoc/>
		public int CompareTo( NEATNetwork other )
		{
			if( this._networkFitness > other._networkFitness )
			{
				return 1;
			}

			if( this._networkFitness < other._networkFitness )
			{
				return -1;
			}

			return 0;
		}
		#endregion // Public Methods
	}
}
