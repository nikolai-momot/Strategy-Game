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
	public int OwnerID; //Keep track of the owner
    public List<Army> OccupyingArmies; //Armies at this town
    public ForceComp Garrison; //Garrison acts like and army, but just defends the town
	public int DefenceLevel; //1-100
	public int SupplyLevel; //Probably just the raw supply number
	public int StrategicValue; //For AI use
	public GameObject MapPosition; //We'll get the town's co-ordinates on the map, and it's connected towns from this object.
	private Dictionary<string,StratObj> ConnectedPoints; //The key being the Point's name. Get this list from TownPosition game object's info

	public StratObj(){}	//Need this for inheritance
	
	//Contructor will take the name, the gameObject(Location) and the enum type
	public StratObj(string n,GameObject p,int OwnerID){
		Name = n; //Name always set
		MapPosition = p;
        this.OwnerID = OwnerID;
		ConnectedPoints = new Dictionary<string,StratObj>();
	}


	public Dictionary<string,StratObj> GetConnectedPoints(){
		Dictionary<string,StratObj> ConnectedPoints = new Dictionary<string, StratObj> ();
		Town[] towns = MapPosition.GetComponent<Town[]> (); //Fetch array of connected towns
		foreach (Town t in towns) {
			ConnectedPoints.Add(t.getName(),t);
		}
		return ConnectedPoints;
	}
	
	public void AddConnectedPoint(StratObj obj){
		this.ConnectedPoints.Add(obj.getName(),obj);
	}
	
	public void DrawConnectionLines(){
		foreach(StratObj pos in ConnectedPoints.Values){
			Debug.DrawRay(MapPosition.transform.position,-((MapPosition.transform.position) - pos.MapPosition.transform.position),Color.green,60.0f);
		}
	}
	
	public int EstimateStrategicValue(){
		if(OwnerID==null){ //Unoccupied
			return SupplyLevel + 5*DefenceLevel + 2*ConnectedPoints.Count;
		}else{ //Occupied
			return SupplyLevel - 5*DefenceLevel + 2*ConnectedPoints.Count;
		}
	}

    public void MoneyToDefences(int Money){
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public void MoneyToSupply(int Money){
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
	
	//Setters
	public void setName(string n){Name=n;}
	

	//Getters
	public string getName(){return Name;}
	public int getDefenceLevel(){return DefenceLevel;}
	public int getSupplyLevel(){return SupplyLevel;}
	public GameObject getMapObject(){return MapPosition;}
	public Transform getMapPosition(){return MapPosition.transform;}
    public int getUpgradeDefenceCost() { return 1; }
    public int getUpgradeSupplyCost() { return 1; }

}
