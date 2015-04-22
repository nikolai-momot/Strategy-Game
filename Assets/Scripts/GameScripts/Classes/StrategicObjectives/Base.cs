using UnityEngine;
using System.Collections;


public class Base : StratObj {
	
	public string TownsInSupplyLine;
	
	public Base(string n,GameObject p,Player Owner):base(n,p,Owner){
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

    public new void MoneyToDefences(float Money) {
        this.DefenceLevel += (int)(Money / getUpgradeDefenceCost());
    }
    public new void MoneyToSupply(float Money) {
        this.SupplyLevel += (int)(Money / getUpgradeSupplyCost());
    }
    public float getUpgradeDefenceCost() {
        return ((1/500)*Mathf.Pow(DefenceLevel,2.2f));
    }
    public float getUpgradeSupplyCost() {
        return ((1 / 500) * Mathf.Pow(SupplyLevel, 2.2f));
    }
	
	public override string ToString ()
	{
		return "Objective " + Name + " is a Base. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
