using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float shootForce = 50f;
    public float upwardForce = 2f;

    [Header("Gun Stats")]
    public float timeBetweenShooting = 0.1f;
    public float spread = 0.0f;
    public float timeBetweenShots = 0.1f;
    public int bulletsPerTap = 1;
    public bool allowButtonHold = false;

    [Header("Hitscan Gun Stats")]
    public float hitscanDamage = 10f;
    public float hitscanRange = 100f;
    public float hitscanSpread = 0.05f;
    public float hitscanReloadTime = 1.2f;
    public float hitscanTimeBetweenShots = 0.2f;
    public int hitscanMagazineSize = 15;

    private int hitscanBulletsLeft;
    private bool hitscanReadyToShoot = true;
    private bool hitscanReloading = false;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce = 2f;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public GameObject rayMuzzleFlashPrefab;
    public GameObject glueMuzzleFlashPrefab;
    public GameObject trailPrefab;
    public GameObject bulletHitEffect;
    public Transform muzzlePos;
    public Transform muzzlePosGlue;
    public GameObject gunBobObj;
    private GunBob gunBob;

    private bool shooting, readyToShoot = true;

    private void Awake()
    {
        hitscanBulletsLeft = hitscanMagazineSize;
        gunBob = gunBobObj.GetComponent<GunBob>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse1) : Input.GetKeyDown(KeyCode.Mouse1);
        bool hitscanShoot = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToShoot && shooting)
        {
            Shoot();
        }

        if (hitscanReadyToShoot && hitscanShoot && !hitscanReloading && hitscanBulletsLeft > 0)
        {
            FireHitscan();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

        Vector3 direction = (targetPoint - attackPoint.position).normalized;
        direction += Random.insideUnitSphere * spread;

        GameObject bullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.LookRotation(direction));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb)
        {
            bulletRb.AddForce(direction * shootForce, ForceMode.Impulse);
            bulletRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        }

        if (glueMuzzleFlashPrefab)
        {
            GameObject muzzleFlash = Instantiate(glueMuzzleFlashPrefab, muzzlePosGlue.position, Quaternion.identity);
            muzzleFlash.transform.parent = muzzlePosGlue;
            muzzleFlash.transform.localRotation = muzzlePosGlue.localRotation;
        }

        if (playerRb)
        {
            playerRb.AddForce(-direction * recoilForce, ForceMode.Impulse);
        }

        CinemachineShake.Instance.Shake(0.05f, 0.1f);

        //play glue gun shot sound
        AudioManager.instance.PlayOneShot(FMODEvents.instance.glueGunShot, this.transform.position);

        Invoke(nameof(ResetShot), timeBetweenShooting);
    }

    private void FireHitscan()
    {
        gunBob.TriggerRecoil();

        hitscanReadyToShoot = false;
        hitscanBulletsLeft--;

        Vector3 direction = fpsCam.transform.forward;
        Vector3 hitPoint = fpsCam.transform.position + (direction * hitscanRange);


        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit hit, hitscanRange))
        {
            hitPoint = hit.point;
            GameObject hitEffect = Instantiate(bulletHitEffect, hitPoint, Quaternion.identity);
            hitEffect.transform.LookAt(transform);
            if (hit.collider.gameObject.TryGetComponent(out Entity entity))
            {
                entity.GetHit(hitscanDamage);
            }
        }

        StartCoroutine(SpawnTrail(muzzlePos.position, hitPoint));

        if (rayMuzzleFlashPrefab)
        {
            GameObject muzzleFlash = Instantiate(rayMuzzleFlashPrefab, muzzlePos.position, Quaternion.identity);
            muzzleFlash.transform.parent = muzzlePos;
            muzzleFlash.transform.localRotation = muzzlePos.localRotation;
        }

        // Play nail gun shot sound
        AudioManager.instance.PlayOneShot(FMODEvents.instance.nailGunShot, transform.position);
        CinemachineShake.Instance.Shake(0.1f, 0.2f);
        Invoke(nameof(ResetHitscanShot), hitscanTimeBetweenShots);

        if (hitscanBulletsLeft <= 0)
        {
            ReloadHitscan();
        }
    }

    private IEnumerator SpawnTrail(Vector3 start, Vector3 end)
    {
        GameObject trailInstance = Instantiate(trailPrefab, start, Quaternion.identity);
        TrailRenderer trail = trailInstance.GetComponent<TrailRenderer>();
        float time = 0;
        float duration = 0.06f;

        while (time < duration)
        {
            trailInstance.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        trailInstance.transform.position = end;
        Destroy(trailInstance, trail.time);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void ResetHitscanShot()
    {
        hitscanReadyToShoot = true;
    }

    private void ReloadHitscan()
    {
        if (hitscanReloading) return;
        hitscanReloading = true;
        Invoke(nameof(FinishHitscanReload), hitscanReloadTime);
    }

    private void FinishHitscanReload()
    {
        hitscanBulletsLeft = hitscanMagazineSize;
        hitscanReloading = false;
    }
}
