#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


[CustomEditor(typeof(MapDivider))]
public class MapDividerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapDivider myScript = (MapDivider)target;
        if (GUILayout.Button("Divide By Rooms"))
        {
            myScript.DivideMapByRoom();
        }
        if (GUILayout.Button("Divide By Sectors"))
        {
            myScript.DivideMapBySector();
        }
    }
}

public class MapDivider : MonoBehaviour
{
    public Texture2D mapIn;
    public int TileSizeX;
    public int TileSizeY;
    public int sectorRoomsX;
    public int sectorRoomsY;
    public int sectorsX;
    public int sectorsY;
    public string storagePath;
    public Texture2D mapOut;
    private int currentX = 0;
    private int currentY = 0;
    private int totalXpixels;
    private int totalYpixels;

    public void DivideMapByRoom()
    {
        totalXpixels = mapIn.width;
        totalYpixels = mapIn.height;
        int EWLine = totalXpixels / 2;
        int NSLine = totalYpixels / 2;
        int maxEWroom = Mathf.FloorToInt(EWLine / 50);
        int maxNSroom = Mathf.FloorToInt(NSLine / 50);
        currentX = 0;
        currentY = 0;
        //Debug.Log("pixel : " + mapIn.GetPixel(totalWpixels, totalYpixels));
        while (currentX <= totalXpixels || currentY <= totalYpixels)
        {
            string EW = currentX >= EWLine ? "E" : "W";
            string NS = currentY >= NSLine ? "N" : "S";
            int tileModX = currentX >= EWLine ? 0 : -TileSizeX;
            int tileModY = currentY >= NSLine ? 0 : -TileSizeY;
            int ewNum = Mathf.Abs(Mathf.FloorToInt((EWLine - currentX + tileModX) / TileSizeX));
            int nsNum = Mathf.Abs(Mathf.FloorToInt((NSLine - currentY + tileModY) / TileSizeY));
            string roomName = EW + ewNum.ToString() + NS + nsNum.ToString();
            Debug.Log(roomName);
            CreateTerrain(roomName,TileSizeX,TileSizeY);
            currentX += TileSizeX;
            if (currentX >= totalXpixels)
            {
                currentY += TileSizeY;
                currentX = currentY < totalYpixels ? 0 : currentX;
            }
        }
    }
    public void DivideMapBySector()
    {
        totalXpixels = mapIn.width;
        totalYpixels = mapIn.height;
        int EWLine = totalXpixels / 2;
        int NSLine = totalYpixels / 2;
        int gapX = TileSizeX * 2;
        int gapY = TileSizeY * 2;
        int sectorPixelsX = TileSizeX * sectorRoomsX + gapX;
        int sectorPixelsY = TileSizeY * sectorRoomsY + gapY;
        int xCounter = -((sectorsX - 2) / 2);
        int yCounter = -((sectorsY - 2) / 2);
        currentX = 0;
        currentY = 0;
        while (currentX <= totalXpixels || currentY <= totalYpixels)
        {
            string EW = currentX >= EWLine - TileSizeX ? "E" : "W";
            string NS = currentY >= NSLine - TileSizeY ? "N" : "S";
            int tileModX = currentX >= EWLine ? 0 : -TileSizeX;
            int tileModY = currentY >= NSLine ? 0 : -TileSizeY;
            //int ewNum = Mathf.Abs(Mathf.FloorToInt((EWLine - currentX + tileModX) / sectorPixelsX));
            int ewNum = Mathf.Abs(xCounter);
            string ewNumS = ewNum > 0 ? ewNum.ToString() + "0" : "0";
            //int nsNum = Mathf.Abs(Mathf.FloorToInt((NSLine - currentY + tileModY) / sectorPixelsY));
            int nsNum = Mathf.Abs(yCounter);
            string nsNumS = nsNum > 0 ? nsNum.ToString() + "0" : "0";
            string roomName = EW + ewNumS + NS + nsNumS;
            Debug.Log(roomName + "  " +currentX+"  "+currentY);
            CreateTerrain(roomName, sectorPixelsX, sectorPixelsY);
            xCounter += currentX == EWLine - sectorPixelsX ? 0 : 1;
            currentX += currentX == EWLine - sectorPixelsX ? sectorPixelsX : sectorPixelsX - TileSizeX;
            if (currentX + gapX >= totalXpixels)
            {
                //break;
                yCounter += currentY == NSLine - sectorPixelsY ? 0 : 1;
                currentY += currentY == NSLine - sectorPixelsY ? sectorPixelsY : sectorPixelsY - TileSizeY;
                currentX = currentY + gapY < totalYpixels ? 0 : currentX;
                xCounter = -((sectorsX - 2) / 2);
            }
        }
    }
    //void OnGUI()
    //{
    //    EditorGUI.ProgressBar(new Rect(3, 45, 200, 20), currentX / totalXpixels, "X");
    //    EditorGUI.ProgressBar(new Rect(3, 70, 200, 20), currentY / totalYpixels, "Y");
    //}
    private void CreateTerrain(string nameIn, int sizeX, int sizeY)
    {
        mapOut = new Texture2D(sizeX, sizeY);
        int roomX = sizeX + currentX;
        int roomY = sizeY + currentY;
        for (int x = currentX; x < roomX; x++)
        {
            for (int y = currentY; y < roomY; y++)
            {
                mapOut.SetPixel(x - currentX, y - currentY, mapIn.GetPixel(x, y));
            }
        }
        mapOut.Apply();
        byte[] a = mapOut.EncodeToPNG();
        File.WriteAllBytes(storagePath + Path.DirectorySeparatorChar + nameIn + ".png", a);
        DestroyImmediate(mapOut);
    }
}
#endif