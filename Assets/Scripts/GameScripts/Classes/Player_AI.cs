using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_AI : Player{

    /*0-100*/
	public int Aggresivness; //Focus on Taking new Towns
	public int Defencivness; //Focus on upgrading Defences 
	public int FocusOnSupplies; //Focus on Upgrading Supply generating buildings
	public int Riskiness; //The theshold to take risks. Less Risky = Will wait for better odds to attack

    public Queue<Army> ArmiesWaitingToMove;
    public Queue<Battle> BattlesWaitingToResolve;

    
	public Player_AI(int id,string name,string country,Base hq):base(id,name,country,hq){	
	//Calls Player Constructor
        /*Aggresivness = Random.Range(1,100);
        Defencivness = Random.Range(1, 100);
        FocusOnSupplies = Random.Range(1, 100);*/
        Aggresivness = 100;
        Defencivness = 50;
        FocusOnSupplies = 0;

        ArmiesWaitingToMove = new Queue<Army>();
        BattlesWaitingToResolve = new Queue<Battle>();
	}
	
	//Turn Sequence
	public new void TakeTurn(){
        Debug.Log(Name + ": Taking my turn...");
        CollectIncome();
        MoveArmies();
        SpendMoneyOnTowns();		
		EndTurn();
	}

    /*All towns and armies will request money to spend, they will get what they request unless the Player doesn't have enough money.
     *In which case, they Player will give the appropriate proportion of it's money to be spent.*/
    public int SpendMoneyOnTowns() {
        Debug.Log(Name + ": Spending money on StratObjs...");
		//Look at money, Strategic points, Armies. 
		//Make decisions about where to spend money
        int totalRequestedMoney = 0;
        int totalSpent=0;
        int[] ObjSpendingBudget = new int[Objectives.Count]; //The Each StratObj's budget this turn
        
        for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Ask all towns how much money they need
            ObjSpendingBudget[i] = getRequestedSpendingMoney(Objectives[i]);
        }
        totalRequestedMoney = SumOfArray(ObjSpendingBudget);

        if (totalRequestedMoney <= Money) {//We have enough money to give out all that was requested!
            for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Give all towns requested money
                totalSpent += SpendOnTownUpgrades(ObjSpendingBudget[i],Objectives[i]);
            }
        }else{//Don't have enough money, We'll have to scale down what we give out :(
            for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Give out scaled down money
                totalSpent += SpendOnTownUpgrades(CalculateAvailableMoney(ObjSpendingBudget[i],SumOfArray(ObjSpendingBudget)),Objectives[i]);
            }
        }
        Debug.Log(Name + ": Spent "+totalSpent+" on StratObjs.");
        return totalSpent;
    }
	
	public void MoveArmies(){
        Debug.Log(Name + ": Moving My armies...");
		//Any Armies that weren't upgraded can now choose where to move
        foreach (Army a in Armies) {            
            int RecDes = RecruitDes(a);
            int AtkDes = AttackDes(a);
            Debug.Log(Name + ": Army " + a.getName() + " RecDes = " + RecDes + " AtkDes = " +  AtkDes);
            if (AtkDes > RecDes) {
                Army HTVArmy = getHTVArmy(a);
                StratObj HTVStratObj = getHTVStratObj();
                Debug.Log(Name + ": Choosing to move: " + a.getName() + " || Best Army Target: " + HTVArmy.getName() + "(" + HTVArmy.getStrategicValueForAI(a) + ") Best StratObj target: " + HTVStratObj.getName()+ "(" + HTVStratObj.getStrategicValueForAI(this) + ")");
                if (HTVArmy.getStrategicValueForAI(a) > HTVStratObj.getStrategicValueForAI(this)) {
                    Battle b = a.AttackTarget(HTVArmy);
                    if (b != null) {
                        ArmiesWaitingToMove.Enqueue(a);
                        BattlesWaitingToResolve.Enqueue(b);
                    } else {
                        ArmiesWaitingToMove.Enqueue(a);
                    }                    
                } else {
                    Battle b = a.AttackTarget(HTVStratObj);
                    if (b != null) {
                        ArmiesWaitingToMove.Enqueue(a);
                        BattlesWaitingToResolve.Enqueue(b);
                    } else {
                        ArmiesWaitingToMove.Enqueue(a);
                    }  
                }
            }else{
                /*Chooses to Recruit, needs to buy units or move to a town/buy units*/
            }
        }
    }
	
	public void EndTurn(){
		//Tell the Manager we're done
        Debug.Log(Name + ": Ending my turn");
        finishedTurn = true;
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

    //Army Movement / Attacking 
    public int AttackDes(Army a) {
        return 50 + Aggresivness;
    }
    public Army getHTVArmy(Army a) {
        List<Army> EnemyArmies = GameManager.GetAllEnemyArmies(ID);
        int hvt_index = 0;
        for (int i = 0; i < EnemyArmies.Count; i++) {
            if (EnemyArmies[hvt_index].getStrategicValueForAI(a) < EnemyArmies[i].getStrategicValueForAI(a)) {
                hvt_index = i;
            }
        }
        return EnemyArmies[hvt_index];
    }
    public StratObj getHTVStratObj() {
        int hvt_index = GameManager.Locations.Count - 1;
        for (int i = 0; i < GameManager.Locations.Count; i++) {
            if (GameManager.Locations[i].OwnerID != this.ID) {
                if (GameManager.Locations[hvt_index].getStrategicValueForAI(this) < GameManager.Locations[i].getStrategicValueForAI(this)) {
                    hvt_index = i;
                }
            }
        }
        return GameManager.Locations[hvt_index];
    }
  
    //Army Recruitment    
    public int RecruitDes(Army army) { //How much the Army wants to be reinforced
        return (50 + Defencivness) - army.Force.GetSoldierCount() - 4 * army.Force.GetVehicleCount(); 
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

    public bool isBusy() {
        return (ArmiesWaitingToMove.Count + BattlesWaitingToResolve.Count)>0;
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
