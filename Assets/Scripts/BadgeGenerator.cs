using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScreepsViewer;

public class BadgeGenerator : MonoBehaviour
{
    public int size;
    public Sprite sprite;
    public Texture2D texture;
    public Color cOne;
    public Color cTwo;
    public Color cThree;
    private Color cutColor = new Color(1f,1f,1f,0f);
    private BadgeStorage bs;
    private ScreepsViewer.ScreepsAPI api;

    public void Start()
    {
        //api = GameObject.Find("ConnectionController").GetComponent<Screeps3D.ScreepsAPI>();
        //bs = gameObject.GetComponent<BadgeStorage>();
        //Badge b = new Badge(1, "#0055ff", "#55ff00", "#ff0055", 0, false);
        //CreateBadge(b,64);
        //StartCoroutine(bs.IRetrieveBadge("smitt33",b));
    }
    public Sprite CreateBadge(Badge badgeIn, int sizeIn)
    {
        size = sizeIn;
        cOne = ColorFromHex(badgeIn.color1);
        cTwo = ColorFromHex(badgeIn.color2);
        cThree = ColorFromHex(badgeIn.color3);
        GetBaseTexture(cOne);
        SetMiddleLayer(true, 0.5f, cTwo);
        bool[,] recPos = SetShapePositions(new Vector2Int(32, 32), new Vector2(0.2f, 0.2f), 0, Shape.Rectangle);
        SetTopLayer(cThree, recPos);
        CutCircle();
        return sprite;
    }
    void GetBaseTexture(Color colorOne)
    {
        texture = new Texture2D(size, size);
        sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f,0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                //Color color = y > 31 ? colorOne : colorTwo;
                texture.SetPixel(x, y, colorOne);
            }
        }
        texture.Apply();
        //return texture;
    }
    void SetMiddleLayer(bool vertical, float percent, Color colorTwo)
    {
        percent = percent > 1 ? 1.0f : percent;
        percent = percent < 0 ? 0.0f : percent;

        int halfway = Mathf.RoundToInt(percent * size);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (x > halfway)
                {
                    if (vertical)
                        texture.SetPixel(x, y, colorTwo);
                    if (!vertical)
                        texture.SetPixel(y, x, colorTwo);
                }
            }
        }
        texture.Apply();
        //return texture;
    }
    void SetTopLayer(Color colorThree, bool[,] positionsIn)
    {
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (positionsIn[x,y])
                    texture.SetPixel(x, y, colorThree);
            }
        }
        texture.Apply();
    }
    void CutCircle()
    {
        int mid = size / 2;
        int d = (5 - 32 * 4) / 4;
        int x = 0;
        int y = mid + 1;

        do
        {
            // ensure index is in range before setting (depends on your image implementation)
            // in this case we check if the pixel location is within the bounds of the image before setting the pixel
            if (mid + x >= 0 && mid + x <= size && mid + y >= 0 && mid + y <= size)
            {
                for (int i = size; i > x+mid; i--)
                {
                    texture.SetPixel(i, mid + y, cutColor);
                }
                texture.SetPixel(mid + x, mid + y, cutColor);
            }
            if (mid + x >= 0 && mid + x <= size && mid - y >= 0 && mid - y <= size)
            {
                for (int i = size; i > x + mid; i--)
                {
                    texture.SetPixel(i, mid - y, cutColor);
                }
                texture.SetPixel(mid + x, mid - y, cutColor);
            }
            if (mid - x >= 0 && mid - x <= size && mid + y >= 0 && mid + y <= size)
            {
                for (int i = 0; i < mid - x; i++)
                {
                    texture.SetPixel(i, mid + y, cutColor);
                }
                texture.SetPixel(mid - x, mid + y, cutColor);
            }
            if (mid - x >= 0 && mid - x <= size && mid - y >= 0 && mid - y <= size)
            {
                for (int i = 0; i < mid - x; i++)
                {
                    texture.SetPixel(i, mid - y, cutColor);
                }
                texture.SetPixel(mid - x, mid - y, cutColor);
            }
            if (mid + y >= 0 && mid + y <= size && mid + x >= 0 && mid + x <= size)
            {
                for (int i = size; i > y+mid; i--)
                {
                    texture.SetPixel(i, mid + x, cutColor);
                }
                texture.SetPixel(mid + y, mid + x, cutColor);
            }
            if (mid + y >= 0 && mid + y <= size && mid - x >= 0 && mid - x <= size)
            {
                for (int i = size; i > y + mid; i--)
                {
                    texture.SetPixel(i, mid - x, cutColor);
                }
                texture.SetPixel(mid + y, mid - x, cutColor);
            }
            if (mid - y >= 0 && mid - y <= size && mid + x >= 0 && mid + x <= size)
            {
                for (int i = 0; i < mid - y; i++)
                {
                    texture.SetPixel(i, mid + x, cutColor);
                }
                texture.SetPixel(mid - y, mid + x, cutColor);
            }
            if (mid - y >= 0 && mid - y <= size && mid - x >= 0 && mid - x <= size)
            {
                for (int i = 0; i < mid - y; i++)
                {
                    texture.SetPixel(i, mid - x, cutColor);
                }
                texture.SetPixel(mid - y, mid - x, cutColor);
            }
            if (d < 0)
            {
                d += 2 * x + 1;
            }
            else
            {
                d += 2 * (x - y) + 1;
                y--;
            }
            x++;
        } while (x <= y);
        texture.Apply();
    }
    bool[,] SetShapePositions(Vector2Int shapePos, Vector2 shapeSize, int shapeAngle, Shape shapeClass)
    {
        bool[,] posArray = new bool[size, size];
        if (shapeClass == Shape.Rectangle)
        {
            float w = size * shapeSize.x;
            float h = size * shapeSize.x;

            Vector2Int corner1 = new Vector2Int(Mathf.RoundToInt(shapePos.x + w / 2), Mathf.RoundToInt(shapePos.y + h / 2));
            Vector2Int corner2 = new Vector2Int(Mathf.RoundToInt(shapePos.x + w / 2), Mathf.RoundToInt(shapePos.y - h / 2));
            Vector2Int corner3 = new Vector2Int(Mathf.RoundToInt(shapePos.x - w / 2), Mathf.RoundToInt(shapePos.y - h / 2));
            Vector2Int corner4 = new Vector2Int(Mathf.RoundToInt(shapePos.x - w / 2), Mathf.RoundToInt(shapePos.y + h / 2));
            
            if (shapeAngle != 0)
            {
                corner1 = RotatePoint(corner1, shapePos, shapeAngle);
                corner2 = RotatePoint(corner2, shapePos, shapeAngle);
                corner3 = RotatePoint(corner3, shapePos, shapeAngle);
                corner4 = RotatePoint(corner4, shapePos, shapeAngle);
            }

            bool[,] tri1 = SetTriPositions(corner1, corner2, corner3);
            bool[,] tri2 = SetTriPositions(corner1, corner4, corner3);
            
            int minY = Mathf.RoundToInt(Mathf.Min(corner1.y, corner2.y, corner3.y, corner4.y));
            int maxY = Mathf.RoundToInt(Mathf.Max(corner1.y, corner2.y, corner3.y, corner4.y));
            int minX = Mathf.RoundToInt(Mathf.Min(corner1.x, corner2.x, corner3.x, corner4.x));
            int maxX = Mathf.RoundToInt(Mathf.Max(corner1.x, corner2.x, corner3.x, corner4.x));

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    try
                    {
                        posArray[x, y] = (tri1[x, y] || tri2[x, y]) ? true : false;
                        //posArray[x, y] = tri2[x, y] ? true : false;
                    } catch
                    {
                        Debug.Log("not in array : " + x + "  " + y);
                    }
                }
            }
        }
        return posArray;
    }
    enum Shape { Circle, Rectangle, Triangle }
    Vector2Int RotatePoint(Vector2Int pointIn, Vector2Int center, float rotation)
    {
        float rotationRadians = rotation * (Mathf.PI / 180);
        float newX = Mathf.Cos(rotationRadians) * (pointIn.x - center.x) - Mathf.Sin(rotationRadians) * (pointIn.y - center.y) + center.x;
        float newY = Mathf.Sin(rotationRadians) * (pointIn.x - center.x) + Mathf.Cos(rotationRadians) * (pointIn.y - center.y) + center.y;
        Vector2Int pointOut = new Vector2Int(Mathf.RoundToInt(newX), Mathf.RoundToInt(newY));
        return pointOut;
    }
    Vector2Int[,] CreateTriFillBuffers(Vector2Int corner1, Vector2Int corner2, Vector2Int corner3)
    {
        int min = Mathf.Min(corner1.x, corner2.x, corner3.x);
        int max = Mathf.Max(corner1.x, corner2.x, corner3.x);
        int buffSize = max - min;
        Vector2Int[,] fillBuffer = new Vector2Int[2,size];

        void fillFillBuffers(Vector2Int[] lineIn, int dIn)
        {
            //Debug.Log("min: "+min+" max: "+max+" delta: "+dIn);
            foreach (Vector2Int point in lineIn)
            {
                if (dIn < 0)
                    //Debug.Log(point);
                    fillBuffer[0,point.y] = point;
                if (dIn > 0)
                {
                    //Debug.Log(point);
                    fillBuffer[1, point.y] = point;
                }
                if (dIn == 0)
                {
                    fillBuffer[0, point.y] = new Vector2Int(min, point.y);
                    fillBuffer[1, point.y] = new Vector2Int(max, point.y);
                }
            }
        }
        int dy1 = corner1.y - corner2.y;
        int dy2 = corner2.y - corner3.y;
        int dy3 = corner3.y - corner1.y;
        Vector2Int[] line1 = GetLinePoints(corner1.x, corner1.y, corner2.x, corner2.y);
        fillFillBuffers(line1, dy1);
        Vector2Int[] line2 = GetLinePoints(corner2.x, corner2.y, corner3.x, corner3.y);
        fillFillBuffers(line2, dy2);
        Vector2Int[] line3 = GetLinePoints(corner3.x, corner3.y, corner1.x, corner1.y);
        fillFillBuffers(line3, dy3);
        return fillBuffer;
    }
    Vector2Int[] GetLinePoints(int x0, int y0, int x1, int y1)
    {
        List<Vector2Int> linePoints = new List<Vector2Int>();
        int deltax = x1 - x0;
        int deltay = y1 - y0;
        // returns strait lines
        if (deltax == 0)
        {
            int yLo = y1 > y0 ? y0 : y1;
            int yHi = y1 > y0 ? y1 : y0;
            for (int ys = yLo; ys <= yHi; ys++)
            {
                //Debug.Log("Vertical points: " + x0 + " " + ys);
                linePoints.Add(new Vector2Int(x0,ys));
            }
            return linePoints.ToArray();
        }
        int xLo = x1 > x0 ? x0 : x1;
        int xHi = x1 > x0 ? x1 : x0;
        if (deltay == 0)
        {
            for (int xs = xLo; xs <= xHi; xs++)
            {
                //Debug.Log("Horizontal points: " + xs + " " + y0);
                linePoints.Add(new Vector2Int(xs, y0));
            }
            return linePoints.ToArray();
        }
        float deltaerr = Mathf.Abs(deltay * 1.0f / deltax );    // Assume deltax != 0 (line is not vertical),
        // note that this division needs to be done in a way that preserves the fractional part
        float error = 0; // No error at start
        int y = y0;
        linePoints.Add(new Vector2Int(x0, y0));
        //int xLo = x1 > x0 ? x0 : x1;
        //int xHi = x1 > x0 ? x1 : x0;
        for (int x = xLo; x <= xHi; x++)
        {
            error = error + deltaerr;
            if (error >= 0.5)
            {
                y = Mathf.RoundToInt(y + Mathf.Sign(deltay));
                error = error - 1.0f;
                //Debug.Log("tilted line points: "+x+" "+y);
                linePoints.Add(new Vector2Int(x,y));
            }
        }
        linePoints.Add(new Vector2Int(x1, y1));
        return linePoints.ToArray();
    }
    bool[,] SetTriPositions(Vector2Int corner1, Vector2Int corner2, Vector2Int corner3)
    {
        Vector2Int[,] xs1 = CreateTriFillBuffers(corner1, corner2, corner3);
        bool[,] posArray = new bool[size, size];
        int minY = Mathf.RoundToInt(Mathf.Min(corner1.y, corner2.y, corner3.y));
        int maxY = Mathf.RoundToInt(Mathf.Max(corner1.y, corner2.y, corner3.y));

        //for (int y = minY; y <= maxY; y++)
        for (int y = minY; y <= maxY; y++)
        {
            Vector2Int leftPoint = xs1[0, y];
            Vector2Int rightPoint = xs1[1, y];
            int minX = Mathf.Min(leftPoint.x, rightPoint.x);
            int maxX = Mathf.Max(leftPoint.x, rightPoint.x);
            //Debug.Log("Y"+y+"left : "+ leftPoint + " right: "+ rightPoint + " min "+ minX + " max "+ maxX);
            for (int x = minX; x <= maxX; x++)
            {
                posArray[x, y] = true;
            }
        }
        return posArray;
    }
    Color ColorFromHex(string hexKeyIn)
    {
        if (hexKeyIn.Length < 6) { return Color.magenta; }
        string rStr = hexKeyIn[1].ToString() + hexKeyIn[2].ToString();
        string gStr = hexKeyIn[3].ToString() + hexKeyIn[4].ToString();
        string bStr = hexKeyIn[5].ToString() + hexKeyIn[6].ToString();
        string aStr = "FF";
        if (hexKeyIn.Length > 8)
            aStr = hexKeyIn[7].ToString() + hexKeyIn[8].ToString();
        float rC = System.Convert.ToInt32(rStr, 16) / 255f;
        float gC = System.Convert.ToInt32(gStr, 16) / 255f;
        float bC = System.Convert.ToInt32(bStr, 16) / 255f;
        float aC = System.Convert.ToInt32(aStr, 16) / 255f;
        Color colorOut = new Color(rC, gC, bC, aC);
        return colorOut;
    }
}
