using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeMapper
{
	public int xTiles, yTiles;
	public List<StratObj> locations;
	public StratNode[,] Map;

	public NodeMapper ( int x, int y, List<StratObj>  objLocations ){
		xTiles = x;
		yTiles = y;
		locations = objLocations;

		GameObject background = GameObject.Find ("Background");

		MeshRenderer backgroundRenderer = background.GetComponent<MeshRenderer> ();

		float	tileWidth = ( backgroundRenderer.bounds.size.x ) / xTiles,
				tileHeight = ( backgroundRenderer.bounds.size.y ) / yTiles;
		
		DrawGridLines (tileHeight, tileWidth, backgroundRenderer);
		FillMap (tileHeight, tileWidth, backgroundRenderer);
		SetNodeNeighbors ();
	}

	public void FillMap ( float tileHeight, float tileWidth, MeshRenderer background){
		Vector3 center = background.bounds.center,
				size = background.bounds.size;
		
		float	topCornerX = center.x - (size.x) / 2,
				topCornerY = center.y + (size.y) / 2;
		
		Vector3 sideOne = new Vector3 ( topCornerX, topCornerY , 0f);
		Vector3 tempVector;

		Map = new StratNode[xTiles, yTiles];

		for (int i = 0; i < xTiles; i++) {
			for(int j = 0; j < yTiles; j++){
				tempVector = new Vector3(topCornerX + i*tileWidth, topCornerY - j*tileHeight, 0f );
				Map[i, j] = new StratNode(tileHeight, tileWidth, tempVector, locations);
				Map[i, j].setXY(i, j);
				Debug.Log("Created MapNode at ("+i+","+j+")");
				 
			}
		}
	}

	public void DrawGridLines(float tileHeight, float tileWidth, Renderer background){
		Vector3 center = background.bounds.center,
				size = background.bounds.size;
		
		float	topCornerX = center.x - (size.x) / 2,
				topCornerY = center.y + (size.y) / 2;
		
		Vector3 sideOne = new Vector3 ( topCornerX, topCornerY , 0f);
		
		for ( int i = 0; i <= xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(0, -size.y, 0), Color.green, Mathf.Infinity );
			sideOne.x += tileWidth;
		}
		
		sideOne = new Vector3 (topCornerX, topCornerY, 0f);
		
		for ( int i = 0; i <= xTiles; i++ ) {
			Debug.DrawRay( sideOne, new Vector3(size.x, 0, 0), Color.green, Mathf.Infinity );
			sideOne.y -= tileHeight;
		}
	}

	public void SetNodeNeighbors(){
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

