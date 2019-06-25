using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartView.GetInstance().onClose += () => GalView.GetInstance().Open();
            GameController.GetInstance().FadeClose<StartView>();
        }
    }
}
