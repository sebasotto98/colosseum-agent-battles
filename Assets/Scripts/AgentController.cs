using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AgentController : MonoBehaviour
{
    private Renderer agentRenderer;
    [SerializeField]
    private Renderer helmetRenderer;

    public int teamID;
    private Color agentColor;

    [SerializeField]
    private Transform weaponPivot;
    [SerializeField]
    private GameObject weaponObject;
    [SerializeField]
    private GameObject attackHitbox;
    private BoxCollider2D hitboxCollider;
    private bool inAttack;
    private bool isMoving;

    public int currentHealthPoints;

    public AgentCharacteristics characteristics;
    public BaseAgent agent;

    public TeamManager teamManager;

    public GameObject currentTarget;
    public AgentController targetController;

    private List<TeamManager> opposingTeams;
    private List<GameObject> allOpposingAgents;

    public List<AgentController> platoonAgents = new List<AgentController>();

    public bool isTraining = false;
    public int currentState;
    public float currentReward = 0;
    public bool tookDamage = false;

    public Vector3 spawnLocation = Vector3.negativeInfinity;

    public Vector3 distanceToTarget;

    private void Awake()
    {
        spawnLocation = Vector3.negativeInfinity;
        hitboxCollider = attackHitbox.GetComponent<BoxCollider2D>();
    }

    public void SetupAgent(TeamManager team)
    {
        this.teamManager = team;
        SetColor(characteristics.materialColor);

        isTraining = EnvironmentManager.instance.isTraining;
        currentTarget = null;

        opposingTeams = EnvironmentManager.instance.teamManagers.FindAll(q => q.teamID != teamID);
        allOpposingAgents = new List<GameObject>();
        opposingTeams.ForEach(p => allOpposingAgents.AddRange(p.spawnedAgents));

        //Currently a coroutine, which allows for execution akin to time steps
        currentHealthPoints = characteristics.healthPoints;
        inAttack = false;
        isMoving = false;

        agent.ResetState();
    }

    public void StartAgent(TeamManager teamManager)
    {
        StartCoroutine(ActionCoroutine());
    }


    IEnumerator ActionCoroutine()
    {
        while (true)
        {
            AgentActions action = AgentActions.NO_ACTION;
            action = agent.SelectAction();

            ExecuteAction(action);

            yield return new WaitForSeconds(0.1f);
        }
    }

    void ExecuteAction(AgentActions action)
    {
        switch (action)
        {
            case AgentActions.UP:
            case AgentActions.DOWN:
            case AgentActions.RIGHT:
            case AgentActions.LEFT:
                if (isMoving || inAttack) return;
                StartCoroutine(AgentMovement(action));
                break;
            case AgentActions.ATTACK:
                if (isMoving || inAttack) return;
                StartCoroutine(WeaponAttackCoroutine(characteristics.weapon.weaponAnimationType));
                break;
            default:
                if(currentTarget)
                {
                    var targetHP = targetController.currentHealthPoints;
                    var previousTargetDistance = (currentTarget.transform.position - transform.position).sqrMagnitude;
                    agent.AdjustReward(action, targetHP, previousTargetDistance);
                }
                break;
        }


        if (currentTarget == null || currentTarget.activeSelf == false)
        {
            switch (characteristics.targetingStrat)
            {
                case TargetingStrategy.RANDOM:
                    SelectRandomTarget();
                    break;
                case TargetingStrategy.CLOSEST:
                    SelectClosestTarget();
                    break;
                default:
                    break;
            }
        }
        agent.NextStep(action);
    }

    IEnumerator AgentMovement(AgentActions action)
    {
        var targetHP = 0;
        var previousTargetDistance = 0f;
        if (currentTarget)
        {
            targetHP = targetController.currentHealthPoints;
            previousTargetDistance = (currentTarget.transform.position - transform.position).sqrMagnitude;

        }

        Vector3 currentPos = transform.position;

        Vector3 addedVec = Vector3.zero;
        switch (action)
        {
            case AgentActions.NO_ACTION:
                break;
            case AgentActions.UP:
                addedVec = Vector3.up;
                break;
            case AgentActions.DOWN:
                addedVec = -Vector3.up;
                break;
            case AgentActions.RIGHT:
                addedVec = Vector3.right;
                break;
            case AgentActions.LEFT:
                addedVec = -Vector3.right;
                break;
            default:
                break;
        }

        Vector3 movVector = addedVec * characteristics.movementSpeedMultiplier + currentPos;
        //}
        float timer = 0.2f;

        transform.LookAt(movVector, Vector3.forward);

        isMoving = true;

        while (timer > 0)
        {
            float clampedTimer = (0.2f - timer) / 0.2f;

            transform.position = Vector3.Slerp(currentPos, movVector, clampedTimer);
            timer -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        isMoving = false;
        agent.AdjustReward(action, targetHP, previousTargetDistance);
    }

    IEnumerator WeaponAttackCoroutine(WeaponMovementType weaponMovementType) {
        if(currentTarget != null && characteristics.agentType != AgentType.RANDOM)
            transform.LookAt(currentTarget.transform.position, Vector3.forward);

        var targetHP = 0;
        var previousTargetDistance = 0f;
        if(currentTarget)
        {
            targetHP = targetController.currentHealthPoints;
            previousTargetDistance = (currentTarget.transform.position - transform.position).sqrMagnitude;

        }

        float timer = 0;
        switch (weaponMovementType)
        {
            case WeaponMovementType.SWING:
                #region Swing Motion
                inAttack = true;
                timer = 0.1f;
                while(timer > 0)
                {
                    float clampedTimer = (0.1f - timer) / 0.1f;
                    weaponPivot.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -89.9f, 0), clampedTimer);
                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                attackHitbox.SetActive(true);

                timer = 0.2f;
                while (timer > 0)
                {
                    float clampedTimer = (0.2f - timer) / 0.2f;
                    weaponPivot.localRotation = Quaternion.Slerp(Quaternion.Euler(0, -89.9f, 0), Quaternion.Euler(0, 90, 0), clampedTimer);
                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                attackHitbox.SetActive(false);

                timer = 0.2f;
                while (timer > 0)
                {
                    float clampedTimer = (0.2f - timer) / 0.2f;
                    weaponPivot.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 0, 0), clampedTimer);
                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                inAttack = false;
                #endregion
                break;
            case WeaponMovementType.POKE:
                #region Poke Motion
                inAttack = true;

                timer = 0.1f;
                Vector3 targetPos = new Vector3(0, 0, -0.7f);
                while (timer > 0)
                {
                    float clampedTimer = (0.1f - timer) / 0.1f;
                    weaponPivot.localPosition = Vector3.Slerp(Vector3.zero, targetPos, clampedTimer);
                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                attackHitbox.SetActive(true);

                timer = 0.2f;

                Vector3 originalPos = new Vector3(0, 0, -0.7f);
                targetPos = new Vector3(0, 0, 1.2f);
                while (timer > 0)
                {
                    float clampedTimer = (0.2f - timer) / 0.2f;

                    weaponPivot.localPosition = new Vector3(0, 0, Mathf.Lerp(-0.7f, 1.2f, clampedTimer));
                    weaponPivot.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 5, 0), clampedTimer);

                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                attackHitbox.SetActive(false);

                timer = 0.2f;
                originalPos = new Vector3(0, 0, 1.2f);
                targetPos = new Vector3(0, 0, 0f);
                while (timer > 0)
                {
                    float clampedTimer = (0.2f - timer) / 0.2f;
                    weaponPivot.localPosition = Vector3.Slerp(originalPos, targetPos, clampedTimer);
                    weaponPivot.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 5, 0), Quaternion.Euler(0, 0, 0), clampedTimer);
                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }

                inAttack = false;
                #endregion
                break;
            default:
                break;
        }
        agent.AdjustReward(AgentActions.ATTACK, targetHP, previousTargetDistance);
    }

    public void InflictDamage()
    {
        teamManager.IncrementInflictedDamage();
    }
    public void SufferDamage()
    {

        tookDamage = true;
        currentHealthPoints--;
        if (teamManager != null)
        {
            if (currentHealthPoints == 0)
            {
                teamManager.AddDeadAgent(gameObject);
                gameObject.SetActive(false);
            }
            teamManager.IncrementSufferedDamage();
        }
    }

    public void ApplyCharacteristics(TeamManager team)
    {
        this.teamManager = team;

        Destroy(weaponObject);

        weaponPivot.localPosition = Vector3.zero; 
        weaponPivot.localRotation = Quaternion.Euler(Vector3.zero);

        characteristics = teamManager.agentCharacteristics;
        SetColor(characteristics.materialColor);
        SetAgentScript();

        var weap = characteristics.weapon;
        weaponObject = Instantiate(weap.weaponPrefab, weaponPivot);

        weaponObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
        weaponObject.transform.localPosition = weap.positionOffset;
        weaponObject.transform.localScale = weap.scaleOffset;

        hitboxCollider.offset = weap.hitBoxOffset;
        hitboxCollider.size = weap.hitBoxSize;
    }

    private void SetAgentScript()
    {
        switch (characteristics.agentType)
        {
            case AgentType.RANDOM:
                agent = new RandomAgent(this);
                break;
            case AgentType.GREEDY:
                agent = new GreedyAgent(this);
                break;
            case AgentType.COOP_GREEDY:
                agent = new CoopGreedyAgent(this);
                break;
            case AgentType.RATIONAL:
                agent = new QLearningAgent(this);
                break;
            default:
                break;
        }
    }

    public void SetColor(Color color)
    {
        agentRenderer = GetComponent<Renderer>();
        agentRenderer.material.color = color;
        helmetRenderer.material.color = teamManager.teamColor;

    }

    private void SelectRandomTarget()
    {

        int randomAgentIndex = Random.Range(0, allOpposingAgents.Count);
        currentTarget = allOpposingAgents[randomAgentIndex];

        targetController = currentTarget.GetComponent<AgentController>();
    }

    private void SelectClosestTarget()
    {
        // get closest character (to referencePos)
        currentTarget = allOpposingAgents.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                                    .Where(t => t.activeSelf)
                                    .FirstOrDefault();

        if (currentTarget)
            targetController = currentTarget.GetComponent<AgentController>();
        else
            targetController = null;
    }

}
