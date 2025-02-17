using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float shootForce = 50f;
    public float upwardForce = 2f;

    [Header("Gun Stats")]
    public float timeBetweenShooting = 0.1f;
    public float spread = 0.1f;
    public float reloadTime = 1.5f;
    public float timeBetweenShots = 0.1f;
    public int magazineSize = 10;
    public int bulletsPerTap = 1;
    public bool allowButtonHold = false;

    private int bulletsLeft;
    private int bulletsShot;

    [Header("Recoil")]
    public Rigidbody playerRb;
    public float recoilForce = 2f;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public GameObject muzzleFlashPrefab;

    private bool shooting, readyToShoot = true, reloading = false;

    private void Awake()
    {
        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

        Vector3 direction = (targetPoint - attackPoint.position).normalized;

        // Apply spread using a random spherical offset
        direction += Random.insideUnitSphere * spread;

        GameObject bullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.LookRotation(direction));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb)
        {
            bulletRb.AddForce(direction * shootForce, ForceMode.Impulse);
            bulletRb.AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        }

        // Muzzle Flash
        if (muzzleFlashPrefab)
        {
            Instantiate(muzzleFlashPrefab, attackPoint.position, Quaternion.identity);
        }

        // Apply recoil
        if (playerRb)
        {
            playerRb.AddForce(-direction * recoilForce, ForceMode.Impulse);
        }

        bulletsLeft--;
        bulletsShot++;

        Invoke(nameof(ResetShot), timeBetweenShooting);

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (reloading) return;
        reloading = true;
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
