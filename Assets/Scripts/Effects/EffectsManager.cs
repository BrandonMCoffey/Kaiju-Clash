using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private static EffectsManager _instance;

    [SerializeField] private List<EffectGroup> _explosion;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public static void Explode(Vector3 pos)
    {
        foreach (var group in _instance._explosion)
        {
            group.Effect.transform.position = pos;
            group.Effect.Emit(group.Count);
        }
    }

    [System.Serializable]
    private class EffectGroup
    {
        public ParticleSystem Effect;
        public int Count;
    }
}