using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainBehaviour : MonoBehaviour
{
    public bool IsActive => _active;

    bool _active = true;

    public abstract void UpdateBehaviour();
}
