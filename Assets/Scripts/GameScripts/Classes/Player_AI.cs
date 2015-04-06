using UnityEngine;
using System.Collections;

public class Player_AI : Player{

	public int Aggresivness; //Focus on Taking new Towns
	public int Defencivness; //Focus on upgrading Defences
	public int FocusOnSupplies; //Focus on Upgrading Supply generating buildings
	public int Riskiness; //The theshold to take risks. Less Risky = Will wait for better odds to attack

	public Player_AI(int id,string name,string country,Base hq):base(id,name,country,hq){	
	//Calls Player Constructor
	}
	
	//Turn Sequence
	public void TakeTurn(){
		ChooseUpgrades();
		MoveArmies();
		EndTurn();
	}
	
	public void ChooseUpgrades(){
		//Look at money, Strategic points, Armies. 
		//Make decisions about where to spend money
	}
	
	public void MoveArmies(){
		//Any Armies that weren't upgraded can now choose where to move
	}
	
	public void EndTurn(){
		//Tell the Manager we're done		
	}	
}
