using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class RenderBlurOutline : MonoBehaviour
{
    public Color outLineColor = Color.red;
    public GameObject naruto;
    public Renderer[] meshes;
    public Shader solidShader;

    Material solidMaterial;
    CommandBuffer command;
    

    private void Awake()
    {
        command = new CommandBuffer();
        command.name = "Draw Solid Color";
        meshes = naruto.GetComponentsInChildren<Renderer>();
        solidMaterial = new Material(solidShader);
    }

    private void OnEnable()
    {
        command.ClearRenderTarget(true, true, Color.clear);
        foreach(var mesh in meshes)
        {
            command.DrawRenderer(mesh,solidMaterial);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //画有物体轮廓的纯色块。
        solidMaterial.SetColor("_Color", outLineColor);
        RenderTexture solidColorTexture = RenderTexture.GetTemporary(Screen.width, Screen.height);
        Graphics.SetRenderTarget(solidColorTexture);
        Graphics.ExecuteCommandBuffer(command);



        RenderTexture.ReleaseTemporary(solidColorTexture);
    }
}
