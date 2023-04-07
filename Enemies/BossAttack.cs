using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAttack : StateMachineBehaviour
{
    [SerializeField] private float attackRange = 1f;

    private Transform player;
    private EnemyRespawn enemyRespawn;
    private Rigidbody2D rb;
    private float attackDelay = 0f;
    private float chargeDelay = 4f;
    private bool isCutscene;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isCutscene = SceneManager.GetActiveScene().name.Equals("Cutscene") || SceneManager.GetActiveScene().name.Equals("Cutscene_2");
        if (!isCutscene)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            rb = animator.GetComponent<Rigidbody2D>();
            enemyRespawn = GameObject.FindGameObjectWithTag("EnemyRespawner").GetComponent<EnemyRespawn>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isCutscene)
        {
            attackDelay += Time.deltaTime;
            if (attackDelay >= chargeDelay)
            {
                if (Vector2.Distance(player.position, rb.position) <= attackRange)
                {
                    animator.GetComponent<Enemy>().TriggerBossAttack();
                    attackDelay = 0;
                }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isCutscene)
        {
            animator.ResetTrigger("Attack");
        }
    }

    public void UpdateStats(Animator animator)
    {
        if (enemyRespawn.GetDifficulty().Equals("Normal"))
        {
            if (animator.GetComponent<Enemy>().GetCharged())
            {
                chargeDelay = 1f;
            }
        } else if (enemyRespawn.GetDifficulty().Equals("Hard"))
        {
            if (animator.GetComponent<Enemy>().GetCharged())
            {
                chargeDelay = 0.8f;
            }
        } else if (enemyRespawn.GetDifficulty().Equals("Expert"))
        {
            if (animator.GetComponent<Enemy>().GetCharged())
            {
                chargeDelay = 0.5f;
            }
        }
    }

    public float getAttackRange()
    {
        return attackRange;
    }
}
