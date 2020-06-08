using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgramBlockSlot : MonoBehaviour
{
    public Color DefaultColor;
    public Color AssignedColor;
    public Color ErrorColor;

    private AProgramBlock _assignedPB;
    private Renderer[] _renderer;

    private void Awake()
    {
        _renderer = GetComponentsInChildren<Renderer>();
    }

    public bool HasPB() => _assignedPB != null;
    public AProgramBlock GetPB() => _assignedPB;

    public void AssignBlock(AProgramBlock programBlock)
    {
        if(programBlock == null)
        {
            foreach (Renderer renderer in _renderer)
                renderer.material.DOColor(DefaultColor, 0.2f);
            
            _assignedPB = null;
            return;
        }

        foreach(Renderer renderer in _renderer)
            renderer.material.DOColor(AssignedColor, 0.2f);

        programBlock.transform.DOMove(transform.position, 0.2f);
        programBlock.transform.rotation = transform.rotation;
        programBlock.SetState(AProgramBlock.BlockState.IN_SLOT);
        _assignedPB = programBlock;
    }

    public void Execute()
    {
        if (HasPB())
            _assignedPB.Execute();
    }

    public void Stop()
    {
        
    }

    public void SetHasError(bool hasError)
    {
        if (hasError)
        {
            foreach (Renderer renderer in _renderer)
                renderer.material.DOColor(ErrorColor, 0.2f);
        }
        else
        {
            if (HasPB())
            {
                foreach (Renderer renderer in _renderer)
                    renderer.material.DOColor(AssignedColor, 0.2f);
            }
            else
            {
                foreach (Renderer renderer in _renderer)
                    renderer.material.DOColor(DefaultColor, 0.2f);
            }
        }
    }
}
