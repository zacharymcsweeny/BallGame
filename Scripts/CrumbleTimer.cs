using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleTimer : MonoBehaviour
{
    float maxTime = 1f;
    float remainingTime = 1f;
    bool isCrumbling = false;
    Color startingColor;

    // Start is called before the first frame update
    void Start()
    {
        startingColor = gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCrumbling)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0)
            {
                Destroy(gameObject);
            }

            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.2f, 0.2f, 0.8f, 0.2f), startingColor, remainingTime / maxTime);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Player")
        {
            isCrumbling = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Player")
        {
            isCrumbling = false;
        }
    }
}
