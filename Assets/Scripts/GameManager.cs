﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Objectives { Keys, ScoreAmt }

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance = null;

    [SerializeField]
    private GameObject BombPrefab, ExplosionParticlePrefab;

    [SerializeField]
    private Objectives Objective;

    [SerializeField]
    private GameObject KeyPrefab;

    [SerializeField]
    private int ScoreGoal;

    public int NumKeys;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void StageObjective()
    {
        switch(Objective)
        {
            case Objectives.Keys:
                SpawnKeys();
                break;
            case Objectives.ScoreAmt:
                StartTimeObjective();
                break;
        }
    }

    private void SpawnKeys()
    {
        Destructable[] destruct = GameObject.FindObjectsOfType<Destructable>();
        
        for (int i = 0; i < NumKeys; i++)
        {
            Transform t = destruct[Random.Range(0, destruct.Length)].transform;
            Instantiate(KeyPrefab, t.position, t.rotation);
        }
        InformationTracker.Instance.ObjectiveText("-Keys- \n" + NumKeys);
    }

    public Objectives GetObjective
    {
        get
        {
            return Objective;
        }
        set
        {
            Objective = value;
        }
    }

    public GameObject GetBombPrefab()
    {
        return BombPrefab;
    }

    public GameObject GetExplosionPrefab()
    {
        return ExplosionParticlePrefab;
    }

    public void CheckKeysInStage()
    {
        if (NumKeys > 0 || Objective != Objectives.Keys)
        {
            return;
        }
        else if (NumKeys <= 0)
        {
            WinGame();
        }
    }

    public void CheckScoreInStage()
    {
        if (ScoreTracker.Instance.GetScore < ScoreGoal || Objective != Objectives.ScoreAmt)
        {
            return;
        }
        else if (ScoreTracker.Instance.GetScore >= ScoreGoal)
        {
            WinGame();
        }
    }

    public void Respawn()
    {
        Player.Instance.enabled = true;

        Character.Instance.GetComponent<MeshRenderer>().enabled = true;

        Character.Instance.transform.position = new Vector3(0, -23.725f, 28);

        Character.Instance.GetComponent<Collider>().enabled = true;
    }

    public void GameOver()
    {
        StageTimer.Instance.GetStartTimer = false;

        SoundManager.Instance.GameOverSE();

        StartCoroutine("RestartLevel");
    }

    void WinGame()
    {
        SoundManager.Instance.LevelWinSE();

        Player.Instance.GetComponent<Player>().enabled = false;
        StageTimer.Instance.GetStartTimer = false;
    }

    private void StartTimeObjective()
    {
        InformationTracker.Instance.ObjectiveText("-Score Goal- \n" + ScoreGoal);

        StageTimer.Instance.GetTimeText().enabled = true;
        StageTimer.Instance.GetStartTimer = true;
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(1);
    }

    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2.0f);
        Respawn();
    }
}
