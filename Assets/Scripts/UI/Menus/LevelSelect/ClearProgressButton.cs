using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearProgressButton : MonoBehaviour, IMenuButton
{
    public void OnHovered()
    {
        // Nothing
    }

    public void OnLeave()
    {
        // Nothing
    }

    public void OnSelected()
    {
        PlayerProgressTracker.ClearAllProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
