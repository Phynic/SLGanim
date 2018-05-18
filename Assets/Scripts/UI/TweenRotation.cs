using UnityEngine;
using System.Collections;
using UGUITween;

public class TweenRotation : MonoBehaviour {
    [SerializeField]
    private Vector3 RotationFrom;
    [SerializeField]
    private Vector3 RotationTo;
    [SerializeField]
    private TweenerClass Tweener;

    private float time_counter;
    private RectTransform this_rectTrans;
    private bool is_runAnim = false;
    private bool is_runOnce = true;
    private Quaternion this_transRotation = new Quaternion();
    private float rotaSpeed;
    private bool PingPong_end = false;

    private Vector3 minVec;
    private Vector3 maxVec;
    /// <summary>
    /// 设定最小的距离，当当前的旋转向量距离目标向量小于这个值的时候就判断为到达目标向量
    /// </summary>
    private float minDistance=2.0f;      
    Vector3 disVec;
    // Use this for initialization
    void Start () {
        this_rectTrans = gameObject.GetComponent<RectTransform>();

        this_transRotation.eulerAngles = RotationFrom;
        //计算各个分量角度的实际角度差
        float dis_anglex = Mathf.Abs(RotationFrom.x - RotationTo.x);
        float dis_angley = Mathf.Abs(RotationFrom.y - RotationTo.y);
        float dis_anglez = Mathf.Abs(RotationFrom.z - RotationTo.z);
        disVec = new Vector3(dis_anglex,dis_angley,dis_anglez);
        rotaSpeed = 1 / Tweener.DurationTime;
       // Debug.Log("dis:"+disVec.magnitude);

        //比较出RotationFrom到RotationTo中各分量之间的的大小
        minVec.x = Mathf.Min(RotationFrom.x, RotationTo.x);
        minVec.y = Mathf.Min(RotationFrom.y, RotationTo.y);
        minVec.z = Mathf.Min(RotationFrom.z, RotationTo.z);

        maxVec.x = Mathf.Max(RotationFrom.x, RotationTo.x);
        maxVec.y = Mathf.Max(RotationFrom.y, RotationTo.y);
        maxVec.z = Mathf.Max(RotationFrom.z, RotationTo.z);

    }
	
	// Update is called once per frame
	void Update () {
        if (is_runOnce)
        {
            time_counter += Time.deltaTime;
            if (time_counter > Tweener.DelayTime)
            {
                time_counter = 0;
                is_runOnce = false;
                is_runAnim = true;
            }
        }
        if (is_runAnim)
        {
            Anim_Run();
        }
        else
        {

        }

    }
    
    private bool Reach_TargetVec(Vector3 targetVec)
    {
        bool isReach = false;

        float angelX = Mathf.Abs(this_transRotation.eulerAngles.x - targetVec.x);
        float angleY = Mathf.Abs(this_transRotation.eulerAngles.y - targetVec.y);
        float angleZ = Mathf.Abs(this_transRotation.eulerAngles.z - targetVec.z);

        Vector3 disVecTemp = new Vector3(angelX, angleY, angleZ);
        float dis = disVecTemp.magnitude;
        //Debug.Log(dis);
        if (dis<minDistance)
        {
            isReach = true;
        }

        return isReach;
    }
    private void Anim_Run()
    {
        switch (Tweener.AnimType)
        {
            case ExeType.Once:
                {
                    this_transRotation.eulerAngles += disVec * rotaSpeed * Time.deltaTime;

                    //一次的情况在到达目标角度向量时候就停止
                    if (Reach_TargetVec(RotationTo))
                    {
                        is_runAnim = false;
                    }
                }
                break;
            case ExeType.Loop:
                {
                    this_transRotation.eulerAngles += disVec * rotaSpeed * Time.deltaTime;

                    //如果距离目标到达目标最小距离就进行下一次
                    if (Reach_TargetVec(RotationTo))
                    {
                        this_transRotation.eulerAngles = RotationFrom;
                    }

                }
                break;
            case ExeType.PinpPong:
                {
                    if (!PingPong_end)
                    {
                        this_transRotation.eulerAngles += disVec * rotaSpeed * Time.deltaTime;
                    }
                    else
                    {
                        this_transRotation.eulerAngles -= disVec * rotaSpeed * Time.deltaTime;
                    }

                    if (Reach_TargetVec(RotationTo))
                    {
                        PingPong_end = true;
                    }
                    else if (Reach_TargetVec(RotationFrom))
                    {
                        PingPong_end = false;
                    }
                }
                break;
        }
      
        if(Tweener.AnimCoordType==CoordinateType.Local)
        {
            this_rectTrans.localRotation = this_transRotation;
        }
        else
        {
            this_rectTrans.rotation = this_transRotation;
        }
    }
}
