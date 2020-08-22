using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SSS_buffers_viewer : MonoBehaviour
{

    public Texture InputBuffer;
    // [ImageEffectOpaque]//con esto se ejecuta antes de stack
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (InputBuffer)
            UnityEngine.Graphics.Blit(InputBuffer, destination);
        else
            UnityEngine.Graphics.Blit(source, destination);
    }
}