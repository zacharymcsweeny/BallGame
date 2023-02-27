using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float expansionRate = 40f;

    // Update is called once per frame
    void Update()
    {
        Vector3 scaleChange = new Vector3(expansionRate * Time.deltaTime, expansionRate * Time.deltaTime, expansionRate * Time.deltaTime);
        transform.localScale += scaleChange;

        if (transform.localScale.x > 10f)
        {
            Destroy(gameObject);
        }
    }
}
