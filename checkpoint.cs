using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class checkpoint : MonoBehaviour
{


    public static UnityEvent OnGetCheckpoint = new UnityEvent();
    [SerializeField] SpriteRenderer staticVisual;
    bool activeCheckpoint;
    [SerializeField] Color activeColor = Color.green;
    [SerializeField] Color inactiveColor = Color.red;
    [SerializeField] GameObject flag;
    [SerializeField] ParticleSystem ps;
    bool off;

    private void Start()
    {
        SetInactiveCheckpoint();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController player))
        {
            if (!player.dead && !activeCheckpoint && LevelManager.mode == LevelMode.regular)
            {
                PlayerController.respawnPoint = transform.position;
                activeCheckpoint = true;
                flag.SetActive(true);
                staticVisual.color = activeColor;
                ps.Play();
                AudioManager.PlaySound("checkpoint", 3, 0.5f);

                OnGetCheckpoint.Invoke();
                OnGetCheckpoint.AddListener(SetInactiveCheckpoint);
                OnGetCheckpoint.AddListener(OnGetCheckpoint.RemoveAllListeners);
            }
        }
    }

    private void Update()
    {
        if(LevelManager.mode == LevelMode.whatever &&!off)
        {
            TurnOff();
        }
    }

    void TurnOff()
    {
        off = true;
        staticVisual.color = Color.clear;
        flag.SetActive(false);
    }

    void SetInactiveCheckpoint()
    {
        staticVisual.color = inactiveColor;
        activeCheckpoint = false;
        flag.SetActive(false);
    }
}
