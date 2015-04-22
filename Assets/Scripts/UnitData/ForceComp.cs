using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* A force composition will be used to hold all of the Soldiers, Vehicles, Crew and Squad info for an 'army' */
//TODO: Differentiate Battlegroup Types: Infantry division, Armour, etc. Maybe.

public class ForceComp {
	string Name;

	Dictionary<string,Squad> Squads;
	Dictionary<string,Human> Infantry;
	Dictionary<string,Vehicle> Vehicles;

	public string getName(){return this.Name;}
	public void setName(string n){this.Name = n;}


	public ForceComp(){
		this.Name = "Garrison";
		this.Squads = new Dictionary<string,Squad> ();
		this.Infantry = new Dictionary<string,Human> ();
		this.Vehicles = new Dictionary<string,Vehicle> ();
	}

	public ForceComp(string n){
		this.Name = n;
		this.Squads = new Dictionary<string,Squad> ();
		this.Infantry = new Dictionary<string,Human> ();
		this.Vehicles = new Dictionary<string,Vehicle> ();
	}

    /* TODO FUNCTINONS */

    public void RemoveSoldiers(int n) { /*Removes N soldiers, only good if soldiers aren't unique...*/
        int i = 0;
        List<string> keys = new List<string>();
        foreach (string key in Infantry.Keys) {
            if (i >= n) break;
            keys.Add(key);
            i++;
        }
        foreach (string k in keys) {
            Infantry.Remove(k);
        }
    }
    public void RemoveSoldiers(List<Human> casualties) { /*Removes all from the list, good for removinig specific soldiers*/ }
    /*Same stuff for vehicles*/
    public void RemoveVehicles(int n) {
        int i = 0;
        List<string> keys = new List<string>();
        foreach (string key in Vehicles.Keys) {
            if (i >= n) break;
            keys.Add(key);
            i++;
        }
        foreach (string k in keys) {
            Vehicles.Remove(k);
        }
    }
    public void RemoveVehicles(List<Vehicle> casualties) { }

   /*******************/
	
	public void AddSoldier(Player p){ //Create Empty soldier.
		this.Infantry.Add ("0x"+p.getNextID(),new Human());
	}
    public void AddVehicle(Player p) { //Create Empty Vehicle
        this.Vehicles.Add("0x" + p.getNextID(), new Vehicle());
	}
    public void AddSoldiers(int n, Player p) { //Create Empty Soldiers, for game start
		for(int i=0;i<n;i++){
            this.Infantry.Add("0x" + p.getNextID(), new Human());
		}
	}
    public void AddVehicles(int n, Player p) { //Create Empty Vehicles, for game start
		for(int i=0;i<n;i++){
            this.Vehicles.Add("0x" + p.getNextID(), new Vehicle());
		}
	}
	
	public void AddSoldier(Human human){
		this.Infantry.Add (human.getHID(),human);
	}

	public void AddVehicle(Vehicle vehicle){
		this.Vehicles.Add (vehicle.getHID(),vehicle);
	}
	public void AddSquad(Squad squad){
		this.Squads.Add (squad.getName(),squad);
	}

	public void setInventory(string hid,string inv){
		//Debug.Log ("Trying to set an inventory... " + hid);
		Human tmp;
		Vehicle tmp2;
		if (Infantry.TryGetValue (hid,out tmp)) {
			//Debug.Log ("Setting a Human Inventory");
			tmp.setInventory (inv);
			return;
		} else if (Vehicles.TryGetValue (hid,out tmp2)) {
			//Debug.Log ("Setting a Vehicle Inventory");
			tmp2.setInventory(inv);
			return;
		}
		//Debug.Log ("Nothing set!");
	}

    public int EstimateStrength() {
        return (int)Mathf.Pow((float)GetSoldierCount(), 1.4f) + 5 * (int)Mathf.Pow((float)GetVehicleCount(), 1.1f);
    }

	public int GetSoldierCount(){
		return this.Infantry.Count;
	}
	public int GetVehicleCount(){
		return this.Vehicles.Count;
	}

	//Takes a list of MIDs in format '12 34 56 78 90'
	//Linearly searches Infantry to find matching MIDs (no better way to search right now...)
	//So, only infantry get saved in squads (good)
	public void MakeGroupFromMIDS(string name,string MID_LIST){
		Squad newsquad = new Squad (name);
		string[] tokens = MID_LIST.Trim ().Split (' ');
		for(int i=0;i<tokens.Length;i++){
			foreach(Human soldier in Infantry.Values){
				if(soldier.getMID().Equals(tokens[i])){
					newsquad.AddMember(soldier);
					break;
				}
			}
		}
		if(newsquad.CountMembers() != 0)
		Squads.Add (name,newsquad);
	}

	public void LinkCrewToVehicle(string crew,string vehicle){
		Vehicles [vehicle].AddCrewMember (Infantry [crew]);
	}

	public string DeployAt(MapData.SpawnPoint Point)
	{	//Same as ToString, but adds coordinates
		int xoff = 0, yoff = 150;
		int x = Point.getX ();
		int y = Point.getY ();
		int dir = Point.getDir();

		string Force = ";=====Start Block for: " + Name + " =====\n\n";
		y -= yoff; 
		foreach (Human soldier in Infantry.Values) {
			Force += soldier.DeployAt(x+xoff,y,dir) + "\n";
			xoff+=20;
		}
		y += yoff; 
		xoff = 0;
		foreach (Vehicle veh in Vehicles.Values) {
			Force += veh.DeployAt(x+xoff,y,dir);
			xoff+=200;
		}
		Force += "{Squads\n";
		foreach (Squad squad in Squads.Values) {
			Force += squad.ToString() + "\n";
		}
		Force += "}\n";
		Force += "\n;=====End Block=====\n\n";
		return Force;
	}

	public string PlayerToString () //Same as ToString but sets AI control to user
	{	//This ToString will do most of the work! Writes the whole force out to mission
		string Force = ";=====Start Block for: " + Name + " =====\n\n";
		foreach (Human soldier in Infantry.Values) {
			Force += soldier.ToString() + "\n";
			Force += "\n{Brain "+soldier.getHID()+
							"{Properties"+
							"{control user}\n}\n}";
		}
		foreach (Vehicle veh in Vehicles.Values) {
			Force += veh.ToString() + "\n" + veh.CrewToLinkers() + "\n";
			Force += "\n{Brain "+veh.getHID()+
							"{Properties"+
							"{control user}\n}\n}";
		}
		
		foreach (Squad squad in Squads.Values) {
			Force += squad.ToString() + "\n";
		}
		Force += "\n;=====End Block=====\n\n";
		return Force;
	}
	public override string ToString ()
	{	//This ToString will do most of the work! Writes the whole force out to mission
		string Force = ";=====Start Block for: " + Name + " =====\n\n";
		foreach (Human soldier in Infantry.Values) {
			Force += soldier.ToString() + "\n";
		}
		foreach (Vehicle veh in Vehicles.Values) {
			Force += veh.ToString() + "\n" + veh.CrewToLinkers() + "\n";
		}
		Force += "\n;=====End Block=====\n\n";
		return Force;
	}

}
