using UnityEngine;
using System.Collections;


public class Base : StratObj {
	
	public string TownsInSupplyLine;
	
	public Base(string n,GameObject p):base(n,p){
		DefenceLevel = 4;
		SupplyLevel = 150;
		TownsInSupplyLine = "";
	}
	
	
	public void getSupplyLine(int ownerID){ //TODO: Implement supply lines
		/*if (this.OwnerID != ownerID) {return;}
		this.TownsInSupplyLine += " " + this.Name;
		foreach (Town t in ConnectedTowns.Values) {
		}*/
	}
	
	public override string ToString ()
	{
		return "Objective " + Name + " is a Base. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
