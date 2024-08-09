using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Native;
using UnityEngine.UIElements;
public static class EnumExtension
{
    /// <summary>
    /// (Min, Max, Count)
    /// </summary>
    private static Dictionary<Type, (int, int, int)> MaxMinCollection = new Dictionary<Type, (int, int, int)>();

    public static int GetCount(Type _enum)
    {
        if (!typeof(Enum).IsAssignableFrom(_enum))
        {
            Debug.LogError($"The type {_enum.GetType().Name} is not a enum!");
            return -1;
        }

        if (MaxMinCollection.ContainsKey(_enum))
        {
            return MaxMinCollection[_enum].Item3;
        }

        var values = Enum.GetValues(_enum);
        var max = (int)values.GetValue(0);
        var min = (int)values.GetValue(0);
        var count = values.Length;

        for (int i = 1; i < values.Length; i++)
        {
            var value = (int)values.GetValue(i);
            if (max < value)
            {
                max = value;
            }

            if (min > value)
            {
                min = value;
            }
        }
        MaxMinCollection.Add(_enum, (min, max, count));
        return count;
    }

    public static int GetMaxValue(Type _enum)
    {
        if (!typeof(Enum).IsAssignableFrom(_enum))
        {
            Debug.LogError($"The type {_enum.GetType().Name} is not a enum!");
            return -1;
        }

        if (MaxMinCollection.ContainsKey(_enum))
        {
            return MaxMinCollection[_enum].Item1;
        }

        var values = Enum.GetValues(_enum);
        var max = (int)values.GetValue(0);
        var min = (int)values.GetValue(0);
        var count = values.Length;

        for (int i = 1; i < values.Length; i++)
        {
            var value = (int)values.GetValue(i);
            if (max < value)
            {
                max = value;
            }

            if (min > value)
            {
                min = value;
            }
        }
        MaxMinCollection.Add(_enum, (min, max, count));
        return max;
    }

    public static int GetMinValue(Type _enum)
    {
        if (!typeof(Enum).IsAssignableFrom(_enum))
        {
            Debug.LogError($"The type {_enum.GetType().Name} is not a enum!");
            return -1;
        }

        if (MaxMinCollection.ContainsKey(_enum))
        {
            return MaxMinCollection[_enum].Item1;
        }

        var values = Enum.GetValues(_enum);
        var max = (int)values.GetValue(0);
        var min = (int)values.GetValue(0);
        var count = values.Length;

        for (int i = 1; i < values.Length; i++)
        {
            var value = (int)values.GetValue(i);
            if (max < value)
            {
                max = value;
            }

            if (min > value)
            {
                min = value;
            }
        }
        MaxMinCollection.Add(_enum, (min, max, count));
        return min;
    }

    public static int ToInt(this System.Enum _enum)
    {
        return _enum.GetHashCode();
    }
}

