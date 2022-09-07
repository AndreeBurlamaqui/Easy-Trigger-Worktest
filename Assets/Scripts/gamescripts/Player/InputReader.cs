using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputReader")]
public class InputReader : ScriptableObject
{
    public UnitData Unit;
    public const string BindPrefs = "UseCustomBind";

    public void Initialize(UnitData _unit)
    {
        Unit = _unit;
        SetupInput();
    }

    void SetupInput()
    {
        bool useCustomBind = PlayerPrefs.HasKey(BindPrefs);

        // TODO: Get custom binds by prefs
        InitializeKeyboard(useCustomBind);
    }

    // INPUT BY CONTROLLER TYPE
    // We can create as many controller type we want, so we can have gamepad e keyboard
    // if both are enabled, then we'll get both input buffer
    // This way we can avoid annoying bugs that makes one controller block the existence of other

    public KeyCode[] LeftInput => new KeyCode[] { r_KeyboardLeft };
    public KeyCode[] RightInput => new KeyCode[] { r_KeyboardRight };
    public KeyCode[] JumpInput => new KeyCode[] { r_KeyboardJump };
    public KeyCode[] DuckInput => new KeyCode[] { r_KeyboardDuck };
    public KeyCode[] HideInput => new KeyCode[] { r_KeyboardHide };
    public KeyCode[] ShootInput => new KeyCode[] { r_KeyboardShoot };
    public KeyCode[] PunchInput => new KeyCode[] { r_KeyboardPunch };

    public void UpdateInput()
    {
        Unit.Horizontal = CheckKey(LeftInput) ? -1 : CheckKey(RightInput) ? 1 : 0;
        Unit.TryDuck = CheckKey(DuckInput);
        Unit.TryJump = CheckKey(JumpInput);
        Unit.TryHide = CheckKey(HideInput);
        Unit.TryShoot = CheckKey(ShootInput);
        Unit.TryPunch = CheckKey(PunchInput);
    }

    public bool CheckKey(KeyCode[] iKey)
    {
        foreach (KeyCode kc in iKey)
            if (Input.GetKey(kc))
                return true;

        return false;
    }

    #region KEYBOARD CONTROLLER

    // s_ keys will be the start value, so we can create a custom input binding system after

    [Header("KEYBOARD INPUTS")]
    [SerializeField] KeyCode s_KeyboardLeft; 
    [SerializeField] KeyCode s_KeyboardRight; 
    [SerializeField] KeyCode s_KeyboardDuck; 
    [SerializeField] KeyCode s_KeyboardJump; 
    [SerializeField] KeyCode s_KeyboardHide; 
    [SerializeField] KeyCode s_KeyboardShoot; 
    [SerializeField] KeyCode s_KeyboardPunch;

    // r_ keys are the runtime ones, if we made a custom input binding system, it'll override these

    KeyCode r_KeyboardLeft;
    KeyCode r_KeyboardRight;
    KeyCode r_KeyboardDuck;
    KeyCode r_KeyboardJump;
    KeyCode r_KeyboardHide;
    KeyCode r_KeyboardShoot;
    KeyCode r_KeyboardPunch;

    void InitializeKeyboard(bool customBind)
    {
        r_KeyboardLeft = customBind ? KeyCode.None : s_KeyboardLeft;
        r_KeyboardRight = customBind ? KeyCode.None : s_KeyboardRight;
        r_KeyboardDuck = customBind ? KeyCode.None : s_KeyboardDuck;
        r_KeyboardJump = customBind ? KeyCode.None : s_KeyboardJump;
        r_KeyboardHide = customBind ? KeyCode.None : s_KeyboardHide;
        r_KeyboardShoot = customBind ? KeyCode.None : s_KeyboardShoot;
        r_KeyboardPunch = customBind ? KeyCode.None : s_KeyboardPunch;
    }

    #endregion
}
