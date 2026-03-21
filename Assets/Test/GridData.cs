using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> PlacedObjects = new();

    public void AddObjectAt(Vector3Int GridPosition,
                            Vector2Int ObjectSize,
                            int ID,
                            int PlacedObjectIndex)
    {
        List<Vector3Int> PositionToOccupy = CalculatePositions(GridPosition, ObjectSize);
        PlacementData data = new PlacementData(PositionToOccupy, ID, PlacedObjectIndex);
        foreach (var pos in PositionToOccupy)
        {
            if (PlacedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell positiojn {pos}");
            PlacedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int GridPosition, Vector2Int ObjectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < ObjectSize.x; x++)
        {
            for (int y = 0; y < ObjectSize.y; y++)
            {
                returnVal.Add(GridPosition + new Vector3Int(x, 0, y));
            }
        }
        // Debug.Log($"<color=#ff00ff>[GridData.cs] returnVal = <color=#00ff00>{returnVal}</color></color>");
        return returnVal;
    }

    public bool CanPlaceObejctAt(Vector3Int GridPosition, Vector2Int ObjectSize)
    {
        List<Vector3Int> PositionToOccupy = CalculatePositions(GridPosition, ObjectSize);
        foreach (var pos in PositionToOccupy)
        {
            if (PlacedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int GridPosition)
    {
        if (PlacedObjects.ContainsKey(GridPosition) == false)
            return -1;
        return PlacedObjects[GridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int GridPosition)
    {
        foreach (var pos in PlacedObjects[GridPosition].occupiedPositions)
        {
            PlacedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        placedObjectIndex = PlacedObjectIndex;
    }
}