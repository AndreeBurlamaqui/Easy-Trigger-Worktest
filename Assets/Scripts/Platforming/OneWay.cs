using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWay : MonoBehaviour
{
    [SerializeField] UnitData Player;
    [SerializeField] float delayToReset = 0.35f;
    [Tooltip("The actual collider that is being used as ground.")]
    [SerializeField] Collider2D colliderWay;

    bool currentAbove = false;
    bool isInverting = false;
    WaitForSeconds waitInvert;

    private void Awake()
    {
        waitInvert = new WaitForSeconds(delayToReset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(Player.uCollider, colliderWay, false);
            isInverting = false;
            currentAbove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            currentAbove = false;

            if (isInverting)
            {
                Physics2D.IgnoreCollision(Player.uCollider, colliderWay, false);
                isInverting = false;
            }
        }
    }



    private void Update()
    {
        if (!isInverting && currentAbove && Player.IsDucking)
        {
            Physics2D.IgnoreCollision(Player.uCollider, colliderWay, true);
            isInverting = true;
            //StartCoroutine(InvertOneWayRoutine());
        }


    }
}
