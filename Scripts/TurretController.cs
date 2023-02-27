using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    float pulseRifleDamage = 1f;
    float pulseRifleRecoilRecoveryRate = 20f;
    float pulseRifleRecoilTimer = 0f;
    float pulseRifleFireRate = 20f;
    float pulseRifleHitMarkDuration = 2f;

    float weaponCooldown = 0f;
    float weaponCooldownRate = 1f;
    float beamLength = 100f;

    [SerializeField]
    GameObject pulseHit, blade, ray;

    GameObject blade0;

    // Start is called before the first frame update
    void Start()
    {
        blade0 = (GameObject)Instantiate(blade, transform.position, transform.rotation);
        blade0.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        PointWeapon();

        if (weaponCooldown > 0)
        {
            weaponCooldown -= Time.deltaTime * weaponCooldownRate;
        }
        else if (weaponCooldown < 0)
        {
            weaponCooldown = 0f;
        }

        if (Input.GetAxis("Fire2") == 1 && weaponCooldown <= 0)
        {
            FireWeapon();
        }
    }
    
    void PointWeapon()
    {
        Ray crosshair, beam;
        Vector3 aimPoint;
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        crosshair = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.green, 4f, false);

        if (Physics.Raycast(crosshair, out hit, beamLength, layerMask))
        {
            aimPoint = hit.point;
            //Debug.DrawRay(aimPoint, Vector3.forward, Color.red, 2f, false);
        }
        else
        {
            aimPoint = crosshair.origin + crosshair.direction * beamLength;
        }

        blade0.transform.position = transform.position + transform.up * 1f;

        Vector3 direction = aimPoint - blade0.transform.position;
        //Debug.DrawRay(blade0.transform.position, direction, Color.blue, 1f, false);
        //blade0.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        //blade0.transform.Rotate(direction, Space.World);
        blade0.transform.LookAt(aimPoint);
        blade0.transform.Rotate(0f, 90f, 0f, Space.Self);
        //Debug.DrawRay(blade0.transform.position, blade0.transform.right * -10, Color.green, 4f, false);

        
    }

    void FireWeapon()
    {
        int layerMask;
        Ray crosshair, beam;
        Vector3 aimPoint;
        RaycastHit hit;

        layerMask = 1 << 8;
        layerMask = ~layerMask;
        crosshair = new Ray(blade0.transform.position, -blade0.transform.right);
        //Debug.DrawRay(blade0.transform.position, blade0.transform.right * -10, Color.green, 2f, false);

        if (Physics.Raycast(crosshair, out hit, beamLength, layerMask))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<EnemyHealth>().AdjustCurrentHealth(-pulseRifleDamage);
            }

            GameObject hitMark = (GameObject)Instantiate(pulseHit, hit.point, transform.rotation);
            hitMark.transform.rotation = Quaternion.FromToRotation (transform.up, hit.normal) * hitMark.transform.rotation;
            hitMark.transform.position += hit.normal * -0.99f;

            hitMark.GetComponent<DestroyTimer>().SetTimer(pulseRifleHitMarkDuration);
            hitMark.transform.parent = hit.transform;

            GameObject trail = (GameObject)Instantiate(ray, blade0.transform.position, blade0.transform.rotation);
            trail.transform.Rotate(0f, 90f, 0f, Space.Self);
            trail.transform.localScale = new Vector3(0.1f, 0.1f, hit.distance * 0.5f);
        }

        weaponCooldown = 1f;
    }

    void OnDestroy()
    {
        Destroy(blade0);
    }
}
