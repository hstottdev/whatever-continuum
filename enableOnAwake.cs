using UnityEngine;

public class enableOnAwake : MonoBehaviour
{
    [SerializeField] GameObject targetObject;
    // Start is called before the first frame update
    void Start()
    {
        targetObject.SetActive(true);
    }
}
