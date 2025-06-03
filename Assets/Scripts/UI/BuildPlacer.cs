using UnityEngine;

public class BuildPlacer : MonoBehaviour
{
    public static BuildPlacer Instance { get; private set; }

    [Header("�]�w")]
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

        // �ƹ����V��m
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 pos = hit.collider.transform.position;
            currentPreview.transform.position = pos + Vector3.up * 0.5f;

            // �u������
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float rotationStep = Input.GetKey(KeyCode.R) ? 15f : 90f;
                currentRotation += scroll > 0 ? rotationStep : -rotationStep;
            }

            currentPreview.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            // ����سy
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

        // �k�����
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

// ��ĳ�f�t�� Unity UI ���c�G
// - Canvas
//   - Panel (�a�k�T�w�w��AVertical Layout Group)
//     - ButtonPrefab (�t icon + name + �إߨƥ�)
// - �Ҧ���l�� Collider + MapTile.cs�A�ݥ[ tag/layer �� Raycast
