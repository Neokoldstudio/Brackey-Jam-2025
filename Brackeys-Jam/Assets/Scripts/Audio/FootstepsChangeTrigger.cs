using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepChangeTrigger : MonoBehaviour
{
    private int state;
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Equals("water"))
        {
            Debug.Log("check water");
            state = 1;
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        state = 0;
    }

    public int GetState()
    {
        return state;
    }
}
