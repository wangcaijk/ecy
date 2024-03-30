using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Cinemachine;
using Photon.Pun;


//[System.Serializable] public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;


    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;
    public event Action<GameObject> OnCharacterClicked;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();
        if (InteractWithUI()) return;
        MouseControl();
    }


    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (InteractWithUI())
            {
                Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto); //如果有UI阻挡着就返回不响应了
                return;
            }

            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            // if (EventSystem.current.IsPointerOverGameObject())
            // {
            //     //Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);//如果有UI阻挡着就返回不响应了
            //     return;
            // }

            //    if (hitInfo.collider.gameObject.CompareTag("Ground"))
            //    {
            //        OnMouseClicked?.Invoke(hitInfo.point);
            //    }

            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.CompareTag("Charater"))
            {
                OnCharacterClicked?.Invoke(hitInfo.collider.gameObject);
            }
        }
    }

    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else return false;
    }
}