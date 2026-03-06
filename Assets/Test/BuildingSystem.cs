using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BuildingSystem : MonoBehaviour
{

    public static BuildingSystem Current;
    public GridLayout GridLayout;
    private Grid Grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase WhiteTile;

    public GameObject Prefab1;
    public GameObject Prefab2;

    private PlaceableObject ObjectToPlace;

    #region Unity Methods
    private void Awake()
    {
        Current = this;
        Grid = GridLayout.GetComponent<Grid>();
    }

    private void Update()
    {
        // For testing purposes only
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitializeWithObject(Prefab1);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            InitializeWithObject(Prefab2);
        }

        if (!ObjectToPlace) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanBePlaced(ObjectToPlace))
            {
                ObjectToPlace.Place();
                Vector3Int Start = GridLayout.WorldToCell(ObjectToPlace.GetStartPosition());
                TakeArea(Start, ObjectToPlace.Size);
            }
            else
            {
                Destroy(ObjectToPlace.gameObject);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(ObjectToPlace.gameObject);
        }
    }
    #endregion

    #region Utils
    public static Vector3 GetMouseWorldPosition()
    {
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Ray, out RaycastHit RaycastHit))
        {
            return RaycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 Position)
    {
        Vector3Int CellPosition = GridLayout.WorldToCell(Position);
        Position = Grid.GetCellCenterWorld(CellPosition);

        return Position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt Area, Tilemap Tilemap)
    {
        TileBase[] Array = new TileBase[Area.size.x * Area.size.y * Area.size.z];
        int Counter = 0;

        foreach (var v in Area.allPositionsWithin)
        {
            Vector3Int Position = new Vector3Int(v.x, v.y, v.z);
            Array[Counter] = Tilemap.GetTile(Position);
            Counter++;
        }

        return Array;
    }

    #endregion

    #region Building Placement
    public void InitializeWithObject(GameObject Prefab)
    {
        Vector3 Position = SnapCoordinateToGrid(Vector3.zero);
        GameObject NewObject = Instantiate(Prefab, Position, Quaternion.identity);
        ObjectToPlace = NewObject.GetComponent<PlaceableObject>();
        NewObject.AddComponent<ObjectDrag>();
    }

    private bool CanBePlaced(PlaceableObject Prefab)
    {
        BoundsInt Area = new BoundsInt();
        Area.position = GridLayout.WorldToCell(ObjectToPlace.GetStartPosition());
        Area.size = Prefab.Size;
        TileBase[] BaseArray = GetTilesBlock(Area, MainTilemap);

        foreach (var Base in BaseArray)
        {
            if(Base == WhiteTile) return false;
        }

        return true;
    }

    public void TakeArea(Vector3Int Start, Vector3Int Size)
    {
        MainTilemap.BoxFill(Start, WhiteTile, Start.x, Start.y, Start.x + Size.x, Start.y + Size.y);

    }
    #endregion
}