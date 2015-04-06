using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSwitcher : MonoBehaviour {

	public Text MapNameBox;
	public Image MapPreview;

	public static int CurrentMap = 0;
	public static string[] MapNames = new string[]{
		"dubovka",
		"frag_town",
		"frostbite",
		"port",
		"railway_station",
		"warehouse"
	};

	void Start(){
		RandMap ();
	}


	public void NextMap(){
		CurrentMap++;
		if (CurrentMap > MapNames.Length-1)CurrentMap = 0;
		SetMap(MapNames [CurrentMap]);
	}
	public void PrevMap(){
		CurrentMap--;
		if (CurrentMap < 0)CurrentMap = MapNames.Length;
		SetMap(MapNames [CurrentMap]);
	}
	public void RandMap(){
		CurrentMap = Random.Range(0,MapNames.Length);
		SetMap(MapNames [CurrentMap]);
	}
	public void SetMap(string MapName){
		MapNameBox.text = MapName;
		MapPreview.sprite =  Resources.Load<Sprite>("Assets/Maps/Map_Safe/" + MapName + "/map");
		Debug.Log ("Loading: " + "Assets/Maps/Map_Safe/" + MapName + "/map");
	}

}
