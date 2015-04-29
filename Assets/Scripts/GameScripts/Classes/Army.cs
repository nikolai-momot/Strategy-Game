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

    private int InfCost = 15; //Cost to buy a soldier
    private int VehicleCost = 45; //Cost to buy a vehicle
    private int InfPerVehicle = 5; // # of infantry that can ride on a vehicle. Effects how far the army can move

    public Queue<Vector3> MovingPath;
    public GameObject CurrentTarget;
    
    /******* Creation *******/
	public Army(string n,GameObject ArmyObject, Player Owner){
		Name = n;
        this.Owner = Owner;
        Leader = new General(GenerateName());
        this.ArmyObject = ArmyObject;        
		Force = new ForceComp(); //Default Army
		Force.AddSoldiers(30,Owner);
		Force.AddVehicles(2,Owner);

        //currentCell = hq; //Starts at the HQ
		Wins = 0;
		Losses = 0;
		Morale = 100; //TODO: set a scale for morale or something
        MovingPath = new Queue<Vector3>();
        CurrentTarget = null;

        if (ArmyObject != null) { //If we have an Army Object, set up the visuals
            ArmyObject.GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Flags/flag_" + Owner.Country);
            TextMeshes = ArmyObject.GetComponentsInChildren<TextMesh>();
            TextMeshes[0].text = n;
            TextMeshes[1].text = "Inf: " + Force.GetSoldierCount();
            TextMeshes[2].text = "Tk: " + Force.GetVehicleCount();
        }
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
        float range = 15;
        if (Force.GetSoldierCount() < (Force.GetVehicleCount() * InfPerVehicle)) {
            range = 45;
        }
        return range;
    }
    public float getPathDistance(List<Vector3> path) {
        float dist = 0;
        for (int i = 0; i < path.Count-1; i++) {
            dist += Vector3.Distance(path[i], path[i + 1]);
        }
            //Debug.Log("getPathDistance(): " + dist);
        return dist;
    }
    public int getStrategicValueForAI(Army a) {
        if (a.ArmyObject == null) return -1;
        return (a.getStrength() - getStrength()) - (int)Mathf.Pow(Vector3.Distance(ArmyObject.transform.position,a.ArmyObject.transform.position),2);
    }
    public bool inObj() { return currentObj != null; } //True if in an Objective (currentObj not null)

    /******* Game actions *******/
    public void Retreat() {
        //float dist = Random.Range(-2, 2);
        /*Vector3 fallback = (Owner.getClosestOwnedObj(this).getMapPosition() - getMapPosition());
        Debug.DrawRay(getMapPosition(),fallback,Color.black,5);
        ArmyObject.SetActive(true);
        Vector3 fallback = new Vector3(getMapPosition().x + dist,getMapPosition().y + dist,getMapPosition().z);
        JumpTo(fallback);
        GameManager.InstantiateRetreatIndAt(fallback);*/
        Vector3 fallback = GameManager.GetRetreatLocation(this);
        JumpTo(fallback);
        GameManager.InstantiateRetreatIndAt(fallback);
    }
    public Battle AttackTarget(Army target) {
        //Debug.Log(Name + ": Moving to attack " + target.getName());
        CurrentTarget = target.ArmyObject;
        if (MoveTo(target.ArmyObject.transform.position)) {
            return new Battle(this, target);
        } else {
            return null;
        }
    }
    public Battle AttackTarget(StratObj target) {
        //Debug.Log(Name + ": Moving to attack " + target.getName());
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
            /*We have enough movement range to move the whole path, enqueue all points*/
            foreach (Vector3 p in path) {
                MovingPath.Enqueue(p);
            }
            MovingPath.Enqueue(dest);
            finish = true;
        } else {
            /*Enqueue the first point on the path, subtract it's distance from our move range*/
            MovingPath.Enqueue(path[0]);
            range -= Vector3.Distance(getPosition(), path[0]);
            for (int i = 1; i < path.Count; i++) {
                /*If we have enough moviing range to get to the next point, Enqueue it and subtract from our range */
                if (Vector3.Distance(path[i], path[i - 1]) <= range) {
                    MovingPath.Enqueue(path[i]);
                    range -= Vector3.Distance(path[i], path[i - 1]);
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
        setPosition(dest);
    }
    public void Enter(StratObj obj) {

        if (obj.getOwnerID() == this.getOwnerID()) { // We own this Obj
            if (obj.getArmy() == null) {
                Debug.Log(Name + " entering " + obj.getName());
                setPosition(obj.getMapPosition());
                obj.setArmy(this);
                currentObj = obj;
            } else {
                if (obj.getArmy() == this) {
                    //Uhh we're already in there? Just clear the army and try again...
                    obj.clearArmy();
                    Enter(obj);
                }
                setPosition(obj.getMapPosition());
                Debug.Log(Name + " Can't enter " + obj.getName() + ". " + obj.getArmy().getName() + " is already there");
            }
        } else { //We don't own the Obj
            Debug.Log(Name + " taking " + obj.getName());
            if(obj.getArmy() != null) obj.getArmy().Leave();            
            setPosition(obj.getMapPosition());
            obj.setArmy(this);
            currentObj = obj;
            obj.ClearGarrison();
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
    public float RecruitInfantry(int n) {
        if(ArmyObject != null)GameManager.InstantiateAddUnitAt(ArmyObject.transform.position);
        /*Add N Infantry to Force*/
        Force.AddSoldiers(n,Owner);
        return n * InfCost; // return what we spent
    }
    public float RecruitVehicle(int n) {
        if (ArmyObject != null) GameManager.InstantiateAddUnitAt(ArmyObject.transform.position);
        /*Add N Vehicles to Force*/
        Force.AddVehicles(n,Owner);
        return n * InfCost; // return what we spent
    }

    public void TakeLosses(int n) {
        if (ArmyObject != null) GameManager.InstantiateLoseUnitAt(ArmyObject.transform.position);
        Debug.Log(Name + " taking " + n + " losses!");
        if (n >= Force.GetSoldierCount() + Force.GetVehicleCount()) {
            Force.RemoveSoldiers(Force.GetSoldierCount());
            Force.RemoveVehicles(Force.GetVehicleCount());
            Destroy();
        } else {
            int inf=0,veh=0;
            for (int i = 0; i < n; i++) {
                //Distribute Losses onto Infantry and vehicles
                if(Random.Range(0,100) > 70){
                    veh++;
                }else{
                    inf++;
                }
            }
            Force.RemoveSoldiers(inf);
            Force.RemoveVehicles(veh);
        }
        if ((Force.GetSoldierCount() + Force.GetVehicleCount()) < 1) Destroy();
    }
    public void UpdateNumbers() {
        if (ArmyObject == null) return;
        TextMeshes[1].text = "Inf: " + Force.GetSoldierCount();
        TextMeshes[2].text = "Tk: " + Force.GetVehicleCount();
    }
    public void Destroy() {
        //ArmyObject.SetActive(false);
        Debug.Log("== !! " + Name + " destroyed! (Returning to Base) !! ==");
        JumpTo(Owner.HQ.getMapPosition());
        Enter(Owner.HQ);
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
    public Vector3 getMapPosition() { return ArmyObject.transform.position; }
    public int getInfCost() { return InfCost; }
    public int getVehicleCost() { return VehicleCost; }
    public Vector3 getPosition() { return ArmyObject.transform.position; }

	//Setters
	public void setWins(int x){this.Wins=x;}
	public void setLosses(int x){this.Losses=x;}	
	public void AddWin(){
        if (ArmyObject != null) GameManager.InstantiateVictoryAt(ArmyObject.transform.position);
        this.Wins++;
    }
	public void AddLoss(){
        if (ArmyObject != null) GameManager.InstantiateDefeatAt(ArmyObject.transform.position);
        this.Losses++;
    }
    public void setPosition(Vector3 newPos) {
        this.ArmyObject.transform.position = newPos;
    }

    public override string ToString() {
        return "The " + Name + ", Commanded by: " + this.Leader.getName() + " has " + Wins + " Victories and " + Losses + " defeats.\n"
               + Force.GetSoldierCount() + " Infantry, and " + Force.GetVehicleCount() + " Vehicles.";
    }

}
