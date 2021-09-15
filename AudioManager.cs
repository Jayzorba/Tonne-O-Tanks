using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioPrefab;
    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(transform);
        }
    }

    public void Play3D(AudioClip clip, Vector3 position, float pitchOffset = 0f)
    {
        if(clip == null)
        {
            return;
        }
        pitchOffset = Random.Range(1 - pitchOffset, 1 + pitchOffset);
        var audioGameObject = Instantiate(audioPrefab, position, Quaternion.identity);
        var audioSourceComponent = audioGameObject.GetComponent<AudioSource>();

        audioSourceComponent.clip = clip;
        audioSourceComponent.pitch = pitchOffset;
        audioSourceComponent.Play();

        Destroy(audioGameObject,clip.length);
    }
}
