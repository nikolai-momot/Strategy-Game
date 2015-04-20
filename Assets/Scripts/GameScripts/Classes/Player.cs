using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Each player in the game is a player Object. 
 * This holds all the player's game related info, 
 * and manages decision making for AI players
 * */
public class Player{

	public int ID;
	public string Name;
	public string Country;//Determines Flag, later on unit types
    public int Money;
	public Base HQ;
	public List<Army> Armies; //List so they can get more than just the starting Armies
	public List<StratObj> Objectives;
    public bool finishedTurn = false;
	
	public Player(){
		
	}
	
	public Player(int id,string name,string country,Base hq){
		ID = id;
		Name = name;
		Country = country;
		HQ=hq;
		Armies = new List<Army>();
        Objectives = new List<StratObj>();
	}

    public void TakeTurn() {

    }

    public void CreateNewArmy(string n, GameObject ArmyObj) {
		Armies.Add(new Army(n,ArmyObj,this.ID));
        ArmyObj.GetComponentInChildren<TextMesh>().text = n;
	}
	
	public void CreateNewArmy_GenerateName(GameObject ArmyObj){
		int num = Random.Range(1,600);
		string n = "";
		if(num%100 == 13){ //Special Case where number ends with a 3, but has a th instead of rd
			n+= num + "th Division";
			Armies.Add(new Army(n,ArmyObj,this.ID));
			return;
		}		
		switch(num%10){
		case 1:
			n+=num + "st Division";
			break;
		case 2:
			n+=num + "nd Division";
			break;
		case 3:
			n+=num + "rd Division";
			break;
		default:
			n+=num + "th Division";
			break;				
		}
        Armies.Add(new Army(n, ArmyObj, this.ID));
        ArmyObj.GetComponentInChildren<TextMesh>().text = n;
	}

    
    
    public int GetRevenue() {
        int income=0;
        income += HQ.getSupplyLevel();
        foreach (StratObj obj in Objectives) {
            income += obj.getSupplyLevel();
        }
        return income;
    }

    public int GetProfit() {
        return GetRevenue() - GetUpkeep();
    }

    public int GetUpkeep() {
        int upkeep=0;
        foreach (Army a in Armies) {
            upkeep += a.GetUpkeep();
        }
        return upkeep;
    }

    public void CollectIncome() {        
        Money += GetProfit();
        Debug.Log(Name + ": Collecting " + GetProfit() + ". I now have: " + Money);
    }
	
	public override string ToString ()
	{
		return "Player " + this.ID + ", " + this.Name + " - " + this.Country;
	}
			
}
