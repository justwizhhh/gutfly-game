using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuButton
{
    public void OnHovered();
    public void OnLeave();
    public void OnSelected();
}
