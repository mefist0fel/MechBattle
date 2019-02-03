using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    [SerializeField]
    private float hitRadius = 1f;
    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private TrailRenderer trail;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float delayTimer;
    private float flyTime;
    private float timer;
    private float distance;

    private void Start() { }

    public void Fire(Vector3 target, Vector3 start, float delay = 0) {
        gameObject.SetActive(true);
        startPosition = start;
        targetPosition = target;
        delayTimer = delay;
        distance = Vector3.Distance(target, start);
        flyTime = distance / speed;
        timer = flyTime;
        transform.position = start;
        if (trail != null)
            trail.Clear();
    }
	
	private void Update () {
        if (delayTimer > 0) {
            delayTimer -= Time.deltaTime;
            return;
        }
        if (timer > 0) {
            timer -= Time.deltaTime;
            var normalizedTime = timer / flyTime;
            var centralTime = normalizedTime * 2f - 1f;
            transform.position = Vector3.Lerp(targetPosition, startPosition, normalizedTime) + Vector3.up * (distance * 0.4f * (1f - centralTime * centralTime));
        } else {
            gameObject.SetActive(false);
            MechController.BlowMechsInPosition(targetPosition, hitRadius, 1);
            Effects.Blow(targetPosition);
            Game.Stress(0.3f);
        }
	}
}
