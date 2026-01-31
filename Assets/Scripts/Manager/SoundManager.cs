using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clip")]
    public AudioClip doorOpening;
    public AudioClip diamondPickup;
    public AudioClip carStarting;

    [Header("Theme music")]
    public float fadeDuration = 2.0f;
    public AudioClip startMusic;

    [Header("Party Playlist")]
    public AudioClip partyMusic1;
    public AudioClip partyMusic2;
    public AudioClip partyMusic3;
    private AudioClip[] partyPlaylist; // Internal array to hold the playlist

    [Header("Other Music")]
    public AudioClip chaseMusic;
    public AudioClip winMusic;

    [Header("Player")]
    public AudioClip player;

    [Header("Attendee")]
    public AudioClip attendeeHit;
    public AudioClip attendeeSusPlayer;
    public AudioClip attendeeFoundPlayer;

    [Header("Event")]
    public AudioClip gameOver;

    [Header("UI")]
    public AudioClip buttonClick;

    // Track the currently running music routine so we can stop it if we switch modes
    private Coroutine currentMusicRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize the playlist array
            partyPlaylist = new AudioClip[] { partyMusic1, partyMusic2, partyMusic3 };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays a single looped track (Start, Chase, Win, etc.)
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        // 1. If a party playlist is running, stop it.
        if (currentMusicRoutine != null)
        {
            StopCoroutine(currentMusicRoutine);
            currentMusicRoutine = null;
        }

        // 2. Standard single track logic
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true; // Single tracks usually loop endlessly
        musicSource.volume = 0f;
        musicSource.Play();

        // Start the fade in
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Starts cycling through the 3 party songs
    /// </summary>
    public void PlayPartyMusic()
    {
        // If we are already running a routine, stop it to restart fresh
        if (currentMusicRoutine != null) StopCoroutine(currentMusicRoutine);

        // Start the playlist coroutine
        currentMusicRoutine = StartCoroutine(PlayPartyLoop());
    }

    /// <summary>
    /// Coroutine that handles alternating between songs
    /// </summary>
    IEnumerator PlayPartyLoop()
    {
        int playlistIndex = 0;

        // Infinite loop to keep the playlist going forever
        while (true)
        {
            AudioClip clipToPlay = partyPlaylist[playlistIndex];

            if (clipToPlay != null)
            {
                musicSource.clip = clipToPlay;
                musicSource.loop = false; // Important: Must be false so the track ends!
                musicSource.volume = 0f;
                musicSource.Play();

                // Fade in the track
                yield return StartCoroutine(FadeIn());

                // Wait while the music is playing
                // We use isPlaying check instead of WaitForSeconds to be more accurate if game lags
                while (musicSource.isPlaying)
                {
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarning("SoundManager: Found a null clip in Party Playlist.");
                yield return null; // Avoid infinite freeze if clip is missing
            }

            // Move to next index, wrap around to 0 if at the end
            playlistIndex = (playlistIndex + 1) % partyPlaylist.Length;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: Attempted to play a null AudioClip.");
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
        // Debug.Log("Playing SFX: " + clip.name); // Commented out to reduce console spam
    }

    IEnumerator FadeIn()
    {
        float currentTime = 0;
        float startVolume = musicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 1f, currentTime / fadeDuration);
            yield return null;
        }

        musicSource.volume = 1f;
    }
}