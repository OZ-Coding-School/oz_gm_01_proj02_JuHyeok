using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorVisualise : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3[] directions = new Vector3[]
                {
                    (Vector3.forward + Vector3.up).normalized,
                    (Vector3.forward + Vector3.down).normalized,
                    (Vector3.back + Vector3.up).normalized,
                    (Vector3.back + Vector3.down).normalized,
                    (Vector3.left + Vector3.up).normalized,
                    (Vector3.left + Vector3.down).normalized,
                    (Vector3.right + Vector3.up).normalized,
                    (Vector3.right + Vector3.down).normalized
                };

        foreach (var dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir);
        }
    }
}
