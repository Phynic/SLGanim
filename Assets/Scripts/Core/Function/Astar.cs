using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Astar
{
    public const int OBLIQUE = 14;
    public const int STEP = 10;
    public int[,] MazeArray { get; private set; }
    List<Point> CloseList = new List<Point>();
    List<Point> OpenList = new List<Point>();
    List<Point> surroundPoints = new List<Point>();
    public Astar(int[,] astar)
    {
        this.MazeArray = astar;
    }

    public Point FindPath(Point start, Point end, bool IsIgnoreCorner)
    {
        OpenList.Add(start);
        while (OpenList.Count != 0)
        {
            //找出F值最小的点
            var tempStart = OpenList.MinPoint();
            OpenList.RemoveAt(0);
            CloseList.Add(tempStart);
            //找出它相邻的点
            surroundPoints.Clear();
            surroundPoints = SurroundPoints(tempStart, IsIgnoreCorner);
            foreach (Point point in surroundPoints)
            {
                if (OpenList.Exists(point))
                    //计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                    FoundPoint(tempStart, point);
                else
                    //如果它们不在开始列表里, 就加入, 并设置父节点,并计算GHF
                    NotFoundPoint(tempStart, end, point);
            }
            if (OpenList.Get(end) != null)
                return OpenList.Get(end);
        }
        return OpenList.Get(end);
    }

    private void FoundPoint(Point tempStart, Point point)
    {
        var G = CalcG(tempStart, point);
        if (G < point.G)
        {
            point.ParentPoint = tempStart;
            point.G = G;
            point.CalcF();
        }
    }

    private void NotFoundPoint(Point tempStart, Point end, Point point)
    {
        point.ParentPoint = tempStart;
        point.G = CalcG(tempStart, point);
        point.H = CalcH(end, point);
        point.CalcF();
        OpenList.Add(point);
    }

    private int CalcG(Point start, Point point)
    {
        int G = (Math.Abs(point.X - start.X) + Math.Abs(point.Y - start.Y)) == 2 ? STEP : OBLIQUE;
        int parentG = point.ParentPoint != null ? point.ParentPoint.G : 0;
        return G + parentG;
    }

    private int CalcH(Point end, Point point)
    {
        int step = Math.Abs(point.X - end.X) + Math.Abs(point.Y - end.Y);
        return step * STEP;
    }

    //获取某个点周围可以到达的点
    public List<Point> SurroundPoints(Point point, bool IsIgnoreCorner)
    {
        surroundPoints.Clear();
        int[,] tempPoints = { { point.X - 1, point.Y }, { point.X, point.Y - 1 }, { point.X, point.Y + 1 }, { point.X + 1, point.Y } };
        for (int i = 0; i < 4; i++)
        {
            int x = tempPoints[i, 0];
            int y = tempPoints[i, 1];
            if (CanReach(point, x, y, IsIgnoreCorner))
                surroundPoints.Add(x, y);
        }
        //for(int x = point.X -1; x <= point.X+1;x++)
        //    for (int y = point.Y - 1; y <= point.Y + 1; y++)
        //    {
        //        if (CanReach(point,x, y,IsIgnoreCorner))
        //            surroundPoints.Add(x, y);
        //    }
        return surroundPoints;
    }

    //在二维数组对应的位置不为障碍物
    private bool CanReach(int x, int y)
    {
        return MazeArray[x, y] == 0;
    }

    public bool CanReach(Point start, int x, int y, bool IsIgnoreCorner)
    {
        if (!CanReach(x, y) || CloseList.Exists(x, y))
            return false;
        else
        {
            if (Math.Abs(x - start.X) + Math.Abs(y - start.Y) == 1)
                return true;
            //如果是斜方向移动, 判断是否 "拌脚"
            else
            {
                if (CanReach(Math.Abs(x - 1), y) && CanReach(x, Math.Abs(y - 1)))
                    return true;
                else
                    return IsIgnoreCorner;
            }
        }
    }
}

//Point 类型
public class Point
{
    public Point ParentPoint { get; set; }
    public int F { get; set; }  //F=G+H
    public int G { get; set; }
    public int H { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public void CalcF()
    {
        this.F = this.G + this.H;
    }
}

//对 List<Point> 的一些扩展方法
public static class ListHelper
{
    public static bool Exists(this List<Point> points, Point point)
    {
        foreach (Point p in points)
            if ((p.X == point.X) && (p.Y == point.Y))
                return true;
        return false;
    }

    public static bool Exists(this List<Point> points, int x, int y)
    {
        foreach (Point p in points)
            if ((p.X == x) && (p.Y == y))
                return true;
        return false;
    }

    public static Point MinPoint(this List<Point> points)
    {
        points.Sort((x, y) => { return x.F.CompareTo(y.F); });
        return points[0];
    }
    public static void Add(this List<Point> points, int x, int y)
    {
        Point point = new Point(x, y);
        points.Add(point);
    }

    public static Point Get(this List<Point> points, Point point)
    {
        foreach (Point p in points)
            if ((p.X == point.X) && (p.Y == point.Y))
                return p;
        return null;
    }

    public static void Remove(this List<Point> points, int x, int y)
    {
        foreach (Point point in points)
        {
            if (point.X == x && point.Y == y)
                points.Remove(point);
        }
    }
}
