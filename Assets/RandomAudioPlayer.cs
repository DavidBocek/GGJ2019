using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RandomAudioPlayer : MonoBehaviour
{
    [System.Serializable]
    public struct ClipCollection 
    {
        public string collectionName;
        public float volume;
        public AudioClip[] clips;
    }

    public ClipCollection[] publicClipCollections;
    
    private Dictionary<string, AudioClip[]> clipCollections = new Dictionary<string, AudioClip[]>();
    private Dictionary<string, AudioSource> clipSources = new Dictionary<string, AudioSource>();
    private int lastSoundPlayedIndex = -1;

    public void Awake()
    {
        for ( int i = 0; i < publicClipCollections.Length; ++i )
        {
            clipCollections.Add( publicClipCollections[i].collectionName, publicClipCollections[i].clips );

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            // default value of 0 just translates to 1 whatever dude don't shame my lazy code
            float volume = publicClipCollections[i].volume == 0.0f ? 1.0f : publicClipCollections[i].volume;
            audioSource.volume = volume;

            clipSources.Add( publicClipCollections[i].collectionName, audioSource );
        }
    }

    public AudioSource PlayRandomSound( string clipCollectionName, bool randomProtection )
    {
        Assert.IsTrue( clipCollections.ContainsKey( clipCollectionName ) );
        Assert.IsTrue( clipSources.ContainsKey( clipCollectionName ) );

        int numClips = clipCollections[clipCollectionName].Length;
        Assert.IsTrue( numClips > 0 );

        int randomChoice = Random.Range( 0, numClips );
        while( randomProtection && numClips > 1 && lastSoundPlayedIndex != -1 && randomChoice == lastSoundPlayedIndex )
        {
            randomChoice = Random.Range( 0, numClips );
        }
        
        clipSources[clipCollectionName].clip = clipCollections[clipCollectionName][randomChoice];
        clipSources[clipCollectionName].Play();


        return clipSources[clipCollectionName];
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
