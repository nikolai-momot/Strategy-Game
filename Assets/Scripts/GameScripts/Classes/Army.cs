using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* An Army is the strategic map representation of a player's army
 * It will hold the army's statistics and information
 * */
public class Army{

	public string Name;
    public Player Owner;
    public General Leader;
	public ForceComp Force;
    public GameObject ArmyObject;
    public StratObj currentObj;
    private TextMesh[] TextMeshes;
	int Wins, Losses, Morale;

    public Queue<Vector3> MovingPath;
    public GameObject CurrentTarget;
    
    /******* Creation *******/
	public Army(string n,GameObject ArmyObject, Player Owner){
		Name = n;
        this.Owner = Owner;
        Leader = new General(GenerateName());
        this.ArmyObject = ArmyObject;
        ArmyObject.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Flags/flag_" + Owner.Country);
		Force = new ForceComp(); //Default Army
		Force.AddSoldiers(30,Owner);
		Force.AddVehicles(2,Owner);

        //currentCell = hq; //Starts at the HQ
		Wins = 0;
		Losses = 0;
		Morale = 100; //TODO: set a scale for morale or something
        MovingPath = new Queue<Vector3>();
        CurrentTarget = null;

        TextMeshes = ArmyObject.GetComponentsInChildren<TextMesh>();
        TextMeshes[0].text = n;
        TextMeshes[1].text = "Inf: " + Force.GetSoldierCount();
        TextMeshes[2].text = "Tk: " + Force.GetVehicleCount();
	}
    
    public string GenerateName() { /*Use a NameList to generate names for generals*/ 
        return "Cmdr." + this.Name; 
    }
        
    /******* Get Data *******/
    public int getStrength() {
        return Force.EstimateStrength();
    }
    public int GetUpkeep() {
        return Force.GetSoldierCount() + 5*Force.GetVehicleCount();
    }
    public float getMoveRange() {
        float range = 50;
        if (Force.GetSoldierCount() < (Force.GetVehicleCount() * 10)) {
            range = 150;
        }
        return range;
    }
    public float getPathDistance(List<Vector3> path) {
        float dist = 0;
        foreach (Vector3 p in path) {
            dist += p.magnitude;
        }
        Debug.Log("getPathDistance(): " + dist);
        return dist;
    }
    public int getStrategicValueForAI(Army a) {
        return (a.getStrength() - getStrength()) - (int)Mathf.Pow(Vector3.Distance(ArmyObject.transform.position,a.ArmyObject.transform.position),2);
    }
    public bool inObj() { return currentObj != null; } //True if in an Objective (currentObj not null)

    /******* Game actions *******/
    public void Retreat() {
        float x = Random.Range(-(getMoveRange() / 2)/10, getMoveRange() / 2/10);
        float y = Random.Range(-(getMoveRange() / 2)/10, getMoveRange() / 2/10);
        JumpTo(ArmyObject.transform.position + new Vector3(x,y,0));
    }
    public Battle AttackTarget(Army target) {
        Debug.Log(Name + ": Moving to attack " + target.getName());
        CurrentTarget = target.ArmyObject;
        if (MoveTo(target.ArmyObject.transform.position)) {
            return new Battle(this, target);
        } else {
            return null;
        }
    }
    public Battle AttackTarget(StratObj target) {
        Debug.Log(Name + ": Moving to attack " + target.getName());
        CurrentTarget = target.gObj;
        if (MoveTo(target.gObj.transform.position)) {
            return new Battle(this, target);
        } else {
            return null;
        }
    }
    public bool MoveToEnter(StratObj target) {
        Debug.Log("Moving to Enter " + target.getName());
        CurrentTarget = target.gObj;
        if (MoveTo(target.gObj.transform.position)) {
            return true;
        } else {
            return false;
        }
    }
    public bool MoveTo(Vector3 dest) {
        GameManager.InstantiateTargetIndAt(dest); //visual indication of target
        if (currentObj != null) {//If army is in a town, exit the town before moving
            Leave();
        }
        //Moves army along path
        //returns true if the army makes it to the end of the path
        bool finish = false;
        Cell startCell = GameManager.pathFinder.cellFromVector(this.ArmyObject.transform.position);
        Cell destCell = GameManager.pathFinder.cellFromVector(dest);
        List<Vector3> path = GameManager.pathFinder.cellToVectors(GameManager.pathFinder.FindPath(startCell, destCell), false);
        float range = getMoveRange();
        if (range >= getPathDistance(path)) {
            foreach (Vector3 p in path) {
                MovingPath.Enqueue(p);
            }
            MovingPath.Enqueue(dest);
            finish = true;
        } else {
            for (int i = 0; i < path.Count; i++) {
                if (path[i].magnitude <= range) {
                    MovingPath.Enqueue(path[i]);
                    range -= path[i].magnitude;
                } else {
                    break;
                }
            }
        }
        return finish;
    }
    public void JumpTo(Vector3 dest) {
        /*Teleports army to destination*/
        if (currentObj != null) {//If army is in a town, exit the town before moving
            Leave();
        }
        this.ArmyObject.transform.position = dest;
    }
    public void Enter(StratObj obj) {
        if (obj.getArmy() == null) {
            Debug.Log(Name + " entering " + obj.getName());
            obj.setArmy(this);
            currentObj = obj;
        } else if (obj.getArmy().getOwnerID() == this.getOwnerID()) {
            Debug.Log(Name + " Can't enter " + obj.getName() + ". " + obj.getArmy().getName() + " is already there");
        } else {
            Debug.Log(Name + " taking " + obj.getName() + " from " + obj.getArmy().getName());
            obj.getArmy().Leave();
            obj.setArmy(this);
            currentObj = obj;
        }
        GameManager.UpdateObjNames();
    }
    public void Leave() {
        Debug.Log(Name + " Leaving " + currentObj.getName());
        currentObj.clearArmy();
        currentObj = null;
        GameManager.UpdateObjNames();
    }

    /******* Modifiers *******/
    public void RecruitInfantry(int n) {
        GameManager.InstantiateAddUnitAt(ArmyObject.transform.position);
        /*Add N Infantry to Force*/
        Force.AddSoldiers(n,Owner);
    }
    public void RecruitVehicle(int n) {
        GameManager.InstantiateAddUnitAt(ArmyObject.transform.position);
        /*Add N Vehicles to Force*/
        Force.AddVehicles(n,Owner);
    }

    public void TakeLosses(int n) {
        GameManager.InstantiateLoseUnitAt(ArmyObject.transform.position);
        Debug.Log(Name + " taking " + n + " losses!");
        if (n >= Force.GetSoldierCount() + Force.GetVehicleCount()) {
            Destroy();
        } else {
            if (n <= Force.GetSoldierCount()) {
                Force.RemoveSoldiers(n);
            } else {
                Force.RemoveSoldiers(n);
                Force.RemoveVehicles(n);
            }
        }
        if ((Force.GetSoldierCount() + Force.GetVehicleCount()) < 1) Destroy();
    }
    public void UpdateNumbers() {
        TextMeshes[1].text = "Inf: " + Force.GetSoldierCount();
        TextMeshes[2].text = "Tk: " + Force.GetVehicleCount();
    }
    public void Destroy() {
        ArmyObject.SetActive(false);
        Debug.Log(Name + " destroyed!");
    }
    public bool isDefeated() {
        return getStrength() == 0;
    }
	
	
	//Getters
	public string getName(){return this.Name;}
	public int getWins(){return this.Wins;}
	public int getLosses(){return this.Losses;}
    public Player getOwner() { return Owner; }
    public int getOwnerID() { return Owner.ID; }

	//Setters
	public void setWins(int x){this.Wins=x;}
	public void setLosses(int x){this.Losses=x;}	
	public void AddWin(){
        GameManager.InstantiateVictoryAt(ArmyObject.transform.position);
        this.Wins++;
    }
	public void AddLoss(){
        GameManager.InstantiateDefeatAt(ArmyObject.transform.position);
        this.Losses++;
    }

    public override string ToString() {
        return "The " + Name + ", Commanded by: " + this.Leader.getName() + " has " + Force.GetSoldierCount() + " infantry, " + Force.GetVehicleCount() + " vehicles.";
    }

}
