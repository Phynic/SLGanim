using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TPSCamera : MonoBehaviour {
    
    public Transform follow;
    [Header("中心偏移")]
    public Vector3 center = new Vector3(0.3f,1.8f,0f); //Y轴偏移

    public float distance = 7.0f;
    public float speed = 50.0f;
    public float yMinLimit = 0.1f;
    public float yMaxLimit = 89.9f;

    private float xRot = 0.0f;

    public float yRot = 0.0f;

    private float wheelSpeed = 10f;

    float minDistance = 1.5f;
    float maxDistance = 1.5f;
    public bool lockCursor = false;
    void Start()
    {
        follow = follow ? follow : GameObject.Find("Player").transform;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Follow(follow);
    }

    public void Follow(Transform character)
    {
        foreach (var item in follow.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 0;
        }

        foreach (var item in character.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 9;
        }
        
        follow = character;

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        distance -= mouseWheel * wheelSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance); //距离限制
        yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        transform.rotation = Quaternion.Euler(yRot, xRot, 0);

        Vector3 finalPosition = follow.position + follow.right * center.x + follow.up * center.y + follow.forward * center.z;

        transform.position = transform.rotation * negDistance + finalPosition;
    }

    void LateUpdate()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject.name == "RoleMenu")
        //    {
        //        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        //        xRot += Input.GetAxis("Mouse X") / 10.0f * speed;
        //        yRot -= Input.GetAxis("Mouse Y") / 10.0f * speed;

        //        distance -= mouseWheel * wheelSpeed;
        //        distance = Mathf.Clamp(distance, minDistance, maxDistance); //距离限制
        //        yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit);
        //        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        //        transform.rotation = Quaternion.Euler(yRot, xRot, 0);

        //        Vector3 finalPosition = follow.position + follow.right * center.x + follow.up * center.y + follow.forward * center.z;

        //        transform.position = transform.rotation * negDistance + finalPosition;
        //    }
        //}
    }
}
