using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StratNode
{
	private StratObj objective;
	public float leftEdge, rightEdge, topEdge, bottomEdge;
	private float height, width;
	public Vector3 position;	//top-right point of the tile
	public bool filled;
	public int x, y;
	public List<StratNode> neighbors;
	public StratNode parent;

	/*public MapNode ( float nodeHeight, float nodeWidth, Vector3 nodePosition)
	{
		filled = false;
		height = nodeHeight;
		width = nodeWidth;
		position = nodePosition;
		leftEdge = position.x;
		rightEdge = position.x + width;
		topEdge = position.y;
		bottomEdge = position.y + height;
	}*/

	public StratNode ( float nodeHeight, float nodeWidth, Vector3 nodePosition, List<StratObj> locations)
	{	
		neighbors = new List<StratNode>();

		//Recording hight, width and location of node
		height = nodeHeight;
		width = nodeWidth;
		position = nodePosition;

		//Determining the edges of the node
		leftEdge = position.x;
		rightEdge = position.x + width;
		topEdge = position.y;
		bottomEdge = position.y + height;

		//Checking if a Strategic Objective is located inside the node
		locations.ForEach (delegate( StratObj location ) {
			if( containsObjective(location) ){
				setObjective(location);
				this.filled = true;
				return;
			}

		});

		//if no Strategic objective is located inside the node, record it as empty
		filled = false;

	}

	public bool containsObjective( StratObj location ){
		if ( (location.MapPosition.x >= leftEdge && location.MapPosition.x < rightEdge) && (location.MapPosition.y >= topEdge && location.MapPosition.y < bottomEdge) )
			return true;

		return false;
	}

	public StratObj getObjective(){
		if (filled) 
			return objective;
		
		return null;
	}

	public void setObjective( StratObj newObj ){
		objective = newObj;
		filled = true;
	}

	public void setXY( int newX, int newY ){
		x = newX;
		y = newY;
	}

	/*public int CompareTo(StratNode that)
	{
		if (this.fValue >  that.fValue) return -1;
		if (this.fValue == that.fValue) return 0;
		return 1;
	}*/
}
