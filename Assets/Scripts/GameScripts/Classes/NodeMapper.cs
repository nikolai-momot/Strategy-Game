using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeMapper
{
	private int xTiles, yTiles;
	private List<StratObj> locations;
	private int playerCount;
	public Cell[,] Map;
	
	public NodeMapper ( int x, int y, List<StratObj>  objLocations, int players ){
		this.xTiles = x;								//Cells on the map on x 
		this.yTiles = y;								//Cells on the map on y
		this.locations = objLocations;					//Strategic Objectives on the map
		this.playerCount = players;						//Players in the game
		this.Map = new Cell[this.xTiles, this.yTiles];	//Cell map

		GameObject background = GameObject.Find ("Background");						//Getting background object

		MeshRenderer backgroundRenderer = background.GetComponent<MeshRenderer> ();	//Getting background object's renderer

		float	tileWidth = ( backgroundRenderer.bounds.size.x ) / this.xTiles,		//Cell width
				tileHeight = ( backgroundRenderer.bounds.size.y ) / this.yTiles;	//Cell height
		
		DrawGridLines (tileHeight, tileWidth, backgroundRenderer);	//Draw grip lines in between cells
		FillMap (tileHeight, tileWidth, backgroundRenderer);		//Fill map with cells
		SetNodeNeighbors ();										//Set neighboring cells list for each cell 

		return;
	}

	//Find the neighboring tile with the lowest heat  
	public Cell lowestHeat( Cell cell, int id, int players){
		Cell[] neighbors 	= cell.neighbors.ToArray();	//Array of neighboring cells
		Cell coolest 		= neighbors[0];		//Cell with the least heat, temporary value
		int lowest			= coolest.heat[0];	//Lowest heat value, temporary value

		for (int i = 0; i < players; i++) {
			//Check every player except the current one
			if(i==id)
				continue;

			for(int j = 0; j < neighbors.Length; j++){
				//Reset coolest tile and lowest heat value if a lower one is found
				if( neighbors[j].heat[i] < lowest ){
					coolest = neighbors[j];
					lowest	= coolest.heat[i];
				}
			}
		}

		return coolest;
	}

	//Fill map with cells
	public void FillMap ( float tileHeight, float tileWidth, MeshRenderer background){
		Vector3 center = background.bounds.center,		//Center of background
				size = background.bounds.size;			//Size of background
		
		float	topCornerX = center.x - (size.x) / 2,	//X value of the background's top-left corner
				topCornerY = center.y + (size.y) / 2;	//Y value of the background's top-left corner
		
		Vector3 tempVector;	//Temporary vector for readability

		for (int i = 0; i < this.xTiles; i++) {
			for(int j = 0; j < this.yTiles; j++){
				//Creating cell
				tempVector = new Vector3(topCornerX + i*tileWidth, topCornerY - j*tileHeight, 0f );
				this.Map[i, j] = new Cell(tileHeight, tileWidth, tempVector, this.locations, this.playerCount);

				this.Map[i, j].setXY(i, j);	//Set cell coordinates

				if(this.Map[i,j].filled){	//Set StratObj coordinate, if there is one 
					StratObj tempObj = this.Map[i,j].getObjective();
					tempObj.x = i;
					tempObj.y = j;
					this.Map[i,j].setObjective(tempObj);
				}
				 
			}
		}

		return;
	}

	//Draw grip lines in between cells
	public void DrawGridLines(float tileHeight, float tileWidth, Renderer background){
		Vector3 center = background.bounds.center,		//Center of background
				size = background.bounds.size;			//Size of background
		
		float	topCornerX = center.x - (size.x) / 2,	//X value of the background's top-left corner
				topCornerY = center.y + (size.y) / 2;	//Y value of the background's top-left corner
		
		//A moving ruler by which the lines are drawn
		Vector3 sideOne = new Vector3 ( topCornerX, topCornerY , 0f);	

		//Draw vertical line and move over by the cell size, left to right
		for ( int i = 0; i <= this.xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(0, -size.y, 0), Color.green, Mathf.Infinity );
			sideOne.x += tileWidth;
		}
		
		sideOne = new Vector3 (topCornerX, topCornerY, 0f);	//Turning ruler

		//Draw horizontal line and move over by the cell size, top to bottom
		for ( int i = 0; i <= this.xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(size.x, 0, 0), Color.green, Mathf.Infinity );
			sideOne.y -= tileHeight;
		}

		return;
	}

	public void SetNodeNeighbors(){
		for(int i = 0; i < this.xTiles; i++){
			for(int j = 0; j < this.yTiles; j++){
				if( (i > 0) && (j > 0) )		//Top Left
					if( this.Map[ i-1, j-1 ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i-1, j-1 ] );

				if( i > 0)						//Top Center
					if( this.Map[ i-1, j ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i-1, j ] );

				if( (i > 0) && (j < this.yTiles-1) )	//Top Right
					if( this.Map[ i-1, j+1 ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i-1, j+1 ] );

				if( j > 0 )						//Middle Left
					if( this.Map[ i, j-1 ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i, j-1 ] );

				if( j < this.yTiles-1 )				//Middle Right
					if( Map[ i, j+1 ] != null )
						Map[ i,j ].neighbors.Add( Map[ i, j+1 ] );

				if( i < this.xTiles-1 && j > 0)		//Bottom Left
					if( this.Map[ i+1, j-1 ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i+1, j-1 ] );

				if( i < this.xTiles-1 )				//Bottom Center
					if( Map[ i+1, j ] != null )
						Map[ i,j ].neighbors.Add( this.Map[ i+1, j ] );

				if( i < this.xTiles-1 && j < this.yTiles-1 )	//Bottom Right
					if( this.Map[ i+1, j+1 ] != null )
						this.Map[ i,j ].neighbors.Add( this.Map[ i+1, j+1 ] );
			}
		}

		return;
	}
}

