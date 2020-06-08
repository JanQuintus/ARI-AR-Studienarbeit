using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(RectTransform))]
public class OutlineButton : MonoBehaviour
{
    public enum NeighborDirection
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    [System.Serializable]
    public struct Neighbor
    {
        public NeighborDirection Direction;
        public OutlineButton Target;
    }

    #region Public Variables
    public List<Neighbor> Neighbors;
    public UnityEvent OnButtonPressed;
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;
    public bool IsSelected { get; private set; }
    [HideInInspector]
    public RectTransform RT;
    #endregion

    #region Private Variables
    private Outline _outline;
    #endregion

    void Awake()
    {
        _outline = GetComponent<Outline>();
        RT = GetComponent<RectTransform>();
        Deselect();
    }

    public void Select()
    {
        _outline.effectColor = new Color(_outline.effectColor.r,
            _outline.effectColor.g, _outline.effectColor.b, 0.5f);
        IsSelected = true;
        OnSelected?.Invoke();

    }

    public void Deselect()
    {
        _outline.effectColor = new Color(_outline.effectColor.r,
            _outline.effectColor.g, _outline.effectColor.b, 0);
        IsSelected = false;
        OnDeselected?.Invoke();
    }

    public void Press()
    {
        OnButtonPressed?.Invoke();
    }

    public Neighbor GetNeigborInDirection(NeighborDirection direction) => Neighbors.Find(n => n.Direction == direction);
}
