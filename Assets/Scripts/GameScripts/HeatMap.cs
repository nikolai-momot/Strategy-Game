using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap
{
	private Cell[,] map;
	private int playerCount;
	private List<StratObj> objectives;
	private Player_AI[] players;
	private int xTiles, yTiles;
	private int heatRange;

	public HeatMap ( Cell[,] cellMap, List<StratObj> locations, Player_AI[] allPlayers, int x, int y )
	{
		this.map = cellMap;					//Cell map
		this.objectives = locations;		//List of objectives
		this.players = allPlayers;			//Array of players
		this.playerCount = players.Length;	//Number of players
		this.xTiles = x;					//Tiles on the y range
		this.yTiles = y;					//Tiles on the x range
		this.heatRange = Convert.ToInt32( Mathf.Sqrt ( (int)x ) )+1;

		setObjectiveHeat ();	//Setting the heat for each objective on the map
		setArmyHeat ();	//Setting the heat for each army on the map
	}

	public void UpdateHeatMap (){
		clearMap ();
		setObjectiveHeat ();	//Setting the heat for each objective on the map
		setArmyHeat ();	//Setting the heat for each army on the map
	}

	public void clearMap(){
		foreach (Cell cell in map) {
			for(int i = 0; i < players.Length; i++)
				cell.heat[i] = 0;
		}
	}

	public void setObjectiveHeat(){

		foreach( StratObj objective in objectives){
			if(objective.OwnerID == 0 )//Proceed if the objective is occupied by someone
				continue;

				Debug.Log(map[objective.x, objective.y].ToString());
				//Setting the heat range based on the type of objective
				switch(objective.DefenceLevel){
					case 75:					//Base
						distributeObjHeat( map[objective.x, objective.y], objective, heatRange );
					break;
					case 50:					//City
						distributeObjHeat( map[objective.x, objective.y], objective, heatRange -1 );
					break;
					case 35:					//Outpost
						distributeObjHeat( map[objective.x, objective.y], objective, heatRange -1 );
					break;
					case 15:					//Town
						distributeObjHeat( map[objective.x, objective.y], objective, heatRange -1 );
					break;
				}

		}
	}

	public void distributeObjHeat( Cell cell, StratObj obj, int range){
		int x 		= cell.x,					//Center x value
			y		= cell.y,					//Center y value
			id		= obj.OwnerID -1,			//Owner ID
			defence = obj.DefenceLevel;			//Objective defence level

		float xDist, yDist;						//Distance from center

		for(int i = x-range; i <= x+range; i++){
			xDist = Mathf.Abs(i-x);				//Absolute distance from the center x value

			for(int j = y-range; j <= y+range; j++){
				//Checking if the coordinatee is on the grid
				if ( (i > 0) && (j > 0) && (i < xTiles-1) && (j < yTiles-1) ){

					yDist = Mathf.Abs(j-y);		//Absolute distance from the center y value


					if(x==i && y==j){
						map[i, j].heat[id] += defence;
						//Debug.Log("CENTER| Heat at ("+i+", "+j+") is "+map[i, j].heat[id]+", xDist is "+xDist+" and the yDist is "+yDist );
						continue;
					}

					int displacement		= Convert.ToInt32( Mathf.Abs( -(yDist)-(xDist) ) ),
						heatBase= ( 2^( displacement ) );


					//Debug.Log( "At ("+i+", "+j+"): xDist is "+xDist+", the yDist is "+yDist+", the displacement is "+displacement);

					map[i, j].heat[id] += Convert.ToInt32( defence/displacement );	//Add heat to the cell's current value for the respected player 
					
					//Debug.Log(	"The heat coming from ("+x+", "+y+") is "+map[i, j].heat[id]+"" + "\nAt ("+i+", "+j+"): xDist is "+xDist+", the yDist is "+yDist+", the displacement is "+displacement+" and the heatBase is "+heatBase+"." );
				}

			}
		}

	}

	public void setArmyHeat(){
		/*if (range < 0)
			range = 1;*/

		foreach (Player player in players) {
			foreach(Army army in player.Armies){
				foreach (Cell cell in map) {
					//Skip the cell if it doesn't have the army inside of it
					if( !cell.containsVector( army.ArmyObject.transform.position ) )
						continue;

					int x 		= cell.x,
						y 		= cell.y, 
						id		= army.OwnerID - 1,
						force	= army.getStrength();
					
					//Debug.Log("Army at ("+x+", "+y+") is heating up");
					
					distributeArmyHeat( x, y, id, force, heatRange/2 );	//Setting the heat, placeholder range
					break;					//Cell location found and heat set, break loop
				}
			}		
		}



	}

	public void distributeArmyHeat( int x, int y, int id, int force, int range ){
		float xDist, yDist;						//Distance from center
		
		for(int i = x-range; i <= x+range; i++){
			xDist = Mathf.Abs(i-x);				//Absolute distance from the center x value
			
			for(int j = y-range; j <= y+range; j++){
				//Checking if the coordinatee is on the grid
				if ( (i > 0) && (j > 0) && (i < xTiles-1) && (j < yTiles-1) ){
					
					yDist = Mathf.Abs(j-y);		//Absolute distance from the center y value
					
					
					if(x==i && y==j){
						map[i, j].heat[id] += force;
						//Debug.Log("ARMY CENTER| Heat at ("+i+", "+j+") is "+map[i, j].heat[id]+", xDist is "+xDist+" and the yDist is "+yDist );
						continue;
					}
					
					int displacement		= Convert.ToInt32( Mathf.Abs( -(yDist)-(xDist) ) ),
					heatBase= ( 2^( displacement ) );
					
					
					//Debug.Log( "At ("+i+", "+j+"): xDist is "+xDist+", the yDist is "+yDist+", the displacement is "+displacement);
					
					map[i, j].heat[id] += Convert.ToInt32( force/displacement );	//Add heat to the cell's current value for the respected player 
					
					/*Debug.Log(	"The heat coming from ("+x+", "+y+") is "+map[i, j].heat[id]+"" +
					          	"\nAt ("+i+", "+j+"): xDist is "+xDist+", the yDist is "+yDist+", the displacement is "+displacement+" and the heatBase is "+heatBase+"." );*/
				}
				
			}
		}
	}
}

