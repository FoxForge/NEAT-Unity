//--------------------------------------------------------------------------------------------------------->
//
//	File: "AgentOriginInfo.cs"
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
	/// Structure containg an agent's origin information for the network and species.
	/// </summary>
	internal class AgentOriginInfo
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="specieIndex">
		///		Specie index.
		/// </param>
		/// <param name="networkIndex">
		///		Network index.
		/// </param>
		public AgentOriginInfo(int specieIndex, int networkIndex) 
		{
			this.SpecieIndex = specieIndex;
			this.NetworkIndex = networkIndex;
		}

		/// <summary>
		/// Get the specie index.
		/// </summary>
		public int SpecieIndex { get; }

		/// <summary>
		/// Get the network index.
		/// </summary>
		public int NetworkIndex { get; }
	}
}
