using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    
    public static AudioManager instance {  get; private set; }

    private EventInstance ambienceEventInstance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
    }

    private void Start()
    {
        InitiliazeAmbience(FMODEvents.instance.submarineAmbience);
    }

    private void InitiliazeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isPlayerUnderwater())
        {
            SetAmbienceState(AmbienceState.IS_SUBMERGED);
        }
        else
        {
            SetAmbienceState(AmbienceState.NOT_SUBMERGED);
        }
    }

    public void SetAmbienceState(AmbienceState submerged)
    {
        float submergedValue = (float)submerged;

        ambienceEventInstance.setParameterByName("Submerged", submergedValue);


    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

}
