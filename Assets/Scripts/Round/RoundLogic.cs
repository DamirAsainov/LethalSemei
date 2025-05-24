using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Round
{
    public class RoundLogic : MonoBehaviour
    {
        [Header("Game Setup")]
        [SerializeField] private Transports.WaypointDrive waypointDrive;
        [SerializeField] private List<GameObject> players;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private List<GameObject> monsters;
        [SerializeField] private List<Transform> monsterSpawnPoints;

        [Header("Round Settings")]
        [SerializeField] private float roundDuration = 1200f; // 20 минут
        [SerializeField] private float monsterSpawnDelay = 100f;
        [SerializeField] private float endOfRoundDelay = 10f; // Задержка завершения

        private bool playersSpawned = false;
        private bool monsterSpawned = false;
        private float roundTimer = 0f;
        private bool roundStarted = false;
        private bool roundEndingStarted = false;
        private bool roundEnded = false;

        private void Start()
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SetActive(false);
            }
        }

        private void Update()
        {
            if (waypointDrive.finishedDriving && !playersSpawned)
            {
                Debug.Log("Players spawned");
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].SetActive(true);
                }

                playersSpawned = true;
                roundStarted = true;
                roundTimer = roundDuration;
            }

            if (roundStarted)
            {
                Debug.Log("Round started");
                roundTimer -= Time.deltaTime;

                if (roundTimer <= 0f && !roundEndingStarted)
                {
                    roundEndingStarted = true;
                    StartCoroutine(DelayedEndRound());
                }
            }

            if (roundDuration - roundTimer >= monsterSpawnDelay && !monsterSpawned)
            {
                monsterSpawned = true;
                SpawnMonsters();
            }

            if (!CheckIfPlayerIsAlive() && !roundEnded && !roundEndingStarted)
            {
                roundEndingStarted = true;
                StartCoroutine(DelayedEndRound());
            }
        }

        private IEnumerator DelayedEndRound()
        {
            Debug.Log("⚠ Раунд скоро завершится...");
            yield return new WaitForSeconds(endOfRoundDelay);
            EndRound();
        }

        private void EndRound()
        {
            if (roundEnded) return;
            roundEnded = true;
            Debug.Log("⏰ Раунд завершён!");
            
        }

        private bool CheckIfPlayerIsAlive()
        {
            foreach (GameObject player in players)
            {
                if (player != null && !player.GetComponent<HealthSystem>().IsDead())
                    return true;
            }

            return false;
        }

        private void SpawnMonsters()
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                Instantiate(monsters[i], monsterSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }
}
