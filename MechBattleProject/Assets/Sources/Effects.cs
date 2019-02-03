using System.Collections.Generic;
using UnityEngine;

public sealed class Effects : MonoBehaviour {
    private static Effects instance;
    [SerializeField]
    private BlowEffectController blowEffectPrefab;

    private List<BlowEffectController> blowEffectsCache = new List<BlowEffectController>();

    public static void Blow(Vector3 position) {
        var effect = instance.CreateBlowEffect();
        effect.Blow(position);
    }

    private BlowEffectController CreateBlowEffect() {
        foreach (var effect in blowEffectsCache) {
            if (!effect.gameObject.activeSelf) {
                return effect;
            }
        }
        var newEffect = Instantiate(blowEffectPrefab);
        blowEffectsCache.Add(newEffect);
        return newEffect;
    }

    private void Awake() {
        instance = this;
    }
}
