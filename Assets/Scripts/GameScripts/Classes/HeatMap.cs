using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class HeatMap{

    private Cell[,] map;
    private int playerCount;
    private List<StratObj> objectives;
    private Player_AI[] players;
    private int xTiles, yTiles;
    private int heatRange;
    private GameObject[,] tileMap;
	private Renderer[,] rendMap;
    private GameObject prefab;
	private bool mapSet;

    public HeatMap(Cell[,] cellMap, List<StratObj> locations, Player_AI[] allPlayers, int x, int y) {
		this.mapSet = false;				//Map set mutex
        this.map = cellMap;					//Cell map
        this.objectives = locations;		//List of objectives
        this.players = allPlayers;			//Array of players
        this.playerCount = players.Length;	//Number of players
        this.xTiles = x;					//Tiles on the y range
        this.yTiles = y;					//Tiles on the x range

        this.prefab 	= Resources.Load<GameObject>("Prefabs/HeatTile");	//TilePrefab
		this.heatRange 	= Convert.ToInt32(Mathf.Sqrt((int)x)) + 1;			//How far the heat flows depending on the size of the map
		this.tileMap	= new GameObject[this.xTiles,this.yTiles];			//Map of all the tiles on the screen
		this.rendMap	= new Renderer[this.xTiles, this.yTiles];			//Map of the renderers for each of the tiles

		createTileMap ();	//Filling tileMap and rendMap
        setObjectiveHeat();	//Setting the heat for each objective on the map
        setArmyHeat();		//Setting the heat for each army on the map
		setMapColors(0);	//Setting the tile color based on it's heat

		this.mapSet = true;	//Map is now ready
		this.show (false);	//Hide the tiles
    }

	//Cleaning the map and reseting the heat values
    public void UpdateHeatMap(int id) {
		clearMap();			//Reseting heat values to 0
        setObjectiveHeat();	//Setting the heat for each objective on the map
        setArmyHeat();		//Setting the heat for each army on the map
        setMapColors(id);	//Setting the tile colors based on it's heat
    }

	//Show the heat map, or hide it
	public void show( bool showMap ){
		if(mapSet)
			foreach (GameObject tile in this.tileMap)
				tile.SetActive (showMap);
	}
	
	public void createTileMap (){
		//Getting background object
		GameObject background = GameObject.FindGameObjectWithTag ("Background");

		//Calculating height and width of tile
		float 	width	= background.transform.localScale.x / this.xTiles,		
				height	= background.transform.localScale.z / this.yTiles;

		//Loop through all of the cells
		for (int i = 0; i < this.xTiles; i++) {
			for (int j = 0; j < this.yTiles; j++) {
				//Get center value of the cell position
				Vector3 position = cornerToCenter( this.map[i,j].position, this.map[i,j].height, this.map[i,j].width );
				//Create tile using prefab
				this.tileMap[i, j]						= GameObject.Instantiate( prefab, position, prefab.transform.rotation ) as GameObject;
				this.tileMap[i,j].transform.localScale	= new Vector3( width, 0, height );				//Resixing the tile to cell size
				this.rendMap[i,j]						= this.tileMap[i, j].GetComponent<Renderer>();	//Getting tile renderer	
				this.rendMap[i,j].material				= new Material(Shader.Find("UI/Default"));		//Setting new material
			}
		}
	}

	//Get cell center value, cell position value is set to top-left corner
	public Vector3 cornerToCenter( Vector3 cornerPoint, float height, float width ){
		float 	newX = cornerPoint.x + ( width / 2 ),
				newY = cornerPoint.y - ( height / 2 );
		
		return new Vector3(newX, newY, 0f);
	}

	//Sett the heat value of every tile to 0
    public void clearMap() {
		foreach (Cell cell in this.map)
			for (int i = 0; i < this.players.Length; i++)
                cell.heat[i] = 0;
    }

	// Set the renderer color of each tile
    public void setMapColors(int id) {
		for (int k = 0; k < players.Length; k++) {
			if (id == k)
				continue;
			for (int i = 0; i < this.xTiles; i++) {
				for (int j = 0; j < this.yTiles; j++) {
					this.setTileColor (this.map [i, j], id, this.map [i, j].heat [id]);
					this.rendMap [i, j].material.color = (Color)this.map [i, j].color;
				}
			}
		}
    }

	//Determine the color of the tile based on it's heat and store it in the cell
    public void setTileColor(Cell cell, int id, int heat) {
		int greenVal= 255 - cell.heat[id],
			blueVal	= 0;

		//Making sure values don't go out of bounds
		if (greenVal < 0)
			greenVal = 0;
		else if (greenVal >= 255) {
			greenVal = 255;
			blueVal  = 255;
		}

		cell.color = new Color32((byte)255f, (byte)greenVal, (byte)blueVal, (byte)50f);	// R, G, B, A
    }

	//Set the heat value for each objective
    public void setObjectiveHeat() {

        foreach (StratObj objective in objectives) {

			//Proceed if the objective is occupied by someone
            if (objective.getOwnerID() == 0)
                continue;
            //Setting the heat range based on the type of objective
            switch (objective.DefenceLevel) {
                case 75:					//Base
					Debug.Log("Heating a base");
					distributeObjHeat(this.map[objective.x, objective.y], objective, this.heatRange);
                    break;
			case 50:					//City
				Debug.Log("Heating a city");
					distributeObjHeat(this.map[objective.x, objective.y], objective, this.heatRange - 1);
                    break;
			case 35:					//Outpost
				Debug.Log("Heating an outpost");
					distributeObjHeat(this.map[objective.x, objective.y], objective, this.heatRange - 2);
                    break;
			case 15:					//Town
				Debug.Log("Heating a town");
					distributeObjHeat(this.map[objective.x, objective.y], objective, this.heatRange - 3);
                    break;
            }

        }

		return;
    }

	//Distribute the heat around the objective based on it's Defence value
    public void distributeObjHeat(Cell cell, StratObj obj, int range) {
        int x = cell.x,					//Center x value
            y = cell.y,					//Center y value
            id = obj.getOwnerID() - 1,	//Owner ID
            defence = obj.DefenceLevel;	//Objective defence level

        float xDist, yDist;				//Distance from center


        for (int i = x - range; i <= x + range; i++) {
            xDist = Mathf.Abs(i - x);	//Absolute distance from the center x value

            for (int j = y - range; j <= y + range; j++) {
                //Checking if the coordinates are on the grid
				if ((i > 0) && (j > 0) && (i < this.xTiles - 1) && (j < this.yTiles - 1)) {

                    yDist = Mathf.Abs(j - y);	//Absolute distance from the center y value

                    if (x == i && y == j) {		//Center Value
						this.map[i, j].heat[id] += defence;
                        continue;
                    }

                    int displacement = Convert.ToInt32(Mathf.Abs(-(yDist) - (xDist))),	//Distance from the objective coordinates
                        heatBase = (2 ^ (displacement));								//Used in calculating dispersed heat

					//Add heat to the cell's current value for the respected player
					this.map[i, j].heat[id] += Convert.ToInt32( (defence / displacement) *2);
                }

            }
        }

    }

	//Set the heat value for each army on the map
    public void setArmyHeat() {
		foreach (Player player in this.players) {
			foreach (Army army in player.Armies) {
				foreach (Cell cell in this.map) {
                    //Skip the cell if it doesn't have the army inside of it
                    if (!cell.containsVector(army.ArmyObject.transform.position))
                        continue;

					int x = cell.x,					//Army x coordinate
						y = cell.y,					//Army y coordinate
						id = army.getOwnerID() - 1,	//Army owner ID
                        force = army.getStrength();	//Army strength value

                    distributeArmyHeat(x, y, id, force, heatRange / 2);	//Setting the heat
                    break;												//Cell location found and heat set, break loop
                }
            }
        }

		return;
    }

	//Distribute the heat around the amry based on it's strength value
    public void distributeArmyHeat(int x, int y, int id, int force, int range) {
        float xDist, yDist;							//Distance from center

        for (int i = x - range; i <= x + range; i++) {
            xDist = Mathf.Abs(i - x);				//Absolute distance from the center x value

            for (int j = y - range; j <= y + range; j++) {

                //Checking if the coordinatee is on the grid
				if ((i > 0) && (j > 0) && (i < this.xTiles - 1) && (j < this.yTiles - 1)) {

                    yDist = Mathf.Abs(j - y);		//Absolute distance from the center y value

                    if (x == i && y == j) {			//Center Value
						this.map[i, j].heat[id] += force;
                        continue;
                    }

					int displacement = Convert.ToInt32(Mathf.Abs(-(yDist) - (xDist))),	//Distance from the objective coordinates
						heatBase = (2 ^ (displacement));								//Used in calculating dispersed heat

					//Add heat to the cell's current value for the respected player
					this.map[i, j].heat[id] += Convert.ToInt32(force / displacement);	 
                }

            }
        }
    }
}

