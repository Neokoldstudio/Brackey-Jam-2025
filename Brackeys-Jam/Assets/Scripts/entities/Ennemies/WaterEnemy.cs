using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        float waterHeight = WaterSystem.Instance.getWaterPlaneHeight();
        if (transform.position.y > waterHeight)
        {
            transform.position = new Vector3(transform.position.x, waterHeight, transform.position.z);
        }
        base.Update();
    }
}
