using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    float initialTimer;
    float timer = 0f;
    float initialXScale;
    float initialYScale;
    float initialZScale;

    // Start is called before the first frame update
    void Start()
    {
        initialTimer = gameObject.GetComponent<DestroyTimer>().timer;
        initialXScale = transform.localScale.x;
        initialYScale = transform.localScale.y;
        initialZScale = transform.localScale.z;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Vector3 scaleChange = new Vector3((initialTimer - timer) * initialXScale, (initialTimer - timer) * initialYScale, initialZScale);
        transform.localScale = scaleChange;
    }
}
