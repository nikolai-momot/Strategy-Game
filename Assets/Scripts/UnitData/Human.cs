using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

public class Human{
	string Type; //"mp/usa/rifle"
	string HID; //0xa004
	string MID; // "{MID x}"
	string MetaData; //Holds unmodified Game data...
	string InventoryData; //Holds unmodified Inventory Data...

	
	public Human(){ //Create Empty Human Object...
		this.Type = "";
		this.HID = "";
		this.MID = "";
		this.MetaData = "";
		this.InventoryData = "";		
	}
	public Human(string meta){ //Create Empty Human Object...
		string[] tokens = meta.Split ('\n');
		string[] firstline = tokens[0].Split (' '); // {Human "mp/usa/rifle" 0xa000
		for (int i=1; i<tokens.Length; i++) {
			if(tokens[i].Contains("{MID")){
				this.MID = Regex.Replace(tokens[i],"[^0-9]", ""); //Remove all but the number. {MID xx} => xx
				break;
			}
			this.MID = ""; //set it to nothing if not found...
		}
		this.Type = firstline[1];
		this.HID = firstline [2].Substring(0,6);
		this.MetaData = meta;
		this.InventoryData = ""; //don't have this info yet
	}

	public string DeployAt(int x,int y,int dir){
		//Sets position to X and Y in metadata...
		string meta = "";
		string[] tokens = this.MetaData.Split ('\n');
		meta += tokens [0];
		meta += "\n\t\t{Position " + x + " " + y + "}";
		meta += "\n\t\t{xform zl " + dir + "}";
		for (int i=1; i<tokens.Length; i++) {
			meta+=	"\n" + tokens[i];
		}
		//Debug.Log ("Deployment metadata:\n" + meta);
		return meta + "\n" + this.InventoryData + this.BrainToString();
	}

	public string BrainToString(){
		return "\n\t{Brain " + HID + 
			"\n\t\t{Properties" + 
				"\n\t\t{control user}\n}\n}";
	}

	public override string ToString ()
	{	//When you Call to ToString, you get Data to be written out to a mission file...
		return this.MetaData + "\n" + this.InventoryData + this.BrainToString();
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

//  'MetaData'
/*{Human "mp/usa/tankman" 0xb075
		{Position -1698.5 2362.82}
		{xform zl 0.93}
		{Extender "vitality"
			{Veterancy 2}
		}
		{Player 1}
		{MID 60}
		{Score 17}
		{NameId 120 10}
		{LastItem "colt"}
}*/

// 'Inventory Data'
/*{Inventory 0xb075
		{box
			{clear}
			{item "colt" filled {cell 0 0}}
			{item "pistol" "ammo" 100 {cell 2 0}}
			{item "tankhelmet_usa" {cell 4 0}{user "head"}}
		}
	}*/