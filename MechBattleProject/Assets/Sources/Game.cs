using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class Game : MonoBehaviour {
    private static Game instance = null;

    [SerializeField]
    private Camera sceneCamera;
    [SerializeField]
    private Vector3 cameraPosition = new Vector3(0, 0, -15f);
    [SerializeField]
    private MechController playerMech;
    [SerializeField]
    private GameObject moveMarker;
    [SerializeField]
    private GameObject fireMarker;
    [SerializeField]
    private Vector2 maxFieldPoint = new Vector2( 5f, 5f);
    [SerializeField]
    private Vector2 minFieldPoint = new Vector2(-5f, -5f);
    [SerializeField]
    private List<MechController> enemyMechs = new List<MechController>();
    [SerializeField]
    private float aiRetargetTimer = 1.0f;
    [SerializeField]
    private float aiControlDelayTimer = 3.0f;

    [SerializeField]
    private Text winText;
    [SerializeField]
    private Text loseText;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button exitButton;

    private float stressTimer = 0;
    private string currentLevel;

    public static void Stress(float time = 0.5f) {
        instance.stressTimer = Mathf.Max(instance.stressTimer, time);
    }

    private void Start () {
        instance = this;
        enemyMechs = FindObjectsOfType<MechController>().ToList();
        enemyMechs.Remove(playerMech);
        foreach (var enemy in enemyMechs) {
            MoveRandomDirection(enemy);
        }
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartClick);
        exitButton.onClick.AddListener(ExitToMenuClick);
        currentLevel = SceneManager.GetActiveScene().name;
    }

    public void RestartClick() {
        SceneManager.LoadScene(currentLevel);
    }

    public void ExitToMenuClick() {
        SceneManager.LoadScene("Menu");
    }

    private void Update () {
        if (Input.GetMouseButtonDown(1)) {
            var position = GetScreenPlanePosition(Input.mousePosition);
            playerMech.MoveTo(position);
            moveMarker.transform.position = position;
        }
        if (Input.GetMouseButtonDown(0)) {
            var position = GetScreenPlanePosition(Input.mousePosition);
            playerMech.FireTo(position);
            fireMarker.transform.position = position;
        }
        ControlEnemyMechs();
        CheckWinConditions();
        if (stressTimer > 0) {
            stressTimer -= Time.deltaTime;
            sceneCamera.transform.localPosition = cameraPosition + Random.insideUnitSphere * stressTimer * 0.5f;
        } else {
            sceneCamera.transform.localPosition = cameraPosition;
        }
    }

    private void CheckWinConditions() {
        if (playerMech.Lives <= 0) {
            loseText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            return;
        }
        foreach (var enemy in enemyMechs) {
            if (enemy.Lives > 0)
                return;
        }
        winText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    private Vector3 GetScreenPlanePosition(Vector3 mousePosition) {
        var plane = new Plane(Vector3.up, Vector3.zero);
        var ray = sceneCamera.ScreenPointToRay(mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance)) {
            return ray.origin + ray.direction * distance;
        }
        return Vector3.zero;
    }

    private void ControlEnemyMechs() {
        foreach (var enemy in enemyMechs) {
            if (Vector3.Distance(enemy.TargetPosition, enemy.transform.position) < 0.01f) {
                MoveRandomDirection(enemy);
            }
        }
        aiControlDelayTimer -= Time.deltaTime;
        if (aiControlDelayTimer < 0) {
            aiControlDelayTimer = aiRetargetTimer;
            foreach (var enemy in enemyMechs) {
                if (Random.Range(0, 100) < 20) {
                    SetUnitAim(enemy);
                }
            }
        }
    }

    private void SetUnitAim(MechController enemy) {
        var dist = Random.Range(1.0f, 2.5f);
        var point = playerMech.transform.position + new Vector3(Random.Range(-dist, dist), 0, Random.Range(-dist, dist));
        enemy.FireTo(point);
    }

    private void MoveRandomDirection(MechController enemy) {
        var randomPosition = new Vector3(Random.Range(minFieldPoint.x, maxFieldPoint.x), 0, Random.Range(minFieldPoint.y, maxFieldPoint.y));
        enemy.MoveTo(randomPosition);
    }
}
