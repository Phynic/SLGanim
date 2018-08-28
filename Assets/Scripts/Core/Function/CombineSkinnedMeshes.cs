using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSkinnedMeshes : MonoBehaviour {
    Transform combineMesh;
    Transform rootBone;

    private void Start()
    {
        GameObject obj = new GameObject("combineMesh");
        combineMesh = obj.transform;
        combineMesh.parent = transform;
        combineMesh.localRotation = Quaternion.identity;
        combineMesh.localScale = Vector3.one;
        combineMesh.localPosition = Vector3.zero;

        rootBone = transform.Find("Bip001");

        CombineMesh();
        
    }

    void CombineMesh()
    {
        SkinnedMeshRenderer[] smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
        CombineInstance[] combine = new CombineInstance[smrs.Length];
        Material[] materials = new Material[smrs.Length];
        Texture2D[] textures = new Texture2D[smrs.Length];

        SkinnedMeshRenderer smrCombine = combineMesh.gameObject.AddComponent<SkinnedMeshRenderer>();
        smrCombine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        smrCombine.receiveShadows = false;

        for (int i = 0; i < smrs.Length; i++)
        {
            materials[i] = smrs[i].sharedMaterial;
            Texture2D tx = materials[i].GetTexture("_Diffuse") as Texture2D;

            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();
            textures[i] = tx2D;
        }

        Material materialNew = new Material(materials[0].shader);
        materialNew.CopyPropertiesFromMaterial(materials[0]);

        Texture2D texture = new Texture2D(1024, 1024);
        Rect[] rects = texture.PackTextures(textures, 10, 1024);
        materialNew.SetTexture("_Diffuse", texture);

        List<Transform> boneTmp = new List<Transform>();

        for (int i = 0; i < smrs.Length; i++)
        {
            if (smrs[i].transform == transform)
            {
                continue;
            }
            Rect rect = rects[i];

            Mesh meshCombine = CreatMeshWithMesh(smrs[i].sharedMesh);
            Vector2[] uvs = new Vector2[meshCombine.uv.Length];

            for (int j = 0; j < uvs.Length; j++)
            {
                uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
            }

            boneTmp.AddRange(smrs[i].bones);

            meshCombine.uv = uvs;
            combine[i].mesh = meshCombine;
            combine[i].transform = smrs[i].transform.localToWorldMatrix;
            GameObject.Destroy(smrs[i].gameObject);
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combine, true, true);

        smrCombine.bones = boneTmp.ToArray();
        smrCombine.rootBone = rootBone;
        smrCombine.sharedMesh = newMesh;
        smrCombine.sharedMaterial = materialNew;
    }
    Mesh CreatMeshWithMesh(Mesh mesh)
    {
        Mesh mTmp = new Mesh();
        mTmp.vertices = mesh.vertices;
        mTmp.name = mesh.name;
        mTmp.uv = mesh.uv;
        mTmp.uv2 = mesh.uv2;
        mTmp.uv2 = mesh.uv2;
        mTmp.bindposes = mesh.bindposes;
        mTmp.boneWeights = mesh.boneWeights;
        mTmp.bounds = mesh.bounds;
        mTmp.colors = mesh.colors;
        mTmp.colors32 = mesh.colors32;
        mTmp.normals = mesh.normals;
        mTmp.subMeshCount = mesh.subMeshCount;
        mTmp.tangents = mesh.tangents;
        mTmp.triangles = mesh.triangles;

        return mTmp;
    }

}
