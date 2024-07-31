using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dimension",menuName = "My Assets/Dimension",order = 0)]
public class Dimension : ScriptableObject
{
    public GameObject background;
    [Space]
    public PlayerController player;
    [Space]
    [SerializedDictionary("Key","Object")]
    public SerializedDictionary<string,GameObject> objects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
