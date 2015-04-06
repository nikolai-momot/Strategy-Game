using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class MapData{
	public class SpawnPoint{
		string name;
		int x,y; //Position
		int dir; //Z-axis rotation
		public SpawnPoint(string name,string x,string y, string dir){
			this.name = name;
			this.x=(int)float.Parse(x);
			this.y=(int)float.Parse(y);
			this.dir=(int)float.Parse(dir);
		}
		public string getName(){return name;}
		public int getX(){return x;}
		public int getY(){return y;}
		public int getDir(){return dir;}
		public override string ToString ()
		{
			return "Spawn point:" + name + "\nCoordinates: " + x + ", " + y + "\nRotation: " + dir;
		}
	}

	string name;
	Dictionary<string,SpawnPoint> SpawnPoints;

	public MapData(string name){
		this.name = name;
		SpawnPoints = new Dictionary<string, SpawnPoint> ();
	}

	public SpawnPoint getSpawnPoint(string Name){
		Debug.Log ("Looking up Spawn: " + Name);
		return this.SpawnPoints [Name];
	}

	public override string ToString ()
	{
		return "This map, " + this.name + " has " + SpawnPoints.Count + " spawn points.";
	}

	//Reads map data from the safe storage location to the mission folder
	public void LoadMapData(string MapName){


		string from = "Assets/Maps/Map_Safe/" + MapName;
		string to = DataManager.MissionOutPath;
		string[] files = Directory.GetFiles (from);

		Debug.Log ("Copying Map...");
		for(int i=0;i<files.Length;i++){
			//Debug.Log("Copying: " + files[i]);
			File.Copy(files[i],to+files[i].Substring(files[i].IndexOf('\\')),true); //to+files[i].Substring(files[i].IndexOf('\\')) Gets file name from path
		}
		if(File.Exists("Assets/Maps/PersistentMapData/"+MapName)){
			Debug.Log("Found saved data! loading map...");
			File.Copy("Assets/Maps/PersistentMapData/"+MapName,to+"map",true);
			/*StreamReader MapReader;
			StreamWriter MapWriter = new StreamWriter(DataManager.MissionOutPath + "map");
			MapReader = new StreamReader ("Assets/Maps/PersistentMapData/"+MapName);
			MapReader.Close ();
			MapWriter.Close ();*/
		}
	}

	public void LoadMapSpawns(){
		StreamReader MapReader = new StreamReader ("Assets/Maps/Map_Safe/" + this.name + "/spawn_points.mi");
		Dictionary<string,string> PointData = new Dictionary<string,string> ();
		string line = "";
		while (line!=null) {
			if(line.Contains("{Entity")){
				string[] tokens = line.Split(' ');
				//Debug.Log("Adding to dictionary: " + tokens[2]);
				PointData.Add(tokens[2],"");
				while(!line.Equals("\t}")){
					if(line.Contains("{Position")){
						string[] tok = line.Split(' ');
						//Debug.Log("Position found. Line: " + line);
						PointData[tokens[2]] += tokens[2] + "/ " +tok[1] + " " + tok[2].Trim('}');
					}else if(line.Contains("{xform")){
						string[] tok = line.Split(' ');
						PointData[tokens[2]] += " / "+  tok[2].Trim('}');
					}
					line = MapReader.ReadLine();
				}
			}
			if(line.Contains("north")||line.Contains("south")||line.Contains("east")||line.Contains("west")){ //keep_places_without_door is a random tag on most mission files...
				string[] tokens = line.Split(' ');
				PointData[tokens[2].Trim ('}')] += " / " + tokens[1];
			}
			line = MapReader.ReadLine();
		}
		MapReader.Close ();

		foreach (string Data in PointData.Values) {
			string[] tokens = Data.Split('/');
			string[] points = tokens[1].Split(' '); //Split up Coordinates
			if(tokens[2].Contains("\"")){//Then there is no rotation value, set it to 0
				SpawnPoints.Add(tokens[2].Substring(2).TrimEnd('"'),new SpawnPoint(tokens[2].Substring(2).TrimEnd('"'),points[1],points[2],"0"));
			}else{
				SpawnPoints.Add(tokens[3].Substring(2).TrimEnd('"'),new SpawnPoint(tokens[3].Substring(2).TrimEnd('"'),points[1],points[2],tokens[2]));
			}
		}
		/*foreach (SpawnPoint spawn in SpawnPoints.Values) {
			Debug.Log(spawn);
		}*/
	}
}
