using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponController : MonoBehaviour {
    [SerializeField]
    private float reloadTime = 3f;
    [SerializeField]
    private float delayTime = 0.2f;
    [SerializeField]
    private int bulletCount = 3;
    [SerializeField]
    private BulletController bulletPrefab;

    private float timer = 0f;

    private Vector3 targetPosition;
    private bool fireStarted = false;
    private List<BulletController> bulletsCache = new List<BulletController>();

    public void FireToPoint(Vector3 position) {
        fireStarted = true;
        targetPosition = position;
    }

    private void Start () {
		
	}

    private void Update () {
        if (timer > 0)
            timer -= Time.deltaTime;
        if (timer <= 0)
            Fire();
    }

    private void Fire() {
        if (!fireStarted)
            return;
        timer = reloadTime;
        FireBullets();
    }

    private void FireBullets() {
        for (int i = 0; i < bulletCount; i++) {
            var bullet = CreateBullet();
            bullet.Fire(targetPosition, transform.position, i * delayTime);
        }
    }

    private BulletController CreateBullet() {
        foreach (var bullet in bulletsCache) {
            if (!bullet.gameObject.activeSelf) {
                return bullet;
            }
        }
        var newBullet = Instantiate(bulletPrefab);
        bulletsCache.Add(newBullet);
        return newBullet;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
    }
}
