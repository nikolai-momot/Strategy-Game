using UnityEngine;
using System.Collections;

/* Should have the same function as game manager, 
 * but won't have visual aspects. Game plays through text
 */
public class VirtualGameManager : MonoBehaviour {
		
		public GameObject[] StrategicObjectives;

        private Player_AI RedPlayer;
        private Player_AI BluePlayer;
		
	void Start () {
		Base RedBase = new Base("Red HQ",StrategicObjectives[0],1);
		Base BlueBase = new Base("Blue HQ",StrategicObjectives[1],2);
		City City1 = new City("The City",StrategicObjectives[2],0);
        Outpost Outpost1 = new Outpost("FOB Red", StrategicObjectives[3], 0);
        Outpost Outpost2 = new Outpost("FOB Blue", StrategicObjectives[4], 0);
        Town Town1 = new Town("Top Left Town", StrategicObjectives[5], 0);
        Town Town2 = new Town("Bottom Left Town", StrategicObjectives[6], 0);
        Town Town3 = new Town("Top Right Town", StrategicObjectives[7], 0);
        Town Town4 = new Town("Bottom Rght Town", StrategicObjectives[8], 0);		
		
		//Make Connections, this won't be manual in the real game
		RedBase.AddConnectedPoint(Outpost1);
		RedBase.AddConnectedPoint(Town1);
		RedBase.AddConnectedPoint(Town2);
		
		BlueBase.AddConnectedPoint(Outpost2);
		BlueBase.AddConnectedPoint(Town3);
		BlueBase.AddConnectedPoint(Town4);
		
		Town1.AddConnectedPoint(RedBase);		
		Town1.AddConnectedPoint(Outpost1);
		Town1.AddConnectedPoint(City1);
		Town1.AddConnectedPoint(Town3);
		
		Town2.AddConnectedPoint(RedBase);
		Town2.AddConnectedPoint(Outpost1);
		Town2.AddConnectedPoint(City1);
		Town2.AddConnectedPoint(Town4);
		
		Town3.AddConnectedPoint(BlueBase);
		Town3.AddConnectedPoint(City1);
		Town3.AddConnectedPoint(Outpost2);
		Town3.AddConnectedPoint(Town1);
		
		Town4.AddConnectedPoint(BlueBase);
		Town4.AddConnectedPoint(City1);
		Town4.AddConnectedPoint(Outpost2);
		Town4.AddConnectedPoint(Town2);
		
		Outpost1.AddConnectedPoint(Town1);
		Outpost1.AddConnectedPoint(Town2);
		Outpost1.AddConnectedPoint(City1);
		Outpost1.AddConnectedPoint(RedBase);
		
		Outpost2.AddConnectedPoint(Town3);
		Outpost2.AddConnectedPoint(Town4);
		Outpost2.AddConnectedPoint(City1);
		Outpost2.AddConnectedPoint(BlueBase);
		
		City1.AddConnectedPoint(Outpost1);
		City1.AddConnectedPoint(Outpost2);
		City1.AddConnectedPoint(Town1);
		City1.AddConnectedPoint(Town2);
		City1.AddConnectedPoint(Town3);
		City1.AddConnectedPoint(Town4);	
		
		RedBase.DrawConnectionLines();
		BlueBase.DrawConnectionLines();
		Outpost1.DrawConnectionLines();
		Outpost2.DrawConnectionLines();
		City1.DrawConnectionLines();
		Town1.DrawConnectionLines();
		Town2.DrawConnectionLines();
		Town3.DrawConnectionLines();
		Town4.DrawConnectionLines();
		
		Debug.Log(RedBase.ToString());
		Debug.Log(BlueBase.ToString());
		Debug.Log(Outpost1.ToString());
		Debug.Log(Outpost2.ToString());
		Debug.Log(Town1.ToString());
		Debug.Log(Town2.ToString());
		Debug.Log(Town3.ToString());
		Debug.Log(Town4.ToString());
		Debug.Log(City1.ToString());
		
		RedPlayer = new Player_AI(1,"Red Player","THE REDS",RedBase);
			RedPlayer.CreateNewArmy_GenerateName();
			RedPlayer.CreateNewArmy_GenerateName();
		
		BluePlayer = new Player_AI(2,"Blue Player","THE BLUES",BlueBase);
			BluePlayer.CreateNewArmy_GenerateName();
			BluePlayer.CreateNewArmy_GenerateName();		
		
		Debug.Log(RedPlayer.ToString());
		Debug.Log(BluePlayer.ToString());
		
		foreach(Army a in RedPlayer.Armies){
			Debug.Log(a.ToString());
		}
		foreach(Army a in BluePlayer.Armies){
			Debug.Log(a.ToString());
		}
		
		//Everything is in place for the AI to take over from here.
        
	}

    
	
	// Update is called once per frame
	void Update () {
        if(!RedPlayer.Victorious && !BluePlayer.Victorious) {
            RedPlayer.TakeTurn();
            BluePlayer.TakeTurn();
        }
	}
}
