using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaShakeCamera : UnitDrama {
    public float shakeOrg = 1f; //The time of shake
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    private float shake;
    private Vector3 originalPos;
    private Transform camTrans;

    void Start () {
        shake = shakeOrg;
        camTrans = Camera.main.transform;
        originalPos = camTrans.localPosition;
    }

    public override IEnumerator Play()
    {
        yield return StartCoroutine(shakeCamera());
    }

    private IEnumerator shakeCamera()
    {
        yield return 1;
        if (shake > 0)
        {
            camTrans.localPosition = originalPos + UnityEngine.Random.insideUnitSphere * shakeAmount;

            shake -= Time.deltaTime * decreaseFactor;

            StartCoroutine(shakeCamera());
        }
        else
        {
            shake = shakeOrg;
            camTrans.localPosition = originalPos;
            yield return 0;
        }
        
    }
}
