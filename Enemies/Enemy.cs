using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private int maxHealth = 500;
    private int currentHealth;

    private GameObject canvas;
    private GameObject minimapIcon;
    [SerializeField] private GameObject respawner;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject floatingText;
    private Health healthbar;
    private BossHealth bossHealthBar;
    private GameObject coin;
    private GameObject redPot;
    private GameObject bluePot;
    private GameObject spectralSword;
    private GameObject instantKill;

    [SerializeField] private AIPath aiPath;
    [SerializeField] private Animator animator;

    private bool isFacingRight = false;
    private bool isDead = false;
    private RigidbodyConstraints2D oldConstraints;
    private int maxDamage = 0;
    private Coroutine hitCoroutine;
    private int knockback = 0;
    private int minCoin = 0;
    private int maxCoin = 0;
    private bool isBoss;
    private bool triggerCharge;

    private void Start()
    {
        GameObject hedgeHog = GameObject.FindGameObjectWithTag("HedgeHog");
        if (hedgeHog != null)
        {
            Physics2D.IgnoreCollision(hedgeHog.GetComponent<CircleCollider2D>(), GetComponent<CircleCollider2D>());
        }
        currentHealth = maxHealth;

        minimapIcon = gameObject.transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();
        if (!isBoss)
        {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>(), GetComponent<CircleCollider2D>());
            aiPath = gameObject.GetComponent<AIPath>();
            canvas = gameObject.transform.GetChild(1).gameObject;
            if (this.name.Contains("Wendigo"))
            {
                canvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.6f, 3);
            }
            else
            {
                canvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0.7f);
            }
            healthbar = canvas.transform.GetChild(0).gameObject.GetComponent<Health>();
            healthbar.SetMaxHealth(maxHealth);
        }
        else
        {
            if (respawner.GetComponent<EnemyRespawn>().GetDifficulty().Equals("Normal"))
            {
                maxHealth = 75000;
            } else if (respawner.GetComponent<EnemyRespawn>().GetDifficulty().Equals("Hard"))
            {
                maxHealth = 100000;
            } else if (respawner.GetComponent<EnemyRespawn>().GetDifficulty().Equals("Expert"))
            {
                maxHealth = 200000;
            }
            currentHealth = maxHealth;
            canvas.transform.parent.GetComponent<Canvas>().enabled = true;
            bossHealthBar = canvas.GetComponent<BossHealth>();
            bossHealthBar.SetMaxHealth(maxHealth);
        }
        oldConstraints = this.GetComponent<Rigidbody2D>().constraints;

    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        GameObject hog = GameObject.FindGameObjectWithTag("HedgeHog");
        if (hog != null)
        {
            if (hog.transform.position.x < gameObject.transform.position.x && !isFacingRight)
            {
                Flip();
            }
            if (hog.transform.position.x > gameObject.transform.position.x && isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            if (!isBoss)
            {
                if (GetComponent<AIDestinationSetter>().target != player)
                {
                    GetComponent<AIDestinationSetter>().target = player;
                }
            }
            if (player.transform.position.x < gameObject.transform.position.x && !isFacingRight)
            {
                Flip();
            }
            if (player.transform.position.x > gameObject.transform.position.x && isFacingRight)
            {
                Flip();
            }
        }
    }


    public void TakeDamage(int damage, bool trap)
    {
        if (isDead)
        {
            return;
        }
        if (player.GetComponent<Player>().GetInstantKill() && !isBoss)
        {
            damage = currentHealth;
        }
        if (isBoss)
        {
            if (Vector2.Distance(player.position, GetComponent<Rigidbody2D>().position) >= 11)
            {
                damage = 0;
            }
            if (triggerCharge)
            {
                currentHealth -= damage == 0 ? damage : damage / 2; 
            } else
            {
                currentHealth -= damage;
            }
            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }
            bossHealthBar.SetHealth(currentHealth);
            if (!triggerCharge && maxHealth / 2 >= currentHealth)
            {
                triggerCharge = true;
                TriggerChargeEffect();
            }
        }
        else
        {
            currentHealth -= damage;
            healthbar.SetHealth(currentHealth);
        }
        if (floatingText)
        {
            ShowFloatingText(damage);
        }
        FindObjectOfType<AudioManager>().Play("damaged");
        float time = 0f;
        if (knockback > 0)
        {
            animator.SetTrigger("Damaged");
            //this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == "Hit_Animation" || clips[i].name == "Vengeful_Spirit_Damaged_Animation")
                {
                    time = clips[i].length;
                    break;
                }
            }
        }
        if (currentHealth <= 0)
        {
            if (trap)
            {
                respawner.GetComponent<EnemyRespawn>().KillMonsterWithTrap();
            }
            Die();
        }
        if (time > 0)
        {
            hitCoroutine = StartCoroutine(TakeHit(time));
        }
    }

    private void ShowFloatingText(int damage)
    {
        var damageNumber = Instantiate(floatingText, transform.position, Quaternion.identity);
        damageNumber.GetComponent<MeshRenderer>().sortingLayerName = "Default";
        damageNumber.GetComponent<MeshRenderer>().sortingOrder = 7;
        damageNumber.GetComponent<TextMesh>().text = "" + damage;
        damageNumber.transform.GetChild(0).GetComponent<MeshRenderer>().sortingLayerName = "Default";
        damageNumber.transform.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = 6;
        damageNumber.transform.GetChild(0).GetComponent<TextMesh>().text = "" + damage;

    }

    private IEnumerator TakeHit(float time)
    {
        yield return new WaitForSeconds(time);
        animator.ResetTrigger("Damaged");
        if (!isDead)
        {
            this.GetComponent<Rigidbody2D>().constraints = oldConstraints;
        }
    }

    public void Die()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        isDead = true;
        EnemyRespawn respawn = respawner.GetComponent<EnemyRespawn>();
        if (respawn != null)
        {
            respawn.DecreaseCurrentMonsters();
        }
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        if (isBoss)
        {
            bossHealthBar.ResetText();
            canvas.transform.parent.GetComponent<Canvas>().enabled = false;
            animator.Play("Vengeful_Spirit_Death_Animation");
        }
        else
        {
            aiPath.canMove = false;
            canvas.SetActive(false);
            animator.Play("Death_Animation");
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
        GameObject coinObj = Instantiate(coin, transform.position, transform.rotation);
        coinObj.GetComponent<Coin>().SetMaxCoin(maxCoin);
        coinObj.GetComponent<Coin>().SetMinCoin(minCoin);
        int hpRandom = Random.Range(0, 100);
        if (hpRandom <= 25)
        {
            Vector3 Offset = new Vector3(0.25f, 0, transform.position.y);
            GameObject healthObj = Instantiate(redPot, transform.position + Offset, transform.rotation);
            Destroy(healthObj, 40f);
        }
        int oxRandom = Random.Range(0, 100);
        if (oxRandom <= 18)
        {
            Vector3 Offset = new Vector3(0.4f, 0, transform.position.y);
            GameObject oxygenObj = Instantiate(bluePot, transform.position + Offset, transform.rotation);
            Destroy(oxygenObj, 49f);
        }
        int instaRandom = Random.Range(0, 200);
        if (instaRandom <= 1)
        {
            Vector3 Offset = new Vector3(0.65f, 0, transform.position.y);
            GameObject instantObj = Instantiate(instantKill, transform.position + Offset, transform.rotation);
            Destroy(instantObj, 40f);
        }
        Destroy(coinObj, 40f);
        //Destroy(this.gameObject);
    }
    public void DeathAnimation()
    {
        if (isBoss)
        {
            if (player.GetComponent<Player>().GetTaskSystem().GetCurrentTask().Equals("Kill"))
            {
                player.GetComponent<Player>().GetTaskSystem().CompleteTask(respawner.GetComponent<EnemyRespawn>().GetCurrentWave());
                respawner.GetComponent<EnemyRespawn>().FinishStoryMode(transform);
                player.GetComponent<Player>().SetOxygenInterval_2(999, 1);
            }
        }
        Destroy(this.gameObject);
    }

    private void TriggerChargeEffect()
    {
        transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = true;
        transform.GetChild(1).GetComponent<Animator>().SetBool("useShield", true);
        StartCoroutine(StartCharge());
    }

    private IEnumerator StartCharge()
    {
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "You will regret doing that.";
        yield return new WaitForSeconds(2.8f);
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "";
        transform.GetChild(1).GetComponent<Animator>().enabled = true;
        animator.SetBool("IsCharging", true);
        yield return new WaitForSeconds(6f);
        animator.SetBool("IsCharging", false);
        transform.GetChild(2).GetChild(0).GetComponent<Animator>().enabled = true;
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "Rise..";
        transform.GetChild(1).GetComponent<Animator>().SetBool("useShield", false);        
        StartCoroutine(SpawnMinions());
        yield return new WaitForSeconds(1f);
        transform.GetChild(1).GetComponent<Animator>().enabled = false;
        transform.GetChild(1).GetComponent<CircleCollider2D>().enabled = false;
        transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.3f);
        transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "";
        transform.GetChild(2).GetChild(0).GetComponent<Animator>().enabled = false;
    }

    private IEnumerator SpawnMinions()
    {
        while (true)
        {
            if (isDead)
            {
                break;
            }
            yield return new WaitForSeconds(1.8f);
            respawner.GetComponent<EnemyRespawn>().SpawnMinion(this.gameObject);
        }
    }

    public bool getFlip()
    {
        return isFacingRight;
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        if (!isBoss)
        {
            Vector3 hpScale = canvas.transform.localScale;
            hpScale.x *= -1;
            canvas.transform.localScale = hpScale;
        } else
        {
            Vector3 textCanvas = transform.GetChild(2).transform.localScale;
            textCanvas.x *= -1;
            transform.GetChild(2).transform.localScale = textCanvas;
        }
    }

    public void AttackPlayer()
    {
        if (isDead)
        {
            return;
        }

        GameObject hog = GameObject.FindGameObjectWithTag("HedgeHog");
        if (hog != null)
        {
            hog.GetComponent<Hedgehog>().TakeDamage(maxDamage);
        }
        else
        {
            player.GetComponent<Player>().TakeDamage(maxDamage);
        }
    }

    public void TriggerBossAttack()
    {
        if (!animator.GetBool("IsCharging"))
        {
            StartCoroutine(BossAttackPlayer());
        }
    }

    public IEnumerator BossAttackPlayer()
    {
        float seconds = 0.6f;
        if (triggerCharge)
        {
            seconds = 0.3f;
            animator.SetFloat("animSpeed", 1f);
        }
        yield return new WaitForSeconds(seconds);
        animator.SetTrigger("Attack");
        Vector3 Offset = new Vector3(1.5f, 1.5f);
        if (getFlip())
        {
            Offset = new Vector3(-1.5f, 1.5f);
        }
        GameObject sword = Instantiate(spectralSword, player.position + Offset, Quaternion.identity);
        sword.GetComponent<SpectralSword>().Charged(triggerCharge);
        if (getFlip())
        {
            Vector3 theScale = sword.transform.localScale;
            theScale.x *= -1;
            sword.transform.localScale = theScale;
        }
    }

    public void SetSpectralSword(GameObject obj)
    {
        spectralSword = obj;
    }

    public bool getDead()
    {
        return isDead;
    }

    public int getKnockback()
    {
        return knockback;
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health;
    }

    public void SetMaxDamage(int damage)
    {
        maxDamage = damage;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }

    public void SetRespawner(GameObject respawner)
    {
        this.respawner = respawner;
    }

    public void SetKnockback(int knockback)
    {
        this.knockback = knockback;
    }

    public void SetCoin(GameObject obj)
    {
        this.coin = obj;
    }

    public void SetMinCoin(int c)
    {
        this.minCoin = c;
    }

    public void SetMaxCoin(int c)
    {
        this.maxCoin = c;
    }

    public void SetRedPot(GameObject obj)
    {
        this.redPot = obj;
    }

    public void SetBluePot(GameObject obj)
    {
        this.bluePot = obj;
    }

    public void SetInstantKill(GameObject obj)
    {
        this.instantKill = obj;
    }

    public void SetBoss(bool boss)
    {
        this.isBoss = boss;
    }

    public bool IsBoss()
    {
        return this.isBoss;
    }

    public void SetCanvas(GameObject canv)
    {
        canvas = canv;
    }

    public bool GetCharged()
    {
        return triggerCharge;
    }
}
