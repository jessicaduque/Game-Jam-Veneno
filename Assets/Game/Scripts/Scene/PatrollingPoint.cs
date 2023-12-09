using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingPoint : MonoBehaviour
{
    private GameObject guard = null;

    public GameObject GetGuard()
    {
        return guard;
    }

    public void SetGuard(GameObject guard)
    {
        this.guard = guard;
    }
}
