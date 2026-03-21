using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField, Header("遊戲的攝影機")] private Camera SceneCamera;

    private Vector3 LastPosition;

    [SerializeField] private LayerMask PlacementLayermask;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if(Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 MousePos = Input.mousePosition;
        MousePos.z = SceneCamera.nearClipPlane;
        Ray ray = SceneCamera.ScreenPointToRay(MousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, PlacementLayermask))
        {
            LastPosition = hit.point;
        }
        //print($"<color=#ff00ff>LastPosition = <color=#00ff00>{LastPosition}</color></color>");
        return LastPosition; // 必須要有碰撞器才會有作用
    }
}
