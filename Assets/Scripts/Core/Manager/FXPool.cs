using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FXPool : Singleton<FXPool>
{
    [Header("VFX Pool")]
    public Transform[] poolItems;                                          // Effect pool prefabs
    public int[] poolLength;                         // Effect pool items count         

    [Header("Audio Pool")]
    public Transform audioSourcePrefab;     // Audio source prefab

    public AudioClip[] audioPoolItems;      // Audio pool prefabs
    public int[] audioPoolLength;           // Audio pool items count

    // Pooled items collections
    private Dictionary<string, Transform[]> pool;
    private Dictionary<AudioClip, AudioSource[]> audioPool;
    
    private void Start()
    {
        audioSourcePrefab = Resources.Load("Prefabs/Audio Source") as Transform;

        //var particles = Resources.LoadAll("Prefabs/Particle");
        //var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "particle"));
        //var particles = myLoadedAssetBundle.LoadAllAssets<GameObject>();
        
        //foreach(var p in particles)
        //{
        //    poolItems.Add(((GameObject)p).transform);
        //    poolLength.Add(10);
        //}
        
        // Initialize effects pool
        if (poolItems.Length > 0)
        {
            pool = new Dictionary<string, Transform[]>();

            for (int i = 0; i < poolItems.Length; i++)
            {
                Transform[] itemArray = new Transform[poolLength[i]];

                for (int x = 0; x < poolLength[i]; x++)
                {
                    GameObject newItem = Instantiate(poolItems[i], Vector3.zero, Quaternion.identity).gameObject;
                    newItem.SetActive(false);
                    newItem.transform.parent = transform;

                    itemArray[x] = newItem.transform;
                }
                pool.Add(poolItems[i].name, itemArray);
            }
        }

        // Initialize audio pool
        if (audioPoolItems.Length > 0)
        {
            audioPool = new Dictionary<AudioClip, AudioSource[]>();

            for (int i = 0; i < audioPoolItems.Length; i++)
            {
                AudioSource[] audioArray = new AudioSource[audioPoolLength[i]];

                for (int x = 0; x < audioPoolLength[i]; x++)
                {
                    AudioSource newAudio = ((Transform)Instantiate(audioSourcePrefab, Vector3.zero, Quaternion.identity)).GetComponent<AudioSource>();
                    newAudio.clip = audioPoolItems[i];

                    newAudio.gameObject.SetActive(false);
                    newAudio.transform.parent = transform;

                    audioArray[x] = newAudio;
                }

                audioPool.Add(audioPoolItems[i], audioArray);
            }
        }
    }

    // Spawn effect prefab and send OnSpawned message
    public Transform Spawn(string obj, Vector3 pos, Quaternion rot, Transform parent)
    {
        for (int i = 0; i < pool[obj].Length; i++)
        {
            if (!pool[obj][i].gameObject.activeSelf)
            {
                Transform spawnItem = pool[obj][i];

                spawnItem.parent = parent;
                spawnItem.position = pos;
                spawnItem.rotation = rot;
                spawnItem.gameObject.SetActive(true);
                spawnItem.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);

                return spawnItem;
            }
        }

        return null;
    }

    // Spawn audio prefab and send OnSpawned message
    public AudioSource SpawnAudio(AudioClip clip, Vector3 pos, Transform parent)
    {
        for (int i = 0; i < audioPool[clip].Length; i++)
        {
            if (!audioPool[clip][i].gameObject.activeSelf)
            {
                AudioSource spawnItem = audioPool[clip][i];

                spawnItem.transform.parent = parent;
                spawnItem.transform.position = pos;

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
        GameController.GetInstance().Invoke(() => {
            obj.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
            obj.gameObject.SetActive(false);
        }, delay);
        GameController.GetInstance().Invoke(() => {
            obj.SetParent(transform);
            obj.localScale = new Vector3(1, 1, 1);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        }, delay + 0.2f);
    }
}
