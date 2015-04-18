using UnityEngine;
using System.Collections;


public class City : StratObj {
	//public List<Building> buildings; //TODO: Add building's that you can build in Cities
	
	public City(string n,GameObject p,int OwnerID):base(n,p,OwnerID){
		DefenceLevel = 50;
		SupplyLevel = 250;
	}

    public override void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public override void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public override int getUpgradeDefenceCost() {
        return (int)((1 / 500) * Mathf.Pow(DefenceLevel, 2.1f));
    }
    public override int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 2.1f));
    }
	public override string ToString ()
	{
		return "Objective " + Name + " is a City. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}	
}
