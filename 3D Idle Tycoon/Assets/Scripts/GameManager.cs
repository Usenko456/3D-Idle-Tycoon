using SimpleCity.AI;
using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public PlacementManager placementManager;
    public UIController uiController;

    public StructureManager structureManager;

    public ObjectDetector objectDetector;

    public PathVisualizer pathVisualizer;

    public SaveSystem saveSystem;

    void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.OnHousePlacement += HousePlacementHandler;
        uiController.OnSpecialPlacement += SpecialPlacementHandler;
        uiController.OnBigStructurePlacement += BigStructurePlacement;
        inputManager.OnEscape += HandleEscape;
    }

    private void HandleEscape()
    {
        ClearInputActions();
        uiController.ResetButtonColor();
        pathVisualizer.ResetPath();
        inputManager.OnMouseClick += TrySelectingAgent;
    }

    private void TrySelectingAgent(Ray ray)
    {
        GameObject hitObject = objectDetector.RaycastAll(ray);
        if(hitObject != null)
        {
            var agentScript = hitObject.GetComponent<AiAgent>();
            agentScript?.ShowPath();
        }
    }

    private void BigStructurePlacement()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceBigStructure, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceSpecial, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(structureManager.PlaceHouse, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
        inputManager.OnMouseHold += (pos) =>
        {
            ProcessInputAndCall(roadManager.PlaceRoad, pos);
        };
        inputManager.OnEscape += HandleEscape;
    }

    private void ClearInputActions()
    {
        inputManager.ClearEvents();
    }

    private void ProcessInputAndCall(Action<Vector3Int> callback, Ray ray)
    {
        Vector3Int? result = objectDetector.RaycastGround(ray);
        if (result.HasValue)
            callback.Invoke(result.Value);
    }
    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }
    public void SaveGame()
    {
        SaveDataSerialization saveData = new SaveDataSerialization();
        saveData.savedBalance = EconomyManager.Instance.Balance;
        saveData.musicVolume = FindObjectOfType<VolumeManager>().CurrentVolume;
        foreach (var structureData in structureManager.GetAllStructures())
        {
            saveData.AddStructureData(structureData.Key, structureData.Value.BuildingPrefabIndex, structureData.Value.BuildingType);
        }
        var jsonFormat = JsonUtility.ToJson(saveData);
        Debug.Log(jsonFormat);
        saveSystem.SaveData(jsonFormat);
    }
    public void LoadGame()
    {
        var jsonFormatData = saveSystem.LoadData();
        if (string.IsNullOrEmpty(jsonFormatData))
            return;

        SaveDataSerialization saveData = JsonUtility.FromJson<SaveDataSerialization>(jsonFormatData);

        EconomyManager.Instance.SetBalance(saveData.savedBalance);
        FindObjectOfType<VolumeManager>().SetMusicVolume(saveData.musicVolume);
        structureManager.ClearMap();

        placementManager.isLoading = true;

       
        HashSet<Vector3Int> processedPositions = new HashSet<Vector3Int>();

        foreach (var structureData in saveData.structuresData)
        {
            Vector3Int position = Vector3Int.RoundToInt(structureData.position.GetValue());

            if (structureData.buildingType == CellType.Road)
            {
                roadManager.PlaceRoad(position);
                roadManager.FinishPlacingRoad();
            }
            else if (structureData.buildingType == CellType.BigStructure)
            {
                // If this cell has already been taken into ccount - skip it
                if (processedPositions.Contains(position))
                    continue;

                
                structureManager.PlaceLoadedStructure(position, structureData.buildingPrefabindex, structureData.buildingType);

                // Add all the cells occupied by this building
                for (int x = 0; x < 2; x++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        processedPositions.Add(position + new Vector3Int(x, 0, z));
                    }
                }
            }
            else
            {
                
                structureManager.PlaceLoadedStructure(position, structureData.buildingPrefabindex, structureData.buildingType);
            }
        }

        placementManager.isLoading = false;

        EconomyManager.Instance.RecalculateIncomePerMinute();
    }


}
