using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class RenderBlurOutline : MonoBehaviour
{
    public List<Color> playerColorList = new List<Color>();
    public Shader solidShader;
    public Shader postOutLineShader;
    public Shader combineShader;
    
    GameObject character;
    Renderer[] meshes;
    public Color outLineColor = Color.black;
    float blurScale = 1f;
    int blurIterCount = 1;
    Material solidMaterial;
    Material postOutLineMaterial;
    Material combineMaterial;
    CommandBuffer command;
    
    private void Awake()
    {
        command = new CommandBuffer();
        command.name = "Draw Solid Color";
        
        solidMaterial = new Material(solidShader);
        postOutLineMaterial = new Material(postOutLineShader);
        combineMaterial = new Material(combineShader);
    }

    public void RenderOutLine(Transform character)
    {
        if(meshes != null)
        {
            CancelRender();
            
            RoundManager.GetInstance().Invoke(() => { RenderOutLine(character); }, 0.02f);
        }
        else
        {
            meshes = character.GetComponent<Unit>().rend;
            DOTween.To(() => outLineColor, x => outLineColor = x, playerColorList[character.GetComponent<CharacterStatus>().playerNumber], 0.2f);
            command.ClearRenderTarget(true, true, Color.clear);
            foreach (var mesh in meshes)
            {
                command.DrawRenderer(mesh, solidMaterial);
            }
        }
    }

    public void CancelRender()
    {
        DOTween.To(() => outLineColor, x => outLineColor = x, Color.black, 0.01f);
        RoundManager.GetInstance().Invoke(() => { meshes = null; }, 0.01f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(meshes != null)
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

            Graphics.Blit(source, destination, combineMaterial);

            RenderTexture.ReleaseTemporary(mBlurSilhouette);
            RenderTexture.ReleaseTemporary(blurTemp);
            RenderTexture.ReleaseTemporary(solidColorTexture);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}