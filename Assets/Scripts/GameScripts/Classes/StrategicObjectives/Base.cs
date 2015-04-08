using UnityEngine;
using System.Collections;


public class Base : StratObj {
	
	public string TownsInSupplyLine;
	
	public Base(string n,GameObject p,int OwnerID):base(n,p,OwnerID){
		DefenceLevel = 75;
		SupplyLevel = 150;
		TownsInSupplyLine = "";
	}
	
	
	public void getSupplyLine(int ownerID){ //TODO: Implement supply lines
		/*if (this.OwnerID != ownerID) {return;}
		this.TownsInSupplyLine += " " + this.Name;
		foreach (Town t in ConnectedTowns.Values) {
		}*/
	}

    public new void MoneyToDefences(int Money) {
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public new void MoneyToSupply(int Money) {
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }
    public new int getUpgradeDefenceCost() {
        return (int)((1/500)*Mathf.Pow(DefenceLevel,2.2f));
    }
    public new int getUpgradeSupplyCost() {
        return (int)((1 / 500) * Mathf.Pow(SupplyLevel, 2.2f));
    }
	
	public override string ToString ()
	{
		return "Objective " + Name + " is a Base. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
