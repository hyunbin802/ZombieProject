using UnityEngine;
using UnityEngine.AI;

public class DoorTrigger : MonoBehaviour
{
    public Animator anim;
    public NavMeshObstacle obstacle;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (obstacle == null) obstacle = GetComponent<NavMeshObstacle>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (obstacle != null) obstacle.enabled = false;
            anim.SetBool("isOpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("isOpen", false);
            if (obstacle != null) obstacle.enabled = true;
        }
    }

}
