using UnityEngine;
using System.Collections;

/* An Army is the strategic map representation of a player's army
 * It will hold the army's statistics and information
 * */
public class Army{

	public string Name;
	public ForceComp Force;
	public StratObj currentTown; 
	int Wins, Losses, Morale;

	public Army(string n,Base hq){
		Name = n;
		
		Force = new ForceComp(); //Default Army
		Force.AddSoldiers(30);
		Force.AddVehicles(2);
		
		currentTown = hq; //Starts at the HQ
		Wins = 0;
		Losses = 0;
		Morale = 100; //TODO: set a scale for morale or something
	}


	public int EstimateStrength(){
		return (Force.GetSoldierCount() + (4 * Force.GetVehicleCount()));
	}
	
	public override string ToString ()
	{
		return "The " + Name + " has " + Force.GetSoldierCount() + " infantry, " + Force.GetVehicleCount() + " vehicles, and is currently in " + currentTown.getName();
	}
	
	//Getters
	public string getName(){return this.Name;}
	public int getWins(){return this.Wins;}
	public int getLosses(){return this.Losses;}

	//Setters
	public void setWins(int x){this.Wins=x;}
	public void setLosses(int x){this.Losses=x;}	
	public void AddWin(){this.Wins++;}
	public void AddLoss(){this.Losses++;}

}
