using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataSerialization
{
    public float savedBalance;
    public float musicVolume;
    public List<BuildingDataSerialization> structuresData = new List<BuildingDataSerialization>();

    public void AddStructureData(Vector3Int position, int buildingPrefabindex, CellType buildingType)
    {
        structuresData.Add(new BuildingDataSerialization(position, buildingPrefabindex, buildingType));
    }
}

[Serializable]
public class BuildingDataSerialization
{
    public Vector3Serialization position;
    public int buildingPrefabindex;
    public CellType buildingType;

    public int width = 1;  // ??????
    public int height = 1;  // ??????

    public BuildingDataSerialization(Vector3Int position, int buildingPrefabindex, CellType buildingType, int width = 1, int height = 1)
    {
        this.position = new Vector3Serialization(position);
        this.buildingPrefabindex = buildingPrefabindex;
        this.buildingType = buildingType;
        this.width = width;
        this.height = height;
    }
}
[Serializable]
public class Vector3Serialization
{
    public float x, y, z;

    public Vector3Serialization(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}