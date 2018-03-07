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

    public float minZPos = 15;
    public float maxZPos = 30;

    private Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        float currentX;
        float currentY;
        float currentZ;
        currentX = transform.position.x;
        currentY = transform.position.y;
        currentZ = transform.position.z;
#if (UNITY_IOS || UNITY_ANDROID)
        
#elif (UNITY_STANDALONE || UNITY_EDITOR)

        //float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
#endif
        
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            deltaMagnitudeDiff = Mathf.Clamp(deltaMagnitudeDiff, -1, 1);
            // Otherwise change the field of view based on the change in distance between the touches.
            camera.fieldOfView += deltaMagnitudeDiff * cameraScrollSpeed / 50 * Time.deltaTime;
            camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10, 50);
            //currentY += deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
            //currentZ -= deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
        }
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            currentX -= Input.GetTouch(0).deltaPosition.x / 80.0f * cameraMoveSpeed * Time.deltaTime;
            currentZ -= Input.GetTouch(0).deltaPosition.y / 80.0f * cameraMoveSpeed * Time.deltaTime;
        }
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //if (Input.mousePosition.x <= 20)
            //{
            //    transform.Translate(Vector3.left * cameraMoveSpeed * Time.deltaTime);
            //}
            //if (Input.mousePosition.x >= (Screen.width - 20))
            //{
            //    transform.Translate(Vector3.right * cameraMoveSpeed * Time.deltaTime);
            //}
            //if (Input.mousePosition.y <= 20)
            //{
            //    transform.Translate(Vector3.back * cameraMoveSpeed * Time.deltaTime, Space.World);
            //}
            //if (Input.mousePosition.y >= (Screen.height - 20))
            //{
            //    transform.Translate(Vector3.forward * cameraMoveSpeed * Time.deltaTime, Space.World);
            //}
            
            //currentY -= mouseWheel * cameraScrollSpeed * Time.deltaTime;
            //currentZ += mouseWheel * cameraScrollSpeed * Time.deltaTime;
            currentX = Mathf.Clamp(currentX, minXPos, maxXPos);
            currentY = Mathf.Clamp(currentY, minYPos, maxYPos);
            currentZ = Mathf.Clamp(currentZ, minZPos - currentY, maxZPos - currentY);

            transform.position = new Vector3(currentX, currentY, currentZ);
        }
    }
}
