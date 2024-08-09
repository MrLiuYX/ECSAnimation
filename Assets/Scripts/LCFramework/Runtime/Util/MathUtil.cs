using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;
using Random = UnityEngine.Random;

public static class MathUtil
{
    /// <summary>
    /// 数字换算k m b t 保留2位小数
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string Numdispose(long num)
    {
        string[] symbol = { "K", "M", "B", "T" };

        string value = string.Empty;
        double d ;

        if (num.ToString().Length > 3 && num.ToString().Length < 7)
        {
            d = num / 1000d;

            value = symbol[0];
        }
        else if (num.ToString().Length > 6 && num.ToString().Length < 10)
        {
            d = num / 1000000d;

            value = symbol[1];
        }
        else if (num.ToString().Length > 9 && num.ToString().Length < 13)
        {
            d = num / 1000000000d;

            value = symbol[2];
        }
        else if (num.ToString().Length > 12 && num.ToString().Length < 16)
        {
            d = num / 1000000000000d;

            value = symbol[3];
        }
        else if(num.ToString().Length > 15)
        {
            throw new Exception("kmb系统不支持！！！");
        }
        else
        {
            d = num;
        }

        //判断是否有小数位
        if (d % 1 != 0)
        {
            d = Math.Round(d, 2);
            string endV = d.ToString();

            if (endV.Length>4)
            {
                endV = endV.Substring(0, 4).TrimEnd('0').TrimEnd('.');
            }

            value = endV + value;
        }
        else
        {
            value = d.ToString() + value;
        }

        return value;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower(int[] powers)
    {
        if (powers.Length == 0) return 0;
        int index = 0;
        var totalPower = 0;
        for (int i = 0; i < powers.Length; i++)
        {
            totalPower += powers[i];
        }        
        var currentPower = Random.Range(0, totalPower+1);
        while (currentPower > powers[index])
        {
            currentPower -= powers[index++];
        }
        return index;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower(List<int> powers)
    {
        if (powers == null || powers.Count == 0) return -1;

        int index = 0;
        var totalPower = 0;
        for (int i = 0; i < powers.Count; i++)
        {
            totalPower += powers[i];
        }
        var currentPower = Random.Range(0, totalPower + 1);
        while (currentPower > powers[index])
        {
            currentPower -= powers[index++];
        }
        return index;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static List<int> RandomPower(int[] powerArray, int chooseCount)
    {
        var alreadChoose = ListPool<int>.Get();
        var powers = ListPool<int>.Get();

        for (int i = 0; i < powerArray.Length; i++)
        {
            powers.Add(powerArray[i]);
        }

        if (chooseCount > powers.Count)
        {
            Debug.LogError($"权重随机选择数目大于内容");
            return null;
        }

        for (int time = 0; time < chooseCount; time++)
        {
            int index;
            do
            {
                index = 0;
                var totalPower = 0;
                for (int i = 0; i < powers.Count; i++)
                {
                    totalPower += powers[i];
                }
                var currentPower = Random.Range(0, totalPower + 1);
                while (currentPower > powers[index])
                {
                    currentPower -= powers[index++];
                }
            } while (alreadChoose.Contains(index));
            powers.RemoveAt(index);
            alreadChoose.Add(index);
        }
        ListPool<int>.Release(powers);
        return alreadChoose;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower(int[] powers, Func<int, bool> condition)
    {
        int index = 0;
        int randomTime = 0;
        do
        {
            randomTime++;
            if (randomTime >= 1000)
            {
                Debug.LogError("Random Error");
                return -1;
            }

            index = 0;
            var totalPower = 0;
            for (int i = 0; i < powers.Length; i++)
            {
                totalPower += powers[i];
            }
            var currentPower = Random.Range(0, totalPower + 1);
            while (currentPower > powers[index])
            {
                currentPower -= powers[index++];
            }
        } while (!condition(index));
        return index;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower<T1>(int[] powers, Func<int, T1, bool> condition, T1 param1)
    {
        int index = 0;
        int randomTime = 0;
        do
        {
            randomTime++;
            if (randomTime >= 1000)
            {
                Debug.LogError("Random Error");
                return -1;
            }

            index = 0;
            var totalPower = 0;
            for (int i = 0; i < powers.Length; i++)
            {
                totalPower += powers[i];
            }
            var currentPower = Random.Range(0, totalPower + 1);
            while (currentPower > powers[index])
            {
                currentPower -= powers[index++];
            }
        } while (!condition(index, param1));        
        return index;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower<T1, T2>(int[] powers, Func<int, T1, T2, bool> condition, T1 param1, T2 param2)
    {
        int index = 0;
        int randomTime = 0;
        do
        {
            randomTime++;
            if (randomTime >= 1000)
            {
                Debug.LogError("Random Error");
                return -1;
            }

            index = 0;
            var totalPower = 0;
            for (int i = 0; i < powers.Length; i++)
            {
                totalPower += powers[i];
            }
            var currentPower = Random.Range(0, totalPower + 1);
            while (currentPower > powers[index])
            {
                currentPower -= powers[index++];
            }
        } while (!condition(index, param1, param2));
        return index;
    }

    /// <summary>
    /// 权重随机
    /// </summary>
    /// <param name="randoms"></param>
    /// <returns></returns>
    public static int RandomPower<T1, T2, T3>(int[] powers, Func<int, T1, T2, T3, bool> condition, T1 param1, T2 param2, T3 param3)
    {
        int index = 0;
        int randomTime = 0;
        do
        {
            randomTime++;
            if (randomTime >= 1000)
            {
                Debug.LogError("Random Error");
                return -1;
            }

            index = 0;
            var totalPower = 0;
            for (int i = 0; i < powers.Length; i++)
            {
                totalPower += powers[i];
            }
            var currentPower = Random.Range(0, totalPower + 1);
            while (currentPower > powers[index])
            {
                currentPower -= powers[index++];
            }
        } while (!condition(index, param1, param2, param3));
        return index;
    }
}
