using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Entity
{
    public Transform playerTarget;

    protected override void Die()
    {
        GameManager.Instance.LoseLevel();
    }

    public Transform GetPlayerTarget()
    {
        return playerTarget;
    }
}
