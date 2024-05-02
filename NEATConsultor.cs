//--------------------------------------------------------------------------------------------------------->
//
//	File: "NEATConsultor.cs"
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
	using System.Linq;
	using System.Collections.Generic;

	/// <summary>
	/// Consultor is the master genome of avaliable to all NEATNet's. 
	/// The main job of conultor is to keep track of new genes as they are discovered and increment global innovation number when a new gene is found.
	/// Consultor is extremely important as it helps identify the history of genome based on innovation number. 
	/// Consultor also keeps coefficient information to calculate disparity between 2 networks.
	/// </summary>
	public class NEATConsultor : IReadOnlyNEATConsultor
	{
		/// <summary>
		/// Threshold value that determines whether 2 neural networks belong to the same species
		/// </summary>
		private float _deltaThreshold;

		/// <summary>
		/// Coefficient effect of disjoint genes  
		/// </summary>
		private readonly float _disjointCoefficient;

		/// <summary>
		/// Coefficient effect of  excess genes
		/// </summary>
		private readonly float _excessCoefficient;

		/// <summary>
		/// Coefficient effect of average weight difference between equal genes
		/// </summary>
		private readonly float _averageWeightDifferenceCoefficient;

		/// <summary>
		/// Innovation number.
		/// </summary>
		private int _innovationNumber;

		/// <summary>
		/// Number of inputs in the neural network
		/// </summary>
		private readonly int _numberOfInputs;

		/// <summary>
		/// Number of outputs in the neural network
		/// </summary>
		private readonly int _numberOfOutputs;

		/// <summary>
		/// Consultor gene list
		/// </summary>
		private List<Gene> _geneList;

		/// <summary>
		/// Mutation parameters.
		/// </summary>
		private IMutationParameters _mutationParameters;

		/// <summary>
		/// Creating consultor structure NEAT packet retrived from database and coefficient information from UI
		/// </summary>
		/// <param name="packet">
		///		NEATPacket retrieved from database.
		///	</param>
		/// <param name="deltaThreshold">
		///		Delta threshold to set.
		///	</param>
		/// <param name="disjointCoefficient">
		///		Disjoint coefficient to set.
		///	</param>
		/// <param name="excessCoefficient">
		///		Excess coefficient to set.
		///	</param>
		/// <param name="averageWeightDifferenceCoefficient">
		///		Averange weight difference coefficient to set.
		///	</param>
		public NEATConsultor( 
			NEATFileInfo packet, 
			IMutationParameters mutationParameters, 
			float deltaThreshold,
			float disjointCoefficient, 
			float excessCoefficient, 
			float averageWeightDifferenceCoefficient )
		{
			this._mutationParameters = mutationParameters;
			this._numberOfInputs = packet.NodeInputCount;
			this._numberOfOutputs = packet.NodeOutputCount;
			this._deltaThreshold = deltaThreshold;
			this._disjointCoefficient = disjointCoefficient;
			this._excessCoefficient = excessCoefficient;
			this._averageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;
			this._geneList = new List<Gene>( 
				Gene.CreateGenesFromInfo( 
					packet.ConsultorGenome,
					( a, b ) => this._innovationNumber++,
					1f,
					true ) );
		}

		/// <summary>
		/// Creating consultor structure from scratch with details given from UI
		/// </summary>
		/// <param name="numberOfInputs">
		///		Number of input nodes in the neural network.
		///	</param>
		/// <param name="numberOfOutputs">
		///		Number of outputs nodes in the neural network.
		///	</param>
		/// <param name="deltaThreshold">
		///		Delta threshold to set.
		///	</param>
		/// <param name="disjointCoefficient">
		///		Disjoint coefficient to set.
		///	</param>
		/// <param name="excessCoefficient">
		///		Excess coefficient to set.
		///	</param>
		/// <param name="averageWeightDifferenceCoefficient">
		///		Averange weight difference coefficient to set.
		///	</param>
		public NEATConsultor( 
			IMutationParameters mutationParameters,
			int numberOfInputs, 
			int numberOfOutputs, 
			float deltaThreshold, 
			float disjointCoefficient, 
			float excessCoefficient, 
			float averageWeightDifferenceCoefficient )
		{
			this._mutationParameters = mutationParameters;
			this._numberOfInputs = numberOfInputs;
			this._numberOfOutputs = numberOfOutputs;
			this._deltaThreshold = deltaThreshold;
			this._disjointCoefficient = disjointCoefficient;
			this._excessCoefficient = excessCoefficient;
			this._averageWeightDifferenceCoefficient = averageWeightDifferenceCoefficient;
			this.InitilizeGenome();
		}

		/// <summary>
		/// Initilize genome where every input is connected to every output
		/// </summary>
		private void InitilizeGenome()
		{
			this._geneList = new List<Gene>();

			// Connect every input to every output
			for( int i = 0; i < this._numberOfInputs; i++ )
			{
				for( int j = this._numberOfInputs; j < this._numberOfInputs + this._numberOfOutputs; j++ )
				{
					this.AddNewGene( this._innovationNumber, i, j );
					this._innovationNumber++;
				}
			}
		}

		/// <summary>
		/// Check if a given gene already exists within the gene list.
		/// </summary>
		/// <param name="inNodeID">
		///		Input node to check.
		///	</param>
		/// <param name="outNodeID">
		///		Output node the check.
		///	</param>
		/// <returns>
		///		Innovation number for this gene.
		///	</returns>
		public int GetOrAddGeneExistance( int inNodeID, int outNodeID )
		{
			// Check if this gene exists in the current consultor gene list
			Gene gene = this._geneList.FirstOrDefault( x => x.GetInID() == inNodeID && x.GetOutID() == outNodeID );
			if( gene != null )
			{
				return gene.GetInnovation();
			}
			else
			{
				// Create a new gene, and return the old innovation number pre-increment
				this.AddNewGene( this._innovationNumber, inNodeID, outNodeID );
				return this._innovationNumber++;
			}
		}

		/// <summary>
		/// Create a new gene and add it to the gene list.
		/// </summary>
		/// <param name="inno">
		///		New innovation number.
		///	</param>
		/// <param name="inNodeID">
		///		New input node.
		///	</param>
		/// <param name="outNodeID">
		///		New output node.
		///	</param>
		public void AddNewGene( int inno, int inNodeID, int outNodeID )
		{
			Gene gene = new Gene( inno, inNodeID, outNodeID, 1f, true );
			this._geneList.Add( gene );
		}

		/// <summary>
		/// Set delta threshold value. Used generally to dynamically change delta value at run time.
		/// </summary>
		/// <param name="deltaThreshold">New delta threshold to be set</param>
		public void SetDeltaThreshold( float deltaThreshold )
		{
			this._deltaThreshold = deltaThreshold;
		}

		/// <summary>
		/// Get the disjoint coefficient
		/// </summary>
		/// <returns>
		///		Disjoint coefficient.
		///	</returns>
		public float GetDisjointCoefficient()
		{
			return this._disjointCoefficient;
		}

		/// <summary>
		/// Get the excess coefficient
		/// </summary>
		/// <returns>
		///		Excess coefficient.
		///	</returns>
		public float GetExcessCoefficient()
		{
			return this._excessCoefficient;
		}

		/// <summary>
		/// Get the average weight difference coefficient
		/// </summary>
		/// <returns>
		///		Average weight difference coefficient.
		///	</returns>
		public float GetAverageWeightDifferenceCoefficient()
		{
			return this._averageWeightDifferenceCoefficient;
		}

		/// <summary>
		/// Get the delta threhold value
		/// </summary>
		/// <returns>
		///		Delta threshold.
		///	</returns>
		public float GetDeltaThreshold()
		{
			return this._deltaThreshold;
		}

		/// <summary>
		/// Get the number of genes in the genome
		/// </summary>
		/// <returns>
		///		Numbers of genes in genome.
		///	</returns>
		public int GetGeneCount()
		{
			return this._geneList.Count;
		}

		/// <summary>
		/// Concat all the gene strings in the consultor genome list 
		/// </summary>
		/// <returns>
		///		Genome converted to string.
		///	</returns>
		public string GetGenomeString()
		{
			return string.Join( "_", this._geneList.Select( x => x.GetGeneString() ) );
		}

		/// <summary>
		/// Gets the mutation parameters for the genes and networks.
		/// </summary>
		/// <returns>
		///		Mutation parameters.
		/// </returns>
		public IMutationParameters GetMutationParameters()
		{
			return this._mutationParameters;
		}
	}
}
