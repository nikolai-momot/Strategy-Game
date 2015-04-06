using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Navigator : MonoBehaviour {

	public GameObject optionsPanel;
	public GameObject LoadGamePanel;

	void Start(){
		optionsPanel.SetActive (false);
		LoadGamePanel.SetActive (false);
	}


	public void Start_Pressed(){
		Debug.Log ("Starting Game");
	}
	public void Single_Battles(){
		Application.LoadLevel ("MissionGenerator");
	}
	public void Load_Open(){
		LoadGamePanel.SetActive (true);
	}
	public void Load_Game(){
		Debug.Log ("Loading Game");			
	}
	public void Load_Close(){
		LoadGamePanel.SetActive (false);
	}

	public void Options_Open(){
		optionsPanel.SetActive (true);
	}
	public void Options_Close(){
		optionsPanel.SetActive (false);				
	}
	public void Options_Apply(){
		Debug.Log ("Applying Options");
		Options_Close ();
	}
	public void Exit_Pressed(){
		Debug.Log ("Exit the Game...");
	}

}
