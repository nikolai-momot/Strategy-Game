using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeMapper
{
	public int xTiles, yTiles;			// # of Horizontal and Vertical tiles, respectively
	public List<StratObj> locations;	// A list of all Strategic Objectives on the map
	public List<GameObject> obstacles;	// A list of all obstacles on the map 
	public Cell[,] Map;					// A map of all cells on the game surface

	public NodeMapper ( int x, int y, List<StratObj>  objLocations, List<GameObject> obsLocations ){
		xTiles = x;					//Setting number of Horizontal Tiles
		yTiles = y;					//Setting number of Vertical Tiles
		locations = objLocations;	//Setting list Strat Obj Locations
		obstacles = obsLocations;	//Setting list of obstacle lcoations

		GameObject background = GameObject.Find ("Background");	//Getting game surface object

		MeshRenderer backgroundRenderer = background.GetComponent<MeshRenderer> ();//Getting renderer component of game surface

		float	tileWidth = ( backgroundRenderer.bounds.size.x ) / xTiles,		
				tileHeight = ( backgroundRenderer.bounds.size.y ) / yTiles;

		//Drawing lines in Scene view to outline cell boundaries
		DrawGridLines (tileHeight, tileWidth, backgroundRenderer);

		//Fillsing each grid of the map with its respected cell
		FillMap (tileHeight, tileWidth, backgroundRenderer);

		//Setting the neighbor nodes for each cell in the map
		SetNodeNeighbors ();
	}

	public void FillMap ( float tileHeight, float tileWidth, MeshRenderer background){
		//Shorthands for center and size of game surface
		Vector3 center = background.bounds.center,
				size = background.bounds.size;

		//Calculating the topleft corner of the game surface
		float	topCornerX = center.x - (size.x) / 2,
				topCornerY = center.y + (size.y) / 2;

		Vector3 tempVector;	//Temporary Vector for readability 

		Map = new Cell[xTiles, yTiles];

		for (int i = 0; i < xTiles; i++) {
			for(int j = 0; j < yTiles; j++){
				tempVector = new Vector3(topCornerX + i*tileWidth, topCornerY - j*tileHeight, 0f );
				Debug.DrawRay(tempVector, new Vector3(0, -1, 0), Color.red, Mathf.Infinity);
				Map[i, j] = new Cell( i, j, tileHeight, tileWidth, tempVector, locations, obstacles);
                Debug.Log("Created Map at (" + i + "," + j + ") that is " + Map[i, j].content.ToString());
			}
		}
	}

	public void DrawGridLines(float tileHeight, float tileWidth, Renderer background){
		//Shorthands for center and size of game surface
		Vector3 center = background.bounds.center,
				size = background.bounds.size;

		//Calculating the topleft corner of the game surface
		float	topCornerX = center.x - (size.x) / 2,
				topCornerY = center.y + (size.y) / 2;

		//A moving point to draw the lines from, rename later
		Vector3 sideOne = new Vector3 ( topCornerX, topCornerY , 0f);

		//Drawing lines at every point at every horizontal edge of the cell  
		for ( int i = 0; i <= xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(0, -size.y, 0), Color.green, Mathf.Infinity );
			sideOne.x += tileWidth;
		}
		
		sideOne = new Vector3 (topCornerX, topCornerY, 0f);

		//Drawing lines at every vertical edge of the cell
		for ( int i = 0; i <= xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(size.x, 0, 0), Color.green, Mathf.Infinity );
			sideOne.y -= tileHeight;
		}
	}

	public void SetNodeNeighbors(){
		//Going through each cell in the map and setting its neighbor cells
		for(int i = 0; i < xTiles; i++){
			for(int j = 0; j < yTiles; j++){
				if( (i > 0) && (j > 0) )		//Top Left
					Map[ i,j ].neighbors.Add( Map[ i-1, j-1 ] );
				if( i > 0)						//Top Center
					Map[ i,j ].neighbors.Add( Map[ i-1, j ] );
				if( (i > 0) && (j < yTiles-1) )	//Top Right
					Map[ i,j ].neighbors.Add( Map[ i-1, j+1 ] );
				if( j > 0 )						//Middle Left
					Map[ i,j ].neighbors.Add( Map[ i, j-1 ] );
				if( j < yTiles-1 )				//Middle Right
					Map[ i,j ].neighbors.Add( Map[ i, j+1 ] );
				if( i < xTiles-1 && j > 0)		//Bottom Left
					Map[ i,j ].neighbors.Add( Map[ i+1, j-1 ] );
				if( i < xTiles-1 )				//Bottom Center
					Map[ i,j ].neighbors.Add( Map[ i+1, j ] );
				if( i < xTiles-1 && j < yTiles-1 )	//Bottom Right
					Map[ i,j ].neighbors.Add( Map[ i+1, j+1 ] );
			}
		}
	}
}

