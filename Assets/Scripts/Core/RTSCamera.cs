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
    //private Vector3 lastPosition;   //超出前的位置保留
    public  Vector3 LRRecord;       //超出时的left/right方向记录，值为1说明禁止继续向该方向移动。
    public Vector3 FBRecord;       //超出时的forward/back方向记录
    void LateUpdate()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //lastPosition = transform.position;
            if (Input.mousePosition.x <= 20 && LRRecord != Vector3.left)
            {
                transform.Translate(Vector3.left * cameraMoveSpeed * Time.deltaTime);
                LRRecord = Vector3.zero;
            }
            if (Input.mousePosition.x >= (Screen.width - 20) && LRRecord != Vector3.right)
            {
                transform.Translate(Vector3.right * cameraMoveSpeed * Time.deltaTime);
                LRRecord = Vector3.zero;
            }
            if (Input.mousePosition.y <= 20 && FBRecord != Vector3.back)
            {
                transform.Translate(Vector3.back * cameraMoveSpeed * Time.deltaTime, Space.World);
                FBRecord = Vector3.zero;
            }
            if (Input.mousePosition.y >= (Screen.height - 20) && FBRecord != Vector3.forward)
            {
                transform.Translate(Vector3.forward * cameraMoveSpeed * Time.deltaTime, Space.World);
                FBRecord = Vector3.zero;
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


        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit))
        //{
            
        //}
        //else
        //{
        //    RaycastHit hitinfo;
        //    //左前射线检测不到且左后射线检测不到，则不允许继续往左边移动。
        //    if(!Physics.Raycast(transform.position + Vector3.left + Vector3.forward, transform.forward, out hitinfo) && !Physics.Raycast(transform.position + Vector3.left + Vector3.back, transform.forward, out hitinfo))
        //    {
        //        LRRecord = Vector3.left;
        //        transform.position = new Vector3(lastPosition.x, transform.position.y, transform.position.z);
        //    }
        //    //右前射线检测不到且右后射线检测不到，则不允许继续往左边移动。
        //    if (!Physics.Raycast(transform.position + Vector3.right + Vector3.forward, transform.forward, out hitinfo) && !Physics.Raycast(transform.position + Vector3.right + Vector3.back, transform.forward, out hitinfo))
        //    {
        //        LRRecord = Vector3.right;
        //        transform.position = new Vector3(lastPosition.x, transform.position.y, transform.position.z);
        //    }
        //    //左前射线检测不到且右前射线检测不到，则不允许继续往左边移动。
        //    if (!Physics.Raycast(transform.position + Vector3.forward + Vector3.left, transform.forward, out hitinfo) && !Physics.Raycast(transform.position + Vector3.forward + Vector3.right, transform.forward, out hitinfo))
        //    {
        //        FBRecord = Vector3.forward;
        //        transform.position = new Vector3(transform.position.x, transform.position.y, lastPosition.z);
        //    }
        //    //左前射线检测不到且左后射线检测不到，则不允许继续往左边移动。
        //    if (!Physics.Raycast(transform.position + Vector3.back + Vector3.left, transform.forward, out hitinfo) && !Physics.Raycast(transform.position + Vector3.back + Vector3.right, transform.forward, out hitinfo))
        //    {
        //        FBRecord = Vector3.back;
        //        transform.position = new Vector3(transform.position.x, transform.position.y, lastPosition.z);
        //    }
        //}
    }
}
