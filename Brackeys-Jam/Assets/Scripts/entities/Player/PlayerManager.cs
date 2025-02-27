using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Entity
{
    public Transform playerTarget;

    public override void GetHit(float damage)
    {
        base.GetHit(damage);
        FindObjectOfType<ScreenFlash>().Flash();
        CinemachineShake.Instance.Shake(5f, 0.2f);
    }

    protected override void Die()
    {
        GameManager.Instance.LoseLevel();
    }

    public Transform GetPlayerTarget()
    {
        return playerTarget;
    }
}
