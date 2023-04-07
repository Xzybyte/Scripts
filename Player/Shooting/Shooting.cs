using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    private GameObject bulletPrefab;
    private Transform gunPosition;
    private Player playerObj;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text gunNameText;
    private Mouse mouse;
    private Gamepad gamepad;
    
    private decimal firingTimer = 0.0m;
    private float bulletTravelTime = 3f;

    private bool firstGun = true;
    private float secondGunAmmo = 40;
    private float maxAmmo = 150;

    // Update is called once per frame

    private void Start()
    {
        mouse = InputSystem.GetDevice<Mouse>();
        gamepad = InputSystem.GetDevice<Gamepad>();
        secondGunAmmo = maxAmmo;
        firstGun = true;
        gunPosition = transform.GetChild(3);
        bulletPrefab = gunPosition.GetComponent<AimController>().getCurrentBullet();
        playerObj = GetComponent<Player>(); // test

    }

    void Update()
    {
        if (this.GetComponent<Player>().getDead())
        {
            return;
        }
        if (mouse != null && mouse.leftButton.isPressed || gamepad != null && gamepad.rightShoulder.isPressed)
        {
            firingTimer -= new decimal(Time.deltaTime); // fire timer;
            if (firingTimer < 0)
            {
                Shoot();
                firingTimer += playerObj.GetShootingDelay();
            }
        }
    }

    public void updateGun(GameObject gun, bool firstGun, string gunName)
    {
        this.firstGun = firstGun;
        if (this.firstGun)
        {
            ammoText.text = "-/-";
            ammoText.rectTransform.anchoredPosition = new Vector2(-649.1f, -482.6f);
            gunNameText.text = "Fire " + gunName; 
            gunNameText.rectTransform.anchoredPosition = new Vector2(-649.1f, -322.6f);
        } else
        {
            ammoText.text = secondGunAmmo + "/" + maxAmmo;
            ammoText.rectTransform.anchoredPosition = new Vector2(-549.1f, -482.6f);
            gunNameText.text = "Fire " + gunName;
            gunNameText.rectTransform.anchoredPosition = new Vector2(-549.1f, -322.6f);
        }
        gunPosition = gun.transform;
        bulletPrefab = gunPosition.GetComponent<AimController>().getCurrentBullet();
    }

    public void resetAmmo()
    {
        secondGunAmmo = maxAmmo;
        if (!firstGun)
        {
            ammoText.text = secondGunAmmo + "/" + maxAmmo;
        }
    }

    void Shoot()
    {
        if (!firstGun)
        {
            if (secondGunAmmo > 0)
            {
                secondGunAmmo -= 1;
                ammoText.text = secondGunAmmo + "/" + maxAmmo;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().StatusMessage("You do not have enough ammo.", 3);
                return;
            }
        }
        // lastShot = Time.time;
        //audioSource.Play();
        FindObjectOfType<AudioManager>().Play("shoot");

        Vector3 bulletPosition = new Vector3(gunPosition.position.x, gunPosition.position.y, gunPosition.position.z); // Set the bullet's shooting position

        GameObject bullet = Instantiate(bulletPrefab, bulletPosition, gunPosition.rotation); // Instantiate the bullet

        Vector3 forcePosition = gunPosition.right; //new Vector3(0, gunPosition.right * bulletForce;

        if (transform.GetComponent<PlayerMovement>().getFlip())
        {
            float bulletAngle = (180 + gunPosition.rotation.eulerAngles.z) % 360;
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, bulletAngle + gunPosition.rotation.z));
            forcePosition = bullet.transform.right;
        }

        bullet.GetComponent<Bullet>().setBulletDamage(transform.GetComponent<Player>().getDamageByGun());

        Physics2D.IgnoreCollision(bullet.GetComponent<CircleCollider2D>(), bullet.GetComponent<CircleCollider2D>());

        GameObject hedgeHog = GameObject.FindGameObjectWithTag("HedgeHog");
        if (hedgeHog != null)
        {
            Physics2D.IgnoreCollision(hedgeHog.GetComponent<CircleCollider2D>(), bullet.GetComponent<CircleCollider2D>());
        }

        Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>(), bullet.GetComponent<CircleCollider2D>());

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); // Get the bullet's rigidbody
        rb.AddForce(forcePosition * (float)playerObj.GetBulletSpeed(), ForceMode2D.Impulse); // Move the bullet
        StartCoroutine(BulletTravelTime(bullet));
        // Destroy(firePoint);
    }
    

    private IEnumerator BulletTravelTime(GameObject bullet)
    {
        yield return new WaitForSeconds(bulletTravelTime);
        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
}
