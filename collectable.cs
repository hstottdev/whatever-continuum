using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectable : MonoBehaviour
{

    public enum collectableType
    {
        major,
        minor,
        whatever
    }
    [SerializeField] collectableType type;
    [SerializeField] List<RuntimeAnimatorController> randomAnimatorPool;
    [SerializeField] GameObject particle;
    [SerializeField] string sfxFile;
    [SerializeField] float sfxVolume = 1;
    bool collected;
    // Start is called before the first frame update
    void Start()
    {
        if (randomAnimatorPool.Count > 0 && TryGetComponent(out Animator ani))
        {
            int index = Random.Range(0, randomAnimatorPool.Count);
            ani.runtimeAnimatorController = randomAnimatorPool[index];
        }

        switch (type)
        {
            case collectableType.minor:
                LevelManager.totalMinor += 1;
                break;
            case collectableType.major:
                LevelManager.totalMajor += 1;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.mode == LevelMode.whatever)
        {
            if(type != collectableType.whatever)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !collected)
        {
            collected = true;

            if (particle != null)
            {
                Instantiate(particle, transform.position, particle.transform.rotation);

                if(sfxFile != null)
                {
                    AudioManager.PlaySound(sfxFile,Random.Range(0.9f,1.1f),sfxVolume);
                }
                //StartCoroutine(Console.Rumble(0.3f, 0.6f));
            }

            switch (type)
            {
                case collectableType.minor:
                    LevelManager.minorFound += 1;
                    break;
                case collectableType.major:
                    LevelManager.majorFound += 1;
                    break;
            }

            Destroy(gameObject);
        }
    }
}
