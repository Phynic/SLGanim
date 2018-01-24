using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;



//扇形区域
public class DrawMesh
{
    public GameObject go;
    public MeshFilter mf;
    public MeshRenderer mr;
    public Shader shader;
    int pointAmount = 100;

    public class PointInfo
    {
        public Vector3 position;
        public Vector3 tangentDirection;

        public PointInfo(Vector3 position, Vector3 tangentDirection)
        {
            this.position = position;
            this.tangentDirection = tangentDirection;
        }
    }

    private GameObject CreateMesh(List<Vector3> vertices)
    {
        int[] triangles;
        Mesh mesh = new Mesh();

        int triangleAmount = vertices.Count - 2;
        triangles = new int[3 * triangleAmount];

        //根据三角形的个数，来计算绘制三角形的顶点顺序（索引）      
        //顺序必须为顺时针或者逆时针
        for (int i = 0; i < triangleAmount; i++)
        {
            if (i % 2 != 0)
            {
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 2;
                triangles[3 * i + 2] = i + 1;
            }
            else
            {
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = i + 2;
            }
        }

        if (go == null)
        {
            go = new GameObject("mesh");
            go.transform.position = new Vector3(go.transform.position.x, 0.01f, go.transform.position.z);//让绘制的图形上升一点，防止被地面遮挡 
            
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();
            shader = Shader.Find("cgwell/Dissolve_Fresnel_Opaque");
        }

        Vector2[] newUV = new Vector2[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            if (i % 2 == 0)
            {
                newUV[i] = new Vector2(0, (vertices.Count - 2f - i) / (vertices.Count - 2f));
            }
            else
            {

                newUV[i] = new Vector2(1, (vertices.Count - 2f - (i - 1)) / (vertices.Count - 2f));
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.uv = newUV;

        mf.mesh = mesh;
        mr.material.shader = shader;
        mr.material.color = Color.black;
        mr.material.SetFloat("_Anim", 1f);
        
        return go;
    }
    
    public List<PointInfo> DrawBezier(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        
        float x = 0;

        Vector2 a0;
        Vector2 a1;
        Vector2 b0;

        List<PointInfo> vertices = new List<PointInfo>();
        for (int i = 0; i < pointAmount; i++)
        {
            x = i / 100f;

            a0 = p0 + (p1 - p0) * x;
            a1 = p1 + (p2 - p1) * x;
            var a0a1 = a1 - a0;
            b0 = x * x * (p0 + p2 - 2 * p1) + 2 * x * (p1 - p0) + p0;
            vertices.Add(new PointInfo(new Vector3(b0.x, 0, b0.y), new Vector3(a0a1.x, 0, a0a1.y)));
        }
        return vertices;
    }

    public List<PointInfo> DrawBezier(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return DrawBezier(new Vector2(p0.x, p0.z), new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z));
    }
    
    List<Vector3> CreateVerticeList(List<Vector3> list1, List<Vector3> list2, int pointCount)
    {
        List<Vector3> list = new List<Vector3>();

        for (int i = 0; i < pointCount; i++)
        {
            list.Add(list1[i]);
            list.Add(list2[i]);
        }

        return list;
    }
    
    void CreateMeshList(List<PointInfo> pList, float width, out List<Vector3> list1, out List<Vector3> list2)
    {
        list1 = new List<Vector3>();
        list2 = new List<Vector3>();

        foreach (var p in pList)
        {
            var x = p.tangentDirection.x;
            var y = p.tangentDirection.z;
            list1.Add(p.position + width * new Vector3(y / (Mathf.Sqrt(x * x + y * y)), 0, -x / (Mathf.Sqrt(x * x + y * y))));
            list2.Add(p.position + width * new Vector3(-y / (Mathf.Sqrt(x * x + y * y)), 0, x / (Mathf.Sqrt(x * x + y * y))));
        }
    }

    public GameObject DrawBezierMesh(Vector3 p0, Vector3 p1, Vector3 p2, float width)
    {
        var pList = DrawBezier(new Vector2(p0.x, p0.z), new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z));

        List<Vector3> list1;
        List<Vector3> list2;

        CreateMeshList(pList, width, out list1, out list2);

        var list = CreateVerticeList(list1, list2, pList.Count);

        var go = CreateMesh(list);
        
        return go;
    }

    public GameObject DrawDoubleBezierMesh(Vector3 p0, Vector3 p11, Vector3 p12, Vector3 p2, float width)
    {
        Vector3 mid = new Vector3((p0.x + p2.x) / 2, 0, (p0.z + p2.z) / 2);

        var pList1 = DrawBezier(p0, p11, mid);
        var pList2 = DrawBezier(mid, p12, p2);

        //去除重复点。
        pList2.Remove(pList2[0]);

        List<PointInfo> pList = new List<PointInfo>();

        foreach (var p in pList1)
        {
            pList.Add(p);
        }

        foreach (var p in pList2)
        {
            pList.Add(p);
        }
        
        List<Vector3> list1;
        List<Vector3> list2;

        CreateMeshList(pList, width, out list1, out list2);

        var list = CreateVerticeList(list1, list2, pList.Count);

        var go = CreateMesh(list);

        return go;
    }

    
}
