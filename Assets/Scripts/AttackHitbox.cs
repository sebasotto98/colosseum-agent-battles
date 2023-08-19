using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField]
    BoxCollider2D hitboxCollider;

    private AgentController ownController;

    private void Awake()
    {
        ownController = transform.parent.GetComponent<AgentController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AgentController hitAgent = collision.GetComponent<AgentController>();
        if (!hitAgent || hitAgent == transform.parent.GetComponent<AgentController>()) return;
        if (hitAgent.teamID == ownController.teamID) return;

        ownController.InflictDamage();
        hitAgent.SufferDamage();
    }
}
