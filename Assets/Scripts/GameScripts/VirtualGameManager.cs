using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Should have the same function as game manager, 
 * but won't have visual aspects. Game plays through text
 */
public class VirtualGameManager : MonoBehaviour {
		
	public GameObject[] StrategicObjectives;
	public List<StratObj> Locations;
	//public List<StratNode> Obstacles;
	public int xTiles = 10;
	public int yTiles = 10;

		
	public void Start () {
		Locations = new List<StratObj> ();
		//Obstacles = new List<StratObj> ();

		Base RedBase = new Base("Red HQ",StrategicObjectives[0]);
		Locations.Add ( RedBase );

		Base BlueBase = new Base("Blue HQ",StrategicObjectives[1]);
		Locations.Add ( BlueBase );

		City City1 = new City("The City",StrategicObjectives[2]);
		Locations.Add ( City1 );

		Outpost Outpost1 = new Outpost("FOB Red",StrategicObjectives[3]);
		Locations.Add ( Outpost1 );

		Outpost Outpost2 = new Outpost("FOB Blue",StrategicObjectives[4]);
		Locations.Add ( Outpost2 );

		Town Town1 = new Town("Top Left Town",StrategicObjectives[5]);
		Locations.Add ( Town1 );

		Town Town2 = new Town("Bottom Left Town",StrategicObjectives[6]);
		Locations.Add ( Town2 );

		Town Town3 = new Town("Top Right Town",StrategicObjectives[7]);
		Locations.Add ( Town3 );

		Town Town4 = new Town("Bottom Rght Town",StrategicObjectives[8]);		
		Locations.Add ( Town4 );

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
		
		Player_AI RedPlayer = new Player_AI(0,"Red Player","THE REDS",RedBase);
			RedPlayer.CreateNewArmy_GenerateName();
			RedPlayer.CreateNewArmy_GenerateName();
		
		Player_AI BluePlayer = new Player_AI(1,"Blue Player","THE BLUES",BlueBase);
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
		NodeMapper map = new NodeMapper ( xTiles, yTiles, Locations );
		PathFinder pathFinder = new PathFinder ();
		List<StratNode> fullPath = pathFinder.FindPath (map.Map [0, 0], map.Map [xTiles - 1, yTiles - 1]);
		int i = 1;
		fullPath.ForEach (delegate( StratNode cell ) {
			Debug.Log("Cell #"+i+" is at ( "+cell.x+", "+cell.y+")");
		});
	}
	
	// Update is called once per frame
	public void Update () {  }

	/*public void ConnectEdges(){

		Locations.ForEach (delegate( StratObj location ) {
			foreach( StratObj point in Locations ){
				if( location.ConnectedPoints.Contains(point.gObj) )
					location.SetEdge(point);
			}
		});
	}*/

}
