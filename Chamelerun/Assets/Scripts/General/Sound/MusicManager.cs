using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource Menu;
    public AudioSource Game;
    public AudioSource Lost;
    public AudioSource Pause;
    public float TransitionDuration;
    
    public void OnGameStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        if (oldState != newState)
        {
            TransitionMusic(GameStateToAudioSource(oldState), GameStateToAudioSource(newState));
        }
        else
        {
            GameStateToAudioSource(newState).Play();
        }
    }

    private AudioSource GameStateToAudioSource(GameManager.State state)
    {
        switch (state)
        {
            case GameManager.State.Start:
                return Menu;
            case GameManager.State.Game:
                return Game;
            case GameManager.State.End:
                return Lost;
            case GameManager.State.Pause:
                return Pause;
            default:
                return null;
        }
    }

    private void TransitionMusic(AudioSource from, AudioSource to)
    {
        from.volume = 1f;
        from.DOFade(0f, TransitionDuration);

        to.volume = 0f;
        to.Play();
        to.DOFade(1f, TransitionDuration);
    }
}
