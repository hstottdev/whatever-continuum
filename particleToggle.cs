using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleToggle : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particlesToToggle;
    [SerializeField] bool toggle = true;
    [SerializeField] bool destroyOnToggle = true;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerController.instance.gameObject)
        {
            foreach (ParticleSystem ps in particlesToToggle)
            {
                if (toggle)
                {
                    ps.Play();
                }
                else
                {
                    ps.Stop();
                }
            }
        }
    }
}
