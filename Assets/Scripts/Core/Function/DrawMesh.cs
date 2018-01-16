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
            shader = Shader.Find("Unlit/Color");
        }

        Vector2[] newUV = new Vector2[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            if (i % 2 == 0)
            {
                
                newUV[i] = new Vector2(0, (198 - i) / 198f);
            }
            else
            {

                newUV[i] = new Vector2(1, (198 - (i - 1)) / 198f);
            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.uv = newUV;

        mf.mesh = mesh;
        mr.material.shader = shader;
        mr.material.color = Color.black;

        
        return go;
    }
    
    public GameObject DrawBendMesh(Transform character, Vector3 to, Vector3 center)
    {
        int pointAmount = 100;//点的数目，值越大曲线越平滑
        float width = 1f;
        var radius = 0.75f;
        var angle = 90f;
        var forward = (character.position - center).normalized + (to - center).normalized;
        float eachAngle = angle / pointAmount;
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 1; i < pointAmount + 1; i++)
        {
            Vector3 pos0 = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * radius + center;
            Vector3 pos1 = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * (radius - width) + center;
            vertices.Add(pos0);
            vertices.Add(pos1);
        }
        
        var go = CreateMesh(vertices);
        return go;
    }

    public List<Vector3> DrawBezier(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        
        float x = 0;
        Vector2 b0;
        b0 = x * x * (p0 + p2 - 2 * p1) + 2 * x * (p1 - p0) + p0;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < pointAmount; i++)
        {
            x = i / 100f;
            b0 = x * x * (p0 + p2 - 2 * p1) + 2 * x * (p1 - p0) + p0;
            vertices.Add(new Vector3(b0.x, 0, b0.y));
        }
        return vertices;
    }

    public GameObject DrawBezierMesh(Transform character, Vector3 p0, Vector3 p1, Vector3 p2, float width, Vector3 right, Vector3 forward, Vector3 focus)
    {
        var p00 = p0 + forward * width / 2;
        var p10 = p1 + right * width / 2;
        //var p20 = p2 + right * width / 2;

        var p01 = p0 - forward * width / 2;
        var p11 = p1 - right * width / 2;
        //var p21 = p2 - right * width / 2;

        if ((p2 - focus).normalized == right)
        {
            p00 = p0 - forward * width / 2;
            //p10 = p1 - right * width / 2;
            // p20 = p2 - right * width / 2;

            p01 = p0 + forward * width / 2;
            //p11 = p1 + right * width / 2;
            // p21 = p2 + right * width / 2;
        }

        var list1 = DrawBezier(new Vector2(p00.x, p00.z), new Vector2(p10.x, p10.z), new Vector2(p2.x, p2.z));
        var list2 = DrawBezier(new Vector2(p01.x, p01.z), new Vector2(p11.x, p11.z), new Vector2(p2.x, p2.z));

        List<Vector3> list = new List<Vector3>();

        for(int i = 0; i < pointAmount; i++)
        {
            list.Add(list1[i]);
            list.Add(list2[i]);
        }

        var go = CreateMesh(list);
        //go.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, -180));

        

        return go;
    }
}
