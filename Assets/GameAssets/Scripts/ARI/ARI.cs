using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ARIMover))]
public class ARI : MonoBehaviour
{
    public static ARI Instance;

    #region Public Variables
    public ARIMover Mover { get; private set; }
    #endregion

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;


    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        Mover = GetComponent<ARIMover>();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    public void ResetTransform()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }
}
