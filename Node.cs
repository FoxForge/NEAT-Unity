//--------------------------------------------------------------------------------------------------------->
//
//	File: "Node.cs"
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

	/// <summary>
	/// Acts like the individual neuron of a network.
	/// </summary>
	public class Node
	{
		/// <summary>
		/// Node Id.
		/// </summary>
		private readonly int _id;

		/// <summary>
		/// Node type.
		/// </summary>
		private readonly NodeType _type;

		/// <summary>
		/// Node value.
		/// </summary>
		private float _value;

		/// <summary>
		/// Deep copy a given copy node
		/// </summary>
		/// <param name="copy">
		///		The node to copy
		///	</param>
		public Node( Node copy )
		{
			this._id = copy._id;
			this._type = copy._type;

			// if this is the bias node set it to 1, else reset value to 0
			if( this._type == NodeType.InputBiasNode )
			{
				this._value = 1f;
			}
			else
			{
				this._value = 0f;
			}
		}

		/// <summary>
		/// Create a node with an id and type
		/// </summary>
		/// <param name="ID">ID of this node</param>
		/// <param name="type">Type of this node</param>
		public Node( int ID, NodeType type )
		{
			this._id = ID;
			this._type = type;

			if( this._type == NodeType.InputBiasNode )
			{
				this._value = 1f;
			}
			else
			{
				this._value = 0f;
			}
		}

		/// <summary>
		/// Get the ID of this node.
		/// </summary>
		/// <returns>Node ID</returns>
		public int GetNodeID()
		{
			return this._id;
		}

		/// <summary>
		/// Get the type of this node
		/// </summary>
		/// <returns>Node type</returns>
		public NodeType GetNodeType()
		{
			return this._type;
		}

		/// <summary>
		/// Get node value
		/// </summary>
		/// <returns>Node value</returns>
		public float GetValue()
		{
			return this._value;
		}

		/// <summary>
		/// Set value of the node if it's not a biased node
		/// </summary>
		/// <param name="value">Value to set</param>
		public void SetValue( float value )
		{
			if( this._type != NodeType.InputBiasNode )
			{
				this._value = value;
			}
		}

		/// <summary>
		/// Run the value through hyperbolic tangent approx
		/// </summary>
		public void Activation()
		{
			this._value = ( float )Math.Tanh( this._value );
			//value= 1.0f / (1.0f + (float)Math.Exp(-value));
		}
	}
}
