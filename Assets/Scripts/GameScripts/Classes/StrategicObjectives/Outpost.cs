using UnityEngine;
using System.Collections;

public class Outpost : StratObj {
	public Outpost(string n,GameObject p,int OwnerID):base(n,p,OwnerID){
		DefenceLevel = 35;
		SupplyLevel = 25;
	}

    public new void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public new void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public new int getUpgradeDefenceCost() {
        return (int)((1 / 500) * Mathf.Pow(DefenceLevel, 1.9f));
    }
    public new int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 2.0f));
    }
	
	public override string ToString ()
	{
		return "Objective " + Name + " is an Outpost. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
