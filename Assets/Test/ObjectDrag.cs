using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 Offset;

    private void OnMouseDown()
    {
        Offset = transform.position - BuildingSystem.GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        Vector3 NewPosition = BuildingSystem.GetMouseWorldPosition() + Offset;
        transform.position = BuildingSystem.Current.SnapCoordinateToGrid(NewPosition);
    }
}
