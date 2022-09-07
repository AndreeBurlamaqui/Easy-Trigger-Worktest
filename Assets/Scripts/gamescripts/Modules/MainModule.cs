using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Module will be the parent of all modules and will contain general stuff that most of them might need
/// </summary>
public abstract class MainModule : MonoBehaviour
{
    public bool IsActive => _active;
    public bool IsPlayer => Unit.isPlayer;

    bool _active = true;

    [HideInInspector] public UnitData Unit; 

    [HideInInspector] public Main main;
    [HideInInspector] public Game game;
    [HideInInspector] public Gfx gfx;
    [HideInInspector] public Snd snd; 

    public virtual void InitializeModule(UnitData _unit)
    {
        Unit = _unit;

        main = FindObjectOfType<Main>();
        game = FindObjectOfType<Game>();
        gfx = FindObjectOfType<Gfx>();
        snd = FindObjectOfType<Snd>();
    }

    /// <summary>
    /// Play an audio by its name
    /// </summary>
    public void PlayAudio(string targetAudio)
    {
        snd.PlayAudioClip(targetAudio);
    }

    public virtual void UpdateModule() { }
}
