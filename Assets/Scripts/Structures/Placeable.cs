﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Entities;
using Untitled.Tiles;
using Untitled.Utils;

public class Placeable : MonoBehaviour
{
    public int cost;
	public List<TileType> placeableTiles;
	public Vector2Int size;
	public Coords coords;
	public Vector2 spriteOffset; // in units of tile count
	private List<Coords> boundsList;
	private List<Coords> surroundingTiles;
	
	protected void Start()
	{
		coords = new Coords(this.gameObject.transform.position);
		
		boundsList = new List<Coords>();
		
		/*
		* Generate the list of all tiles this Placeable is on
		* Calculate offset by rounding down
		*/ 
		int xStart = -(int)((size.x - 1) / 2);
		int xEnd = (int)(size.x / 2);
		int yStart = -(int)((size.y - 1) / 2);
		int yEnd = (int)(size.y / 2);

		for(int xOff = xStart; xOff <= xEnd; xOff++)
			for(int yOff = yStart; yOff <= yEnd; yOff++)
				boundsList.Add(coords + new Vector2Int(xOff, -yOff));
			
			
		/*
		* Generate list of surrounding tiles
		*/
		surroundingTiles = new List<Coords>();
		foreach(Coords coords in boundsList)
		{
			if(!boundsList.Contains(coords + Vector2Int.up))
				surroundingTiles.Add(coords + Vector2Int.up);
			if(!boundsList.Contains(coords + Vector2Int.right))
				surroundingTiles.Add(coords + Vector2Int.right);
			if(!boundsList.Contains(coords + Vector2Int.down))
				surroundingTiles.Add(coords + Vector2Int.down);
			if(!boundsList.Contains(coords + Vector2Int.left))
				surroundingTiles.Add(coords + Vector2Int.left);
		}
		
		OnPlaceableCreateEventP1?.Invoke(this);
		OnPlaceableCreateEventP2?.Invoke(this);
		
		// Offset the sprite without offsetting the grid location
		float xOffset = spriteOffset.x * GridUtils.Instance.dx.x;
		float yOffset = spriteOffset.y * GridUtils.Instance.dy.y;
		this.gameObject.transform.position += new Vector3(xOffset, yOffset, 0);
	}
	
	protected void OnDestroy() 
	{
		OnPlaceableDestroyEventP1?.Invoke(this);
		OnPlaceableDestroyEventP2?.Invoke(this);
	}
	
	public bool IsBuilding()
	{
		return this.gameObject.GetComponent<Building>() != null;
	}
	public bool IsCable()
	{
		return this.gameObject.GetComponent<Cable>() != null;
	}
	public bool IsNextTo(Placeable other)
	{
		// Don't check GridUtils in O(n) time because
		// it isn't guarunteed to work in PlaceableCreated
		// or PlaceableDestroyed event handlers. Instead 
		// use O(n^2) and check both lists
		foreach(Coords c1 in surroundingTiles)
			foreach(Coords c2 in other.GetBounds()) 
				if(c1 == c2)
					return true;
		
		return false;
	}
	
	// Returns a list of Coord objects for all
	// tiles the placeable is on
	public List<Coords> GetBounds()
	{
		return boundsList;
	}
	
	// Returns a list of all tiles surrounding this placeable
	public List<Coords> GetSurroundingTiles()
	{
		return surroundingTiles;
	}
	
	/***************
	***  Events  ***
	****************/
	/* 
	* Split creation and deletion into 2 phases so 
	* GridUtils and PowerGridManager can process creations 
	* and deletions before anyone else
	*
	* For guaranteed behavior, any class that needs to 
	* use GridUtils or PowerGridManager in their handlers
	* should use CreateEventP2 and DestroyEventP2
	*/
	public static event Action<Placeable> OnPlaceableCreateEventP1;
	public static event Action<Placeable> OnPlaceableDestroyEventP1;
	public static event Action<Placeable> OnPlaceableCreateEventP2;
	public static event Action<Placeable> OnPlaceableDestroyEventP2;
}
