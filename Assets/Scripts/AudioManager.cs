using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton Instance (Allows other scripts to find this easily)
    // This static variable allows you to call AudioManager.instance.PlaySound() from any other script
    public static AudioManager instance;

    [Header("Audio Clips")]
    // Drag your specific audio files into these slots in the Unity Inspector
    public AudioClip collectSound;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    // Reference to the actual component that emits the sound
    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one AudioManager exists (Singleton Pattern)
        if (instance == null)
        {
            instance = this;
            // Keeps this GameObject alive when switching between scenes (e.g., Main Menu -> Level 1)
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // If a duplicate AudioManager exists (e.g., when reloading a scene), destroy this one
            Destroy(gameObject); 
        }

        // Fetch the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        // Only play if a clip is actually assigned!
        // This check prevents errors if you forgot to assign a sound in the Inspector
        if (clip != null)
        {
            // PlayOneShot is used so sounds can overlap (e.g., collecting a coin while jumping)
            // .Play() would cut off the previous sound
            audioSource.PlayOneShot(clip);
        }
    }
}