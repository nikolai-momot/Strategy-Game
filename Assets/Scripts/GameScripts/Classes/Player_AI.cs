using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_AI : Player{

    /*0-100*/
	public int Aggresivness; //Focus on Taking new Towns
	public int Defencivness; //Focus on upgrading Defences 
	public int FocusOnSupplies; //Focus on Upgrading Supply generating buildings
	public int Riskiness; //The theshold to take risks. Less Risky = Will wait for better odds to attack

    private bool Verbose;
    
    
	public Player_AI(int id,string name,string country,Base hq,bool v):base(id,name,country,hq){	
	//Calls Player Constructor
        Aggresivness = Random.Range(35,350);
        Defencivness = Random.Range(35, 200);
        FocusOnSupplies = Random.Range(10, 200);
        Verbose = v;
        //Aggresivness = 10;
        //Defencivness = 100;
        //FocusOnSupplies = 0;        
	}
	
	//Turn Sequence
	public new void TakeTurn(){
        if(Verbose) Debug.Log(Name + ": Taking my turn...");
        CollectIncome();
        Money -= (int)SpendMoneyOnTowns();
        //if(Verbose) Debug.Log(Name + ", Money: " + Money);
        MoveArmies();
        //if(Verbose) Debug.Log(Name + ", Money: " + Money);
		EndTurn();
	}

    /*All towns and armies will request money to spend, they will get what they request unless the Player doesn't have enough money.
     *In which case, they Player will give the appropriate proportion of it's money to be spent.*/
    public float SpendMoneyOnTowns() {
        if(Verbose) Debug.Log(Name + ": Spending money on StratObjs...");
        if (GetRevenue() <= 50) {
            if(Verbose) Debug.Log(Name + ": I don't have enough income...");            
            FocusOnSupplies += 50;          
        }
        if (GetRevenue() < 0) {
            return 0f;
        }
		//Look at money, Strategic points, Armies. 
		//Make decisions about where to spend money
        float totalRequestedMoney = 0;
        float totalSpent = 0;
        float[] ObjSpendingBudget = new float[Objectives.Count]; //The Each StratObj's budget this turn
        
        for (int i = 0; i < ObjSpendingBudget.Length; i++) { //Ask all towns how much money they need
            ObjSpendingBudget[i] = getRequestedSpendingMoney(Objectives[i]);
            if (Verbose) Debug.Log("== " + Objectives[i].getName() + " requests " + ObjSpendingBudget[i]);
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
        if(Verbose) Debug.Log(Name + ": Spent "+totalSpent+" on StratObjs.");
        return totalSpent;
    }
	
	public void MoveArmies(){
        if(Verbose) Debug.Log(Name + ": Moving My armies...");
		//Any Armies that weren't upgraded can now choose where to move
        foreach (Army a in Armies) {
            a.CurrentTarget = null;
            Army HVTArmy = getHVTArmy(a);
            StratObj HVTStratObj = getHVTStratObj();
            int RecDes = RecruitDes(a);
            int AtkDes = AttackDes(a,HVTArmy,HVTStratObj);
            if(Verbose) Debug.Log(Name + ": Army " + a.getName() + " RecDes = " + RecDes + " AtkDes = " +  AtkDes);
            if (AtkDes > RecDes) {                
                if(Verbose) Debug.Log(Name + ": Choosing to move: " + a.getName() + " \n"+
                "|| Best Army Target: " + HVTArmy.getName() + "(" + HVTArmy.getStrategicValueForAI(a) + ") Best StratObj target: " + HVTStratObj.getName()+ "(" + HVTStratObj.getStrategicValueForAI(this) + ")");                
                if (HVTArmy.getStrategicValueForAI(a) > HVTStratObj.getStrategicValueForAI(this)) {
                    if (HVTArmy.currentObj != null) {
                        OrderAttackOnObj(a, HVTArmy.currentObj);
                        if (Verbose) Debug.Log("Sending " +a.getName() + " at " + HVTArmy.getName());
                    } else {
                        OrderAttackOnArmy(a, HVTArmy);
                        if (Verbose) Debug.Log("Sending " + a.getName() + " at " + HVTArmy.getName());
                    }
                } else {
                    OrderAttackOnObj(a,HVTStratObj);
                    if (Verbose) Debug.Log("Sending " + a.getName() + " at " + HVTArmy.getName());
                }
            }else{
                /*Chooses to Recruit, needs to buy units or move to a town/buy units*/
                if (a.currentObj != null) {
                    /*Recruit!*/
                    /*Spends at least 10% of it's money, but up to all of it. Based on how weak the army is*/
                    Money -= (int)SpendOnRecruitment(CalculateRecruitmentMoney(), a);
                } else {
                    /*Move to closest OBJ!*/
                    if (a.MoveToEnter(getClosestOwnedObj(a))) {
                        ArmiesWaitingToEnter.Enqueue(a); // Is close enough to enter base
                    } else {
                        ArmiesWaitingToMove.Enqueue(a); //Will just move for now...
                    }
                }
            }
        }
    }
    public void OrderAttackOnArmy(Army a, Army HVTArmy) {
        Battle b = a.AttackTarget(HVTArmy);
        if (b != null) {
            ArmiesWaitingToMove.Enqueue(a);
            BattlesWaitingToResolve.Enqueue(b);
        } else {
            ArmiesWaitingToMove.Enqueue(a);
        }  
    }
    public void OrderAttackOnObj(Army a, StratObj HVTStratObj) {
        Battle b = a.AttackTarget(HVTStratObj);
        if (b != null) {
            ArmiesWaitingToMove.Enqueue(a);
            BattlesWaitingToResolve.Enqueue(b);
        } else {
            ArmiesWaitingToMove.Enqueue(a);
        }  
    }
	
	public void EndTurn(){
		//Tell the Manager we're done
        if(Verbose) Debug.Log(Name + ": Ending my turn");
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
    public float getRequestedSpendingMoney(StratObj town) { //Asks the town how much money it wants for upgrades
        float request = 0;
        request = ((-0.5f*(town.getDefenceLevel()) + Defencivness) + (-0.5f*(town.getSupplyLevel()) + FocusOnSupplies));        
        if (request < 0) return 0;
        return Mathf.Clamp(request,0,Money); 
    }

    //Army Movement / Attacking 
    public int AttackDes(Army a,Army HVTArmy,StratObj HVTStratObj) {
        /*No armies to attack, return the OBJ desirability*/
        if (HVTArmy == null) return a.getStrength() - (2 * HVTStratObj.getStrength()) + Aggresivness;
        /*Return the highest of the two*/
        if (HVTArmy.getStrategicValueForAI(a) > HVTStratObj.getStrategicValueForAI(this)) {
            return a.getStrength() - HVTArmy.getStrength() + Aggresivness;
        } else {
            return a.getStrength() - (2*HVTStratObj.getStrength()) + Aggresivness;
        }
    }
    public Army getHVTArmy(Army a) {
        List<Army> EnemyArmies = GameManager.GetAllEnemyArmies(ID);
        if (EnemyArmies.Count == 0) return null;
        int hvt_index = 0;
        bool AlreadyTargeted;
        for (int i = 0; i < EnemyArmies.Count; i++) {
            AlreadyTargeted = false;
            if (EnemyArmies[i].currentObj == null) { //Can only attack Army if not in an Obj, you need to attack the Obj to get to the army.
                if (EnemyArmies[hvt_index].getStrategicValueForAI(a) < EnemyArmies[i].getStrategicValueForAI(a)) {
                    foreach (Army b in Armies) {
                        if (b.CurrentTarget == EnemyArmies[hvt_index].ArmyObject) {
                            AlreadyTargeted = true;
                        }
                    }
                    if (!AlreadyTargeted) hvt_index = i;
                }
            }
        }
        return EnemyArmies[hvt_index];
    }
    public StratObj getHVTStratObj() {
        int hvt_index = 0;
        bool AlreadyTargeted;
        for (int i = 0; i < GameManager.Locations.Count; i++) {
            AlreadyTargeted = false;
            if (GameManager.Locations[hvt_index].getStrategicValueForAI(this) < GameManager.Locations[i].getStrategicValueForAI(this)) {
                foreach (Army b in Armies) {
                    if (b.CurrentTarget == GameManager.Locations[i].gObj) {
                        AlreadyTargeted = true;
                    }
                }
                if(!AlreadyTargeted)hvt_index = i;
            }
        }
        return GameManager.Locations[hvt_index];
    }
    
  
    //Army Recruitment    
    public int RecruitDes(Army army) { //How much the Army wants to be reinforced
        return (50 + Defencivness) - army.Force.GetSoldierCount() - 3 * army.Force.GetVehicleCount(); 
    }
    public float getRequestedSpendingMoney(Army army) {//Asks the Army how much money it wants to spend on recruitment
        float request = (100-army.Force.GetSoldierCount()) + (10 - army.Force.GetVehicleCount());
        if (request < 0) return 0;
        else return request; 
    } 
    /**************************/

    /*Action Functions*/
    public float SpendOnTownUpgrades(float SpendingMoney, StratObj town) { //Tell the town how much money it has to spend, return money spent
        float MoneyforDefence = (SpendingMoney * ((Defencivness * 0.50f)/100));
        float MoneyforSupply = SpendingMoney - MoneyforDefence;
        if (Verbose) Debug.Log(Name + "; " + town.getName() + ": Spending $" + MoneyforDefence + " on defences, and " + MoneyforSupply + " on Supply");
        town.MoneyToDefences(MoneyforDefence);
        town.MoneyToSupply(MoneyforSupply);
        return MoneyforDefence + MoneyforSupply; 
    }
    public float SpendOnRecruitment(float SpendingMoney, Army army) { //Gives the army money to spend, Returns Amount spent
        if (SpendingMoney < 0) return 0;
        float MoneyforInfantry = (SpendingMoney * 0.6f);
        float MoneyforVehicles = SpendingMoney - MoneyforInfantry;
        int NumberofInfantry = (int)MoneyforInfantry / army.getInfCost();
        int NumberofVehicles = (int)MoneyforVehicles / army.getVehicleCost();
        float Spent = 0;
        if (NumberofInfantry > 0) {
            Spent += army.RecruitInfantry(NumberofInfantry);            
        }
        if (NumberofVehicles > 0) {
            Spent += army.RecruitVehicle(NumberofVehicles);
        }
        if(Verbose) Debug.Log(army.Name + " spent $" + Spent + " on recruitment, Attained: " + NumberofInfantry + " infantry and " + NumberofVehicles + " vehicles.");
        if (Spent < 15) Aggresivness += 15;
        return Spent; 
    }

    /* Utility Functions */
    public bool isBusy() {
        return (ArmiesWaitingToMove.Count + BattlesWaitingToResolve.Count + ArmiesWaitingToEnter.Count)>0;
    }

    public float CalculateAvailableMoney(float Request, float TotalRequested) {
    /*Takes the percentage of the total money requested, and gives that percentage of the available money*/
        return Money*(Request/TotalRequested);
    }
    public float CalculateRecruitmentMoney() {
        float initialSpending = Money / 2;
        float AggressionSpending = (Mathf.Clamp(Aggresivness, 0, Money - initialSpending));
        return initialSpending + AggressionSpending;
    }

    public float SumOfArray(float[] a) {
        float sum = 0;
        for (int i = 0; i < a.Length; i++) {
            sum += a[i];
        }
        return sum;
    }
    
}
