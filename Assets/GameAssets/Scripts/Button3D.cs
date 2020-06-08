using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public UnityEvent OnButtonPressed;

    public void Press()
    {
        OnButtonPressed?.Invoke();
    }
}
