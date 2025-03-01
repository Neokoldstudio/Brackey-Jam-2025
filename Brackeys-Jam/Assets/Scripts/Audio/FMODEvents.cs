using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference submarineAmbience {  get; private set; }
    [field: Header("Music")]
    [field: SerializeField] public EventReference mainThemeOST { get; private set; }
    [field: SerializeField] public EventReference inGameThemeOST { get; private set; }  
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootstepsDry { get; private set; }
    [field: SerializeField] public EventReference playerFootstepsWet { get; private set; }
    [field: SerializeField] public EventReference playerJump { get; private set; }
    [field: SerializeField] public EventReference playerJumpLand { get; private set; }
    [field: SerializeField] public EventReference playerDolphinJump { get; private set; }
    [field: SerializeField] public EventReference playerJumpInWater { get; private set; }
    [field: Header("Gun SFX")]
    [field: SerializeField] public EventReference nailGunShot { get; private set; }
    [field: SerializeField] public EventReference glueGunShot {  get; private set; }

    public static FMODEvents instance {  get; private set; }

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }

}
