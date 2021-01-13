using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factory;


[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Audio/SoundEmitter Factory")]
public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
{
	public SoundEmitter prefab = default;

	public override SoundEmitter Create()
	{
		return Instantiate(prefab);
	}
}
