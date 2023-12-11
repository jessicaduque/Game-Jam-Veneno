using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingPoint : MonoBehaviour
{
    private SpriteRenderer thisSpriteRenderer;
    private GameObject guard = null;

    private void OnValidate()
    {
        if(thisSpriteRenderer == null)
        {
            thisSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public GameObject GetGuard()
    {
        return guard;
    }

    public void SetGuard(GameObject guard)
    {
        this.guard = guard;
    }

    public bool IsVisible()
    {
        return thisSpriteRenderer.isVisible;
    }
}
