using UnityEngine;

public class ExclusiveMode : MonoBehaviour
{
    [SerializeField] LevelMode mode;
    [SerializeField] bool activeInMode;
    [SerializeField] bool fadeIfOnScreen;
    [SerializeField] bool debug;
    // Start is called before the first frame update
    void Awake()
    {

    }

    bool IsVisible()
    {
            if(GetComponent<SpriteRenderer>() != null)
            {
                if(GetComponent<SpriteRenderer>().isVisible)
                {
                    return true;
                }
            }
        return false;
    }

    // Update is called once per framw
    void Update()
    {
        bool inMode = LevelManager.mode == mode;
        bool atTargetState = gameObject.activeInHierarchy == activeInMode;

        if(inMode && !atTargetState)
        {
            if(fadeIfOnScreen && activeInMode == false && IsVisible())
            {
                colourFadeRenderer.FadeObject(gameObject);
                if(debug) Debug.Log("fading out exclusive");
                Destroy(this);
            }

            if(!IsVisible())
            {
                gameObject.SetActive(activeInMode);
                if (debug) Debug.Log("setting state of exclusive");
            }    
        }
    }
}
