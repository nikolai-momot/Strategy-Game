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
    public Army Garrison; //Garrison acts like and army, but just defends the town
	public int DefenceLevel; //1-100
	public int SupplyLevel; //Probably just the raw supply number
    public GameObject gObj;
    private TextMesh[] TextMeshes;
    public int x, y;		//cell map coordinates

    private SpriteRenderer flag;

	public StratObj(){}	//Need this for inheritance
	
	//Contructor will take the name, the gameObject(Location) and the enum type
	public StratObj(string n,GameObject p,Player Owner){
		Name = n; //Name always set
		gObj = p;
        p.tag = "Objective";
        this.Owner = Owner;
        OccupyingArmy = null;
        if(this.Owner != null)Garrison = new Army(n + " Garrison",null,Owner);

        flag = gObj.GetComponentInChildren<SpriteRenderer>();
        flag.sprite = Resources.Load<Sprite>("Sprites/Flags/flag_empty");

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

    public void MoneyToDefences(float Money){
        if (Money < 0) return;
        Garrison.RecruitInfantry((int)((Money * 0.2f) ));
        Garrison.RecruitVehicle((int)((Money * 0.1f)));
        this.DefenceLevel += (int)(Money * 0.7f / getUpgradeDefenceCost());
    }
    public void MoneyToSupply(float Money) {
        this.SupplyLevel += (int)Money / getUpgradeSupplyCost();
    }

    public void AddSoldiersToGarrison(int n) {
        Garrison.RecruitInfantry(n);
    }
    public void AddVehiclesToGarrison(int n) {
        Garrison.RecruitVehicle(n);
    }
		
    public void clearArmy() {         
        //OccupyingArmy.ArmyObject.transform.position = this.gObj.transform.position - Vector3.down;
        //OccupyingArmy.ArmyObject.transform.localScale = OccupyingArmy.ArmyObject.transform.localScale * 2;
        OccupyingArmy.ArmyObject.SetActive(true);
        OccupyingArmy = null;
    }

    public void TakeLosses(int n) {
        if (OccupyingArmy != null) {
            Debug.Log(OccupyingArmy.Force.getName() + " in " + Name + " taking " + n + " losses!");
            OccupyingArmy.TakeLosses(n);
        }
    }

    public void UpdateInfo() {
        if(Owner!=null)
            flag.sprite = Resources.Load<Sprite>("Sprites/Flags/flag_" + Owner.getCountry()); //Update Flag Sprite
        else
            flag.sprite = Resources.Load<Sprite>("Sprites/Flags/flag_empty");
        TextMeshes[1].text = "D: " + getDefenceLevel() +
                           "\nS: " + getSupplyLevel();
        if (Garrison != null) TextMeshes[3].text = "Inf: " + Garrison.Force.GetSoldierCount();
        if (Garrison != null) TextMeshes[2].text = "Tk: " + Garrison.Force.GetVehicleCount();
    }

    public void ClearGarrison() {
        Garrison.Force.RemoveSoldiers(Garrison.Force.GetSoldierCount());
        Garrison.Force.RemoveVehicles(Garrison.Force.GetVehicleCount());
        Garrison.Owner = this.Owner;
    }

	//Getters
	public string getName(){
        if (OccupyingArmy != null) return Name + "(" + OccupyingArmy.getName() + ")";
        else return Name;
    }
	public int getDefenceLevel(){return DefenceLevel;}
	public int getSupplyLevel(){return SupplyLevel;}
	public GameObject getMapObject(){return gObj;}
	public Vector3 getMapPosition(){return gObj.transform.position;}
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
            str += OccupyingArmy.getStrength() + Garrison.getStrength();
        } else if(Garrison != null){
            str += Garrison.getStrength();
        }
        if (str == 0)return 0;        
        return str + 5*DefenceLevel;
    }

    
    //Setters
    public void setName(string n) { Name = n; }
    public void setOwner(Player Owner) {
        if (this.Owner == null)
            Garrison = new Army(getName() + " Garrison", null, Owner); //If owner was null, initialise the garrison
        else
            Garrison.Owner = Owner; //Otherwise, switch the Owner
        this.Owner = Owner; 
    }
    public void setArmy(Army a) {
        OccupyingArmy = a;
        //a.ArmyObject.transform.position = this.gObj.transform.position + Vector3.down;
        //a.ArmyObject.transform.localScale = a.ArmyObject.transform.localScale / 2;
        OccupyingArmy.ArmyObject.SetActive(false);
        setOwner(a.getOwner());
    }
    public void setFlag(string country) {
        flag.sprite = Resources.Load<Sprite>("Sprites/Flags/flag_" + country);
    }
}
