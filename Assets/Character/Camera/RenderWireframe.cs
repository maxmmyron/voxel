using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderWireframe : MonoBehaviour
{
    [SerializeField]
    private bool renderWireframe = false;

    void OnPreRender()
    {
        GL.wireframe = renderWireframe;
    }
    void OnPostRender()
    {
        GL.wireframe = false;
    }
}
