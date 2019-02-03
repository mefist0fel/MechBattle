using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MechController : MonoBehaviour {
    [SerializeField]
    private int maxLives = 3;
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private WeaponController weapon; // Set from editor
    [SerializeField]
    private Material deathMaterial; // Set from editor

    public int Lives { get; private set; }

    public Vector3 TargetPosition { get; private set; }
    private static List<MechController> livedMechs = new List<MechController>();

    public void MoveTo(Vector3 position) {
        TargetPosition = position;
    }

    public void FireTo(Vector3 position) {
        weapon.FireToPoint(position);
    }

    public static void BlowMechsInPosition(Vector3 position, float radius, int damage = 3) {
        foreach (var mech in livedMechs) {
            if (mech != null && mech.Lives > 0)
                mech.TryHit(position, radius, damage);
        }
    }

    private void TryHit(Vector3 position, float radius, int damage) {
        if (Vector3.Distance(transform.position, position) < radius) {
            Hit(damage);
        }
    }

    private void Hit(int damage) {
        Lives -= damage;
        if (Lives <= 0) {
            Death();
        }
    }

    private void Death() {
        Lives = 0;
        var renderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (var renderer in renderers) {
            renderer.sharedMaterial = deathMaterial;
        }
        Effects.Blow(transform.position);
        weapon.gameObject.SetActive(false);
        Game.Stress(0.7f);
    }

    private void Start() {
        TargetPosition = transform.position;
        livedMechs.Add(this);
        Lives = maxLives;
    }

    private void OnDestroy() {
        RemoveFromHitList(this);
    }

    private void RemoveFromHitList(MechController mech) {
        if (livedMechs.Contains(mech)) {
            livedMechs.Remove(mech);
        }
    }

    private void Update() {
        if (Lives <= 0)
            return;
        Move();
    }

    private float sigma = 0.01f;
    private void Move() {
        var distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance < sigma)
            return;
        if (distance < moveSpeed * Time.deltaTime) {
            transform.position = TargetPosition;
            return;
        }
        var direction = (TargetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        var directionAngle = Mathf.Acos(direction.normalized.x) / Mathf.PI * 180f;
        if (direction.normalized.z > 0)
            directionAngle = - directionAngle;
        transform.localEulerAngles = new Vector3(0, directionAngle - 90f, 0);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, TargetPosition);
        Gizmos.DrawWireCube(TargetPosition, Vector3.one * 0.2f);
    }
}