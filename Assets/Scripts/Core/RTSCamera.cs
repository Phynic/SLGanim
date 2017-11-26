using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RTSCamera : MonoBehaviour
{
    public float cameraMoveSpeed = 10;
    //public float cameraRotateSpeed = 100;
    public float cameraScrollSpeed = 300;

    public float minXPos = 30;
    public float maxXPos = 50;

    public float minYPos = 10;
    public float maxYPos = 30;

    public float minZPos = 30;
    public float maxZPos = 50;
    
    public float minD = 10;
    public float maxD = 30;
    public float distanceToGround;
    
    void LateUpdate()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.mousePosition.x <= 20)
            {
                transform.Translate(Vector3.left * cameraMoveSpeed * Time.deltaTime);
            }
            if (Input.mousePosition.x >= (Screen.width - 20))
            {
                transform.Translate(Vector3.right * cameraMoveSpeed * Time.deltaTime);
            }
            if (Input.mousePosition.y <= 20)
            {
                transform.Translate(Vector3.back * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y >= (Screen.height - 20))
            {
                transform.Translate(Vector3.forward * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            distanceToGround = transform.position.y;
            if (distanceToGround <= minD)
            {
                if (mouseWheel > 0)
                {
                    mouseWheel = 0;
                    distanceToGround = minD;
                }
            }
            else if (distanceToGround >= maxD)
            {
                if (mouseWheel < 0)
                {
                    mouseWheel = 0;
                    distanceToGround = maxD;
                }
            }
            
            float currentX;
            float currentY;
            float currentZ;
            
            currentX = transform.position.x;
            currentY = transform.position.y;
            currentZ = transform.position.z;
            currentY -= mouseWheel * cameraScrollSpeed * Time.deltaTime;
            currentZ += mouseWheel * cameraScrollSpeed * Time.deltaTime;
            currentX = Mathf.Clamp(currentX, minXPos, maxXPos);
            currentY = Mathf.Clamp(currentY, minYPos, maxYPos);
            currentZ = Mathf.Clamp(currentZ, minZPos - currentY, maxZPos - currentY);

            transform.position = new Vector3(currentX, currentY, currentZ);
        }
    }
}
