using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cell {
	private StratObj objective;
	public float leftEdge, rightEdge, topEdge, bottomEdge;
	public float height, width;
	public Vector3 position;	//top-right point of the tile
	public bool filled;
	//public bool obstructed;
	public int x, y;
	public List<Cell> neighbors;
	public Cell parent;
	public int[] heat;

	public Cell ( float nodeHeight, float nodeWidth, Vector3 nodePosition, List<StratObj> locations, int players)
	{	
		this.neighbors = new List<Cell>();
		this.heat = new int[players];
		//Recording hight, width and location of node
		this.height = nodeHeight;
		this.width = nodeWidth;
		this.position = nodePosition;

		//Determining the edges of the node
		this.leftEdge = position.x;
		this.rightEdge = position.x + width;
		this.topEdge = position.y;
		this.bottomEdge = position.y + height;

		//Checking if a Strategic Objective is located inside the node
		locations.ForEach (delegate( StratObj location ) {
			if( containsVector(location.MapPosition) ){
				setObjective(location);
				this.filled = true;
				return;
			}

		});

		//if no Strategic objective is located inside the node, record it as empty
		this.filled = false;

	}

	public bool containsVector( Vector3 location ){
		if ( (location.x >= leftEdge && location.x < rightEdge) && (location.y >= topEdge && location.y < bottomEdge) )
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

	public override string ToString(){
		if(this.filled)
			return "Cell at ("+this.x+", "+this.y+") is filled";
		else
			return "Cell at ("+this.x+", "+this.y+")";
	}
	
}
