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
    public StratObj currentObj;
    private TextMesh[] TextMeshes;
	int Wins, Losses, Morale;

    public Queue<Vector3> MovingPath;
    
    /******* Creation *******/
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
        MovingPath = new Queue<Vector3>();

        TextMeshes = ArmyObject.GetComponentsInChildren<TextMesh>();
        TextMeshes[0].text = n;
        TextMeshes[1].text = "Inf: " + Force.GetSoldierCount();
        TextMeshes[2].text = "Tk: " + Force.GetVehicleCount();
	}
    
    public string GenerateName() { /*Use a NameList to generate names for generals*/ 
        return "Cmdr." + this.Name; 
    }

    public void Destroy() {
        Debug.Log(Name + " destroyed!");
    }

    /******* Get Data *******/
    public int getStrength() {
        return Force.EstimateStrength();
    }
    public int GetUpkeep() {
        return Force.GetSoldierCount() + 5*Force.GetVehicleCount();
    }
    public float getMoveRange() {
        float range = 25;
        if (Force.GetSoldierCount() < (Force.GetVehicleCount() * 10)) {
            range = 50;
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
        return (a.getStrength() - getStrength()) - (int)(ArmyObject.transform.position - a.ArmyObject.transform.position).magnitude;
    }
    public bool inObj() { return currentObj != null; } //True if in an Objective (currentObj not null)

    /******* Game actions *******/
    public void Retreat() {
        float x = Random.Range(-(getMoveRange() / 2)/10, getMoveRange() / 2/10);
        float y = Random.Range(-(getMoveRange() / 2)/10, getMoveRange() / 2/10);
        MoveTo(ArmyObject.transform.position + new Vector3(x,y,0));
    }
    public Battle AttackTarget(Army target) {
        Debug.Log(Name + ": Moving to attack " + target.getName());
        if (MoveTo(target.ArmyObject.transform.position)) {
            return new Battle(this, target);
        } else {
            return null;
        }
    }
    public Battle AttackTarget(StratObj target) {
        Debug.Log(Name + ": Moving to attack " + target.getName());
        if (MoveTo(target.gObj.transform.position)) {
            return new Battle(this, target);
        } else {
            return null;
        }
    }
    public bool MoveTo(Vector3 dest) {
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
    public void Enter(StratObj obj) {
        Debug.Log(Name + " entering " + obj.getName());
        obj.setArmy(this);
        currentObj = obj;
    }
    public void Leave() {
        Debug.Log(Name + " Leaving " + currentObj.getName());
        currentObj.clearArmy();
        currentObj = null;
    }

    /******* Modifiers *******/
    public void RecruitInfantry(int n) { 
        /*Add N Infantry to Force*/
        Force.AddSoldiers(n);
    }
    public void RecruitVehicle(int n) { 
        /*Add N Vehicles to Force*/
        Force.AddVehicles(n);
    }

    public void TakeLosses(int n) {
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
      	
	
	
	//Getters
	public string getName(){return this.Name;}
	public int getWins(){return this.Wins;}
	public int getLosses(){return this.Losses;}

	//Setters
	public void setWins(int x){this.Wins=x;}
	public void setLosses(int x){this.Losses=x;}	
	public void AddWin(){this.Wins++;}
	public void AddLoss(){this.Losses++;}

    public override string ToString() {
        return "The " + Name + ", Commanded by: " + this.Leader.getName() + " has " + Force.GetSoldierCount() + " infantry, " + Force.GetVehicleCount() + " vehicles.";
    }

}
