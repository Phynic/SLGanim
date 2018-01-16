using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//扇形区域
public class DrawMesh
{
    public GameObject go;
    public MeshFilter mf;
    public MeshRenderer mr;
    public Shader shader;

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
            if (i % 2 == 0)
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
            go.transform.position = new Vector3(0, 0.1f, 0);//让绘制的图形上升一点，防止被地面遮挡 
            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();
            shader = Shader.Find("Unlit/Color");
        }

        Vector2[] newUV = new Vector2[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            if (i % 2 == 0)
            {
                newUV[i] = new Vector2(i / 100, 1);
            }
            else
            {
                newUV[i] = new Vector2(0, i / 100);
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
    
    public GameObject DrawBendMesh(Transform character, Vector3 center, float angle, float radius)
    {
        int pointAmount = 100;//点的数目，值越大曲线越平滑
        float width = 0.1f;
        var forward = character.forward;

        float eachAngle = angle / pointAmount;
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 1; i < pointAmount - 1; i++)
        {
            Vector3 pos0 = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * radius + center;
            Vector3 pos1 = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * (radius - width) + center;
            
            vertices.Add(pos0);
            vertices.Add(pos1);
        }
        
        var go = CreateMesh(vertices);
        go.transform.parent = character;
        return go;
    }
}
