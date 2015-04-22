﻿using UnityEngine;
using System.Collections;


public class City : StratObj {
	//public List<Building> buildings; //TODO: Add building's that you can build in Cities
	
	public City(string n,GameObject p,Player Owner):base(n,p,Owner){
		DefenceLevel = 50;
		SupplyLevel = 50;
	}

    public new void MoneyToDefences(float Money) {
        this.DefenceLevel += (int)(Money / getUpgradeDefenceCost());
    }
    public new void MoneyToSupply(float Money) {
        this.SupplyLevel += (int)(Money / getUpgradeSupplyCost());
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
