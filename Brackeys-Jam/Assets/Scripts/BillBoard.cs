using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BillBoard : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(90, 180, 0);
    }
}
