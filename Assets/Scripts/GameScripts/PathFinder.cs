using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {

	private Cell[,] cellMap;	//Cell Map

	//Custom IComparer for checking heuristic values
	public class HeuristicComparer : IComparer<int>  {

		// Calls CaseInsensitiveComparer. Compare with the parameters reversed. 
		public int Compare( int x, int y )  {
			if (x < y)
				return -1;
			else
				return 1;
		}
		
	}

	//Constructor
	public PathFinder( Cell[,] newMap){
		cellMap = newMap;	//Setting cell map
	}

	//Finds path between two cells
	public List<Cell> FindPath( Cell start, Cell end ){
		//Cells to look at
		SortedList<int, Cell> openQueue = new SortedList<int, Cell> (new HeuristicComparer ());
		//Cells already looked at
		SortedList<int, Cell> closedQueue = new SortedList<int, Cell> ( new HeuristicComparer() );
		//Storing path here
		List<Cell> fullPath = new List<Cell>();

		if (start == end) {	//No need tofind path if you're already there
			fullPath.Add(end);
			return fullPath;
		}

		//Adding start cell to open queue
		openQueue.Add( getFValue( start, start, end ), start );

		//Keep looking so long as there are cells in the open queue
		while ( ( openQueue.First() ).Value != end ) {


			Cell current = ( openQueue.First() ).Value;						//Remove the first cell form the open queue,
			openQueue.RemoveAt(0);											//place it into the closed queue
			closedQueue.Add( getFValue( start, current, end ), current );	//and set it current cell

			//Check each neighboring cells
			foreach( Cell neighbor in current.neighbors ){
				int cost = getFValue( start, neighbor, end );				//Movement cost

				//if neighbor in OPEN and cost less than g(neighbor)
				//remove neighbor from OPEN, because new path is better
				if ( openQueue.ContainsValue(neighbor) && ( cost < getGValue( start, neighbor ) ) )
					openQueue.RemoveAt( openQueue.IndexOfValue( neighbor ) );
				//if neighbor is in the closed queue and movement cost less than g(neighbor)
				//remove neighbor from closed queue
				if( closedQueue.ContainsValue(neighbor) && ( cost < getGValue( start, neighbor ) ) )
					closedQueue.RemoveAt( closedQueue.IndexOfValue( neighbor ) );
				//if neighbor is not in the open queue and not in the closed queue
				if( !openQueue.ContainsValue( neighbor ) && !closedQueue.ContainsValue(neighbor) ){
					neighbor.parent = current;	//set neighbor's parent to current
					openQueue.Add( cost + getHValue(neighbor, end), neighbor );	// add neighbor to open queue
				}
			}
		}

		//reconstructing reverse path from goal to start by following parent pointers
		Cell pathNode = openQueue.Values[0];
		fullPath.Add( pathNode );

		while(pathNode.parent != start){
			pathNode = pathNode.parent;
			fullPath.Add( pathNode );
		}

        drawFullPath(fullPath);
        fullPath.Reverse();

		return fullPath;
	}

	//Calculating F value
	private int getFValue( Cell start, Cell current, Cell end ){
		return getGValue( start, current ) + getHValue( current, end );
	}

	//Calculating G value
	private int getGValue( Cell start, Cell current){
		int xValue = Math.Abs ( current.x - start.x );
		int yValue = Math.Abs ( current.y - start.y );
		
		return xValue + yValue;
	}

	//Calculating H value
	private int getHValue( Cell current, Cell end ){
		int xValue = Math.Abs ( current.x - end.x );
		int yValue = Math.Abs ( current.y - end.y );
		
		return xValue + yValue;
	}
	
	//Using vector3 to find the cell it is located in
	public Cell cellFromVector( Vector3 objLocation ){
		foreach (Cell cell in cellMap) {
			if ( cell.containsVector( objLocation ) )
				return cell;
		}

		return null;
	}

	//Draws path from start to finish
	public void drawFullPath( List<Cell> cellPath ){
		List<Vector3> vectorPath = cellToVectors ( cellPath, false );
		Vector3[] pathArray = vectorPath.ToArray ();
		for (int i = 0; i < pathArray.Length-1; i++) {
			Debug.DrawLine( pathArray[i], pathArray[i+1], Color.blue, 30 );
		}
	}

	//Converts list of cells to list of vectors
	public List<Vector3> cellToVectors( List<Cell> fullPath, bool adjustVector ) {
		float	height	= fullPath.First().height, 
				width	= fullPath.First().width;

		List<Vector3> vectorPath = new List<Vector3> ();

		fullPath.ForEach(delegate(Cell cell) {
			if(adjustVector)
				vectorPath.Add( cornerToCenter( cell.position, height, width ) );
			else
				vectorPath.Add( cell.position );
		});

		return vectorPath;
	}

	//Converting corner vector to center vector
	public Vector3 cornerToCenter( Vector3 cornerPoint, float height, float width ){
		float 	newX = cornerPoint.x + ( width / 2 ),
				newY = cornerPoint.y - ( height / 2 );

		return new Vector3(newX, newY, 0f);
	}

}
