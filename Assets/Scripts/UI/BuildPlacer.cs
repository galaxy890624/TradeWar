using UnityEngine;

public class BuildPlacer : MonoBehaviour
{
    public static BuildPlacer Instance { get; private set; }

    [Header("設定")]
    public LayerMask groundMask;
    public Material previewMaterial;

    private GameObject currentPreview;
    private BuildingData currentData;
    private float currentRotation = 0f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartPlacing(BuildingData data)
    {
        currentData = data;
        if (currentPreview != null) Destroy(currentPreview);

        currentPreview = Instantiate(data.prefab);
        ApplyPreviewMaterial(currentPreview);
        currentRotation = 0f;
    }

    void Update()
    {
        if (currentPreview == null) return;

        // 滑鼠指向位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 pos = hit.collider.transform.position;
            currentPreview.transform.position = pos + Vector3.up * 0.5f;

            // 滾輪旋轉
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float rotationStep = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? rotationStep : -rotationStep;
            }

            currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            // 左鍵建造
            if (Input.GetMouseButtonDown(0))
            {
                MapTile tile = hit.collider.GetComponent<MapTile>();
                if (tile != null && tile.Build(currentData, currentRotation))
                {
                    Destroy(currentPreview);
                    currentPreview = null;
                    currentData = null;
                }
            }
        }

        // 右鍵取消
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }

    public void CancelPlacing()
    {
        if (currentPreview != null) Destroy(currentPreview);
        currentPreview = null;
        currentData = null;
    }

    private void ApplyPreviewMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = previewMaterial;
        }
    }
}

// 建議搭配的 Unity UI 結構：
// - Canvas
//   - Panel (靠右固定定位，Vertical Layout Group)
//     - ButtonPrefab (含 icon + name + 建立事件)
// - 所有格子有 Collider + MapTile.cs，需加 tag/layer 給 Raycast
