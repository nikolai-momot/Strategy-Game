using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cell {

	private StratObj objective;
	private float leftEdge, rightEdge, topEdge, bottomEdge;
	public float height, width;
	public Vector3 position;
	public bool filled;
	public int x, y;
	public List<Cell> neighbors;
	public Cell parent;
	public int[] heat;
	public Color32 color;

	public Cell ( float nodeHeight, float nodeWidth, Vector3 nodePosition, List<StratObj> locations, int players)
	{	
		this.neighbors = new List<Cell>();		//Cell Neighbors
		this.heat = new int[players];			//Cell Heat Value
		this.height = nodeHeight;				//Cell Height
		this.width = nodeWidth;					//Cell Width
		this.position = nodePosition;			//Cell Position, top-left point of the tile

		this.leftEdge = position.x;				//Left edge of the cell
		this.rightEdge = position.x + width;	//Right edge of the cell
		this.topEdge = position.y;				//Top edge of the cell
		this.bottomEdge = position.y + height;	//Bottom edge of the cell

		this.filled = false;					//Occupied by StratObj 

		//Checking if a Strategic Objective is located inside the node
		foreach (StratObj location in locations) {
			if (this.containsVector (location.getMapPosition())) {
				this.setObjective (location);//Set cell StratObj
				break;
			}
		}

		return;
	}

	//Checks the vector3 coordinates are within the cell  borders
	public bool containsVector( Vector3 location ){
		if ( ( location.x >= this.leftEdge && location.x < this.rightEdge ) && ( location.y >= this.topEdge && location.y < this.bottomEdge ) )
			return true;

		return false;
	}

	//Return the cell's objective, if there is one
	public StratObj getObjective(){
		if ( this.filled ) 
			return this.objective;

		return null;
	}

	//Setting the objective inside the cell
	public void setObjective( StratObj newObj ){
		this.objective = newObj;
		this.filled = true;

		return;
	}

	//Setting the cell's coordinates in the cell map
	public void setXY( int newX, int newY ){
		this.x = newX;
		this.y = newY;

		return;
	}

	//Writing out the cell's location and whether or not it has a StratObj inside of it
	public override string ToString(){
		if(this.filled)
			return "Cell at ("+this.x+", "+this.y+") is filled";
		else
			return "Cell at ("+this.x+", "+this.y+")";
	}
	
}
