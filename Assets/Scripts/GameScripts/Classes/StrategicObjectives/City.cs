using UnityEngine;
using System.Collections;


public class City : StratObj {
	//public List<Building> buildings; //TODO: Add building's that you can build in Cities
	
	public City(string n,GameObject p,Player Owner):base(n,p,Owner){
		DefenceLevel = 35;
		SupplyLevel = 35;
	}

    
    public float getUpgradeDefenceCost() {
        return ((1 / 500) * Mathf.Pow(DefenceLevel, 2.1f));
    }
    public float getUpgradeSupplyCost() {
        return ((1 / 500) * Mathf.Pow(SupplyLevel, 2.1f));
    }
	public override string ToString ()
	{
		return "Objective " + Name + " is a City. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}	
}
