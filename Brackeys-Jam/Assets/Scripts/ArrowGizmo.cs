using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGizmo : MonoBehaviour
{


    public bool DrawX;
    public bool DrawY;
    public bool DrawZ;
    public float Length;
    public float HeadSize;


    void Start()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        Vector3 arrowHead;
        Vector3 arrowRim;
        if (DrawX)
        {
            Gizmos.color = Color.red;

            arrowHead = transform.position + Length * transform.right;
            arrowRim = transform.position + Length * 0.66f * transform.right;

            Gizmos.DrawLine(transform.position, arrowHead);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.up * Length * HeadSize, arrowHead, arrowRim - transform.up * Length * HeadSize }, true);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.forward * Length * HeadSize, arrowHead, arrowRim - transform.forward * Length * HeadSize }, true);
        }
        if (DrawY)
        {
            Gizmos.color = Color.red;

            arrowHead = transform.position + Length * transform.up;
            arrowRim = transform.position + Length * 0.66f * transform.up;

            Gizmos.DrawLine(transform.position, arrowHead);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.right * Length * HeadSize, arrowHead, arrowRim - transform.right * Length * HeadSize }, true);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.forward * Length * HeadSize, arrowHead, arrowRim - transform.forward * Length * HeadSize }, true);
        }
        if (DrawZ)
        {
            Gizmos.color = Color.blue;

            arrowHead = transform.position + Length * transform.forward;
            arrowRim = transform.position + Length * 0.66f * transform.forward;

            Gizmos.DrawLine(transform.position, arrowHead);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.up * Length * HeadSize, arrowHead, arrowRim - transform.up * Length * HeadSize }, true);
            Gizmos.DrawLineStrip(new Vector3[3] { arrowRim + transform.right * Length * HeadSize, arrowHead, arrowRim - transform.right * Length * HeadSize }, true);
        }
    }
}