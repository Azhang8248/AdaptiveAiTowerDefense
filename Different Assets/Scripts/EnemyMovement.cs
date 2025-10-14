using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

   [Header("Attributes")]
   [SerializeField] private float moveSpeed;

   [Header("Checkpoints")]
   [SerializeField] private Transform[] checkpoints;
   private int currentCheckpointIndex = 0;

   void Update()
   {
      if (checkpoints.Length == 0)
      {
         return;
      }

      // current target to look at (checkpoint)
      Transform target = checkpoints[currentCheckpointIndex];


      GetComponent<Rigidbody2D>().linearVelocity = (target.position - transform.position).normalized * moveSpeed;
      // transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

      transform.right = target.position - transform.position;
      // transform.rotation = Quaternion.LookRotation(target.position - transform.position);


      // Checks if enemy reaches the checkpoint, increment if it does
      if (Vector2.Distance(transform.position, target.position) <= 0.1f)
      {

         currentCheckpointIndex++;
         if (currentCheckpointIndex < checkpoints.Length)
         {
            Debug.Log("Checkpoint" + " " + currentCheckpointIndex);
         }

         // If enemy reaches the last checkpoint
         if (currentCheckpointIndex >= checkpoints.Length)
         {
            Debug.Log("End reached");
            Destroy(gameObject);
         }


      }




   }
}
