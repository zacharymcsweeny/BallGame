using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    float moveSpeed = 24f;
    float sprintSpeedMult = 1.2f;
    float sprintSpeedCapMult = 1.1f;
    float speedSoftCap = 30f;
    float softCapForce = 50f;
    float jumpForce = 150f;
    float gravityMultiplier = 1f;
    float localGravity = -9.81f;
    bool grounded = false;
    float beamLength = 100f;

    string currentWeapon = "None";
    float weaponCooldown = 0f;
    float weaponCooldownRate = 1f;
    float weaponRearmFlashLength = 0.1f;
    float weaponRearmFlashTimer = 0f;
    float weaponRechargeDelay = 0f;

    float currentPulseRifleAmmo = 60f;
    float maxPulseRifleAmmo = 60f;
    float pulseRifleAmmoRechargeRate = 60f;
    float pulseRifleAmmoRechargeDelay = 1f;
    float currentPulseRifleRechargeDelay = 0f;
    float pulseRifleDamage = 1f;
    float pulseRifleRecoilRecoveryRate = 20f;
    float pulseRifleRecoilTimer0 = 0f;
    float pulseRifleRecoilTimer1 = 0f;
    float pulseRifleFiringSet = 0;
    float pulseRifleFireRate = 20f;
    float pulseRifleHitMarkDuration = 0.3f;

    float currentHeavyCannonAmmo = 5f;
    float maxHeavyCannonAmmo = 5f;
    float heavyCannonAmmoRechargeRate = 1f;
    float heavyCannonAmmoRechargeDelay = 3f;
    float currentHeavyCannonRechargeDelay = 0f;
    float heavyCannonDamage = 10f;
    float heavyCannonFireRate = 1f;
    float heavyCannonBlastRadius = 5f;
    float heavyCannonBlastForce = 1000f;
    float heavyCannonBlastLiftForce = 0.5f;
    float heavyCannonBlastDamage = 10f;

    int nextTurretIndex = 0;
    int maxTurrets = 3;

    public Text currentWeaponNameDisplay;
    public Text currentWeaponAmmoDisplay;


    [SerializeField]
    GameObject pulseHit, explosion, blade, turret;

    GameObject blade0;
    GameObject blade1;
    GameObject blade2;
    GameObject blade3;

    //GameObject[] turrets = new GameObject[3];
    GameObject turrets;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        blade0 = (GameObject)Instantiate(blade, transform.position, transform.rotation);
        blade1 = (GameObject)Instantiate(blade, transform.position, transform.rotation);
        blade2 = (GameObject)Instantiate(blade, transform.position, transform.rotation);
        blade3 = (GameObject)Instantiate(blade, transform.position, transform.rotation);

        SwitchWeapon("None");
    }

    // Update is called once per frame
    void Update()
    {
        var forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var right = Camera.main.transform.right;
        right.y = 0f;
        right.Normalize();

        float appliedMoveSpeed = moveSpeed;
        appliedMoveSpeed += moveSpeed * sprintSpeedMult * Input.GetAxis("Sprint");

        rb.AddForce(forward * appliedMoveSpeed * Input.GetAxis("Vertical"), ForceMode.Acceleration);
        rb.AddForce(right * appliedMoveSpeed * Input.GetAxis("Horizontal"), ForceMode.Acceleration);
        rb.AddForce(gravityMultiplier * localGravity * Vector3.up, ForceMode.Acceleration);

        Vector3 netSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float appliedSpeedSoftCap = speedSoftCap;
        appliedSpeedSoftCap += speedSoftCap * sprintSpeedCapMult * Input.GetAxis("Sprint");

        if (netSpeed.magnitude > appliedSpeedSoftCap)
        {
            rb.AddForce(netSpeed.normalized * -softCapForce, ForceMode.Acceleration);
        }

        if (Input.GetKeyDown(KeyCode.Space) & grounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
        }

        if (Input.GetAxis("Fire1") == 1 && weaponCooldown <= 0)
        {
            FireWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon("Pulse Rifle");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon("Heavy Cannon");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon("Blades");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (grounded)
            {
                // Debug.Log(nextTurretIndex);
                // Destroy(turrets[nextTurretIndex]);
                // nextTurretIndex++;
                // DeployTurret(nextTurretIndex);
                // if (nextTurretIndex >= maxTurrets)
                // {
                //     nextTurretIndex = 0;
                // }
                DeployTurret();
            }
        }

        if (weaponRearmFlashTimer > 0)
        {
            weaponRearmFlashTimer -= Time.deltaTime;
        }

        if (weaponCooldown > 0)
        {
            weaponCooldown -= Time.deltaTime * weaponCooldownRate;
        }
        else if (weaponCooldown < 0)
        {
            weaponCooldown = 0f;
            weaponRearmFlashTimer = weaponRearmFlashLength;
        }

        if (pulseRifleRecoilTimer0 > 0)
        {
            pulseRifleRecoilTimer0 -= Time.deltaTime * pulseRifleRecoilRecoveryRate;
        }
        else if (pulseRifleRecoilTimer0 < 0)
        {
            pulseRifleRecoilTimer0 = 0f;
        }

        if (pulseRifleRecoilTimer1 > 0)
        {
            pulseRifleRecoilTimer1 -= Time.deltaTime * pulseRifleRecoilRecoveryRate;
        }
        else if (pulseRifleRecoilTimer1 < 0)
        {
            pulseRifleRecoilTimer1 = 0f;
        }

        if (currentPulseRifleAmmo < maxPulseRifleAmmo && currentPulseRifleRechargeDelay <= 0)
        {
            currentPulseRifleAmmo += pulseRifleAmmoRechargeRate * Time.deltaTime;
        }
        else if (currentPulseRifleRechargeDelay > 0)
        {
            currentPulseRifleRechargeDelay -= Time.deltaTime;
        }

        if (currentHeavyCannonAmmo < maxHeavyCannonAmmo && currentHeavyCannonRechargeDelay <= 0)
        {
            currentHeavyCannonAmmo += heavyCannonAmmoRechargeRate * Time.deltaTime;
        }
        else if (currentHeavyCannonRechargeDelay > 0)
        {
            currentHeavyCannonRechargeDelay -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        UpdateWeaponText();
        MoveWeapon();
    }

    void SwitchWeapon(string newWeapon)
    {
        switch (newWeapon)
        {
            case "None":
                blade0.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                blade1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                blade2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                blade3.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                
                currentWeapon = "None";
                break;

            case "Pulse Rifle":
                blade0.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade3.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                currentWeapon = "Pulse Rifle";
                break;

            case "Heavy Cannon":
                blade0.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade3.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                currentWeapon = "Heavy Cannon";
                break;

            case "Blades":
                blade0.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                blade3.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                currentWeapon = "Blades";
                break;

            default:
                break;
        }
    }

    void FireWeapon()
    {
        int layerMask;
        Ray crosshair, beam;
        Vector3 aimPoint;
        RaycastHit hit;

        switch (currentWeapon)
        {
            case "None":
                break;

            case "Pulse Rifle":
                if (currentPulseRifleAmmo > 0)
                {
                    if (pulseRifleFiringSet == 0)
                    {
                        pulseRifleRecoilTimer0 = 1f;
                        pulseRifleFiringSet = 1f;
                        weaponCooldown = 1f;
                        weaponCooldownRate = pulseRifleFireRate;
                    }
                    else if (pulseRifleFiringSet == 1)
                    {
                        pulseRifleRecoilTimer1 = 1f;
                        pulseRifleFiringSet = 0f;
                        weaponCooldown = 1f;
                        weaponCooldownRate = pulseRifleFireRate;
                    }

                    layerMask = 1 << 8;
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

                    beam = new Ray(transform.position, aimPoint - transform.position);
                    //Debug.DrawRay(transform.position, aimPoint - transform.position * 100, Color.blue, 4f, false);
                    
                    if (Physics.Raycast(beam, out hit, beamLength, layerMask))
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
                    }

                    currentPulseRifleAmmo--;
                    currentPulseRifleRechargeDelay = pulseRifleAmmoRechargeDelay;
                }
                
                break;

            case "Heavy Cannon":
                if (currentHeavyCannonAmmo > 0)
                {
                    layerMask = 1 << 8;
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

                    beam = new Ray(transform.position, aimPoint - transform.position);
                    //Debug.DrawRay(transform.position, aimPoint - transform.position * 100, Color.blue, 4f, false);
                    
                    if (Physics.Raycast(beam, out hit, beamLength, layerMask))
                    {
                        if (hit.collider.tag == "Enemy")
                        {
                            hit.collider.GetComponent<EnemyHealth>().AdjustCurrentHealth(-heavyCannonDamage);
                        }

                        Collider[] blastedColliders = Physics.OverlapSphere(hit.point, heavyCannonBlastRadius);

                        foreach (Collider pushed in blastedColliders)
                        {
                            Rigidbody victim = pushed.GetComponent<Rigidbody>();
                            if (victim != null)
                            {
                                victim.AddExplosionForce(heavyCannonBlastForce, hit.point, heavyCannonBlastRadius, heavyCannonBlastLiftForce);
                            }

                            if (pushed.tag == "Enemy")
                            {
                                pushed.GetComponent<EnemyHealth>().AdjustCurrentHealth(-heavyCannonBlastDamage);
                            }
                        }
                        
                        GameObject boom = (GameObject)Instantiate(explosion, hit.point, transform.rotation);

                        blade0.transform.LookAt(boom.transform);
                        blade1.transform.LookAt(boom.transform);
                        blade2.transform.LookAt(boom.transform);
                        blade3.transform.LookAt(boom.transform);

                        blade0.transform.Rotate(0f, 90f, 0f, Space.Self);
                        blade1.transform.Rotate(0f, 90f, 0f, Space.Self);
                        blade2.transform.Rotate(0f, 90f, 0f, Space.Self);
                        blade3.transform.Rotate(0f, 90f, 0f, Space.Self);
                    }

                    weaponCooldown = 1f;
                    weaponCooldownRate = heavyCannonFireRate;

                    currentHeavyCannonAmmo--;
                    currentHeavyCannonRechargeDelay = heavyCannonAmmoRechargeDelay;
                }
                
                break;

            case "Blades":
                break;

            default:
                break;
        }
    }

    void MoveWeapon()
    {
        switch (currentWeapon)
        {
            case "None":
                blade0.transform.position = transform.position;
                blade1.transform.position = transform.position;
                blade2.transform.position = transform.position;
                blade3.transform.position = transform.position;

                break;

            case "Pulse Rifle":
                blade0.transform.rotation = Camera.main.transform.rotation;
                blade1.transform.rotation = Camera.main.transform.rotation;
                blade2.transform.rotation = Camera.main.transform.rotation;
                blade3.transform.rotation = Camera.main.transform.rotation;

                blade0.transform.Rotate(0f, 90f, 0f, Space.Self);
                blade1.transform.Rotate(0f, 90f, 0f, Space.Self);
                blade2.transform.Rotate(0f, 90f, 0f, Space.Self);
                blade3.transform.Rotate(0f, 90f, 0f, Space.Self);

                if (weaponCooldown > 0 && weaponCooldown <= 1)
                {
                    blade0.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(0f, 0.3f, 1f), weaponCooldown);
                    blade1.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(0f, 0.3f, 1f), weaponCooldown);
                    blade2.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(0f, 0.3f, 1f), weaponCooldown);
                    blade3.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(0f, 0.3f, 1f), weaponCooldown);
                }
                else
                {
                    blade0.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                    blade1.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                    blade2.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                    blade3.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                }
                
                if (pulseRifleRecoilTimer0 <= 0)
                {
                    blade0.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * -0.5f;
                    blade2.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.right * 1f;
                }
                else
                {
                    blade0.transform.position = transform.position + Camera.main.transform.forward * -1.2f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * -0.5f;
                    blade2.transform.position = transform.position + Camera.main.transform.forward * -1.2f + Camera.main.transform.right * 1f;
                }

                if (pulseRifleRecoilTimer1 <= 0)
                {
                    blade1.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * 0.5f;
                    blade3.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.right * -1f;
                }
                else
                {
                    blade1.transform.position = transform.position + Camera.main.transform.forward * -1.2f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * 0.5f;
                    blade3.transform.position = transform.position + Camera.main.transform.forward * -1.2f + Camera.main.transform.right * -1f;
                }

                break;

            case "Heavy Cannon":
                if (weaponRearmFlashTimer > 0 && weaponCooldown == 0)
                {
                    blade0.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                    blade1.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                    blade2.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                    blade3.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
                }
                else
                {
                    if (weaponCooldown > 0 && weaponCooldown <= 1)
                    {
                        blade0.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(1f, 0f, 0f), weaponCooldown);
                        blade1.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(1f, 0f, 0f), weaponCooldown);
                        blade2.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(1f, 0f, 0f), weaponCooldown);
                        blade3.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), new Color(1f, 0f, 0f), weaponCooldown);
                    }
                    else
                    {
                        blade0.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                        blade1.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                        blade2.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                        blade3.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                }

                if (weaponCooldown <= 0)
                {
                    blade0.transform.rotation = Camera.main.transform.rotation;
                    blade1.transform.rotation = Camera.main.transform.rotation;
                    blade2.transform.rotation = Camera.main.transform.rotation;
                    blade3.transform.rotation = Camera.main.transform.rotation;

                    blade0.transform.Rotate(0f, 90f, 0f, Space.Self);
                    blade1.transform.Rotate(0f, 90f, 0f, Space.Self);
                    blade2.transform.Rotate(0f, 90f, 0f, Space.Self);
                    blade3.transform.Rotate(0f, 90f, 0f, Space.Self);
                }
                
                blade0.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * -0.5f;
                blade1.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * 0.5f + Camera.main.transform.right * 0.5f;
                blade2.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * -0.5f + Camera.main.transform.right * 0.5f;
                blade3.transform.position = transform.position + Camera.main.transform.forward * -1f + Camera.main.transform.up * -0.5f + Camera.main.transform.right * -0.5f;

                break;

            case "Blades":
                break;

            default:
                break;
        }
    }

    void DeployTurret()
    {
        turrets = (GameObject)Instantiate(turret, transform.position, Quaternion.Euler(0, 0, 0));
    }

    void UpdateWeaponText()
    {
        switch (currentWeapon)
        {
            case "None":
                break;
            
            case "Pulse Rifle":
                currentWeaponNameDisplay.text = currentWeapon;
                if (currentPulseRifleAmmo < 0)
                {
                    currentPulseRifleAmmo = 0f;
                }
                currentWeaponAmmoDisplay.text = Mathf.Floor(currentPulseRifleAmmo).ToString() + "/" + Mathf.Floor(maxPulseRifleAmmo).ToString();

                break;

            case "Heavy Cannon":
                currentWeaponNameDisplay.text = currentWeapon;
                if (currentHeavyCannonAmmo < 0)
                {
                    currentHeavyCannonAmmo = 0f;
                }
                currentWeaponAmmoDisplay.text = Mathf.Floor(currentHeavyCannonAmmo).ToString() + "/" + Mathf.Floor(maxHeavyCannonAmmo).ToString();

                break;

            case "Blades":
                break;

            default:
                break;
        }
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    void OnCollisionExit()
    {
        grounded = false;
    }
}
