using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject MouseIndicator;
    [SerializeField] private GameObject CellIndicator;
    [SerializeField] private InputManager InputManager;
    [SerializeField] private Grid Grid;

    /// <summary>
    /// Database ScriptableObject
    /// </summary>
    [SerializeField] private ObjectsDatabaseSO Database;
    private int SelectedObjectID = -1;
    
    /// <summary>
    /// 3D Object - Plane
    /// </summary>
    [SerializeField] private GameObject GridVisualization;

    //[SerializeField] AudioSource Audio;

    private GridData FloorData, BuildingData;
    private Renderer PreviewRenderer;
    private List<GameObject> PlacedGameObjects = new();

    private void Start()
    {
        StopPlacement();
        FloorData = new();
        BuildingData = new();
        PreviewRenderer = CellIndicator.GetComponentInChildren<Renderer>(); // Hirearchy : CursorIndicatorParent/CursorIndicator
    }
    /// <summary>
    /// 開始放置物件<br></br>
    /// 傳入物件ID，會在Update中顯示預覽圖，並在點擊地圖時放置物件
    /// </summary>
    /// <param name="ID">物件的ID</param>
    public void StartPlacement(int ID)
    {
        StopPlacement();
        SelectedObjectID = Database.ObjectsData.FindIndex(Data => Data.ID == ID);
        if(SelectedObjectID < 0)
        {
            Debug.Log($"<color=#ff00ff>[PlacementSystem.cs] No ID found <color=#00ff00>{ID}</color></color>");
            return;
        }
        GridVisualization.SetActive(true);
        CellIndicator.SetActive(true);
        InputManager.OnClicked += PlaceStructure;
        InputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        // 點擊UI時不放置物件
        if (InputManager.IsPointerOverUI())
        {
            // Debug.Log($"<color=#ff00ff>[PlacementSystem.cs] IsPointerOverUI == <color=#00ff00>{InputManager.IsPointerOverUI()}</color></color>");
            return;
        }

        Vector3 MousePosition = InputManager.GetSelectedMapPosition();
        Vector3Int GridPosition = Grid.WorldToCell(MousePosition);

        bool PlacementValidity = CheckPlacementValidity(GridPosition, SelectedObjectID);
        if (PlacementValidity == false) return;

        // Audio.Play();
        GameObject NewObject = Instantiate(Database.ObjectsData[SelectedObjectID].Prefab);
        NewObject.transform.position = Grid.CellToWorld(GridPosition);
        PlacedGameObjects.Add(NewObject);
        GridData SelectedData = Database.ObjectsData[SelectedObjectID].ID == 0 ? FloorData : BuildingData;
        SelectedData.AddObjectAt(GridPosition, Database.ObjectsData[SelectedObjectID].Size, Database.ObjectsData[SelectedObjectID].ID, PlacedGameObjects.Count - 1);
    }

    private bool CheckPlacementValidity(Vector3Int GridPosition, int SelectedObjectID)
    {
        GridData SelectedData = Database.ObjectsData[SelectedObjectID].ID == 0 ? FloorData : BuildingData;
        return SelectedData.CanPlaceObejctAt(GridPosition, Database.ObjectsData[SelectedObjectID].Size);
    }

    private void StopPlacement()
    {
        SelectedObjectID = -1;
        GridVisualization.SetActive(false);
        CellIndicator.SetActive(false);
        InputManager.OnClicked -= PlaceStructure;
        InputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (SelectedObjectID < 0) return;
        Vector3 MousePosition = InputManager.GetSelectedMapPosition();
        Vector3Int GridPosition = Grid.WorldToCell(MousePosition);

        bool PlacementValidity = CheckPlacementValidity(GridPosition, SelectedObjectID);
        PreviewRenderer.material.color = PlacementValidity ? Color.green : Color.red;

        MouseIndicator.transform.position = MousePosition;
        CellIndicator.transform.position = Grid.GetCellCenterWorld(GridPosition);
    }
}
