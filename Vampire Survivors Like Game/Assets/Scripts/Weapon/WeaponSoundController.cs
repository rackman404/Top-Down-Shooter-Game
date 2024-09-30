using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundController : MonoBehaviour
{

    AudioClip weaponTriggerSFX;

    AudioSource weaponAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        weaponAudioSource = gameObject.AddComponent<AudioSource>();
        weaponAudioSource.loop = false;
        weaponAudioSource.volume = 0.05f;

        weaponTriggerSFX = Resources.Load<AudioClip>("Sound/270396_spell_01");
    }

    public void FireTriggerSFX()
    {
        weaponAudioSource.clip = weaponTriggerSFX;
        weaponAudioSource.Play();
    }
}
