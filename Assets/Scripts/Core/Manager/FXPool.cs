using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FXPool : SceneSingleton<FXPool>
{
    [Header("VFX Pool")]
    public List<Transform> poolItems;                   // Effect pool prefabs
    public List<int> poolDepth;                         // Effect pool items count         

    // Pooled items collections
    private Dictionary<string, Transform[]> pool;
    
    public void Init()
    {
        poolItems = new List<Transform>();
        poolDepth = new List<int>();

        foreach (var config in FXConfigDictionary.GetParamList())
        {
            if (config.loadTag.Contains(1))
            {
                LoadFX(config);
            }
            else
            {
                foreach (var unit in RoundManager.GetInstance().Units)
                {
                    if (config.loadTag.Contains(unit.characterInfoID))
                    {
                        LoadFX(config);
                        break;
                    }
                }
            }
        }

        // Initialize effects pool
        if (poolItems.Count > 0)
        {
            pool = new Dictionary<string, Transform[]>();

            for (int i = 0; i < poolItems.Count; i++)
            {
                Transform[] itemArray = new Transform[poolDepth[i]];
                for (int x = 0; x < poolDepth[i]; x++)
                {
                    GameObject newItem = Instantiate(poolItems[i], Vector3.zero, Quaternion.identity).gameObject;
                    newItem.SetActive(false);
                    newItem.transform.parent = transform;
                    itemArray[x] = newItem.transform;
                }
                pool.Add(poolItems[i].name, itemArray);
            }
        }
    }

    public void LoadFX(FXConfig fxConfig)
    {
        var item = Resources.Load(Global.fxPath + fxConfig.name) as GameObject;
        poolItems.Add(item.transform);
        poolDepth.Add(fxConfig.depth);
    }

    // Spawn effect prefab and send OnSpawned message
    public Transform Spawn(string obj, Vector3 pos, Quaternion rot, Transform parent)
    {
        for (int i = 0; i < pool[obj].Length; i++)
        {
            if (!pool[obj][i].gameObject.activeSelf)
            {
                Transform spawnItem = pool[obj][i];

                spawnItem.SetParent(parent);
                spawnItem.position = pos;
                spawnItem.rotation = rot;
                spawnItem.gameObject.SetActive(true);
                spawnItem.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

                return spawnItem;
            }
        }

        return null;
    }

    // Despawn effect or audio and send OnDespawned message
    public void Despawn(Transform obj,float delay)
    {
        Utils_Coroutine.GetInstance().Invoke(() => {
            obj.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
            obj.gameObject.SetActive(false);
        }, delay);
        Utils_Coroutine.GetInstance().Invoke(() => {
            obj.SetParent(transform);
            obj.localScale = new Vector3(1, 1, 1);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        }, delay + 0.2f);
    }
}
