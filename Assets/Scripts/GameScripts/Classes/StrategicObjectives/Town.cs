using UnityEngine;
using System.Collections;

public class Town : StratObj {
	public Town(string n,GameObject p,Player Owner):base(n,p,Owner){
		DefenceLevel = 15;
		SupplyLevel = 35;
	}

    
    public new float getUpgradeDefenceCost() {
        return ((1 / 500) * Mathf.Pow(DefenceLevel, 2.0f));
    }
    public new int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 1.9f));
    }

	public override string ToString (){
		return "Objective " + Name + " is a Town. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
