using UnityEngine;
[CreateAssetMenu(fileName = "Sound", menuName = "Audio Manager/Sound")]
public class Sound : ScriptableObject
{
    public string soundName;
    public AudioClip clip;
    public bool loop;

    [Range(0, 1)]
    public float volume;
    [Range(0.1f, 3)]
    public float pitch;
}