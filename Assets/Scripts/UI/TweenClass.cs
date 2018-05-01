using UnityEngine;
using System.Collections;

namespace UGUITween
{
    [System.Serializable]
    public struct TweenerClass
    {
        /// <summary>
        /// 动画类型
        /// </summary>
        public ExeType AnimType;
        /// <summary>
        /// 坐标类型
        /// </summary>
        public CoordinateType AnimCoordType;
        /// <summary>
        /// 完成动画所需要的时间
        /// </summary>
        public float DurationTime;
        /// <summary>
        /// 开始动画需要延迟的时间
        /// </summary>
        public float DelayTime;
    }
    public enum ExeType
    {
        Once, Loop, PinpPong
    }
    public enum CoordinateType
    {
        Local,World
    }
}

