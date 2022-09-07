using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Drop : MonoBehaviour
{
    [SerializeField] UnitData PlayerData;
    [SerializeField] string playerTag;
    [SerializeField] GameEvent OnDropPickEvent;
    bool aboveDrop = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!aboveDrop && collision.CompareTag(playerTag))
            aboveDrop = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (aboveDrop && collision.CompareTag(playerTag))
            aboveDrop = false;
    }

    private void Update()
    {
        if (aboveDrop && PlayerData.IsDucking)
            PickupDrop();
    }

    void PickupDrop()
    {
        // TODO: Spawn some effect to indicate that it's being picked

        OnDropPickEvent.Raise();
        Destroy(gameObject);
    }
}
