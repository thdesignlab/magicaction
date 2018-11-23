using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{
    [SerializeField]
    private float start;

    private ParticleSystem myParticle;
    private ParticleSystem[] myParticles;

    protected void Awake()
    {
        myParticle = GetComponent<ParticleSystem>();
        myParticle.Simulate(start, true);
        myParticle.Play();
        //myParticles = transform.GetComponentsInChildren<ParticleSystem>();
        //Debug.Log(myParticles.Length);
        //foreach (ParticleSystem p in myParticles)
        //{
        //    p.Simulate
        //}
    }
}
