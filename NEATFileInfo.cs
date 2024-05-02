//--------------------------------------------------------------------------------------------------------->
//
//	File: "NEATFileInfo.cs"
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
	/// NEAT File Info which describes how neural network information is stored as data.
	/// This class is used to map with a JSON object to store and retrieve its information.
	/// </summary>
	public class NEATFileInfo
	{
		/// <summary>
		/// Fitness of the neural network
		/// </summary>
		public double Fitness { get; set; }

		/// <summary>
		/// Number of nodes in the neural network 
		/// </summary>
		public int NodeTotalCount { get; set; }

		/// <summary>
		/// Number of inputs nodes in the neural network
		/// </summary>
		public int NodeInputCount { get; set; }

		/// <summary>
		/// Number of outputs in the neural network
		/// </summary>
		public int NodeOutputCount { get; set; }

		/// <summary>
		/// Number of genes in the genome of the neural network
		/// </summary>
		public int GeneTotalCount { get; set; }

		/// <summary>
		/// Number of genes in the master consultor genome
		/// </summary>
		public int GenomeTotalCount { get; set; }

		/// <summary>
		/// Neural network genome in string form
		/// </summary>
		public string Genome { get; set; }

		/// <summary>
		/// Master consultor neural network in string form
		/// </summary>
		public string ConsultorGenome { get; set; } 
	}
}
