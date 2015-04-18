using UnityEngine;
using System.Collections;

public class Outpost : StratObj {
	public Outpost(string n,GameObject p,int OwnerID):base(n,p,OwnerID){
		DefenceLevel = 35;
		SupplyLevel = 25;
	}

    public override void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public override void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public override int getUpgradeDefenceCost() {
        return (int)((1 / 500) * Mathf.Pow(DefenceLevel, 1.9f));
    }
    public override int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 2.0f));
    }
	
	public override string ToString ()
	{
		return "Objective " + Name + " is an Outpost. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
