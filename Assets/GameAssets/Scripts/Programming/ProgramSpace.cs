using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramSpace : MonoBehaviour
{
    public ProgramBlockSlot[] Slots;

    public static ProgramSpace Instance;

    private int _currentPC = 0;
    private bool _isExecuting = false;
    private bool _isStopped = false;

    public bool IsExecuting() => _isExecuting;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RemoveFromProgramSpace(AProgramBlock pb)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].GetPB() == pb)
            {
                RemovePB(i);
            }
        }
    }

    public void Execute()
    {
        if(!HasError() && !_isExecuting)
            StartCoroutine(ExecuteCor());
    }

    public void Stop()
    {
        _isStopped = true;
    }

    public void Step()
    {

    }

    private IEnumerator ExecuteCor()
    {
        ARI.Instance.ResetTransform();
        _isExecuting = true;
        for(int i = 0; i < Slots.Length; i++)
        {
            if (_isStopped)
            {
                Slots[_currentPC].Stop();
                _currentPC = 0;
                ARI.Instance.Mover.StopAction();
                ARI.Instance.ResetTransform();
                _isExecuting = false;
                _isStopped = false;
                break;
            }
            _currentPC = i;
            if (Slots[i].HasPB())
            {
                Slots[i].Execute();
                yield return new WaitUntil(() => Slots[i].GetPB().IsExecuting() == false);
            }
        }
        _isExecuting = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("ProgramBlock"))
        {
            if (other.GetComponent<Rigidbody>().useGravity)
            {
                int nextSlotIndex = FindNextSlotIndex(other.transform.position);
                if (nextSlotIndex != -1)
                    InsertPB(other.GetComponent<AProgramBlock>(), nextSlotIndex);
            }
        }
    }

    private int FindNextSlotIndex(Vector3 position)
    {
        int nextPBSIndex = -1;
        float minDist = float.MaxValue;
        for (int i = 0; i < Slots.Length; i++)
        {
            float dist = Vector3.Distance(Slots[i].transform.position, position);
            if (dist <= minDist)
            {
                minDist = dist;
                nextPBSIndex = i;
            }
        }

        return nextPBSIndex;
    }

    private void RemovePB(int slot)
    {
        Slots[slot].AssignBlock(null);
        HasError();
    }

    private void InsertPB(AProgramBlock pb, int slot)
    {
        bool assigend = false;
        if (!Slots[slot].HasPB())
        {
            Slots[slot].AssignBlock(pb);
            assigend = true;
        }
        else
        {
            if (TryShiftRight(slot))
            {
                Slots[slot].AssignBlock(pb);
                assigend = true;
            }
            else if (TryShiftLeft(slot))
            {
                Slots[slot].AssignBlock(pb);
                assigend = true;
            }
        }
        if (!assigend)
        {
            Vector3 force = ProgramBlockGrabber.Instace.transform.position - pb.transform.position;
            force = force.normalized * 1f;
            force.y = 2f;
            pb.GetComponent<Rigidbody>().velocity = Vector3.zero;
            pb.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }
        else
        {
            HasError();
        }
    }

    private bool TryShiftRight(int from)
    {
        for (int i = from; i < Slots.Length; i++)
        {
            if (!Slots[i].HasPB())
            {
                for (int j = i-1; j > from - 1; j--)
                {
                    AProgramBlock toShift = Slots[j].GetPB();
                    Slots[j + 1].AssignBlock(toShift);
                    Slots[j].AssignBlock(null);
                }
                return true;
            }
        }
        return false;
    }

    private bool TryShiftLeft(int from)
    {
        for (int i = from; i > -1; i--)
        {
            if (!Slots[i].HasPB())
            {
                for (int j = i + 1; j < from + 1; j++)
                {
                    AProgramBlock toShift = Slots[j].GetPB();
                    Slots[j - 1].AssignBlock(toShift);
                    Slots[j].AssignBlock(null);
                }
                return true;
            }
        }
        return false;
    }

    private bool HasError()
    {
        bool hasError = false;

        for (int i = 0; i < Slots.Length; i++)
        {
            if (!Slots[i].HasPB())
            {
                bool foundLaterPB = false;
                for (int j = i + 1; j < Slots.Length; j++)
                {
                    if (Slots[j].HasPB())
                    {
                        Slots[i].SetHasError(true);
                        hasError = true;
                        foundLaterPB = true;
                    }
                }
                if(!foundLaterPB)
                    Slots[i].SetHasError(false);
            }
        }
        return hasError;
    }
}
