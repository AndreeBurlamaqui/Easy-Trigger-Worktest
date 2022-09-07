using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData")]
public class UnitData : ScriptableObject
{
    public bool isPlayer;
    public Collider2D uCollider;
    public Transform uTransform;
    public SpriteRenderer uVisual;
    public Rigidbody2D uRigibody;

    #region INPUT VARIABLES

    [Header("INPUT VARIABLES")]
    /// <summary>
    /// Horizontal information. Ranges from -1 (left) to 1 (right)
    /// </summary>
    public float Horizontal;

    /// <summary>
    /// If unit is trying to jump this will be set to true.
    /// <para>It's not saying that IT'S jumping, just REQUESTING</para>
    /// </summary>
    public bool TryJump;

    /// <summary>
    /// If unit is trying to duck this will be set to true.
    /// <para>It's not saying that IT'S ducking, just REQUESTING</para>
    /// </summary>
    public bool TryDuck;

    /// <summary>
    /// If unit is trying to hide this will be set to true.
    /// <para>It's not saying that IT'S hiding, just REQUESTING</para>
    /// </summary>
    public bool TryHide;

    /// <summary>
    /// If unit is trying to shoot this will be set to true.
    /// <para>It's not saying that IT'S shooting, just REQUESTING</para>
    /// </summary>
    public bool TryShoot;

    /// <summary>
    /// If unit is trying to punch this will be set to true.
    /// <para>It's not saying that IT'S punching, just REQUESTING</para>
    /// </summary>
    public bool TryPunch;

    #endregion


    #region ACTION VARIABLES

    [Header("ACTION VARIABLES")]

    public bool IsFalling;
    public bool IsJumping;
    public bool IsDucking;
    public bool IsHiding;
    public bool IsShooting;
    public bool IsPunching;
    public bool IsHurting;
    public bool IsReloading;
    public bool IsDead;

    public bool IsMoving => Horizontal != 0;
    public bool IsAttacking => IsShooting || IsPunching;

    #endregion


    public void InitializeUnit(GameObject unit)
    {
        uCollider = unit.GetComponent<Collider2D>();
        uTransform = unit.transform;
        uVisual = unit.GetComponentInChildren<SpriteRenderer>();
        uRigibody = unit.GetComponentInChildren<Rigidbody2D>();

        // Reset variables
        Horizontal = 0;
        TryJump = false;
        TryDuck = false;
        TryHide = false;
        TryShoot = false;
        TryPunch = false;

        IsFalling = false;
        IsJumping = false;
        IsDucking = false;
        IsHiding = false;
        IsShooting = false;
        IsPunching = false;
        IsHurting = false;
        IsReloading = false;
        IsDead = false;
    }
}
