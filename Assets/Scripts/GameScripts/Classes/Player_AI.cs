using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_AI : Player{

    /*0-100*/
	public int Aggresivness; //Focus on Taking new Towns
	public int Defencivness; //Focus on upgrading Defences 
	public int FocusOnSupplies; //Focus on Upgrading Supply generating buildings
	public int Riskiness; //The theshold to take risks. Less Risky = Will wait for better odds to attack

	public Player_AI(int id,string name,string country,Base hq):base(id,name,country,hq){	
	//Calls Player Constructor
        Aggresivness = Random.Range(1,100);
        Defencivness = Random.Range(1, 100);
        FocusOnSupplies = Random.Range(1, 100);
	}
	
	//Turn Sequence
	public void TakeTurn(){
        CollectIncome();
        MoveArmies();
		SpendMoney();		
		EndTurn();
	}

    /*All towns and armies will request money to spend, they will get what they request unless the Player doesn't have enough money.
     *In which case, they Player will give the appropriate proportion of it's money to be spent.*/
    public int SpendMoney() {
		//Look at money, Strategic points, Armies. 
		//Make decisions about where to spend money
        int totalRequestedMoney = 0;
        int totalSpent=0;
        int[] ObjSpendingBudget = new int[Objectives.Count]; //The Each StratObj's budget this turn
        int[] ArmySpendingBudget = new int[Armies.Count]; //Each Armies Spending budget
        
        for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Ask all towns how much money they need
            ObjSpendingBudget[i] = getRequestedSpendingMoney(Objectives[i]);
        }
        for (int i = 0; i < ArmySpendingBudget.Length; i++) { //Ask all armies how much money they need
            if (Armies[i].getActions() > 0) {//If army hasn't moved
                ArmySpendingBudget[i] = getRequestedSpendingMoney(Armies[i]);
            } else {
                ArmySpendingBudget[i] = 0;
            }
        }
        totalRequestedMoney = SumOfArray(ObjSpendingBudget) + SumOfArray(ArmySpendingBudget);

        if (totalRequestedMoney <= Money) {//We have enough money to give out all that was requested!
            for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Give all towns requested money
                totalSpent += SpendOnTownUpgrades(ObjSpendingBudget[i],Objectives[i]);
            }
            for (int i = 0; i < ArmySpendingBudget.Length; i++) { //Give all armies requested money
                totalSpent += SpendOnRecruitment(ArmySpendingBudget[i],Armies[i]);
            }
        }else{//Don't have enough money, We'll have to scale down what we give out :(
            for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Give out scaled down money
                totalSpent += SpendOnTownUpgrades(CalculateAvailableMoney(ObjSpendingBudget[i],SumOfArray(ObjSpendingBudget)),Objectives[i]);
            }
            for (int i = 0; i < ObjSpendingBudget.Length; i++) { 
                totalSpent += SpendOnRecruitment(CalculateAvailableMoney(ArmySpendingBudget[i],SumOfArray(ArmySpendingBudget)),Armies[i]);
            }
        }
        return totalSpent;
    }
	
	public void MoveArmies(){
		//Any Armies that weren't upgraded can now choose where to move
        foreach (Army a in Armies) {
            if (a.getActions() > 0) { //If Army has actions
                if (MoveArmy(a)) { //If the Army wants to move
                    if (AttackDes(a) >= MoveDes(a)) {
                        /*Chooses to attack, look for best target*/
                        a.setActions(0);
                    } else {
                        /*Chooses to move, look for best loaction*/
                        a.setActions(0);
                    }
                }
            }
        }
	}
	
	public void EndTurn(){
		//Tell the Manager we're done	
        ResetAllArmyActions();
	}


    /* Desirability(Des) Functions */
    //Town Management
    public int getTownVulerability(StratObj town) {
        int v = 0;
        /*Look at how many nearby StratObjs there are, and how many are hostile*/
        return v; 
    }
    public int UpgradeTownDefenceDes(StratObj town) {
        int des = 0;
        des = getTownVulerability(town) * (Defencivness/10);
        return des; 
    }
    public int UpgradeTownSupplyDes(StratObj town) {
        int des = 0;
        des = FocusOnSupplies - (int)Mathf.Pow((float)getTownVulerability(town), 2);
        return des; 
    }
    public int getRequestedSpendingMoney(StratObj town) { //Asks the town how much money it wants for upgrades
        int request = 0;
        request = (int)((-0.5f*(town.getDefenceLevel()) + 50) + (-0.5f*(town.getSupplyLevel()) + 50));
        return request; 
    }
    //Army Management
    public bool MoveArmy(Army army) { //Return true if the army wants to move, false if it wants to recruit units
        bool Move;
        if (MoveDes(army) >= RecruitDes(army)) {
            Move = true;
        } else {
            Move = false;
        }
        return Move; 
    }
    //Army Movement / Attacking    
    public int MoveTargetDes(StratObj target) { //Run all available move targets through this, to get the best one
        /* See how far it is from friendly positions, maybe? */
        return 0; 
    }
    public int AttackTargetDes(Army target) { //Run all available attack targets through this, to get the best one
        /*Run a strength estimate to guess chances of winning*/
        return 0; 
    }
    public int AttackDes(Army army) { //How much the army wants to attack
        /*Look at all attack options in the area, to see if there's a good target*/
        return 0; 
    }
    public int MoveDes(Army army) { //How much the army wants to move
        /*Probably if the options for attacking don't look good, should move away from them?*/
        return 0; 
    }   
  
    //Army Recruitment    
    public int RecruitDes(Army army) { //How much the Army wants to be reinforced
        return (100 + Defencivness) - army.Force.GetSoldierCount() - 4 * army.Force.GetVehicleCount(); 
    }
    public int getRequestedSpendingMoney(Army army) {//Asks the Army how much money it wants to spend on recruitment
        return (100-army.Force.GetSoldierCount()) + (10 - army.Force.GetVehicleCount()); 
    } 
    /**************************/

    /*Action Functions*/
    public int SpendOnTownUpgrades(int SpendingMoney, StratObj town) { //Tell the town how much money it has to spend, return money spent
        int MoneyforDefence = (int)(SpendingMoney * (Defencivness / 100));
        int MoneyforSupply = SpendingMoney - MoneyforDefence;
        town.MoneyToDefences(MoneyforDefence);
        town.MoneyToSupply(MoneyforSupply);
        return MoneyforDefence + MoneyforSupply; 
    } 
    public int SpendOnRecruitment(int SpendingMoney, Army army) { //Gives the army money to spend, Returns Amount spent 
        int MoneyforInfantry = (int)(SpendingMoney * 0.7f);
        int MoneyforVehicles = SpendingMoney - MoneyforInfantry;
        int NumberofInfantry = MoneyforInfantry / 3;
        int NumberofVehicles = MoneyforVehicles / 10;
        int Spent = 0;
        if (NumberofInfantry > 0) {
            army.Force.AddSoldiers(NumberofInfantry);
            Spent += NumberofInfantry*3;
        }
        if (NumberofVehicles > 0) {
            army.Force.AddVehicles(NumberofVehicles);
            Spent += NumberofVehicles * 10;
        }
        Debug.Log(army.Name + " spent $" + Spent + " on recruitment, Attained: " + NumberofInfantry + " infantry and " + NumberofVehicles + " vehicles.");
        return Spent; 
    }
    
   

    public int CalculateAvailableMoney(int Request, int TotalRequested) {
    /*Takes the percentage of the total money requested, and gives that percentage of the available money*/
        return Money*(Request/TotalRequested);
    }

    public int SumOfArray(int[] a) {
        int sum = 0;
        for (int i = 0; i < a.Length; i++) {
            sum += a[i];
        }
        return sum;
    }
    
}
