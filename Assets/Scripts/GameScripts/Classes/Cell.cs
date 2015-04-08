using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cell{

	public enum contents {
		Empty, 
		Obstructed, 
		Strategic
	};

	private StratObj objective;
	public float leftEdge, rightEdge, topEdge, bottomEdge;
	private float height, width;
	public Vector3 position;	//top-left point of the tile
	public bool filled;
	public int x, y;
	public List<Cell> neighbors;
	public Cell parent;
	public contents content;

	public Cell ( int i, int j, float nodeHeight, float nodeWidth, Vector3 nodePosition, List<StratObj> locations, List<GameObject> obstacles)
	{	
		this.neighbors = new List<Cell>();
		//if no Strategic objective is located inside the node, record it as empty
		contents content = contents.Empty;
		//Recording hight, width and location of node
		this.height = nodeHeight;
		this.width = nodeWidth;
		this.position = nodePosition;

		//Setting Cell Position
		this.x = i;
		this.y = j;

		//Determining the edges of the node
		this.leftEdge = position.x;
		this.rightEdge = position.x + width;
		this.topEdge = position.y;
		this.bottomEdge = position.y - height;

		//Checking if a Strategic Objective is located inside the node
		locations.ForEach (delegate( StratObj location ) {
			if( containsObjective(location) ){
				setObjective(location);
				this.content = contents.Strategic;
				//return;
			}

		});

		obstacles.ForEach (delegate( GameObject obstacle ) {
			if( containsObstacle( obstacle ) ){
				this.content = contents.Obstructed;
				//return;
			}
		});

		Debug.Log("Cell ("+ x + "," + y + ") is "+this.content.ToString());
		return;
	}

	public bool containsObstacle( GameObject obstacle ){

		if ( (obstacle.transform.position.x >= this.leftEdge && obstacle.transform.position.x < this.rightEdge) && (obstacle.transform.position.y <= this.topEdge && obstacle.transform.position.y > this.bottomEdge) )
			return true;

		return false;
	}

	public bool containsObjective( StratObj location ){
		if ( (location.MapPosition.x >= this.leftEdge && location.MapPosition.x < this.rightEdge) && (location.MapPosition.y <= this.topEdge && location.MapPosition.y > this.bottomEdge) )
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
	
}
