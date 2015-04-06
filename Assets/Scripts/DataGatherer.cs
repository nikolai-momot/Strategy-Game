using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//This class will read a saved game and build the ForceComps
public class DataGatherer : MonoBehaviour {

	public void LoadPlayerArmy(string path){
		//Reads army at path into PlayerArmy
		DataManager.PlayerArmy = null;
		DataManager.PlayerArmy = new ForceComp ("Player Army " + path.Substring(14));	
		//Helpers
		int squad = 0;
		string line = "";
		string Unit="",SquadName="";
		
		//I/O		
		StreamReader MissionReader = new StreamReader (path);
		// Reading through the file...
		while (line != null) {
			Unit="";
			

			if(line.Contains("{Human")){ //Start of a Human Block...
				if(line.Contains("{Position")){
					line=""; 
				}
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					Unit+=line+"\n";
				} //Finished Reading Unit now...
				DataManager.PlayerArmy.AddSoldier(new Human(Unit));
				Unit="";
			}
			
			if(line.Contains("{Entity")){ //Start of a Human Block...
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					Unit+=line+"\n";
				} //Finished Reading Unit now...
				DataManager.PlayerArmy.AddVehicle(new Vehicle(Unit));
				Unit="";
			}			
			if(line.Contains("{Inventory")){//Start of an inventory block...
				Unit += line;
				string[] tokens = line.Split(' ');
				
				while(!line.Equals("\t}")){
					line = MissionReader.ReadLine ();
					Unit+=line+"\n";
				}
				DataManager.PlayerArmy.setInventory(tokens[1],Unit);
				Unit="";
			}
			
			if(line.Contains("{Link")){
				while(line.Contains("{Link")){//Grabs all Links
					string[] tokens = line.Split(' ');
					DataManager.PlayerArmy.LinkCrewToVehicle(tokens[1],tokens[2].Trim('{'));
					line = MissionReader.ReadLine ();
				}
			}

			if (line.Contains ("{Squad")) {
				line = MissionReader.ReadLine ();
				while(!line.Equals("\t}")){
					if(line.Contains("{Actors")){
						//Debug.Log ("Line : " + line);
						SquadName = "squad" + squad + ":" +Regex.Replace (line, "[^0-9 ]", ""); //Remove all non numeric chars (leave spaces)
						squad++;
						//Debug.Log ("Squad : " + SquadName);
						string[] tokens = SquadName.Split (':');
						DataManager.PlayerArmy.MakeGroupFromMIDS (tokens [0], tokens [1]);
						//Debug.Log ("Found Squad: " + tokens [0] + " " + tokens [1]);
					}
					line = MissionReader.ReadLine ();
					SquadName = "";
				}
			}
			
			
			line = MissionReader.ReadLine ();
		}

		//Debug.Log (DataManager.PlayerArmy.ForceReport ());
		MissionReader.Close ();
	}

	public void LoadCPUArmy(string path){
		//Reads army at path into CPUArmy
		DataManager.CPUArmy = null;
		DataManager.CPUArmy = new ForceComp ("CPU Army " + path.Substring(14));	
		//Helpers
		string line = "";
		string Unit;

		
		//I/O		
		StreamReader MissionReader = new StreamReader (path);
		// Reading through the file...
		while (line != null) {
			Unit="";
			

			if(line.Contains("{Human")){ //Start of a Human Block...
				if(line.Contains("{Position")){
					line=""; 
				}
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					Unit+=line+"\n";
				} //Finished Reading Unit now...
				DataManager.CPUArmy.AddSoldier(new Human(Unit));
				Unit="";
			}
			
			if(line.Contains("{Entity")){ //Start of a Human Block...
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					Unit+=line+"\n";
				} //Finished Reading Unit now...
				DataManager.CPUArmy.AddVehicle(new Vehicle(Unit));
				Unit="";
			}			
			if(line.Contains("{Inventory")){//Start of an inventory block...
				Unit += line;
				string[] tokens = line.Split(' ');
				
				while(!line.Equals("\t}")){
					line = MissionReader.ReadLine ();
					Unit+=line+"\n";
				}
				DataManager.CPUArmy.setInventory(tokens[1],Unit);
				Unit="";
			}
			
			if(line.Contains("{Link")){
				while(line.Contains("{Link")){//Grabs all Links
					string[] tokens = line.Split(' ');
					DataManager.CPUArmy.LinkCrewToVehicle(tokens[1],tokens[2].Trim('{'));
					line = MissionReader.ReadLine ();
				}
			}
			
			
			line = MissionReader.ReadLine ();
		}
		//Debug.Log (DataManager.CPUArmy.ForceReport ());
		MissionReader.Close ();
	}



	public void GenerateForcesFromSave(string path){
		//Read from the save file...
		int squad = 0;
		//Data Structures...
		DataManager.PlayerArmy = new ForceComp ("Player Army");
		DataManager.CPUArmy = new ForceComp ("CPU Army");
		Dictionary<string,Vehicle> Entities = new Dictionary<string, Vehicle> ();
		string PlayerVehicleList = "", CPUVehicleList = "";
		string VehicleList = GetVehicleTags (path + "/mission.scn");
		//Helpers
		bool isPlayer = false;
		string line = "";
		string Unit,SquadName="";

		//I/O		
		StreamReader MissionReader = new StreamReader (path + "/mission.scn");
		StreamReader SquadReader = new StreamReader (path + "/game.scn");

		StreamWriter MapWriter = new StreamWriter ("Assets/Maps/PersistentMapData/" + MapSwitcher.MapNames[MapSwitcher.CurrentMap]);
		StreamReader MapReader = new StreamReader (path + "/map.scn");

		StreamWriter PlayerWriter = new StreamWriter ("Assets/PostBattleArmies/player_forces");
		StreamWriter CPUWriter = new StreamWriter ("Assets/PostBattleArmies/cpu_forces");

		
		// Reading through the file...
		while (line != null) {
			Unit="";
			
			if(line.Contains("{Human")){ //Start of a Human Block...
				if(line.Contains("{Position")){
					//Debug.Log("Clearing line: " + line);
					line=""; 
				}
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					if(line.Contains("{Player 0}")){
						isPlayer=true;
					}else if(line.Contains("{Player 1}")){//Is AI
						isPlayer=false;
					}
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					if(line.Contains("{xform"))line=""; //Clear Rotation line
					Unit+= "\n" + line;
				} //Finished Reading Unit now...
				
				//Writes the Unit to either the CPU or player force
				if(isPlayer){ 
					//Debug.Log("Adding Soldier to PlayerArmy...");
					DataManager.PlayerArmy.AddSoldier(new Human(Unit));
				}else{
					//Debug.Log("Adding Soldier to CPUArmy...");
					DataManager.CPUArmy.AddSoldier(new Human(Unit));
				}
				Unit="";
			}
			
			if(line.Contains("{Entity")){ //Start of an Entity Block
				string[] tokens = line.Split(' ');
				Unit += line;			
				while(!line.Equals("\t}")){ //Read to end of Block...
					line = MissionReader.ReadLine ();
					if(line.Contains("{Position"))line=""; //Clear Position line 
					if(line.Contains("{xform"))line=""; //Clear Rotation line 
					if(line.Contains("{Player 0}")){
						isPlayer=true;
					}else if(line.Contains("{Player 1}")){//Is AI
						isPlayer=false;
					}
					Unit+= "\n" + line;
				} //Finished Reading Unit now...
				if(isPlayer && VehicleList.Contains(tokens[2])){ 
					PlayerVehicleList += " " + tokens[2];
					DataManager.PlayerArmy.AddVehicle(new Vehicle(Unit));
				}else if(VehicleList.Contains(tokens[2])){
					CPUVehicleList += " " + tokens[2];
					DataManager.CPUArmy.AddVehicle(new Vehicle(Unit));
				}
				Unit="";
			}


			
			if(line.Contains("{Inventory")){//Start of an inventory block...
				Unit += line;
				string[] tokens = line.Split(' ');
				
				while(!line.Equals("\t}")){
					line = MissionReader.ReadLine ();
					Unit+=line+"\n";
				}
				//Not sure if there's a better way...
				DataManager.PlayerArmy.setInventory(tokens[1],Unit);
				DataManager.CPUArmy.setInventory(tokens[1],Unit);
				Unit="";
			}

			if(line.Contains("{Link")){
				while(line.Contains("{Link")){//Grabs all Links
					string[] tokens = line.Split(' ');
						if(PlayerVehicleList.Contains(tokens[2].Trim('{'))){
							DataManager.PlayerArmy.LinkCrewToVehicle(tokens[1],tokens[2].Trim('{'));
						}else if(CPUVehicleList.Contains(tokens[2].Trim('{'))){
							DataManager.CPUArmy.LinkCrewToVehicle(tokens[1],tokens[2].Trim('{'));
						}
					line = MissionReader.ReadLine ();
				}
			}


			line = MissionReader.ReadLine ();
		}
		line = SquadReader.ReadLine ();

		while(line != null){
			if (line.Contains ("{Squad")) {
				line = SquadReader.ReadLine ();
				while(!line.Equals("\t}")){
					if(line.Contains("{Actors")){
						SquadName = "Squad" + squad + ":" +Regex.Replace (line, "[^0-9 ]", ""); //Remove all non numeric chars (leave spaces)
						squad++;
						string[] tokens = SquadName.Split (':');
						DataManager.PlayerArmy.MakeGroupFromMIDS (tokens [0], tokens [1]);
					}
					line = SquadReader.ReadLine ();
					SquadName = "";
				}
			}
			line = SquadReader.ReadLine ();
		}

		MapWriter.Write (MapReader.ReadToEnd());
		/*line = MapReader.ReadLine ();
		while (line!=null) {
			if(line.Contains("{Clips"){
				while(!line.Equals("\t}")){
					line = "\n" + MapReader.ReadLine();
					MapWriter.Write(line);
				}
				foreach(Vehicle entitiy in Entities.Values){
					line = "\n" + entitiy.ToString();
				}
			}

					
			line = "\n" + MapReader.ReadLine();
			MapWriter.Write(line);	
		}*/



			PlayerWriter.Write (DataManager.PlayerArmy.ToString ());
			CPUWriter.Write (DataManager.CPUArmy.ToString ());

			MapReader.Close ();
			MapWriter.Close ();
			SquadReader.Close ();
			MissionReader.Close ();
			PlayerWriter.Close ();
			CPUWriter.Close ();
		
	}


	public string GetVehicleTags(string path){
		StreamReader TagReader = new StreamReader (path);
		string line = "",VehicleList="";
		while(line!=null){//Read ahead to find vehicle tags
			if(line.Contains("{Tags \"vehicle\"")){
				string[] tokens = line.Split(' ');
				VehicleList += " " + tokens[2].Trim('}');
			}
			line = TagReader.ReadLine();
		}
		TagReader.Close ();
		return VehicleList;
	}
}
