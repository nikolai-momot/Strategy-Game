using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Parent Object for any strategc point.
 * Base: Starting point for a player, source of supply line **Only one per Player!**
 * City: Will produce lots of resources, not much defences to start
 * Town: Same as city, but less reward
 * Outpost: Opposite from City, lots of defence, little money to start 
 * */

public class StratObj{

	public string Name;
	public Player Owner; //Keep track of the owner
    public Army OccupyingArmy; //Armies at this town
    public ForceComp Garrison; //Garrison acts like and army, but just defends the town
	public int DefenceLevel; //1-100
	public int SupplyLevel; //Probably just the raw supply number
    public Vector3 MapPosition; //We'll get the town's co-ordinates on the map, and it's connected towns from this object.
    public GameObject gObj;
    private TextMesh[] TextMeshes;
    public int x, y;		//cell map coordinates

	public StratObj(){}	//Need this for inheritance
	
	//Contructor will take the name, the gameObject(Location) and the enum type
	public StratObj(string n,GameObject p,Player Owner){
		Name = n; //Name always set
		gObj = p;
        p.tag = "Objective";
        MapPosition = p.transform.position;
        this.Owner = Owner;
        OccupyingArmy = null;
        Garrison = new ForceComp();

        TextMeshes = gObj.GetComponentsInChildren<TextMesh>();
        TextMeshes[0].text = n;
        TextMeshes[1].text = "";
   	}
	
	public int getStrategicValueForAI(Player_AI player){
        if (getOwnerID() == player.ID) return 0; //No value to attack if it's owned already!
        int dist_from_base = (int)Vector3.Distance(this.gObj.transform.position,player.HQ.gObj.transform.position);
        //Debug.Log("STRAT VALUE: " + player.Name + " sees " + this.Name + " as " + ((100 + player.FocusOnSupplies) - dist_from_base));
        return (100+player.FocusOnSupplies) - dist_from_base;
	}

    public void MoneyToDefences(int Money){
        this.DefenceLevel += Money / getUpgradeDefenceCost();
    }
    public void MoneyToSupply(int Money){
        this.SupplyLevel += Money / getUpgradeSupplyCost();
    }

    public void AddSoldiersToGarrison(int n) {
        Garrison.AddSoldiers(n, this.Owner);
    }
    public void AddVehiclesToGarrison(int n) {
        Garrison.AddVehicles(n, this.Owner);
    }
	
	//Setters
	public void setName(string n){Name=n;}
    public void setOwner(Player Owner){ this.Owner = Owner; }
    public void setArmy(Army a) {
        OccupyingArmy = a;
        //a.ArmyObject.transform.position = this.gObj.transform.position + Vector3.down;
        //a.ArmyObject.transform.localScale = a.ArmyObject.transform.localScale / 2;
        OccupyingArmy.ArmyObject.SetActive(false);
        setOwner(a.getOwner());
    }
    public void clearArmy() {         
        //OccupyingArmy.ArmyObject.transform.position = this.gObj.transform.position - Vector3.down;
        //OccupyingArmy.ArmyObject.transform.localScale = OccupyingArmy.ArmyObject.transform.localScale * 2;
        OccupyingArmy.ArmyObject.SetActive(true);
        OccupyingArmy = null;
    }

    public void TakeLosses(int n) {
        Debug.Log(OccupyingArmy.Force.getName() + " in "+ Name +" taking " + n + " losses!");
        if (n >= OccupyingArmy.Force.GetSoldierCount() + OccupyingArmy.Force.GetVehicleCount()) {
            OccupyingArmy.Destroy();
        } else {
            if (n <= OccupyingArmy.Force.GetSoldierCount()) {
                OccupyingArmy.Force.RemoveSoldiers(n);
            } else {
                OccupyingArmy.Force.RemoveSoldiers(n);
                OccupyingArmy.Force.RemoveVehicles(n);
            }
        }
        if ((OccupyingArmy.Force.GetSoldierCount() + OccupyingArmy.Force.GetVehicleCount()) < 1) {
            OccupyingArmy.Destroy();
        }
    }

    public void UpdateInfo() {
        TextMeshes[1].text = "D: " + getDefenceLevel() +
                           "\nS: " + getSupplyLevel();
    }

	//Getters
	public string getName(){
        if (OccupyingArmy != null) return Name + " (" + getOwnerID() + ")" + "(" + OccupyingArmy.getName() + ")";
        else return Name + " ("+getOwnerID()+")";
    }
	public int getDefenceLevel(){return DefenceLevel;}
	public int getSupplyLevel(){return SupplyLevel;}
	public GameObject getMapObject(){return gObj;}
	public Vector3 getMapPosition(){return MapPosition;}
    public int getUpgradeDefenceCost() { return 1; }
    public int getUpgradeSupplyCost() { return 1; }
    public Army getArmy() { return OccupyingArmy; }
    public Player getOwner(){ return Owner; }
    public int getOwnerID() {
        if (Owner == null) return 0;
        return Owner.ID; 
    }

    public int getStrength() {
        int str = 0;
        if(OccupyingArmy != null){
        str += OccupyingArmy.getStrength() + Garrison.EstimateStrength();
        } else {
            str += Garrison.EstimateStrength(); ;
        }
        if (str == 0)return 0;        
        return str + DefenceLevel;
    }
}
