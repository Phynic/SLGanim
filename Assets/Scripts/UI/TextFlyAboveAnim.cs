using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlyAboveAnim : MonoBehaviour {
    public int speed = 15; //fly in speed
    public Vector3 unitPosition; //show text position

    private Text text;

    void Start()
    {
        unitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    // Use this for initialization
    public void doAnim () {
        text = GetComponent<Text>();
        text.GetComponent<RectTransform>().localScale = new Vector3(10, 10);
        Vector3 textPosition = Camera.main.GetComponent<Camera>().WorldToScreenPoint(unitPosition);
        text.transform.position = textPosition;
        StartCoroutine(setSkillText());
    }


    IEnumerator setSkillText() {
        text.GetComponent<RectTransform>().localScale -= new Vector3(Time.deltaTime,Time.deltaTime,0)*(++speed);

        yield return new WaitForSeconds(0.01f);

        if (text.GetComponent<RectTransform>().localScale.x<1)
        {
            //Debug.Log("Skill Name have shown.");
            StartCoroutine(disappearEffect());
        }
        else {
            StartCoroutine(setSkillText());
        }
    }

    IEnumerator disappearEffect() {
        DramaShakeCamera dsc = AIManager.GetInstance().AIDrama.GetComponent<DramaShakeCamera>();
        yield return StartCoroutine(dsc.Play());
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
