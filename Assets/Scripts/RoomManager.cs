using UnityEngine;

public class RoomManager : MonoBehaviour
{
    //start barrier first, then end barrier
    public GameObject[] barriers;
    public Enemy[] enemies;
    public EnemyBall[] enemyBall;
    public BossEnemy[] bossEnemy;
    public GameManager gameManager;
    public GameObject powersUI;
    private bool roomActivated = false;
    private bool powerChosen = false;
    private bool firstEntry = false;
    private bool musicReset = false;


    private void OnTriggerEnter(Collider other)
    {
        if (roomActivated) 
        {
            return; 
        }


        if (other.CompareTag("Player"))
        {
            roomActivated = true;
            if (!firstEntry)
            {
                firstEntry = true;
                gameManager.PlayCombatMusic();
            }
            foreach (var barrier in barriers)
            {
                barrier.SetActive(true);
            }
            
        }
    }

    private void Update()
    {
        if (roomActivated && RoomClear())
        {
            barriers[1].SetActive(false);
            if (!musicReset)
            {
                gameManager.PlayNormalMusic();
                musicReset = true;
            }
            
            if (!powersUI.activeSelf && !powerChosen)
            {
                gameManager.ShowRandomPowers();
                powerChosen = true;
            }
        }
    }

    private bool RoomClear()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.isActiveAndEnabled)
            {

                return false;
            }
        }
        foreach(var roller in enemyBall)
        {
            if(roller != null && roller.isActiveAndEnabled)
            {
                return false;
            }
        }
        foreach(var boss in bossEnemy)
        {
            if(boss != null && boss.isActiveAndEnabled)
            {
                return false;
            }
        }
        return true;
    }

    private void SetBarriers(bool state)
    {
        foreach(var barrier in barriers)
        {
            barrier.SetActive(state);
        }
    }
}
