using UnityEngine;
using System.Collections;

public class Outpost : StratObj {
	public Outpost(string n,GameObject p,Player Owner):base(n,p,Owner){
		DefenceLevel = 35;
		SupplyLevel = 15;
	}

    
    public new float getUpgradeDefenceCost() {
        return ((1 / 500) * Mathf.Pow(DefenceLevel, 1.9f));
    }
    public new float getUpgradeSupplyCost() {
        return ((1 / 500) * Mathf.Pow(SupplyLevel, 2.0f));
    }
	
	public override string ToString ()
	{
		return "Objective " + Name + " is an Outpost. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
