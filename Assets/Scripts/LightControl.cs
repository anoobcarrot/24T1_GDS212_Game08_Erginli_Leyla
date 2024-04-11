using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightControl : MonoBehaviour
{
    public Light Light;

    private void OnPreCull()
    {
        Light.enabled = false;
    }
    private void OnPreRender()
    {
        Light.enabled = false;
    }
    private void OnPostRender()
    {
        Light.enabled = false;
    }
}