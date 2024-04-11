using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class IgnoreFog : MonoBehaviour
{
    // Flag to track whether fog has been toggled
    private bool fogToggled = false;

    // Unity calls this method automatically when it enables this component
    private void OnEnable()
    {
        // Add WriteLogMessage as a delegate of the RenderPipelineManager.beginCameraRendering event
        RenderPipelineManager.beginCameraRendering += BeginRender;
        RenderPipelineManager.endCameraRendering += EndRender;
    }

    // Unity calls this method automatically when it disables this component
    private void OnDisable()
    {
        // Remove WriteLogMessage as a delegate of the  RenderPipelineManager.beginCameraRendering event
        RenderPipelineManager.beginCameraRendering -= BeginRender;
        RenderPipelineManager.endCameraRendering -= EndRender;
    }

    // When this method is a delegate of RenderPipeline.beginCameraRendering event, Unity calls this method every time it raises the beginCameraRendering event
    void BeginRender(ScriptableRenderContext context, Camera camera)
    {
        // Check if the camera is in spectral mode
        bool isSpectralMode = CameraController.spectralMode;

        if (!isSpectralMode && camera.name == "Camera")
        {
            Debug.Log("Turn fog on for " + camera.name);
            RenderSettings.fog = true;
            fogToggled = true; // Set flag to true to indicate fog has been toggled
        }
        if (camera.name == "MainCamera")
        {
            Debug.Log("Turn fog on for " + camera.name);
            RenderSettings.fog = true;
            fogToggled = true; // Set flag to true to indicate fog has been toggled
        }
    }

    void EndRender(ScriptableRenderContext context, Camera camera)
    {
        bool isSpectralMode = CameraController.spectralMode;

        // Check if fog has already been toggled
        if (isSpectralMode && camera.name == "Camera")
        {
            Debug.Log("Turn fog off for " + camera.name);
            RenderSettings.fog = false;
            fogToggled = false; // Reset flag to false
        }
        if (camera.name == "MainCamera")
        {
            Debug.Log("Turn fog off for " + camera.name);
            RenderSettings.fog = false;
            fogToggled = false; // Reset flag to false
        }
    }
}



