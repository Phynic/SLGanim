using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombineMeshes : MonoBehaviour {
    
    public SkinnedMeshRenderer[] targetParts;
    public List<Texture2D> textures;
    void Start()
    {
        Combine(transform);
    }
    
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
        textures = new List<Texture2D>();
        Material material = null;
        int width = 0;
        int height = 0;

        int uvCount = 0;

        List<Vector2[]> uvList = new List<Vector2[]>();

        // 遍历所有蒙皮网格渲染器，以计算出所有需要合并的网格、UV、骨骼的信息
        targetParts = root.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        foreach (SkinnedMeshRenderer smr in targetParts)
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

            if (smr.material.GetTexture("_Diffuse") != null)
            {
                textures.Add(smr.GetComponent<Renderer>().material.GetTexture("_Diffuse") as Texture2D);
                width += smr.GetComponent<Renderer>().material.GetTexture("_Diffuse").width;
                height += smr.GetComponent<Renderer>().material.GetTexture("_Diffuse").height;
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
            if (material == null && !smr.sharedMaterial.name.Contains("Eye"))
            {
                material = smr.sharedMaterial;
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
        
        tempRenderer.material = material;

        #region 贴图处理
        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        
        Vector2[] atlasUVs = new Vector2[uvCount];
        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i < uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                atlasUVs[j].x = packingResult[i].x + uv.x * packingResult[i].width;
                atlasUVs[j].y = packingResult[i].y + uv.y * packingResult[i].height;
                j++;
                if (i == 0)
                    Debug.Log(packingResult[i].y + uv.y * packingResult[i].height);
            }
            //Debug.Log("第" + i + "张 : " + packingResult[i].x + " " + packingResult[i].y);
            //Debug.Log("第" + i + "张 : " + packingResult[i].width + " " + packingResult[i].height);
        }

        //Sprite sprite = Sprite.Create(skinnedMeshAtlas, new Rect(0, 0, 1024, 1024), Vector2.zero);
        //GameObject.Find("Canvas").transform.Find("Image").GetComponent<Image>().sprite = sprite;

        // 设置贴图和UV
        tempRenderer.material.SetTexture("_Diffuse", skinnedMeshAtlas);
        tempRenderer.sharedMesh.uv = atlasUVs;
        tempRenderer.rootBone = transform.Find("Bip001");
        #endregion

        foreach (var g in targetParts)
        {
            //g.SetActive(false);
            Destroy(g.gameObject);
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
