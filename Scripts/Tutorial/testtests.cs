using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testtests : MonoBehaviour
{
    public RectTransform panel;
    public GameObject image;
    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(panel, image.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
