using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPool : MonoBehaviour {
    private static FXPool instance;

    [Header("VFX Pool")]
    List<Transform> poolItems = new List<Transform>();              // Effect pool prefabs
    List<int> poolLength = new List<int>();                         // Effect pool items count         

    [Header("Audio Pool")]
    public Transform audioSourcePrefab;     // Audio source prefab

    public AudioClip[] audioPoolItems;      // Audio pool prefabs
    public int[] audioPoolLength;           // Audio pool items count

    // Pooled items collections
    private Dictionary<Transform, Transform[]> pool;
    private Dictionary<AudioClip, AudioSource[]> audioPool;

    public static FXPool GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        instance = this;

        //audioSourcePrefab = Resources.Load("Prefabs/Audio Source") as Transform;

        var re = Resources.LoadAsync("Prefabs/Audio Source", typeof(Transform));

        var particles = Resources.LoadAll("Prefabs/Particle");
        
        foreach(var p in particles)
        {
            poolItems.Add(((GameObject)p).transform);
            poolLength.Add(5);
        }
        
        // Initialize effects pool
        if (poolItems.Count > 0)
        {
            pool = new Dictionary<Transform, Transform[]>();

            for (int i = 0; i < poolItems.Count; i++)
            {
                Transform[] itemArray = new Transform[poolLength[i]];

                for (int x = 0; x < poolLength[i]; x++)
                {
                    Transform newItem = (Transform)Instantiate(poolItems[i], Vector3.zero, Quaternion.identity);
                    newItem.gameObject.SetActive(false);
                    newItem.parent = transform;

                    itemArray[x] = newItem;
                }
                pool.Add(poolItems[i], itemArray);
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
    public Transform Spawn(Transform obj, Vector3 pos, Quaternion rot, Transform parent)
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

    public Transform Spawn(Transform obj, Transform parent)
    {
        for (int i = 0; i < pool[obj].Length; i++)
        {
            if (!pool[obj][i].gameObject.activeSelf)
            {
                Transform spawnItem = pool[obj][i];

                spawnItem.position = obj.position;
                spawnItem.rotation = obj.rotation;

                spawnItem.SetParent(parent, false);
                
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
        RoundManager.GetInstance().Invoke(() => {
            obj.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
            obj.gameObject.SetActive(false);
        }, delay);
        RoundManager.GetInstance().Invoke(() => {
            obj.SetParent(transform);
        }, delay + 0.2f);
    }
}
