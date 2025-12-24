using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public const float MUSIC_VOLUME_DEFAULT = 0.8f;
    public const float SFX_VOLUME_DEFAULT = 0.8f;
    public const float MOUSE_SENSITIVITY_DEFAULT = 1.0f;
    public const float MIN_MOUSE_SENSITVITY = 0.1f;
    public const float MAX_MOUSE_SENSITVITY = 10.0f;
    public const string LANGUAGE_DEFAULT = "English";

    [Header("Audio")]
    [Range(0f, 1f)] public float musicVolume;
    [Range(0f, 1f)] public float sfxVolume;

    [Header("Language")]
    public Language gameLanguage;

    [Header("Gameplay")]
    [Range(MIN_MOUSE_SENSITVITY, MAX_MOUSE_SENSITVITY)] public float mouseSensitivity;

}
