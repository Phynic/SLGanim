using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseDirection : Skill
{
    //GameObject arrows;
    List<GameObject> allArrows;
    GameObject chooseTrickUI;
    GameObject chooseDirectionPanel;
    List<Transform> other;
    Color redColor = new Color(1, 0, 0, 1);
    Color yellowColor = new Color(1, 0.92f, 0.016f, 160f/255f);
    //Quaternion startRotation;

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    GameObject arrowsUI;
#endif
    public override bool Init(Transform character)
    {
        this.character = character;
        Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
        allArrows = new List<GameObject>();
        other = new List<Transform>();
        allArrows.Add(CreateArrow(character.GetComponent<Unit>().arrowPosition + character.position));

        var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Key.character.GetComponent<Unit>().playerNumber);

        if (player is HumanPlayer || (player is AIPlayer && ((AIPlayer)player).AIControl == false))
        {
            chooseDirectionPanel = CreatePanel();
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
            arrowsUI = CreateArrowUI();
#endif
        }


        foreach (var u in RoundManager.GetInstance().Units)
        {
            if (u.GetComponent<Unit>())
            {
                
                if (u.GetComponent<Unit>().roleEName == character.GetComponent<Unit>().roleEName && u.GetComponent<Unit>().playerNumber == character.GetComponent<Unit>().playerNumber)
                {
                    if (u.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(DirectionBuff)) != null)
                    {
                        allArrows.Add(CreateArrow(u.GetComponent<Unit>().arrowPosition + u.transform.position));
                        other.Add(u.transform);
                    }
                }
            }
        }



        return true;
    }

    private GameObject CreateArrowUI()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/ArrowsUI");
        var arrowsUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);

        var arrowUIImage = arrowsUI.GetComponentsInChildren<Image>();
        foreach (var a in arrowUIImage)
        {
            if(a.gameObject != arrowsUI.gameObject)
            {
                a.color = yellowColor;
                EventTriggerListener.Get(a.gameObject).onEnter = g => {
                    g.GetComponent<Image>().color = redColor;
                    OnArrowHovered(g.name);
                };
                EventTriggerListener.Get(a.gameObject).onExit = g => {
                    g.GetComponent<Image>().color = yellowColor;
                };
                EventTriggerListener.Get(a.gameObject).onClick = g => {
                    ShowUI(this, null);
                };
            }
        }

        return arrowsUI;
    }

    private GameObject CreateArrow(Vector3 position)
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Arrows");
        var arrows = UnityEngine.Object.Instantiate(go, position, character.rotation, null);
        
        var arrowRenderer = arrows.GetComponentsInChildren<Renderer>();
        foreach (var a in arrowRenderer)
        {
            a.material.color = yellowColor;
        }

        var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Key.character.GetComponent<Unit>().playerNumber);
        if (player is HumanPlayer || (player is AIPlayer && ((AIPlayer)player).AIControl == false))
        {
            foreach (var a in arrows.GetComponentsInChildren<Arrow>())
            {
                a.ArrowClicked += ShowUI;
                a.ArrowHovered += OnArrowHovered;
                a.ArrowExited += OnArrowExited;
            }
        }
        return arrows;
    }

    private GameObject CreatePanel()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/ChooseDirectionPanel");
        var chooseDirectionPanel = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform) as GameObject;
        return chooseDirectionPanel;
    }

    private void OnArrowHovered(object sender, EventArgs e)
    {
        var dir = ((GameObject)sender).name;
        List<GameObject> sameDir = new List<GameObject>();
        foreach (var arrows in allArrows)
        {
            sameDir.Add(arrows.transform.Find(dir).gameObject);
            var arrowRenderer = arrows.GetComponentsInChildren<Renderer>();
            foreach (var a in arrowRenderer)
            {
                a.material.color = yellowColor;
            }
        }
        foreach(var arrow in sameDir)
        {
            var arrowRenderer = arrow.GetComponentsInChildren<Renderer>();
            foreach (var a in arrowRenderer)
            {
                if (a.material.color != redColor)
                {
                    a.material.color = redColor;
                }
            }
        }
    }

    public void OnArrowHovered(string dir)
    {
        List<GameObject> sameDir = new List<GameObject>();
        foreach (var arrows in allArrows)
        {
            sameDir.Add(arrows.transform.Find(dir).gameObject);
            var arrowRenderer = arrows.GetComponentsInChildren<Renderer>();
            foreach (var a in arrowRenderer)
            {
                a.material.color = yellowColor;
            }
        }
        foreach (var arrow in sameDir)
        {
            var arrowRenderer = arrow.GetComponentsInChildren<Renderer>();
            foreach (var a in arrowRenderer)
            {
                if (a.material.color != redColor)
                {
                    a.material.color = redColor;
                }
            }
        }
    }

    private void OnArrowExited(object sender, EventArgs e)
    {
        foreach(var arrows in allArrows)
        {
            foreach(var a in arrows.GetComponentsInChildren<Renderer>())
            {
                if (a.material.color != yellowColor)
                {
                    a.material.color = yellowColor;
                }
            }
        }
    }

    private void ShowUI(object sender, EventArgs e)
    {
        //GameObject.Destroy(arrows);
        //foreach (var a in allArrows)
        //{
        //    GameObject.Destroy(a);
        //}
        //allArrows.Clear();

        DestroyAllArrows();
        GameObject.Destroy(chooseDirectionPanel);
        var tempSkillList = new List<UnitSkill>();
        foreach(var skill in character.GetComponent<Unit>().skills)
        {
            var t = SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == skill.Key);
            if(t is UnitSkill)
            {
                tempSkillList.Add((UnitSkill)t);
            }
            
        }

        //无防御·闪避技能则跳过||有BanBuff跳过。
        if (tempSkillList.FindAll(s => s.skillInfo.skillType == SkillType.dodge).Count == 0
            || character.GetComponent<Unit>().Buffs.Find(b => b is BanBuff) != null
            )
        {
            Confirm();
        }
        else
        {
            var go = (GameObject)Resources.Load("Prefabs/UI/Judge");
            chooseTrickUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);

            chooseTrickUI.transform.Find("No").GetComponent<Button>().onClick.AddListener(Confirm);
            chooseTrickUI.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(ChooseTrick);
            chooseTrickUI.transform.Find("Text").GetComponent<Text>().text = "是否使用防御·回避忍术？";
        }
    }

    public override bool OnUpdate(Transform character)
    {
        if (RoundManager.GetInstance().gameEnded)
            return false;
        switch (skillState)
        {
            case SkillState.init:
                Init(character);
                skillState = SkillState.waitForInput;
                break;
            case SkillState.waitForInput:
                if (allArrows.Count > 0)
                {
                    var arrowRenderer = allArrows[0].GetComponentsInChildren<Renderer>();
                    foreach (var a in arrowRenderer)
                    {
                        if (a.material.color == redColor)
                        {
                            character.forward = (a.transform.position - allArrows[0].transform.position).normalized;
                            if(other.Count > 0)
                            {
                                foreach (var o in other)
                                {
                                    o.forward = (a.transform.position - allArrows[0].transform.position).normalized;
                                }
                            }
                        }
                    }
                }
                break;
            case SkillState.confirm:
                return true;
            case SkillState.reset:
                return true;
        }
        
        return false;
    }

    private void Confirm()
    {
        if(chooseTrickUI)
            GameObject.Destroy(chooseTrickUI);
        skillState = SkillState.confirm;
        character.GetComponent<Unit>().OnUnitEnd();   //真正的回合结束所应执行的逻辑。
        RoundManager.GetInstance().EndTurn();
    }

    public void Confirm_AI()
    {
        DestroyAllArrows();
        if (chooseTrickUI)
            GameObject.Destroy(chooseTrickUI);
        skillState = SkillState.confirm;
    }

    private void ChooseTrick()
    {
        GameObject.Destroy(chooseTrickUI);
        skillState = SkillState.confirm;
        character.GetComponent<CharacterAction>().SetSkill("ChooseTrick");
    }

    //AI
    //public void Confirm(Transform arrow)
    //{
    //    character.forward = (arrow.transform.position - arrows.transform.position).normalized;
    //    arrow.GetComponent<SpriteRenderer>().color = redColor;
    //}

    public override void Reset()
    {
        ResetSelf();
    }
    
    private void DestroyAllArrows()
    {
        foreach (var a in allArrows)
        {
            GameObject.Destroy(a);
        }

        foreach (var arrows in allArrows)
        {
            foreach (var a in arrows.GetComponentsInChildren<Arrow>())
            {
                a.ArrowClicked -= ShowUI;
                a.ArrowHovered -= OnArrowHovered;
                a.ArrowExited -= OnArrowExited;
            }
        }

        allArrows.Clear();
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        if (arrowsUI != null)
            GameObject.Destroy(arrowsUI);
#endif
    }

    private void ResetSelf()
    {
        DestroyAllArrows();
        GameObject.Destroy(chooseDirectionPanel);
        GameObject.Destroy(chooseTrickUI);
        skillState = SkillState.init;
    }

    public override bool Check()
    {
        return true;
    }
}
