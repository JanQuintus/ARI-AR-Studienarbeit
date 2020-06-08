using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashbin : MonoBehaviour
{
    public ParticleSystem ThrowInParticle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ProgramBlock")
        {
            ThrowInParticle.Play();
            Destroy(other.gameObject);
        }
    }
}
