using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeStamp
{
    public static long GetUnixTimestampMilliseconds()
    {
        DateTime currentTime = DateTime.Now.ToUniversalTime();

        long unixTimestamp =
       (long)(currentTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        return unixTimestamp; // 밀리초 단위
    }
}
