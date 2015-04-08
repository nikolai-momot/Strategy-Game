using UnityEngine;
using System.Collections;

public class Town : StratObj {
	public Town(string n,GameObject p,int OwnerID):base(n,p,OwnerID){
		DefenceLevel = 15;
		SupplyLevel = 50;
	}

    public new void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public new void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public new int getUpgradeDefenceCost() {
        return (int)((1 / 500) * Mathf.Pow(DefenceLevel, 2.0f));
    }
    public new int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 1.9f));
    }

	public override string ToString (){
		return "Objective " + Name + " is a Town. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
