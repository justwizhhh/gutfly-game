using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResizer : MonoBehaviour
{
    // ----------------------
    //
    // Class for resizing the game's window screen, at any point during the game
    // TO-DO: add toggleable screen sizes when you press F4 multiple times maybe
    //
    // ----------------------

    public RenderTexture FullscreenTexture;
    public RenderTexture WindowedTexture;

    [Space(10)]
    public Camera Cam;
    public RawImage GameDisplay;

    private void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1280, 720, false);
        Cam.targetTexture = WindowedTexture;
        GameDisplay.texture = WindowedTexture;

        PlayerPrefs.SetInt("IsFullScreen", 0);
    }

    private void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(1920, 1080, true);
        Cam.targetTexture = FullscreenTexture;
        GameDisplay.texture = FullscreenTexture;

        PlayerPrefs.SetInt("IsFullScreen", 1);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("IsFullScreen"))
        {
            if (PlayerPrefs.GetInt("IsFullScreen") == 0)
            {
                SetWindowed();
            }
            else
            {
                SetFullscreen();
            }
        }
        else
        {
            SetWindowed();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (Screen.fullScreenMode != FullScreenMode.FullScreenWindow)
            {
                SetFullscreen();
            }
            else
            {
                SetWindowed();
            }
        }
    }
}
