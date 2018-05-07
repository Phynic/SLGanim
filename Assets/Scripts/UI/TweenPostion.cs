using UnityEngine;
using System.Collections;
using UGUITween;
public class TweenPostion : MonoBehaviour {

    /// <summary>
    /// 动画的初始位置
    /// </summary>
    [SerializeField]
    private Vector3 PostionFrom; 
    /// <summary>
    /// 动画的终点位置
    /// </summary>
    [SerializeField]
    private Vector3 PostionTo;
    /// <summary>
    /// 动画参数
    /// </summary>
    [SerializeField]
    private TweenerClass Tweener;

    private float time_counter;
    private RectTransform this_rectTrans;
    private bool is_runAnim = false;
    private bool is_runOnce = true;
    private Vector3 this_transPos;
    private Vector3 disVec;
    private float moveSpeed;
    private bool PingPong_end = false;

    // Use this for initialization
    void Start () {
        this_rectTrans = gameObject.GetComponent<RectTransform>();

        this_transPos = PostionFrom;
        disVec = PostionTo - PostionFrom;
        moveSpeed = disVec.magnitude / Tweener.DurationTime;
       // Debug.Log("dis:"+ disVec.magnitude + "moveSpeed:"+moveSpeed);

    }
	
	// Update is called once per frame
	void Update () {
        if (is_runOnce)
        {
            time_counter += Time.deltaTime;
            if (time_counter > Tweener.DelayTime)
            {
                is_runAnim = true;
                is_runOnce = false;
                time_counter = 0;
            }
        }

        if(is_runAnim)
        {
             Anim_Run();
        }
	    

	}
    private void Anim_Run()
    {
        switch(Tweener.AnimType)
        {
            case ExeType.Once:
                {
                    //  this_transPos
                    this_transPos += (disVec * moveSpeed * Time.deltaTime)/disVec.magnitude;
                    if (this_transPos == PostionTo)
                    {
                        is_runAnim = false;
                    }
                }
                break;
            case ExeType.Loop:
                {
                    if(this_transPos==PostionTo)
                    {
                        this_transPos = PostionFrom;
                    }
                    else
                    {
                        this_transPos += (disVec * moveSpeed * Time.deltaTime) / disVec.magnitude;
                    }

                }break;
            case ExeType.PinpPong:
                {
                    if (this_transPos == PostionTo)
                    {
                        PingPong_end = true;
                    }
                    else if(this_transPos==PostionFrom)
                    {
                        PingPong_end = false;
                    }
                    if(PingPong_end)
                    {
                        this_transPos -= (disVec * moveSpeed * Time.deltaTime) / disVec.magnitude;
                    }
                    else
                    {
                        this_transPos += (disVec * moveSpeed * Time.deltaTime) / disVec.magnitude;
                    }
                }
                break;
        }

        //限制坐标值的大小，这样可以使用==来直接判断位置到达的地方
        this_transPos.x = Mathf.Clamp(this_transPos.x, PostionFrom.x, PostionTo.x);
        this_transPos.y = Mathf.Clamp(this_transPos.y, PostionFrom.y, PostionTo.y);
        this_transPos.z = Mathf.Clamp(this_transPos.z, PostionFrom.z, PostionTo.z);

        //先判断坐标类型
        if (Tweener.AnimCoordType == CoordinateType.Local)
        {
            this_rectTrans.localPosition = this_transPos;
        }
        else
        {
            this_rectTrans.position = this_transPos;
        }
    }
}

