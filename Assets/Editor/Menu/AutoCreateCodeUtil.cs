using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class AutoCreateCodeUtil
{
    public static void WriteNamespace(StringBuilder sb, params string[] @namespace)
    {
        for (int i = 0; i < @namespace.Length; i++)
        {
            sb.Append($"using {@namespace[i]};\n");
        }
    }

    public static void StartWriteNamespace(StringBuilder sb, string @namespace)
    {
        sb.Append($"namespace {@namespace}\n");
        sb.Append("{\n");
    }

    public static void EndWriteNamespace(StringBuilder sb)
    {
        sb.Append("}");
    }

    public static void StartWriteClass(StringBuilder sb, string className, bool isPartial, params string[] parents)
    {
        if (isPartial)
            sb.Append($"\tpublic partial class {className} : ");
        else
            sb.Append($"\tpublic class {className} : ");
        for (int i = 0; i < parents.Length; i++)
        {
            sb.Append($"{parents[i]}");
            if (i != parents.Length - 1)
            {
                sb.Append(", ");
            }
            else
                sb.Append("\n");
        }
        sb.Append("\t{\n");
    }

    public static void StartWriteInterface(StringBuilder sb, string className, bool isPartial, params string[] parents)
    {
        if (isPartial)
            sb.Append($"\tpublic interface class {className} : ");
        else
            sb.Append($"\tpublic interface {className} : ");
        for (int i = 0; i < parents.Length; i++)
        {
            sb.Append($"{parents[i]}");
            if (i != parents.Length - 1)
            {
                sb.Append(", ");
            }
            else
                sb.Append("\n");
        }
        sb.Append("\t{\n");
    }

    public static void EndWriteClass(StringBuilder sb)
    {
        sb.Append("\t}\n");
    }

    public static void EndWriteInterface(StringBuilder sb)
    {
        sb.Append("\t}\n");
    }

    public static void WriteField(StringBuilder sb, string field)
    {
        sb.Append("\t\t" + field + "\n");
    }

    public static void WriteFunction(StringBuilder sb, string function)
    {
        sb.Append($"\t\t{function}\n");
        sb.Append("\t\t{\n\n");
        sb.Append("\t\t}\n");
    }
}
