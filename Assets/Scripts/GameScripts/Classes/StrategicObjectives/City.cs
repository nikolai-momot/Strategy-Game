using UnityEngine;
using System.Collections;


public class City : StratObj {
	//public List<Building> buildings; //TODO: Add building's that you can build in Cities
	
	public City(string n,GameObject p,Player Owner):base(n,p,Owner){
		DefenceLevel = 50;
		SupplyLevel = 50;
	}

    public new void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public new void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public new int getUpgradeDefenceCost() {
        return (int)((1 / 500) * Mathf.Pow(DefenceLevel, 2.1f));
    }
    public new int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 2.1f));
    }
	public override string ToString ()
	{
		return "Objective " + Name + " is a City. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}	
}
