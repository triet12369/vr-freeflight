using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFractureController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PostFracture()
    {
        //Debug.Log("HAHAHA");
        //float scaleFactor = 1.5f;
        //Vector3 translation = -gameObject.transform.position;
        //gameObject.transform.Translate(translation);
        //gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        ////gameObject.transform.Translate(-translation);
        //gameObject.transform.position = gameObject.transform.TransformPoint(-translation);
        //gameObject.SetActive(true);
        //gameObject.tag = "Building";
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
