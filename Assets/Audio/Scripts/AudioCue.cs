using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCue : MonoBehaviour
{
	[Header("Sound definition")]
	[SerializeField] private AudioCueSO _audioCue = default;
	[SerializeField] private bool _playOnStart = false;

	[Header("Configuration")]
	[SerializeField] private AudioCueEventChannelSO _audioCueEventChannel = default;
	[SerializeField] private AudioConfigurationSO _audioConfiguration = default;

	private void Start()
	{
		if (_playOnStart)
			PlayAudioCue();
	}

	public void PlayAudioCue()
	{
		_audioCueEventChannel.RaiseEvent(_audioCue, _audioConfiguration, transform.position);
	}

	public void PlayAudioCue(AudioCueSO cue)
	{
		if (cue == null) return;
		else
		{
			_audioCueEventChannel.RaiseEvent(cue, _audioConfiguration, transform.position);
		}
	}
	

	public void PlayAudioCue(AudioCueSO cue, int chance)
    {
		if (cue == null) return;
		else
		{
			int roll = Random.Range(1, 100);
			if (roll <= chance)
			{
				Debug.Log("chance sound played with roll: " + roll);
				_audioCueEventChannel.RaiseEvent(cue, _audioConfiguration, transform.position);
			}
		}
    }


}
