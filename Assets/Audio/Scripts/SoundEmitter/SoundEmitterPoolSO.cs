using System.Linq;
using UnityEngine;
using Pool;
using Factory;

[CreateAssetMenu(fileName = "NewSoundEmitterPool", menuName = "Audio/SoundEmitter Pool")]
public class SoundEmitterPoolSO : ComponentPoolSO<SoundEmitter>
{
	[SerializeField]
	private SoundEmitterFactorySO _factory;

	public override IFactory<SoundEmitter> Factory
	{
		get
		{
			return _factory;
		}
		set
		{
			_factory = value as SoundEmitterFactorySO;
		}
	}
}
