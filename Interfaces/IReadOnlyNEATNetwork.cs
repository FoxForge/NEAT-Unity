//--------------------------------------------------------------------------------------------------------->
//
//	File: "IReadOnlyNEATNetwork.cs"
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
	/// Interface for reading a NEAT network.
	/// </summary>
	public interface IReadOnlyNEATNetwork
	{
		/// <summary>
		/// Return consultor of this network (read only)
		/// </summary>
		/// <returns>
		///		Consultor.
		///	</returns>
		IReadOnlyNEATConsultor GetConsultorReadOnly();

		/// <summary>
		/// Compile and return genome in a large string to be saved in a database
		/// </summary>
		/// <returns>
		///		Genome string.
		///	</returns>
		string GetGenomeString();

		/// <summary>
		/// Return total number of nodes (perceptrons) in this network
		/// </summary>
		/// <returns>
		///		Number of total nodes.
		///	</returns>
		int GetNodeCount();

		/// <summary>
		/// Return number of input perceptrons
		/// </summary>
		/// <returns>
		///		Number of input nodes.
		///	</returns>
		int GetNumberOfInputNodes();

		/// <summary>
		/// Return number of output perceptrons
		/// </summary>
		/// <returns>
		///		Number of output nodes.
		///	</returns>
		int GetNumberOfOutputNodes();

		/// <summary>
		/// Return number of genes in the genome
		/// </summary>
		/// <returns>
		///		Number of genes in the genome.
		///	</returns>
		int GetGeneCount();

		/// <summary>
		/// Returns the fitness of this network
		/// </summary>
		/// <returns>
		///		Fitness
		///	</returns>
		float GetNetworkFitness();

		/// <summary>
		/// Compile and return gene connections information which include weight, in node, and out node in a 2D array
		/// </summary>
		/// <returns>
		///		Array of gene connections information in a 2D array.
		///	</returns>
		IEnumerable<GeneInfo> GetGeneInfo();

		/// <summary>
		/// Prints all neural network details.
		/// </summary>
		string PrintDetails();
	}
}
