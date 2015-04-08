using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Vehicle{
	enum Veh_Type{Tank,Gun,Truck,Transport}; //Use this to choose which type of seats to fill
	string Type; //"m4a1"
	string HID; //0xa004
	string MID; // "{MID x}"
	Dictionary<string,Human> CrewMemebers;
	string MetaData; //Holds unmodified Game data...
	string InventoryData; //Holds unmodified Inventory Data...
	string Tag;
    Veh_Type VType;

	
	public Vehicle(){ //Create Empty Vehicle Object...
		this.Type = "";
		this.HID = "";
		this.MID = "";
		CrewMemebers = new Dictionary<string,Human>();
		this.MetaData = "";
		this.InventoryData = "";
        VType = Veh_Type.Transport;
	}

	public Vehicle(string meta){ //Create Empty Human Object...
		string[] tokens = meta.Split ('\n');
		string[] firstline = meta.Split (' '); // {Entity "m4a1" 0xa000
		
		for (int i=1; i<tokens.Length; i++) {
			if(tokens[i].Contains("{MID")){
				this.MID = Regex.Replace(tokens[i],"[^0-9]", ""); //Remove all but the number. {MID xx} => xx
				break;
			}
			this.MID = ""; //set it to nothing if not found...
		}
		this.Type = firstline[1];
		this.HID = firstline [2].Substring(0,6);
		this.Tag = "{Tags \"vehicle\" " + this.HID + "}";
		this.MetaData = meta;
		this.InventoryData = ""; //don't have this info yet		
		CrewMemebers = new Dictionary<string,Human>();
        VType = Veh_Type.Tank; //TODO: know what type of vehicle it is
	}
	
	public string CrewToLinkers(){
		int i = 0;
		string Link = "";
		foreach (Human crew in CrewMemebers.Values) {
            Link += "\n{Link " + crew.getHID() + " {" + this.HID + " \"" + SeatTypes.SeatList[(int)VType][i] + "\"}}"; /*{Link 0xa006 {0x8004 "gunner"}}*/
			i++;
		}
		return Link;
	}

	public void AddCrewMember(Human newGuy){
		CrewMemebers.Add (newGuy.getHID(), newGuy);
	}

	public string DeployAt(int x,int y,int dir){
		//Sets position to X and Y in metadata...
		string meta = "";
		string[] tokens = this.MetaData.Split ('\n');
		meta += tokens [0];
		meta += "\n\t\t{Position " + x + " " + y + "}" + "\n";
		meta += "\n\t\t{xform zl " + dir + "}";
		for (int i=1; i<tokens.Length; i++) {
			meta+= "\n" + tokens[i];
		}
		//Debug.Log ("Deployment metadata:\n" + meta);
		return meta + "\n" + CrewToLinkers() + "\n" +this.InventoryData + this.CrewToLinkers() + "\n"  + this.BrainToString() + "\n" + this.Tag;
	}

	public string BrainToString(){
		return "\n\t{Brain " + HID + 
			"\n\t\t{Properties" + 
				"\n\t\t{control user}\n}\n}";
	}

	public override string ToString ()
	{	//When you Call to ToString, you get Data to be written out to a mission file...
		return this.MetaData + "\n" + CrewToLinkers() + "\n" +this.InventoryData + "\n"  + this.BrainToString() + "\n" + this.Tag;
	}

	//Setters
	public void setType(string t){ this.Type = t; }
	public void setHID(string h){ this.HID = h; }
	public void setMID(string m){ this.MID = m; }
	public void setMeta(string meta){this.MetaData = meta;}
	public void setInventory(string inv){this.InventoryData = inv;}
	//Getters
	public string getType(){ return this.Type; }
	public string getHID(){ return this.HID; }
	public string getMID(){ return this.MID; }
	public string getMeta(){return this.MetaData;}
	public string getInventory(){return this.InventoryData;}
}
