using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

public class RTSCamera : MonoBehaviour
{
    public float cameraMoveSpeed = 20;
    //public float cameraRotateSpeed = 100;
#if (UNITY_STANDALONE || UNITY_EDITOR)
    float cameraScrollSpeed = 4;
#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    float cameraScrollSpeed = 300;
#endif
    float cameraAnimSpeed = 0.5f;
    public bool cameraFollow = true;

    public GameObject cameraRange;

    Transform min;
    Transform max;

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

        min = cameraRange.transform.Find("Min");
        max = cameraRange.transform.Find("Max");

    }

    void LateUpdate()
    {
        
        var hor = Vector3.Cross(Vector3.up, transform.right);


#if (UNITY_STANDALONE || UNITY_EDITOR)
        float minY = 4f;
        float maxY = 5.5f;
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (!EventSystem.current.IsPointerOverGameObject() && cameraState == CameraState.idle)
        {
            if (Input.mousePosition.x <= 20)
            {
                transform.Translate(-transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.x >= (Screen.width - 20))
            {
                transform.Translate(transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y <= 20)
            {
                transform.Translate(hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y >= (Screen.height - 20))
            {
                transform.Translate(-hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            }

            if (mouseWheel > 0 && transform.position.y > minY)
            {
                //由远到近
                //transform.Translate(transform.forward * mouseWheel * cameraScrollSpeed * Time.deltaTime, Space.World);

                var endValue = transform.position + transform.forward * (mouseWheel / Mathf.Abs(mouseWheel)) * cameraScrollSpeed;

                var dis = endValue.y / Mathf.Cos(Mathf.Deg2Rad * 60);
                Vector3 center = endValue + transform.forward * dis;
                center = new Vector3(Mathf.Clamp(center.x, min.transform.position.x - 1, max.transform.position.x + 1), center.y, Mathf.Clamp(center.z, min.transform.position.z - 1, max.transform.position.z + 1));
                endValue = center - transform.forward * dis;
                transform.DOMove(endValue, cameraAnimSpeed).OnPlay(() => {
                    cameraState = CameraState.move;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    min.position -= new Vector3(1, 0, 1);
                    max.position += new Vector3(1, 0, 1);
                });
            }
            if (mouseWheel < 0 && transform.position.y < maxY)
            {
                //由近到远
                //transform.Translate(transform.forward * mouseWheel * cameraScrollSpeed * Time.deltaTime, Space.World);

                var endValue = transform.position + transform.forward * (mouseWheel / Mathf.Abs(mouseWheel)) * cameraScrollSpeed;

                var dis = endValue.y / Mathf.Cos(Mathf.Deg2Rad * 60);
                Vector3 center = endValue + transform.forward * dis;
                center = new Vector3(Mathf.Clamp(center.x, min.transform.position.x + 1, max.transform.position.x - 1), center.y, Mathf.Clamp(center.z, min.transform.position.z + 1, max.transform.position.z - 1));
                endValue = center - transform.forward * dis;
                transform.DOMove(endValue, cameraAnimSpeed).OnPlay(() => {
                    cameraState = CameraState.move;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    min.position += new Vector3(1, 0, 1);
                    max.position -= new Vector3(1, 0, 1);
                });
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateCamera(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateCamera(false);
            }

            AxisClamp();
        }

#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        float minY = 4f;
        float maxY = 5.5f;
        if (!(Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) && cameraState == CameraState.idle)
        {
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
                if (deltaMagnitudeDiff > 0 && transform.position.y < maxY)
                {
                    transform.Translate(-transform.forward * deltaMagnitudeDiff * cameraScrollSpeed * 0.02f * Time.deltaTime, Space.World);
                }
                if (deltaMagnitudeDiff < 0 && transform.position.y > minY)
                {
                    transform.Translate(-transform.forward * deltaMagnitudeDiff * cameraScrollSpeed * 0.02f * Time.deltaTime, Space.World);
                }
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                transform.Translate(-transform.right * Input.GetTouch(0).deltaPosition.x / 60 * cameraMoveSpeed * Time.deltaTime, Space.World);
                transform.Translate(hor * Input.GetTouch(0).deltaPosition.y / 60 * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            AxisClamp();
        }
#endif
    }

    public void FollowTarget(Vector3 targetPosition)
    {
        if (cameraFollow && cameraState != CameraState.rotate)
        {
            targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, min.transform.position.x, max.transform.position.x), targetPosition.y, Mathf.Clamp(targetPosition.z, min.transform.position.z, max.transform.position.z));
            var position = targetPosition - transform.forward * (transform.position.y / Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.x));
            if (cameraMove != null)
                cameraMove.Kill();
            cameraMove = transform.DOMove(position, cameraAnimSpeed).OnPlay(() => {
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
                anchor.transform.DORotate(new Vector3(0, anchor.transform.rotation.eulerAngles.y + 90, 0), cameraAnimSpeed).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
            else
            {
                anchor.transform.DORotate(new Vector3(0, anchor.transform.eulerAngles.y - 90, 0), cameraAnimSpeed).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
        }
    }

    public bool AxisClamp()
    {
        var dis = transform.position.y / Mathf.Cos(Mathf.Deg2Rad * 60);
        Vector3 center = transform.position + transform.forward * dis;

        
        var minX = min.position.x;
        var maxX = max.position.x;
        var minZ = min.position.z;
        var maxZ = max.position.z;

        if (center.z < minZ)
        {
            center.z = minZ;
            transform.position = center - transform.forward * dis;
        }
        if (center.z > maxZ)
        {
            center.z = maxZ;
            transform.position = center - transform.forward * dis;
        }
        if (center.x < minX)
        {
            center.x = minX;
            transform.position = center - transform.forward * dis;
        }
        if (center.x > maxX)
        {
            center.x = maxX;
            transform.position = center - transform.forward * dis;
        }
       
        return true;
    }
}

