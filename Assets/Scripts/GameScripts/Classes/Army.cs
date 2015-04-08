using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* An Army is the strategic map representation of a player's army
 * It will hold the army's statistics and information
 * */
public class Army{

	public string Name;
    public int OwnerID;
    public General Leader;
	public ForceComp Force;
    public GameObject ArmyObject;
	int Wins, Losses, Morale;
    private int Actions=0;

	public Army(string n,GameObject ArmyObject, int Owner){
		Name = n;
        this.OwnerID = Owner;
        Leader = new General(GenerateName());
        this.ArmyObject = ArmyObject;
		Force = new ForceComp(); //Default Army
		Force.AddSoldiers(30);
		Force.AddVehicles(2);

        //currentCell = hq; //Starts at the HQ
		Wins = 0;
		Losses = 0;
		Morale = 100; //TODO: set a scale for morale or something
        Actions = 0;
	}

    public void UpdateArmyObject() {
        
    }

	public int getStrength(){
		return Force.EstimateStrength();
	}

    /* TODO FUNCTIONS */

    public string GenerateName() { /*Use a NameList to generate names for generals*/ 
        return "Cmdr." + this.Name; 
    }

    public int GetUpkeep() {
        /*Return the cost to upkeep this army*/
        return 0;
    }

    
    public void Retreat(){
        /*Choose and adjacent town and move to it.*/
    }
    public void RecruitInfantry(int n) { 
        /*Add N Infantry to Force*/
        Force.AddSoldiers(n);
    }
    public void RecruitVehicle(int n) { 
        /*Add N Vehicles to Force*/
        Force.AddVehicles(n);
    }

    public void AttackTarget(Army target) {
        //GameManager.pathFinder.FindPath();
    }
    public void AttackTarget(StratObj target) { }

    public void MoveToTarget() {  }


    /*****************/

    public int getStrategicValueForAI(Army a) {
        return (a.getStrength() - getStrength()) - (int)(ArmyObject.transform.position - a.ArmyObject.transform.position).magnitude;
    }
	
	public override string ToString (){
		return "The " + Name + ", Commanded by: " + this.Leader.getName() + " has " + Force.GetSoldierCount() + " infantry, " + Force.GetVehicleCount() + " vehicles.";
	}
	
	//Getters
	public string getName(){return this.Name;}
	public int getWins(){return this.Wins;}
	public int getLosses(){return this.Losses;}
    public int getActions() { return this.Actions; }

	//Setters
	public void setWins(int x){this.Wins=x;}
	public void setLosses(int x){this.Losses=x;}	
	public void AddWin(){this.Wins++;}
	public void AddLoss(){this.Losses++;}
    public void setActions(int a) { this.Actions = a;}

}
