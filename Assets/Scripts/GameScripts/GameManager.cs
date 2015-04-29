using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Will manage turn sequence, and distribute resources
 * Basically tells each player when to take their turn.
 * */
public class GameManager : MonoBehaviour {

    public bool VerboseAI = false;
    public bool VerboseGameManager = false;
    public bool DebugDisplay = false;
    //public List<Cell> Obstacles;
    public bool GenerateNamesForBases = false;
    public bool GenerateNamesForTowns = false;
    public bool GenerateNamesForCities = false;
    public bool GenerateNamesForOutPosts = false;
    public int StartingArmies = 2;
    public float ArmyMoveSpeed = 1.0f;

    public static PathFinder pathFinder;
    public static NodeMapper map;
    public static HeatMap heatmap;
    public static float MoveSpeed = 10.0f;

    public int xTiles = 50;
    public int yTiles = 50;

    public static Player_AI[] players;
    private GameObject[] bases;
    private GameObject[] cities;
    private GameObject[] outposts;
    private GameObject[] towns;

    private static bool heatMapVisible;
    private int turnSeq = 0;
    private int TurnCount = 1;

    public static List<StratObj> Locations;
    public static int InfantryCost = 3;
    public static int VehicleCost = 10;

    private Object ArmyObj;
    public static Object TargetInd,AddVictoryInd,AddLossInd,AddUnitsInd,LoseUnitsInd,RetreatInd;

    private string[] Countries = new string[] { "eng","ger","usa","jap","rus" };

    void OnGUI() {
        if(DebugDisplay)GUI.Label(new Rect(10, 10, 1000, 1000), "Turn Counter: " + TurnCount+
                                                              "\nTimeScale: " + Time.timeScale+
                                                              "\nTurn: " + players[turnSeq].Name+
                                                              "\nheatMapVisible: " + heatMapVisible);
    }

    public void Start() {
        bases = GameObject.FindGameObjectsWithTag("Base");
        cities = GameObject.FindGameObjectsWithTag("City");
        outposts = GameObject.FindGameObjectsWithTag("Outpost");
        towns = GameObject.FindGameObjectsWithTag("Town");        
        Locations = new List<StratObj>();

        ArmyObj = Resources.Load("Prefabs/ArmyObj"); //Load the Prefab for instantiating
        TargetInd = Resources.Load("Prefabs/TargetInd");
        AddVictoryInd = Resources.Load("Prefabs/Victory");
        AddLossInd = Resources.Load("Prefabs/Defeat"); 
        AddUnitsInd = Resources.Load("Prefabs/AddUnit"); 
        LoseUnitsInd = Resources.Load("Prefabs/LoseUnit");
        RetreatInd = Resources.Load("Prefabs/Retreat");

        players = new Player_AI[bases.Length]; //One Player per Base

        for (int i = 0; i < bases.Length; i++) { //Create Bases and Players
            bases[i].GetComponentInChildren<TextMesh>().text = "Player " + (i + 1) + "'s Base";
            Locations.Add(new Base("Player " + (i + 1) + "'s Base", bases[i], players[i]));
            players[i] = new Player_AI(i + 1, "Player " + (i + 1), Countries[i], (Base)Locations[i], VerboseAI);
            Locations[i].setOwner(players[i]);
            Locations[i].AddSoldiersToGarrison(100);
            Locations[i].AddVehiclesToGarrison(10);
            for (int j = 0; j < StartingArmies;j++)
                players[i].CreateNewArmy_GenerateName(InstantiateArmyObjectAt(players[i].HQ.getMapObject()));
        }
        //Create the rest of the objectives, gets the Names from the TextMesh components
        if (!GenerateNamesForCities) {
            for (int i = 0; i < cities.Length; i++) {
                Locations.Add(new City(cities[i].GetComponentInChildren<TextMesh>().text, cities[i], null));
            }
        } else {
            for (int i = 0; i < cities.Length; i++) {
                Locations.Add(new City("City " + Alphabet[i], cities[i], null));
                cities[i].GetComponentInChildren<TextMesh>().text = "City " + Alphabet[i];
            }
        }
        if (!GenerateNamesForOutPosts) {
            for (int i = 0; i < outposts.Length; i++) {
                Locations.Add(new Outpost(outposts[i].GetComponentInChildren<TextMesh>().text, outposts[i], null));
            }
        } else {
            for (int i = 0; i < outposts.Length; i++) {
                Locations.Add(new Outpost("FOB " + Alphabet[i], outposts[i], null));
                outposts[i].GetComponentInChildren<TextMesh>().text = "FOB " + Alphabet[i];
            }
        }
        if (!GenerateNamesForTowns) {
            for (int i = 0; i < towns.Length; i++) {
                Locations.Add(new Town(towns[i].GetComponentInChildren<TextMesh>().text, towns[i], null));
            }
        } else {
            for (int i = 0; i < towns.Length; i++) {
                Locations.Add(new Town("Town " + Alphabet[i], towns[i], null));
                towns[i].GetComponentInChildren<TextMesh>().text = "Town " + Alphabet[i];
            }
        }        
        
        map = new NodeMapper(xTiles, yTiles, Locations, players.Length);
        pathFinder = new PathFinder(map.Map);
        heatmap = new HeatMap(map.Map, Locations, players, xTiles, yTiles);

        heatMapVisible = false;

        //Everything is in place for the AI to take over from here.
        players[0].TakeTurn();
        UpdateObjNames(); //Updates Names to Include Ownership ID
        UpdateObjInfo();
    }

    public void showMap() {
        heatMapVisible = !heatMapVisible;
        heatmap.show(heatMapVisible);
    }

    public static List<Army> GetAllEnemyArmies(int ID) {
        List<Army> enemyArmies = new List<Army>();
        for (int i = 0; i < players.Length; i++) {
            if (players[i].ID != ID) {
                foreach (Army e in players[i].Armies) {
                    if(e.ArmyObject != null)
                     enemyArmies.Add(e);
                }
            }
        }
            return enemyArmies;
    }

    /*Iinstatiation functions, Visual indicators*/
    public GameObject InstantiateArmyObjectAt(GameObject pos) {
        return (GameObject)Instantiate(ArmyObj, pos.transform.position, Quaternion.identity);
    }
    public static void InstantiateTargetIndAt(Vector3 pos){
        Instantiate(TargetInd, pos - Vector3.forward, Quaternion.identity);
    }
    public static void InstantiateVictoryAt(Vector3 pos) {
        Instantiate(AddVictoryInd, pos - Vector3.forward, Quaternion.identity);
    }
    public static void InstantiateDefeatAt(Vector3 pos) {
        Instantiate(AddLossInd, pos - Vector3.forward, Quaternion.identity);
    }
    public static void InstantiateAddUnitAt(Vector3 pos) {
        Instantiate(AddUnitsInd, pos - Vector3.forward, Quaternion.identity);
    }
    public static void InstantiateLoseUnitAt(Vector3 pos) {
        Instantiate(LoseUnitsInd, pos - Vector3.forward, Quaternion.identity);
    }
    public static void InstantiateRetreatIndAt(Vector3 pos) {
        Instantiate(RetreatInd, pos - Vector3.forward, Quaternion.identity);
    }


    public void NextTurn() {
        turnSeq++;
        if (turnSeq >= players.Length) {
            turnSeq = 0;
            TurnCount++;
            if (VerboseGameManager) Debug.Log("Turn: " + TurnCount);
        }

        if (VerboseGameManager) {
            string str = "";
            foreach (Player p in players) {
                str += "\n==== " + p.Name + " ====\n ==== Objs:";
                foreach (StratObj o in p.Objectives) {
                    str += "\n- " + o.ToString();
                }
                str += "\n====================\n ==== Armies:";
                foreach (Army a in p.Armies) {
                    str += "\n- " + a.ToString();
                }
            }
            str += "\n====================\n";
            Debug.Log(str);
        }
    }

    public void Update() {
        

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if(Time.timeScale <= 9.5f)
                Time.timeScale += 0.5f;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if(Time.timeScale >= 0.5f)
                Time.timeScale -= 0.5f;
        }  else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (Time.timeScale >0.1f)
                Time.timeScale -= 0.1f;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (Time.timeScale <=9.9f)
                Time.timeScale += 0.1f;
        } else if (Input.GetKeyDown(KeyCode.H)) {
            this.showMap();
        }

        if (players[turnSeq].finishedTurn && !players[turnSeq].isBusy()) {
            Debug.Log("GameManager: Next turn!");            
            //CleanUpArmies(); //Not implemented yet, Armies will just teleport back to base when detroyed
            CleanUpTargets();
            UpdateObjInfo();
            UpdateObjNames();
            players[turnSeq].finishedTurn = false;

            NextTurn();

            if (heatMapVisible)
                heatmap.UpdateHeatMap(turnSeq);

            players[turnSeq].TakeTurn();
        } else {

            if (heatMapVisible)
                heatmap.UpdateHeatMap(turnSeq);

            MoveArmiesOnMap();//Checks Armies to see if they have movement paths Queued up
            MoveArmiesOnMapToEnter();
            ResolvePendingBattles();
        }
    }

    public static void UpdateObjNames() {
        foreach (StratObj obj in Locations) {
            obj.gObj.GetComponentInChildren<TextMesh>().text = obj.getName();
        }
        foreach (Player_AI p in players) {
            foreach (Army a in p.Armies) {
                a.UpdateNumbers();
            }
        }
    }
    public static void UpdateObjInfo() {
        foreach (StratObj obj in Locations) {
            obj.UpdateInfo();
            if (obj.getOwnerID() != 0) {
                players[obj.getOwnerID()-1].addObjective(obj);                
            }
            foreach (Player p in players) {
                if (p.ID != obj.getOwnerID() && p.Objectives.Contains(obj)) {
                    p.RemoveObjective(obj);
                }
            }
        }
    }
    public void CleanUpArmies() {
        foreach (Player p in players) {
            if (p.Armies.Count != 0) {
                for (int i = 0; i < p.Armies.Count; i++) {
                    if (p.Armies[i].isDefeated()) {
                        p.Armies.Remove(p.Armies[i]);
                        GameObject.Destroy(p.Armies[i].ArmyObject);
                    }
                }
            }
        }
    }
    public void CleanUpTargets() {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("target")) {
            Destroy(o);
        }
    }

    public void MoveArmiesOnMap() {
        if (players[turnSeq].ArmiesWaitingToMove.Count > 0) {
            Army a = players[turnSeq].ArmiesWaitingToMove.Peek();
            if (a.MovingPath.Count > 0 ) {
                if (a.ArmyObject == null) players[turnSeq].ArmiesWaitingToMove.Dequeue();
                a.ArmyObject.transform.Translate((a.MovingPath.Peek() - a.ArmyObject.transform.position).normalized * Time.deltaTime * ArmyMoveSpeed);
                if ((a.MovingPath.Peek() - a.ArmyObject.transform.position).magnitude < 0.1) {
                    a.MovingPath.Dequeue();
                }
            } else {
                players[turnSeq].ArmiesWaitingToMove.Dequeue();
            }
        }
    }
    public void MoveArmiesOnMapToEnter() {
        if (players[turnSeq].ArmiesWaitingToEnter.Count > 0) {
            Army a = players[turnSeq].ArmiesWaitingToEnter.Peek();
            if (a.MovingPath.Count > 0) {                
                a.ArmyObject.transform.Translate((a.MovingPath.Peek() - a.ArmyObject.transform.position).normalized * Time.deltaTime * ArmyMoveSpeed);
                if ((a.MovingPath.Peek() - a.ArmyObject.transform.position).magnitude < 0.1) {
                    a.MovingPath.Dequeue();
                }
            } else {
                a.Enter(FindStratObjFromGameObj(a.CurrentTarget));
                players[turnSeq].ArmiesWaitingToEnter.Dequeue();
            }
        }
    }
    public void ResolvePendingBattles() {
        if (players[turnSeq].BattlesWaitingToResolve.Count > 0 && players[turnSeq].ArmiesWaitingToMove.Count == 0) {
            Battle b = players[turnSeq].BattlesWaitingToResolve.Dequeue();
            b.AutoResolve();
            b = null;
        }
    }

    public static Vector3 GetRetreatLocation(Army a) {
        return map.lowestHeat(pathFinder.cellFromVector(a.getMapPosition()), a.getOwnerID(), players.Length).position;
    }

    public StratObj FindStratObjFromGameObj(GameObject gobj) {
        foreach (StratObj o in Locations) {
            if (o.gObj == gobj) {
                return o;
            }
        }
        return null;
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
