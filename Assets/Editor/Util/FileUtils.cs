using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class FileUtils
{
    public static string GetFullPath(string path)
    {
        return Normalize(Path.GetFullPath(path));
    }

    public static string PathCombine(string root, params string[] paths)
    {
        StringBuilder path = new StringBuilder(root);

        var iter = paths.GetEnumerator();
        while (iter.MoveNext())
        {
            string subPath = (string)iter.Current;
            if (path.Length < 1)
            {
                path.Append(subPath);
            }
            else
            {
                if (path[path.Length - 1] != '/') path.Append('/');
                if (!string.IsNullOrEmpty(subPath)) path.Append(subPath);
            }
        }

        return path.ToString();
    }

    public static bool IsDirectory(string path)
    {
        if (path.Contains('.'))
            return false;
        return true;
    }

    public static string Normalize(string path)
    {
        return path.Replace('\\', '/');
    }

    public static void DeleteFile(string filename)
    {
        File.Delete(filename);
    }

    public static void RenameFile(string filename, string newFilename)
    {
        File.Move(filename, newFilename);
    }

    public static long GetLastWriteTime(string filePath)
    {
        DateTime dt = new DateTime(1970, 1, 1).ToLocalTime();
        DateTime data = File.GetLastWriteTime(filePath);
        TimeSpan ts = data - dt;
        return (long)ts.TotalSeconds;
    }

    public static bool IsDirectoryExist(string path)
    {
        return Directory.Exists(path);
    }
    public static bool IsFileExist(string filename)
    {
        return File.Exists(filename);
    }

    public static void ClearDirectory(string path)
    {
        if (IsDirectoryExist(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir != null)
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }
                foreach (var subDir in dir.GetDirectories())
                {
                    subDir.Delete(true);
                }
            }
        }
    }

    public static void CopyFile(string srcFilename, string dstFilename)
    {
        int split = dstFilename.LastIndexOf("/", System.StringComparison.Ordinal);
        string pathFolder = dstFilename.Substring(0, split);
        if (!Directory.Exists(pathFolder))
        {
            Directory.CreateDirectory(pathFolder);
        }

        if (IsFileExist(dstFilename))
        {
            File.Delete(dstFilename);
        }

        File.Copy(srcFilename, dstFilename);
    }

    public static void DeleteDirectory(string path)
    {
        if (IsDirectoryExist(path))
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            dir.Delete(true);
        }
    }

    public static void DeleteAppData()
    {
        DeleteDirectory(Application.persistentDataPath);
    }

    public static string GetDirectoryName(string path)
    {
        return Normalize(Path.GetDirectoryName(path));
    }

    public static bool CreateDirectory(string path)
    {
        if (!IsDirectoryExist(path))
        {
            return Directory.CreateDirectory(path) != null;
        }
        return true;
    }

    public static void RemoveDirectory(string path)
    {
        Directory.Delete(path, true);
    }

    public static bool IsAbsolutePath(string path)
    {
#if UNITY_EDITOR_WIN
        if (path.Length > 2
            && ((path[0] >= 'a' && path[0] <= 'z') || (path[0] >= 'A' && path[0] <= 'Z'))
            && path[1] == ':')
        {
            return true;
        }
        else
        {
            return false;
        }
#elif UNITY_EDITOR_OSX
        if (!string.IsNullOrEmpty(path) && path[0] == '/')
        {
            return true;
        }
        else
        {
            return false;
        }
#else
            return false;
#endif
    }
    public static void CheckDirectory(string filename)
    {
        if (!IsAbsolutePath(filename))
        {
            filename = GetFullPath(filename);
        }
        string dirname = Path.GetDirectoryName(filename);
        if (!IsDirectoryExist(dirname))
        {
            Directory.CreateDirectory(dirname);
        }
    }
    public static long GetFileSize(string filename)
    {
        FileInfo info = new FileInfo(filename);
        if (info != null)
        {
            return info.Length;
        }
        return 0;
    }
    public static string GetFileMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                StringBuilder result = new StringBuilder();
                for (int i = 0, len = hash.Length; i < len; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }
                return result.ToString();
            }
        }
    }

    public static string ReadTextFromFile(string filename)
    {
        return File.ReadAllText(filename);
    }

    public static void WriteTextToFile(string content, string filename, bool flush = true)
    {
        CheckDirectory(filename);
        File.WriteAllText(filename, content);
    }

    public static byte[] ReadBytesFromFile(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    public static void WriteBytesToFile(byte[] content, string filename, bool flush = true)
    {
        CheckDirectory(filename);
        File.WriteAllBytes(filename, content);
    }
}