using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Parent Object for any strategc point.
 * Base: Starting point for a player, source of supply line **Only one per Player!**
 * City: Will produce lots of resources, not much defences to start
 * Town: Same as city, but less reward
 * Outpost: Opposite from City, lots of defence, little money to start 
 * */

public class StratObj{

	public string Name;
	public int OwnerID;						//Keep track of the owner
	public int DefenceLevel;				//Scale of 1-5
	public int SupplyLevel;					//Probably just the raw supply number
	public int StrategicValue;				//For AI use
	public Vector3 MapPosition; 			//We'll get the town's co-ordinates on the map, and it's connected towns from this object.
	public List<GameObject> ConnectedPoints;//The key being the Point's name. Get this list from TownPosition game object's info
	//private StratObj[] Edges;				//Might prove useless, discard in future
	public GameObject gObj;

	public StratObj(){ }	//Need this for inheritance

	//Contructor will take the name, the gameObject(Location) and the enum type
	public StratObj(string n,GameObject p){
		Name = n; 							//Setting Name
		gObj = p;							//Recording GameObject
		p.tag = "Objective";				//Recording Tag - possibly useless; check later
		MapPosition = p.transform.position;	//World position shorthand

		//Review Neccesity of this part, needs refactoring
		var points = p.GetComponent<Points> ();
		ConnectedPoints = new List<GameObject> ();
		foreach(GameObject connectedPoint in points.connectedPoints)
			ConnectedPoints.Add(connectedPoint);
	}

	public void DrawConnectionLines(){
		foreach(GameObject pos in ConnectedPoints){
			Debug.DrawRay( MapPosition,-( (MapPosition) - pos.transform.position ), Color.red, 60.0f );
		}
	}
	
	public int EstimateStrategicValue(){
		if(OwnerID==null){ //Unoccupied
			return SupplyLevel + 5*DefenceLevel + 2*ConnectedPoints.Count;
		}else{ //Occupied
			return SupplyLevel - 5*DefenceLevel - 2*ConnectedPoints.Count;
		}
	}
	
	//Setters
	public void setName(string n){ Name=n; }
	public void incDefenceLevel(){ DefenceLevel++; }
	public void incSupplyLevel(){ SupplyLevel++; }
	public void decDefenceLevel(){ DefenceLevel--; }
	public void decSupplyLevel(){ SupplyLevel--; }
	/*Might prove useless, discard in future*/
	/*public void SetEdge( StratObj newEdge ){
		int index = Edges.Length;

		if (index > 0)
			index = index - 1; 

		Edges [index] = newEdge;
	}*/

	//Getters
	public string getName(){ return Name; }
	public int getDefenceLevel(){ return DefenceLevel; }
	public int getSupplyLevel(){ return SupplyLevel; }
	public Vector3 getMapObject(){ return MapPosition; }
	public Vector3 getMapPosition(){ return MapPosition; }
	/*Might prove useless, discard in future*/
	/*public StratObj[] getEdges(){ return Edges; }*/
}
