using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // ----------------------
    //
    // Class for controlling the behaviour of the level camera, and making sure its rotation is alinged with that of the LevelController
    //
    // ----------------------

    private CinemachineBrain brain;

    private void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }

    private void FixedUpdate()
    {
        if (brain.ActiveVirtualCamera != null)
        {
            brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.eulerAngles = new Vector3(0, 0, LevelController.Instance.UpAngle);
        }
    }
}
