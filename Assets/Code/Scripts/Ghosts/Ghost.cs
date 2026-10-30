using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class Ghost : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] protected TowerAnimator towerAnimator;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected Transform firingPoint;
    [SerializeField] protected TowerHealthBar healthBar;

    [Header("Base Attributes")]
    [SerializeField] protected float targetingRange = 5f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float lifetime = 16f;
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float preDeathDuration = 1f;
    
    public int PathIndex { get; private set; }
    public int CurrentTier { get; private set; }

    protected Transform target;
    protected float timeUntilFire;

    public float TargetingRange => targetingRange;

    private bool isAttackAnimationPlaying = false;

    protected virtual void Start()
    {
        towerAnimator = GetComponent<TowerAnimator>();
        StartCoroutine(LifetimeCoroutine());
    }

    protected virtual void Update()
    {
        timeUntilFire += Time.deltaTime;
        
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            if (timeUntilFire >= attackSpeed && !isAttackAnimationPlaying)
            {
                if (towerAnimator != null)
                {
                    isAttackAnimationPlaying = true;
                    towerAnimator.SetAttacking(); // no argument needed
                    timeUntilFire = 0f;
                }
                else
                {
                    Shoot();
                    timeUntilFire = 0f;
                }

            }
        }
    }

    // each tower implements its own shoot logic
    protected abstract void Shoot();

    public void OnAttackFire()
    {
        if (target == null) return;
        Shoot();
    }

    public void OnAttackEnd()
    {
        isAttackAnimationPlaying = false;
    }

    private IEnumerator LifetimeCoroutine()
    {
        float timeRemaining = lifetime;

        while (timeRemaining > preDeathDuration)
        {
            timeRemaining -= Time.deltaTime;
            if (healthBar != null) healthBar.SetProgress(timeRemaining / lifetime);
            yield return null;
        }

        OnPreDeath();

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (healthBar != null) healthBar.SetProgress(timeRemaining / lifetime);
            yield return null;
        }

        OnLifetimeExpired();
    }

    protected virtual void OnLifetimeExpired()
    {
        enabled = false;
        if (towerAnimator != null)
        {
            towerAnimator.SetDying();
        }
        else
        {
            Destroy(gameObject); // fallback if no animator
        }
    }

    protected virtual void OnPreDeath()
    {
        if (towerAnimator != null)
            towerAnimator.SetPreDeath();
    }

    public void OnDeathEnd()
    {
        Destroy(gameObject);
    }

    protected void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
            target = hits[0].transform;
    }

    protected bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    protected virtual void RotateTowardsTarget()
    {
        transform.localScale = target.position.x > transform.position.x
            ? new Vector3(-1f, 1f, 1f)
            : new Vector3(1f, 1f, 1f);
    }

    public void Initialize(int pathIndex)
    {
        PathIndex = pathIndex;
    }

    public virtual void ApplyUpgrade()
    {
        CurrentTier++;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Handles.color = Color.cyan;
    //    Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    //}
}
