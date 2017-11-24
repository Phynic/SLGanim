using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class RenderBlurOutline : MonoBehaviour
{
    public Color outLineColor = Color.blue;
    //private int offsetPixel = 2;
    public GameObject naruto;
    public Renderer[] meshes;
    public Shader solidShader;
    public Shader postOutLineShader;
    public Shader combineShader;

    public float blurScale = 1f;
    int blurIterCount = 1;
    Material solidMaterial;
    Material postOutLineMaterial;
    Material combineMaterial;
    CommandBuffer command;
    

    private void Awake()
    {
        command = new CommandBuffer();
        command.name = "Draw Solid Color";
        meshes = naruto.GetComponentsInChildren<Renderer>();
        solidMaterial = new Material(solidShader);
        postOutLineMaterial = new Material(postOutLineShader);
        combineMaterial = new Material(combineShader);
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

        RenderTexture mBlurSilhouette = RenderTexture.GetTemporary(Screen.width >> 1, Screen.height >> 1);
        Graphics.Blit(solidColorTexture, mBlurSilhouette, postOutLineMaterial, 0);
        

        
        RenderTexture blurTemp = RenderTexture.GetTemporary(Screen.width >> 1, Screen.height >> 1);
        
        postOutLineMaterial.SetFloat("_DownSampleValue", blurScale);
        for (int i = 0; i < blurIterCount; ++i)
        {
            Graphics.Blit(mBlurSilhouette, blurTemp, postOutLineMaterial, 1);//vertical blur
            Graphics.Blit(blurTemp, mBlurSilhouette, postOutLineMaterial, 2);//horizontal blur
            
        }
        combineMaterial.SetTexture("_Source", source);
        combineMaterial.SetTexture("_g_SolidColor", solidColorTexture);
        combineMaterial.SetTexture("_g_mBlur", mBlurSilhouette);
        
        Graphics.Blit(source, destination,combineMaterial);

        
        RenderTexture.ReleaseTemporary(mBlurSilhouette);
        RenderTexture.ReleaseTemporary(blurTemp);
        RenderTexture.ReleaseTemporary(solidColorTexture);
    }
}
