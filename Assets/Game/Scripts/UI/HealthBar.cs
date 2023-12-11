using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private GameObject parentEnemy;
    private float offSetY;

    public void SetEnemy(GameObject enemy, float offSetY)
    {
        this.offSetY = offSetY;
        parentEnemy = enemy;
    }

    private void Update()
    {
        if(parentEnemy != null)
        {
            transform.position = new Vector2(parentEnemy.transform.position.x, parentEnemy.transform.position.y + offSetY);
        }
    }

}
