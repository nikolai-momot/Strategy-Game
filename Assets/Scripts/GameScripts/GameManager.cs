using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Will manage turn sequence, and distribute resources
 * Basically tells each player when to take their turn.
 * */
public class GameManager : MonoBehaviour {

    //public List<Cell> Obstacles;
    public bool GenerateNamesForBases = false;
    public bool GenerateNamesForTowns = false;
    public bool GenerateNamesForCities = false;
    public bool GenerateNamesForOutPosts = false;
    public float ArmyMoveSpeed = 1.0f;

    public static PathFinder pathFinder;
    public static NodeMapper map;
	public static HeatMap heatmap;
    public static float MoveSpeed = 10.0f;

    public int xTiles = 25;
    public int yTiles = 25;

    public static Player_AI[] players;
    private GameObject[] bases;
    private GameObject[] cities;
    private GameObject[] outposts;
    private GameObject[] towns;
    
    private int turnSeq = 0;

    public static List<StratObj> Locations;

    private Object ArmyObj;



    public void Start() {
        bases = GameObject.FindGameObjectsWithTag("Base");
        cities = GameObject.FindGameObjectsWithTag("City");
        outposts = GameObject.FindGameObjectsWithTag("Outpost");
        towns = GameObject.FindGameObjectsWithTag("Town");        
        Locations = new List<StratObj>();

        ArmyObj = Resources.Load("Prefabs/ArmyObj"); //Load the Prefab for instantiating
        players = new Player_AI[bases.Length]; //One Player per Base

        for (int i = 0; i < bases.Length; i++) { //Create Bases and Players
            bases[i].GetComponentInChildren<TextMesh>().text = "Player " + (i + 1) + "'s Base";
            Locations.Add(new Base("Player " +(i+1)+ "'s Base", bases[i],(i+1)));
            players[i] = new Player_AI(i+1,"Player " + (i+1), "Country" ,(Base)Locations[i]);
            players[i].CreateNewArmy_GenerateName(InstantiateArmyObjectAt(players[i].HQ.getMapObject()));
            players[i].CreateNewArmy_GenerateName(InstantiateArmyObjectAt(players[i].HQ.getMapObject()));
        }
        //Create the rest of the objectives, gets the Names from the TextMesh components
        if (!GenerateNamesForCities) {
            for (int i = 0; i < cities.Length; i++) {
                Locations.Add(new City(cities[i].GetComponentInChildren<TextMesh>().text, cities[i], 0));
            }
        } else {
            for (int i = 0; i < cities.Length; i++) {
                Locations.Add(new City("City " + Alphabet[i], cities[i], 0));
                cities[i].GetComponentInChildren<TextMesh>().text = "City " + Alphabet[i];
            }
        }
        if (!GenerateNamesForOutPosts) {
            for (int i = 0; i < outposts.Length; i++) {
                Locations.Add(new Outpost(outposts[i].GetComponentInChildren<TextMesh>().text, outposts[i], 0));
            }
        } else {
            for (int i = 0; i < outposts.Length; i++) {
                Locations.Add(new Outpost("FOB " + Alphabet[i], outposts[i], 0));
                outposts[i].GetComponentInChildren<TextMesh>().text = "FOB " + Alphabet[i];
            }
        }
        if (!GenerateNamesForTowns) {
            for (int i = 0; i < towns.Length; i++) {
                Locations.Add(new Town(towns[i].GetComponentInChildren<TextMesh>().text, towns[i], 0));
            }
        } else {
            for (int i = 0; i < towns.Length; i++) {
                Locations.Add(new Town("Town " + Alphabet[i], towns[i], 0));
                towns[i].GetComponentInChildren<TextMesh>().text = "Town " + Alphabet[i];
            }
        }
        
               
        
       //Write everything to the log to check!
       /* foreach (StratObj o in Locations){
            Debug.Log(o.ToString());
        }   

        foreach(Player p in players){
            Debug.Log(p.ToString());
            foreach (Army a in p.Armies) {
                Debug.Log(a.ToString());
            }
        }*/

        
       
        /*foreach (StratObj p in Locations) {
            Debug.DrawRay(Locations[0].getMapPosition(),p.getMapPosition() - Locations[0].getMapPosition(),Color.blue,60);
            Debug.Log("Distance from " + Locations[0].Name + " to " + p.getName() + ": " + (Locations[0].getMapPosition() - p.getMapPosition()).magnitude);
        }  */

        map = new NodeMapper(xTiles, yTiles, Locations, players.Length);
        pathFinder = new PathFinder(map.Map);
		heatmap = new HeatMap ( map.Map, Locations, players, xTiles, yTiles );	//<<<<<<<<<Heat Map Init

       /* List<Cell> fullPath = pathFinder.FindPath(map.Map[0, 0], map.Map[3, 7]); //Test path
        int n = 1;
        fullPath.ForEach(delegate(Cell cell) {
            Debug.Log("Cell #" + n + " is at ( " + cell.x + ", " + cell.y + ")");
        });*/

        //players[0].Armies[0].MoveTo(new Vector3(0, 0, 0));

        //Everything is in place for the AI to take over from here.
        players[0].TakeTurn();
        UpdateObjNames(); //Updates Names to Include Ownership ID
    }

    public static List<Army> GetAllEnemyArmies(int ID) {
        List<Army> enemyArmies = new List<Army>();
        for (int i = 0; i < players.Length; i++) {
            if (players[i].ID != ID) {
                foreach (Army e in players[i].Armies) {
                    enemyArmies.Add(e);
                }
            }
        }
            return enemyArmies;
    }

    public GameObject InstantiateArmyObjectAt(GameObject pos) {
        return (GameObject)Instantiate(ArmyObj, pos.transform.position, Quaternion.identity);
    }

    public void NextTurn() {
        turnSeq++;
        if (turnSeq >= players.Length)turnSeq = 0;
    }

    public void Update() {
        MoveArmiesOnMap();//Checks Armies to see if they have movement paths Queued up
        ResolvePendingBattles();

        if (players[turnSeq].finishedTurn && !players[turnSeq].isBusy()) {
            Debug.Log("GameManager: Next turn!");
            UpdateObjNames();
            players[turnSeq].finishedTurn = false;
			heatmap.UpdateHeatMap();				//<<<<<<<<<Heat Map Update
            NextTurn();
            players[turnSeq].TakeTurn();
        } else {
            
        }
    }

    public void UpdateObjNames() {
        foreach (StratObj obj in Locations) {
            obj.gObj.GetComponentInChildren<TextMesh>().text = obj.getName();
        }
        foreach (Player_AI p in players) {
            foreach (Army a in p.Armies) {
                a.UpdateNumbers();
            }
        }
    }

    public void MoveArmiesOnMap() {
        if (players[turnSeq].ArmiesWaitingToMove.Count > 0) {
            Army a = players[turnSeq].ArmiesWaitingToMove.Peek();
            if (a.MovingPath.Count > 0) {
                a.ArmyObject.transform.Translate((a.MovingPath.Peek() - a.ArmyObject.transform.position).normalized * Time.deltaTime * ArmyMoveSpeed);
                if ((a.MovingPath.Peek() - a.ArmyObject.transform.position).magnitude < 0.1) {
                    a.MovingPath.Dequeue();
                }
            } else {
                players[turnSeq].ArmiesWaitingToMove.Dequeue();
            }
        }
    }
    public void ResolvePendingBattles() {
        if (players[turnSeq].BattlesWaitingToResolve.Count > 0 && players[turnSeq].ArmiesWaitingToMove.Count == 0) {
            Battle b = players[turnSeq].BattlesWaitingToResolve.Dequeue();
            b.AutoResolve();
        }
    }
    
    private string[] Alphabet = new string[] { 
        "Alpha",
        "Bravo",
        "Charlie",
        "Delta",
        "Echo",
        "Foxtrot",
        "Golf",
        "Hotel",
        "India",
        "Juliett",
        "Kilo",
        "Lima",
        "Mike",
        "November",
        "Oscar",
        "Papa",
        "Quebec",
        "Romeo",
        "Sierra",
        "Tango",
        "Uniform",
        "Victor",
        "Whiskey",
        "Xray",
        "Yankee",
        "Zulu",
    };

}
