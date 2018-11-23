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
    }
}
