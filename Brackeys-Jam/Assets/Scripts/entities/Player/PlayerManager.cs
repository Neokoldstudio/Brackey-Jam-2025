using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Entity
{
    protected override void Die()
    {
        GameManager.Instance.LoseLevel();
        base.Die();
    }
}
