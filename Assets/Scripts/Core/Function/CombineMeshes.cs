using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour {

    // 目标物体（必须是骨骼的父物体，不然蒙皮失效）
    public GameObject target;

    // 最终材质（合并所有模型后使用的材质）
    public Material[] materials;

    public GameObject[] targetParts;
 
    // 物体所有的部分
    //private GameObject[] targetParts = new GameObject[9];

    //private string[] defaultEquipPartPaths = new string[9];

    void Start()
    {
        // 把FBX的模型按部件分别放入Resources下对应的文件夹里，可以留空，模型需要蒙皮，而且所有模型使用同一骨骼
        // 最后的M是Fbx的模型，需要的Unity3D里设置好材质和贴图，部件贴图要勾选Read/Write Enabled
        //defaultEquipPartPaths[0] = "Model/Player/GirlPlayer/Head/Head0000/M";
        //defaultEquipPartPaths[1] = "Model/Player/GirlPlayer/Face/Face0000/M";
        //defaultEquipPartPaths[2] = "Model/Player/GirlPlayer/Hair/Hair0000/M";
        //defaultEquipPartPaths[3] = "";
        //defaultEquipPartPaths[4] = "Model/Player/GirlPlayer/Body/Body0000/M";
        //defaultEquipPartPaths[5] = "Model/Player/GirlPlayer/Leg/Leg0000/M";
        //defaultEquipPartPaths[6] = "Model/Player/GirlPlayer/Hand/Hand0000/M";
        //defaultEquipPartPaths[7] = "Model/Player/GirlPlayer/Foot/Foot0000/M";
        //defaultEquipPartPaths[8] = "Model/Player/GirlPlayer/Wing/Wing0001/M";

        //Destroy(target.GetComponent<SkinnedMeshRenderer>());
        //for (int i = 0; i < defaultEquipPartPaths.Length; i++)
        //{
        //    UnityEngine.Object o = Resources.Load(defaultEquipPartPaths[i]);
        //    if (o)
        //    {
        //        GameObject go = Instantiate(o) as GameObject;
        //        go.transform.parent = target.transform;
        //        go.transform.localPosition = new Vector3(0, -1000, 0);
        //        go.transform.localRotation = new Quaternion();
        //        targetParts[i] = go;
        //    }
        //}
        Combine(target.transform);
        //StartCoroutine(DoCombine());
    }

    /// <summary>
    /// 使用延时，不然某些GameObject还没有创建
    /// </summary>
    /// <returns></returns>
    //IEnumerator DoCombine()
    //{
    //    yield return null;
    //    Combine(target.transform);
    //}


    /// <summary>
    /// 合并蒙皮网格，刷新骨骼
    /// 注意：合并后的网格会使用同一个Material
    /// </summary>
    /// <param name="root">角色根物体</param>
    private void Combine(Transform root)
    {
        float startTime = Time.realtimeSinceStartup;

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Transform> boneList = new List<Transform>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>();
        List<Texture2D> textures = new List<Texture2D>();

        int width = 0;
        int height = 0;

        int uvCount = 0;

        List<Vector2[]> uvList = new List<Vector2[]>();

        // 遍历所有蒙皮网格渲染器，以计算出所有需要合并的网格、UV、骨骼的信息
        foreach (SkinnedMeshRenderer smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }

            uvList.Add(smr.sharedMesh.uv);
            uvCount += smr.sharedMesh.uv.Length;

            if (smr.material.mainTexture != null)
            {
                textures.Add(smr.GetComponent<Renderer>().material.mainTexture as Texture2D);
                width += smr.GetComponent<Renderer>().material.mainTexture.width;
                height += smr.GetComponent<Renderer>().material.mainTexture.height;
            }

            foreach (Transform bone in smr.bones)
            {
                foreach (Transform item in transforms)
                {
                    if (item.name != bone.name) continue;
                    boneList.Add(item);
                    break;
                }
            }
            
        }

        // 获取并配置角色所有的SkinnedMeshRenderer
        SkinnedMeshRenderer tempRenderer = root.gameObject.GetComponent<SkinnedMeshRenderer>();
        if (!tempRenderer)
        {
            tempRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();
        }

        tempRenderer.sharedMesh = new Mesh();

        // 合并网格，刷新骨骼，附加材质
        tempRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        tempRenderer.bones = boneList.ToArray();
        tempRenderer.materials = materials;

        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];

        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i < uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                //atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                //atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                atlasUVs[j].x = packingResult[i].x + uv.x * packingResult[i].width;
                atlasUVs[j].y = packingResult[i].y + uv.y * packingResult[i].height;

                j++;
            }
        }
        
        // 设置贴图和UV
        tempRenderer.material.SetTexture("_Diffuse", skinnedMeshAtlas);
        tempRenderer.sharedMesh.uv = atlasUVs;
        
        foreach (var g in targetParts)
        {
            Destroy(g);
        }

        //Debug.Log("合并耗时 : " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
    }


    /// <summary>
    /// 获取最接近输入值的2的N次方的数，最大不会超过1024，例如输入320会得到512
    /// </summary>
    private int get2Pow(int into)
    {
        int outo = 1;
        for (int i = 0; i < 10; i++)
        {
            outo *= 2;
            if (outo > into)
            {
                break;
            }
        }

        return outo;
    }
}
