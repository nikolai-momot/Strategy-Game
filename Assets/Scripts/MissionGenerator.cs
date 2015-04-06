using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

enum Direction {North,East,South,West};

public class MissionGenerator : MonoBehaviour{



	public static string saveHead = "{mission\n";

	public static string saveTail ="\n\t{Helpers"+
		"\n\t\t{reinforcements"+
			"\n\t\t\t{scoreTotal 0}"+
			"\n\t\t\t{scoreCurrent 0}"+
		"\n\t\t}"+
		"\n\t\t{triggers"+
		"{Players"+
		"{user 0}"+
		"}"+
	"\n\t\t}"+
	"\n\t}"+
	"\n}";

	public void ExitToMainMenu(){
		Application.LoadLevel ("MainMenu");
	}

	public void writeMission(){//This will write the entities saved in playerforce to a new mission file...
		if (DataManager.PlayerArmy == null || DataManager.CPUArmy == null) {
			Debug.Log("Armies not set!");
			return;
		}

		DataManager.Map = new MapData (MapSwitcher.MapNames[MapSwitcher.CurrentMap]);
		DataManager.Map.LoadMapData (MapSwitcher.MapNames[MapSwitcher.CurrentMap]);
		DataManager.Map.LoadMapSpawns ();
		
		StreamReader theReader = new StreamReader ("Assets/MissionTypes/BasicMission");
		StreamWriter theWriter = new StreamWriter (DataManager.MissionOutPath + "/0.mi");
		//Debug.Log ("Writing Basic Mission...");
		theWriter.WriteLine (saveHead);

		
		theWriter.WriteLine(DataManager.CPUArmy.DeployAt(DataManager.Map.getSpawnPoint("north")));
		theWriter.WriteLine(DataManager.PlayerArmy.DeployAt(DataManager.Map.getSpawnPoint("south")));
		
		saveTail = theReader.ReadToEnd ();
		theWriter.WriteLine(saveTail);
		theReader.Close ();
		theWriter.Close ();
	}


}
