using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    // enemies remember their index in the enemies array
    // this helps avoid finding the enemy in the array
    public int index;

    // true if the enemy is marked for destruction
    // enemies do not have health.  they get destoryed on a single bullet hit
    public bool toDestory;

    void OnTriggerEnter(Collider other) {
        if (toDestory) return;
        if (other.gameObject.TryGetComponent(out BulletBehavior bullet)){
            
            // it seems this helps avoid generating multiple collision triggers
            toDestory = true;

            // clear enemy's spot in the array
            Spawner.Instance.enemyStatuses[index] = false;

            // clearn bullet's spot in the array
            int bulletIndex = bullet.index;
            Spawner.Instance.bulletStatuses[bulletIndex] = false;
        }
    }
}
