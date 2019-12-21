using UnityEngine;
using System.Collections.Generic;

public class BattleFieldManager : SingletonComponent<BattleFieldManager>
{
    public Dictionary<Vector3, GameObject> floors = new Dictionary<Vector3, GameObject>();       //查找地板块的键值对,<坐标，obj>

    public static float anchorPoint = 0.5f;
    private GameObject floorPrefab;
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    GameObject obj;
    
    public GameObject GetFloor(Vector3 pos)
    {
        if (floors.TryGetValue(pos, out obj))
        {
            return obj;
        }
        return null;
    }
    
    public void BuildFloors(int GridX, int GridY)
    {
        Utils_Transform.DeleteAllChild(transform);
        this.GridX = GridX;
        this.GridY = GridY;
        floorPrefab = (GameObject)Resources.Load("Prefabs/UI/Floor");
        //Debug.Log("X : " + GridX.ToString() + "; Y : " + GridY.ToString());
        //地板块铺满地板
        for (int i = 0; i < GridX; i++)
        {
            for (int j = 0; j < GridY; j++)
            {
                Vector3 position = new Vector3(i + anchorPoint, 0f, j + anchorPoint);

                GameObject floor = (GameObject)Instantiate(floorPrefab, position, Quaternion.identity);
                floor.transform.SetParent(transform);
                floor.tag = "Floor";
                floors.Add(position, floor);
                floor.SetActive(false);
            }
        }
    }

    public void HideAllFloors()
    {
        foreach (var floor in floors)
        {
            floor.Value.SetActive(false);
        }
    }
}
