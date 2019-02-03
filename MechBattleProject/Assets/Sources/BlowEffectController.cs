using UnityEngine;

public sealed class BlowEffectController : MonoBehaviour {
    [SerializeField]
    private float blowTime = 0.3f;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 1, 1f, 0f);
    [SerializeField]
    private float scale = 2f;

    private float timer = 0;

    public void Blow(Vector3 position) {
        transform.position = position;
        timer = blowTime;
        gameObject.SetActive(true);
	}
	
	private void Update () {
        if (timer > 0) {
            timer -= Time.deltaTime;
            transform.localScale = Vector3.one * scale * curve.Evaluate(1f - timer / blowTime);
        } else {
            gameObject.SetActive(false);
        }
	}
}
