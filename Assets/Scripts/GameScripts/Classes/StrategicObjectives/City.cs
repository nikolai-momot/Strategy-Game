using UnityEngine;
using System.Collections;


public class City : StratObj {
	//public List<Building> buildings; //TODO: Add building's that you can build in Cities
	
	public City(string n,GameObject p):base(n,p){
		DefenceLevel = 0;
		SupplyLevel = 250;
	}
	
	public override string ToString ()
	{
		return "Objective " + Name + " is a City. Def: " + DefenceLevel + " Sup: " + SupplyLevel; 
	}	
}
