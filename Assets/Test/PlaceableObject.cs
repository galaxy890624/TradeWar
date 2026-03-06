using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }
    private Vector3[] Vertices;

    private void GetColliderVertexPositionLocal()
    {
        BoxCollider Collider = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = Collider.center + new Vector3(-Collider.size.x / 2, -Collider.size.y / 2, -Collider.size.z / 2);
        Vertices[1] = Collider.center + new Vector3(Collider.size.x / 2, -Collider.size.y / 2, -Collider.size.z / 2);
        Vertices[2] = Collider.center + new Vector3(Collider.size.x / 2, -Collider.size.y / 2, Collider.size.z / 2);
        Vertices[3] = Collider.center + new Vector3(-Collider.size.x / 2, -Collider.size.y / 2, Collider.size.z / 2);
    }

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[Vertices.Length]; // Don't name Uppercase
        for(int i = 0; i < Vertices.Length; i++)
        {
            Vector3 WorldPosition = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingSystem.Current.GridLayout.WorldToCell(WorldPosition);
        }

        Size = new Vector3Int(
            Mathf.Abs(vertices[1].x - vertices[0].x),
            Mathf.Abs(vertices[3].y - vertices[0].y),
            1
        );
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionLocal();
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        Object Drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(Drag);

        Placed = true;

        // Invoke events of placement here

    }
}
