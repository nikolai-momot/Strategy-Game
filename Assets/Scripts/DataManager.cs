using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

//This will hold all data structures at runtime
public class DataManager : MonoBehaviour {

	public Text PlayerName,Pinfo,CPUName,Cinfo,SaveName,LoadName;

	public static ForceComp PlayerArmy;
	public static ForceComp CPUArmy;

	public static ForceComp PlayerArmy_postbattle;
	public static ForceComp CPUArmy_postbattle;

	public static MapData Map;

	public static string MissionOutPath="F:/SteamLibrary/steamapps/common/Men of War Assault Squad 2/mods/MissionGenerator/resource/map/single/DynamicMissions/DynamicMissionOutput/";

	public void DisplayArmyInfo(){
		if (PlayerArmy == null || CPUArmy == null) {
			Debug.Log("Armies not set!"); 
			return;
		}

		PlayerName.text = PlayerArmy.getName ();
		CPUName.text = CPUArmy.getName ();

		
	}

	public void RefreshPlayer(){
		if (PlayerArmy == null){
			PlayerName.text = "No Army Loaded"; 
			return;
		}
		PlayerName.text = PlayerArmy.getName ();
	}

	public void RefreshCPU(){
		if (CPUArmy == null){
			CPUName.text = "No Army Loaded"; 
			return;
		}
		CPUName.text = CPUArmy.getName ();
	}

	public void SavePlayerArmy(){
		if (SaveName.text == "" || PlayerArmy == null) {return;}
		Debug.Log ("Saving Army...");
		StreamWriter writer = new StreamWriter ("Assets/Armies/save/" + SaveName.text + "_army");
		writer.Write (PlayerArmy.ToString ());
		writer.Close ();
	}

	public void LoadPlayerArmy(){
		if (LoadName.text == "") {return;}
		string path = "Assets/Armies/save/" + LoadName.text + "_army";
	}
}
