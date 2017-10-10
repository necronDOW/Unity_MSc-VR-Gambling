using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Helper
{
    public static string[] ReadFile(string directory, int splitLimiter)
    {
        TextAsset txtFile = (TextAsset)Resources.Load(directory);

        if (!txtFile)
            return null;
        
        string[] lines = txtFile.text.Split('\n');
        int lineCount = ValidLineCount(lines);

        if (splitLimiter == 0)
            splitLimiter = lines[0].Split(',').Length;
        else splitLimiter = Math.Min(splitLimiter, lines[0].Split(',').Length);

        string[] output = new string[lineCount * splitLimiter];
        int ptr = 0;

        for (int i = 0; i < lineCount; i++)
        {
            string[] subStr = lines[i].Split(',');
            for (int j = 0; j < splitLimiter; j++)
                output[ptr++] = subStr[j];
        }

        return output;
    }

    private static int ValidLineCount(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                return i - 1;
        }

        return lines.Length;
    }

    public static Material[] MaterialsFromExisting(Material existing, float stepX, float stepY, int maximum = int.MaxValue)
    {
        return MaterialsFromExisting(existing, existing.mainTextureOffset, existing.mainTextureScale, stepX, stepY);
    }

    public static Material[] MaterialsFromExisting(Material existing, Vector2 start, Vector2 cellSize, float stepX, float stepY, int maximum = int.MaxValue)
    {
        int cellCountX = Mathf.RoundToInt((1.0f - start.x) / stepX);
        int cellCountY = Mathf.RoundToInt((start.y + stepY) / stepY);
        int materialsLength = cellCountX * cellCountY;

        Material[] materials = new Material[(materialsLength < maximum) ? materialsLength : maximum];
        int counter = 0;

        for (int j = 0; j < cellCountY; j++)
        {
            for (int i = 0; i < cellCountX; i++)
            {
                materials[counter] = new Material(existing);
                materials[counter].mainTextureOffset = new Vector2(start.x + (stepX * i), start.y - (stepY * j));
                materials[counter].mainTextureScale = cellSize;

                if (++counter >= materials.Length)
                    return materials;
            }
        }

        return materials;
    }

    public static void SwitchToScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
    }
}