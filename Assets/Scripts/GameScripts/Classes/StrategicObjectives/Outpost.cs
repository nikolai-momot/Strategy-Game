using UnityEngine;
using System.Collections;

public class Outpost : StratObj {
	public Outpost(string n,GameObject p):base(n,p){
		DefenceLevel = 3;
		SupplyLevel = 25;
	}
	
	public override string ToString ()
	{
		return "Objective " + Name + " is an Outpost. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
