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

    public static PathFinder pathFinder;
    public static NodeMapper map;

    public int xTiles = 25;
    public int yTiles = 25;

    public static Player[] players;
    private GameObject[] bases;
    private GameObject[] cities;
    private GameObject[] outposts;
    private GameObject[] towns;

    public static List<StratObj> Locations;

    private Object ArmyObj;



    public void Start() {
        bases = GameObject.FindGameObjectsWithTag("Base");
        cities = GameObject.FindGameObjectsWithTag("City");
        outposts = GameObject.FindGameObjectsWithTag("Outpost");
        towns = GameObject.FindGameObjectsWithTag("Town");        
        Locations = new List<StratObj>();

        ArmyObj = Resources.Load("Prefabs/ArmyObj"); //Load the Prefab for instantiating
        players = new Player[bases.Length]; //One Player per Base

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
        foreach (StratObj o in Locations){
            Debug.Log(o.ToString());
        }   

        foreach(Player p in players){
            Debug.Log(p.ToString());
            foreach (Army a in p.Armies) {
                Debug.Log(a.ToString());
            }
        }

       
        /*foreach (StratObj p in Locations) {
            Debug.DrawRay(Locations[0].getMapPosition(),p.getMapPosition() - Locations[0].getMapPosition(),Color.blue,60);
            Debug.Log("Distance from " + Locations[0].Name + " to " + p.getName() + ": " + (Locations[0].getMapPosition() - p.getMapPosition()).magnitude);
        }  */

        map = new NodeMapper(xTiles, yTiles, Locations);
        pathFinder = new PathFinder();
        List<Cell> fullPath = pathFinder.FindPath(map.Map[0, 0], map.Map[3, 7]);
        int n = 1;
        fullPath.ForEach(delegate(Cell cell) {
            Debug.Log("Cell #" + n + " is at ( " + cell.x + ", " + cell.y + ")");
        });

        //Everything is in place for the AI to take over from here.
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
        return (GameObject)Instantiate(ArmyObj, pos.transform.position - Vector3.forward, Quaternion.identity);
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
