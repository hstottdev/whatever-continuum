using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourMatcher : MonoBehaviour
{
    [SerializeField] Graphic graphicToMatch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TryGetComponent(out SpriteRenderer rend))
        {
            rend.color = graphicToMatch.color;
        }

        if (TryGetComponent(out Graphic grap))
        {
            grap.color = graphicToMatch.color;
        }
    }
}
