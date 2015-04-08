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
    public Vector3 MapPosition; //We'll get the town's co-ordinates on the map, and it's connected towns from this object.
    public GameObject gObj;

	public StratObj(){}	//Need this for inheritance
	
	//Contructor will take the name, the gameObject(Location) and the enum type
	public StratObj(string n,GameObject p,int OwnerID){
		Name = n; //Name always set
		gObj = p;
        p.tag = "Objective";
        MapPosition = p.transform.position;
        this.OwnerID = OwnerID;
   	}

	
	public int getStrategicValueForAI(Vector3 AI_Base){
		return (int)(MapPosition - AI_Base).magnitude;
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
	public GameObject getMapObject(){return gObj;}
	public Vector3 getMapPosition(){return MapPosition;}
    public int getUpgradeDefenceCost() { return 1; }
    public int getUpgradeSupplyCost() { return 1; }

}
