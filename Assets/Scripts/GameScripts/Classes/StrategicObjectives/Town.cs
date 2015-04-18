using UnityEngine;
using System.Collections;

public class Town : StratObj {
	public Town(string n,GameObject p):base(n,p){
		DefenceLevel = 1;
		SupplyLevel = 50;
	}	
	
	public override string ToString ()
	{
		return "Objective " + Name + " is a Town. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}
}
