using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    TextMeshProUGUI tmp;
    [SerializeField] int _decimalPlaces;
    public float startValue;
    public enum countType
    {
        down,
        up,
        stop,
    }
    public countType count;
    countType startCountType;
    [Header("Limits")]
    [SerializeField] float maximum = 100;
    [SerializeField] float minimum = 0;

    [Header("Delay")]
    [SerializeField] float startDelay;

    [HideInInspector] public float time;

    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        string numberText = Console.RoundToDP(time, _decimalPlaces).ToString();
        if (!numberText.Contains(".")) numberText += ".00";
        if (numberText.Split('.')[0].Length == 1) numberText = "0" + numberText;
        tmp.text = numberText;
        switch (count)
        {
            case countType.up:
                Count(Time.deltaTime);
                break;
            case countType.down:
                Count(-Time.deltaTime);
                break;
            case countType.stop:
                if (!(time == minimum || time == maximum))
                {
                    count = startCountType;
                }
                break;
        }       
    }
    public bool ChangeStartValue(float change,float min = 0,float max = 1000)
    {
        startValue += change;
        if(startValue < min)
        {
            startValue = min;
            return false;
        }
        if (startValue > max)
        {
            startValue = max;
            return false;
        }
        return true;
    }

    void Count(float amount)
    {
        time += amount;
        if(time < minimum)
        {
            time = minimum;
            StopCount();

        }
        else if(time > maximum)
        {
            time = maximum;
            StopCount();
        }
    }

    void StopCount()
    {
        switch (count)
        {
            case countType.up:
                startCountType = countType.up;
                break;
            case countType.down:
                startCountType = countType.down;
                break;
        }
        count = countType.stop;
    }
    void StartCount()
    {
        count = startCountType;
    }
    

    public float GetStartValue()
    {
        return startValue;
    }

    public void Reset()
    {
        time = startValue;
        StopCount();
        Invoke("StartCount", startDelay);
    }
}
