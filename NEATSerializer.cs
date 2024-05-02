//--------------------------------------------------------------------------------------------------------->
//
//	File: "NEATSerializer.cs"
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
	using System.IO;
	using Newtonsoft.Json;

	/// <summary>
	/// Static class for quick serialization of NEAT information for a network, topology and gene makeup.
	/// </summary>
	public static class NEATSerializer
	{
		/// <summary>
		/// Save a network with the gathered NEAT information.
		/// </summary>
		/// <param name="network">
		///		Network.
		/// </param>
		/// <param name="filename">
		///		Filename.
		/// </param>
		public static void Save( IReadOnlyNEATNetwork network, string filename )
		{
			IReadOnlyNEATConsultor consultor = network.GetConsultorReadOnly(); 
			string genome = network.GetGenomeString();
			string consultorGenome = consultor.GetGenomeString(); 
			int nodeTotal = network.GetNodeCount();
			int nodeInputs = network.GetNumberOfInputNodes(); 
			int nodeOutputs = network.GetNumberOfOutputNodes(); 
			int geneTotal = network.GetGeneCount(); 
			int genomeTotal = consultor.GetGeneCount(); 
			float fitness = network.GetNetworkFitness();

			NEATFileInfo fileInfo = new NEATFileInfo
			{
				Fitness = fitness,
				NodeTotalCount = nodeTotal,
				NodeInputCount = nodeInputs,
				NodeOutputCount = nodeOutputs,
				GeneTotalCount = geneTotal,
				GenomeTotalCount = genomeTotal,
				Genome = genome,
				ConsultorGenome = consultorGenome
			};

			Directory.CreateDirectory( Path.GetDirectoryName( filename ) );
			File.WriteAllText(
				filename, 
				JsonConvert.SerializeObject( fileInfo ) );
		}

		/// <summary>
		/// Load NEAT information with the given filename.
		/// </summary>
		/// <param name="filename">
		///		Filename.
		/// </param>
		/// <returns>
		///		NEAT Information.
		/// </returns>
		public static NEATFileInfo Load( string filename )
		{
			if( !File.Exists( filename ) )
			{
				throw new FileNotFoundException( filename );
			}

			return JsonConvert.DeserializeObject<NEATFileInfo>( File.ReadAllText( filename ) );
		}
	}
}
