using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Native;

public static class StringExtension 
{
    public static string ToLanguage(this string key)
    {
        return Native.ConfigManager.Instance.GetKey(key);
    }

    public static int GetStrCount(this string src, string matchStr)
    {
        int count = 0;
        string temp = string.Empty;
        for (int i = 0; i < src.Length; i++)
        {
            temp += src[i];
            if (temp.Contains(matchStr))
            {
                count++;
                temp = string.Empty;
            }
        }

        return count;
    }
}
