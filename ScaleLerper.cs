using UnityEngine;
using static Console;

public class ScaleLerper : MonoBehaviour
{
    public enum scaleTypes
    {
        linear,
        easeInOutCubic,

        easeInOutQuint,

        easeOutCubic
    }


    [SerializeField] scaleTypes scaleType;
    [SerializeField] Vector3 targetScale;
    Vector3 startScale;
    [SerializeField] float scaleRate;
    bool scaling;
    [SerializeField] float scaleDelay;
    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;
        scaling = false;
        Invoke("StartScaling",scaleDelay);
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed;
        if(scaling)
        {
            currentSpeed = Time.deltaTime * GetCurrentScaleRate(GetProgression(startScale,targetScale));

            transform.localScale = Vector3.Lerp(transform.localScale,targetScale,currentSpeed);

            if (GetProgression(startScale,targetScale) > 0.999)
            {
                scaling = false;
            }
        }
    }

    void StartScaling()
    {
        scaling = true;
    }

    float GetProgression(Vector3 startScale, Vector3 endScale)
    {
        float totalDistance = Vector3.Distance(startScale,endScale);//diistance from start to end
        float currentDistance = Vector3.Distance(transform.localScale,startScale);//distance from start

        return currentDistance / totalDistance;//return as a fraction for progression
    }

    float GetCurrentScaleRate(float progression)
    {
        switch(scaleType)
        {
            case scaleTypes.linear:
                return scaleRate;
            case scaleTypes.easeInOutCubic:
                return easeInOutCubic(progression)*scaleRate;
            case scaleTypes.easeInOutQuint:
                return easeInOutQuint(progression)*scaleRate;
            case scaleTypes.easeOutCubic:
                return easeOutCubic(progression) * scaleRate;
        }
        return scaleRate;
    }
}
