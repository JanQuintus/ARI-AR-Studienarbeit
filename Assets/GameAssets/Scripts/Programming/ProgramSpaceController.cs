using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramSpaceController : MonoBehaviour
{
    private Animator _animator;
    public static ProgramSpaceController Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _animator = GetComponent<Animator>();
    }

    public void Execute()
    {
        _animator.SetBool("IsPlaying", true);
        ProgramSpace.Instance.Execute();
        StartCoroutine(WaitForExecutionDone());
    }

    private IEnumerator WaitForExecutionDone()
    {
        yield return new WaitUntil(() => ProgramSpace.Instance.IsExecuting() == false);
        _animator.SetBool("IsPlaying", false);
    }

    public void Stop()
    {
        _animator.SetBool("IsPlaying", false);
        _animator.SetTrigger("Stop");
        ProgramSpace.Instance.Stop();
    }

    public void Step()
    {
        _animator.SetBool("IsPlaying", false);
        _animator.SetTrigger("Step");
        ProgramSpace.Instance.Step();
    }
}
