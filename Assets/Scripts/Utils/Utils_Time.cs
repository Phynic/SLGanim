using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils_Time
{
    public static string GenerateTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public static string StampToDateTime(string timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
        long mTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(mTime);
        //Debug.Log("\n 当前时间为：" + startTime.Add(toNow).ToString("yyyy年MM月dd日 HH:mm:ss"));
        return startTime.Add(toNow).ToString("yyyy年MM月dd日 HH:mm:ss");
    }
}
