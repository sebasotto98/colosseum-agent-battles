using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CoopGreedyAgent : BaseAgent
{
    internal struct RolePositions
    {
        public int role;
        public Vector3 targetOffset;

        public RolePositions(int role, Vector3 offset)
        {
            this.role = role;
            this.targetOffset = offset;
        }
    }

    internal struct RolePotentials
    {
        public RolePositions rolePos;
        public AgentController agent;
        public float potential;

        public RolePotentials(RolePositions rolePos, AgentController agent, float potential)
        {
            this.rolePos = rolePos;
            this.agent = agent;
            this.potential = potential;
        }
    }


    RolePositions currentRole;

    public CoopGreedyAgent(AgentController newController) : base(newController)
    {
    }

    public override void ResetState()
    {
        SetupPlatoon();
    }

    public override AgentActions SelectAction()
    {
        if (controller.platoonAgents.Count > 0 && controller.currentTarget != controller.platoonAgents.First(a => a.gameObject.activeSelf).currentTarget)
        {
            controller.currentTarget = controller.platoonAgents.First(a => a.gameObject.activeSelf).currentTarget;
            controller.targetController = controller.currentTarget.GetComponent<AgentController>();
        }

        var currentTarget = controller.currentTarget;
        if (!currentTarget) return AgentActions.NO_ACTION;


        if (currentRole.targetOffset == null) return AgentActions.NO_ACTION;

        Vector3 distanceToPositionTarget = (currentTarget.transform.position + currentRole.targetOffset) - controller.transform.position;
        Vector3 distanceToTarget = currentTarget.transform.position - controller.transform.position;

        if (distanceToTarget.magnitude < controller.characteristics.weapon.weaponRange) return AgentActions.ATTACK;
        else
        {

            if (Mathf.Abs(distanceToPositionTarget.x) > Mathf.Abs(distanceToPositionTarget.y))
            {
                if (distanceToPositionTarget.x > 0) return AgentActions.RIGHT;
                else if (distanceToPositionTarget.x < 0) return AgentActions.LEFT;
                else return AgentActions.NO_ACTION;
            }
            else if (Mathf.Abs(distanceToPositionTarget.x) < Mathf.Abs(distanceToPositionTarget.y))
            {
                if (distanceToPositionTarget.y > 0) return AgentActions.UP;
                else if (distanceToPositionTarget.y < 0) return AgentActions.DOWN;
                else return AgentActions.NO_ACTION;
            }
            else
            {
                return (AgentActions)Random.Range(1, 5);
            }
        }
    }

    private float CooperativePotential(Vector3 source, Vector3 targetPosition)
    {
        return -(targetPosition - source).sqrMagnitude;
    }

    public override void NextStep(AgentActions action)
    {
        Debug.Log("Next step frequency");
        var platoonAgents = controller.platoonAgents;

        if (platoonAgents.Count > 0 && platoonAgents.FindIndex(t => t == controller) == 0 || (platoonAgents.Count > 0 && !platoonAgents[0].gameObject.activeSelf))
        {
            List<RolePositions> adjacentTargetPos = new List<RolePositions>{
                new RolePositions(1, Vector3.up * controller.characteristics.weapon.weaponRange),
                new RolePositions(2, Vector3.down * controller.characteristics.weapon.weaponRange),
                new RolePositions(3, Vector3.right * controller.characteristics.weapon.weaponRange),
                new RolePositions(4, Vector3.left * controller.characteristics.weapon.weaponRange)
            };
            List<RolePotentials> rolePotentials = new List<RolePotentials>();

            foreach (var item in adjacentTargetPos)
            {
                platoonAgents.FindAll(a => a.gameObject.activeSelf).ForEach(a => rolePotentials.Add(new RolePotentials(item, a, CooperativePotential(a.transform.position, item.targetOffset))));
            }

            var ordered = rolePotentials.OrderByDescending(rp => rp.potential).ToList();
            var agentOrder = ordered.Select(t => t.agent).Distinct().ToList();
            foreach (var item in agentOrder)
            {
                currentRole = ordered.Where(t => t.agent == item).First().rolePos;
                ordered.RemoveAll(t => t.rolePos.role == currentRole.role);
            }
        }

    }

    private void SetupPlatoon()
    {
        var platoonAgents = controller.platoonAgents;

        if (platoonAgents.Count == 0)
        {
            platoonAgents.Add(controller);

            platoonAgents.AddRange(controller.teamManager.spawnedAgentControllers.FindAll(t => t.gameObject != controller.gameObject && t.gameObject.activeSelf && t.platoonAgents.Count <= 0).OrderBy(t => (t.transform.position - controller.transform.position).sqrMagnitude).Take(2).ToList());

            platoonAgents.ForEach(t => t.platoonAgents = new List<AgentController>(platoonAgents));
            platoonAgents.ForEach(t => t.currentTarget = controller.currentTarget);
        }
    }
}
