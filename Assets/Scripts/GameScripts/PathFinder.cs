using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {

	public Cell[,] cellMap;

	public class HeuristicComparer : IComparer<int>  {
		
		// Calls CaseInsensitiveComparer.Compare with the parameters reversed. 
		public int Compare( int x, int y )  {
			if (x < y)
				return -1;
			else
				return 1;
		}
		
	}

	public PathFinder( Cell[,] newMap){
		cellMap = newMap;
	}

	public List<Cell> FindPath( Cell start, Cell end ){
		SortedList<int, Cell> openQueue = new SortedList<int, Cell> ( new HeuristicComparer() );
		SortedList<int, Cell> closedQueue = new SortedList<int, Cell> ( new HeuristicComparer() );


		openQueue.Add( getFValue( start, start, end ), start );

		while ( ( openQueue.First() ).Value != end ) {
			Cell current = ( openQueue.First() ).Value;
			openQueue.RemoveAt(0);
			closedQueue.Add( getFValue( start, current, end ), current );

			foreach( Cell neighbor in current.neighbors ){
				int cost = getFValue( start, neighbor, end );

				if ( openQueue.ContainsValue(neighbor) && ( cost < getGValue( start, neighbor ) ) )
					openQueue.RemoveAt( openQueue.IndexOfValue( neighbor ) );
				if( closedQueue.ContainsValue(neighbor) && ( cost < getGValue( start, neighbor ) ) )
					closedQueue.RemoveAt( closedQueue.IndexOfValue( neighbor ) );
				if( !openQueue.ContainsValue( neighbor ) && !closedQueue.ContainsValue(neighbor) ){
					neighbor.parent = current;
					openQueue.Add( cost + getHValue(neighbor, end), neighbor );//A bit shacky around here
				}
			}
		}

		List<Cell> fullPath = new List<Cell>();

		Cell pathNode = openQueue.Values[0];
		fullPath.Add( pathNode );

		while(pathNode.parent != start){
			pathNode = pathNode.parent;
			fullPath.Add( pathNode );
		}
        drawFullPath(fullPath);
		return fullPath;

		/*OPEN = priority queue containing START
		CLOSED = empty set
		while lowest rank in OPEN is not the GOAL:
		  current = remove lowest rank item from OPEN
		  add current to CLOSED
		  for neighbors of current:
		    cost = g(current) + movementcost(current, neighbor)
		    if neighbor in OPEN and cost less than g(neighbor):
		      remove neighbor from OPEN, because new path is better
		    if neighbor in CLOSED and cost less than g(neighbor): **
		      remove neighbor from CLOSED
		    if neighbor not in OPEN and neighbor not in CLOSED:
		      set g(neighbor) to cost
		      add neighbor to OPEN
		      set priority queue rank to g(neighbor) + h(neighbor)
		      set neighbor's parent to current

		reconstruct reverse path from goal to start
		by following parent pointers*/
	}

	public Cell cellFromVector( Vector3 objLocation ){
		foreach (Cell cell in cellMap) {
			if ( cell.containsVector( objLocation ) )
				return cell;
		}

		return null;
	}

	public void drawFullPath( List<Cell> cellPath ){
		List<Vector3> vectorPath = cellToVectors ( cellPath, true );
		Vector3[] pathArray = vectorPath.ToArray ();
		for (int i = 0; i < pathArray.Length-1; i++) {
			Debug.DrawLine( pathArray[i], pathArray[i+1], Color.blue, Mathf.Infinity );
		}
	}

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

	public Vector3 cornerToCenter( Vector3 cornerPoint, float height, float width ){
		float 	newX = cornerPoint.x + ( width / 2 ),
				newY = cornerPoint.y - ( height / 2 );

		return new Vector3(newX, newY, 0f);
	}

	private int getFValue( Cell start, Cell current, Cell end ){
		return getGValue( start, current ) + getHValue( current, end );
	}
	
	private int getGValue( Cell start, Cell current){
		int xValue = Math.Abs ( current.x - start.x );
		int yValue = Math.Abs ( current.y - start.y );
		
		return xValue + yValue;
	}
	
	private int getHValue( Cell current, Cell end ){
		int xValue = Math.Abs ( current.x - end.x );
		int yValue = Math.Abs ( current.y - end.y );
		
		return xValue + yValue;
	}

}
