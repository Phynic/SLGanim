using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Save save = new Save();
            save.CreateNewSave();
            Utils_Save.Save(save, "0000");
        }
    }
}
