using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class RTSCamera : MonoBehaviour
{
    public float cameraMoveSpeed = 10;
    //public float cameraRotateSpeed = 100;
    public float cameraScrollSpeed = 300;

    public bool cameraFollow = true;
    [Range(0, 6)]
    [SerializeField]
    float horizontal = 3f;
    float horizontalMin = 0f;
    float horizontalMax = 6f;
    [Range(0, 6)]
    [SerializeField]
    float vertical = 3f;
    float verticalMin = 0f;
    float verticalMax = 6f;

    enum CameraAxis
    {
        hor,
        ver,
        z
    }

    public enum CameraState
    {
        idle,
        move,
        rotate,
    }
    
    [SerializeField]
    private CameraState cameraState = CameraState.idle;

    private GameObject anchor;

    Tween cameraMove;

    private void Start()
    {
        anchor = new GameObject("CameraAnchor");

        DebugLogPanel.GetInstance().Log(Application.streamingAssetsPath + "/XML/gameData.xml");
    }

    void LateUpdate()
    {
        //float currentX;
        //float currentY;
        //float currentZ;

#if (UNITY_IOS || UNITY_ANDROID)
        //if (Input.touchCount == 2)
        //{
        //    // Store both touches.
        //    Touch touchZero = Input.GetTouch(0);
        //    Touch touchOne = Input.GetTouch(1);

        //    // Find the position in the previous frame of each touch.
        //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        //    // Find the magnitude of the vector (the distance) between the touches in each frame.
        //    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //    // Find the difference in the distances between each frame.
        //    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        //    deltaMagnitudeDiff = Mathf.Clamp(deltaMagnitudeDiff, -1, 1);
        //    // Otherwise change the field of view based on the change in distance between the touches.
        //    camera.fieldOfView += deltaMagnitudeDiff * cameraScrollSpeed / 50 * Time.deltaTime;
        //    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10, 50);
        //    //currentY += deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
        //    //currentZ -= deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
        //}
        //else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        //{
        //    currentX -= Input.GetTouch(0).deltaPosition.x / 80.0f * cameraMoveSpeed * Time.deltaTime;
        //    currentZ -= Input.GetTouch(0).deltaPosition.y / 80.0f * cameraMoveSpeed * Time.deltaTime;
        //}
#elif (UNITY_STANDALONE || UNITY_EDITOR)
        float minY = 5f;
        float maxY = 7f;
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (!EventSystem.current.IsPointerOverGameObject() && cameraState == CameraState.idle)
        {
            //var hor = Vector3.Cross(Vector3.up, transform.right);

            //if (Input.mousePosition.x <= 20 && AxisClamp(CameraAxis.hor, false))
            //{
            //    transform.Translate(-transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            //    horizontal -= Time.deltaTime * 10;
            //}
            //if (Input.mousePosition.x >= (Screen.width - 20) && AxisClamp(CameraAxis.hor, true))
            //{
            //    transform.Translate(transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            //    horizontal += Time.deltaTime * 10;
            //}
            //if (Input.mousePosition.y <= 20 && AxisClamp(CameraAxis.ver, true))
            //{
            //    transform.Translate(hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            //    vertical += Time.deltaTime * 10;
            //}
            //if (Input.mousePosition.y >= (Screen.height - 20) && AxisClamp(CameraAxis.ver, false))
            //{
            //    transform.Translate(-hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            //    vertical -= Time.deltaTime * 10;
            //}
            if (mouseWheel > 0 && transform.position.y > minY)
            {
                transform.Translate(transform.forward * mouseWheel * cameraScrollSpeed * Time.deltaTime, Space.World);
                
            }
            if (mouseWheel < 0 && transform.position.y < maxY)
            {
                transform.Translate(transform.forward * mouseWheel * cameraScrollSpeed * Time.deltaTime, Space.World);
                
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateCamera(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateCamera(false);
            }
        }
#endif
    }

    void Update()
    {
        
    }

    bool AxisClamp(CameraAxis a, bool max)
    {
        switch (a)
        {
            case CameraAxis.hor:
                horizontal = Mathf.Clamp(horizontal, horizontalMin, horizontalMax);
                if (max)
                {
                    return horizontal != horizontalMax;
                }
                else
                {
                    return horizontal != horizontalMin;
                }
            case CameraAxis.ver:
                vertical = Mathf.Clamp(vertical, verticalMin, verticalMax);
                if (max)
                {
                    return vertical != verticalMax;
                }
                else
                {
                    return vertical != verticalMin;
                }
        }
        return false;
    }


    public void FollowTarget(Vector3 targetPosition)
    {
        if (cameraFollow && cameraState != CameraState.rotate)
        {
            var position = targetPosition - transform.forward * (transform.position.y / Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.x));
            if (cameraMove != null)
                cameraMove.Kill();
            cameraMove = transform.DOMove(position, 0.5f).OnPlay(() => {
                cameraState = CameraState.move;
            }).OnComplete(() => {
                cameraState = CameraState.idle;
            });
        }
    }

    public void RotateCamera(bool left)
    {
        if(cameraState == CameraState.idle)
        {
            anchor.transform.position = transform.position.y * 2 * transform.forward + transform.position;
            transform.SetParent(anchor.transform);
            if (left)
            {
                anchor.transform.DORotate(new Vector3(0, anchor.transform.rotation.eulerAngles.y + 90, 0), 0.5f).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
            else
            {
                anchor.transform.DORotate(new Vector3(0, anchor.transform.eulerAngles.y - 90, 0), 0.5f).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
        }
    }
}
