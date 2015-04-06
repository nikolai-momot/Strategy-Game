using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {

	public PathFinder(){}

	public List<StratNode> FindPath( StratNode start, StratNode end ){
		SortedDictionary<int, StratNode> openQueue = new SortedDictionary<int, StratNode> ();
		SortedDictionary<int, StratNode> closedQueue = new SortedDictionary<int, StratNode> ();



		openQueue.Add( getFValue( start, start, end ), start );

		while (openQueue.Values[0] != end) {
			StratNode current = openQueue.Values[0];
			openQueue.RemoveAt(0);
			closedQueue.Add( getFValue( start, current, end ), current );

			foreach( StratNode neighbor in current.neighbors ){
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

		List<StratNode> fullPath = new List<StratNode>();

		StratNode pathNode = openQueue.Values[0];
		fullPath.Add( pathNode );

		while(pathNode.parent != start){
			pathNode = pathNode.parent;
			fullPath.Add( pathNode );
		}

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

	public void drawFullPath( List<StratNode> fullPath ){
		foreach (StratNode point in fullPath) {
			Debug.DrawLine( point.position, point.parent.position, Color.blue);
		}
	}

	private int getFValue( StratNode start, StratNode current, StratNode end ){
		return getGValue( start, current ) + getHValue( current, end );
	}
	
	private int getGValue( StratNode start, StratNode current){
		int xValue = Math.Abs ( current.x - start.x );
		int yValue = Math.Abs ( current.y - start.y );
		
		return xValue + yValue;
	}
	
	private int getHValue( StratNode current, StratNode end ){
		int xValue = Math.Abs ( current.x - end.x );
		int yValue = Math.Abs ( current.y - end.y );
		
		return xValue + yValue;
	}

}
