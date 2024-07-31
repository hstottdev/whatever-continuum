using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Console;

public class MouseLerp : MonoBehaviour
{
    public enum movementTypes
    {
        linear,
        easeInOutCubic,

        easeInOutQuint,

        easeOutCubic
    }
    [SerializeField] movementTypes movementType;
    [SerializeField] float lerpSpeed;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cursorLocation;
        float currentSpeed = 0;
        if(InputChecker.Instance.GetInputType() == InputChecker.InputType.MouseKeyboard)
        {
            cursorLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentSpeed = GetCurrentLerpSpeed(GetProgression(transform.position,cursorLocation));
        }
        else
        {
            if(EventSystem.current.currentSelectedGameObject != null)
            {
                float offset = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().rect.width / 75;
                cursorLocation = EventSystem.current.currentSelectedGameObject.transform.position + new Vector3(-offset,0,0);
                currentSpeed = lerpSpeed;
            }
            else
            {
                cursorLocation = startPos;
                transform.position = cursorLocation;                
            }
        }
             
        transform.position = Vector3.Lerp(transform.position,cursorLocation, Time.deltaTime*currentSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    float GetProgression(Vector3 start, Vector3 end)
    {
        float totalDistance = Vector3.Distance(start, end);//diistance from start to end
        float currentDistance = Vector3.Distance(transform.localPosition, start);//distance from start

        return currentDistance / totalDistance;//return as a fraction for progression
    }

    float GetCurrentLerpSpeed(float progression)
    {
        switch (movementType)
        {
            case movementTypes.linear:
                return lerpSpeed;
            case movementTypes.easeInOutCubic:
                return easeInOutCubic(progression) * lerpSpeed;
            case movementTypes.easeInOutQuint:
                return easeInOutQuint(progression) * lerpSpeed;
            case movementTypes.easeOutCubic:
                return easeOutCubic(progression) * lerpSpeed;
        }
        return lerpSpeed;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

}
