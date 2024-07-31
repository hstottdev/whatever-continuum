using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableHitbox : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToEnable;
    [SerializeField] bool setActive = true;
    [SerializeField] bool destroyOnToggle = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == PlayerController.instance.gameObject)
        {
            foreach(GameObject ob in objectsToEnable)
            {
                ob.SetActive(setActive);
                if (destroyOnToggle)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
