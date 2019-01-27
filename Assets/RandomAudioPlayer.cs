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
        public AudioClip[] clips;
    }

    public ClipCollection[] publicClipCollections;
    
    private Dictionary<string, AudioClip[]> clipCollections = new Dictionary<string, AudioClip[]>();
    private AudioSource audioSource;
    private int lastSoundPlayedIndex = -1;

    public void Awake()
    {
        for ( int i = 0; i < publicClipCollections.Length; ++i )
        {
            clipCollections.Add( publicClipCollections[i].collectionName, publicClipCollections[i].clips );
        }
    }

    public void PlayRandomSound( string clipCollectionName, bool randomProtection )
    {
        Assert.IsTrue( clipCollections.ContainsKey( clipCollectionName ) );

        int numClips = clipCollections[clipCollectionName].Length;
        Assert.IsTrue( numClips > 0 );

        int randomChoice = Random.Range( 0, numClips );
        while( randomProtection && numClips > 1 && lastSoundPlayedIndex != -1 && randomChoice == lastSoundPlayedIndex )
        {
            randomChoice = Random.Range( 0, numClips );
        }
        
        audioSource.clip = clipCollections[clipCollectionName][randomChoice];
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {     
        audioSource = gameObject.GetComponent<AudioSource>();
        Assert.AreNotEqual( audioSource, null );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
