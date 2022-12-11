using UnityEngine;
using System.Collections;

namespace Project.Scripts.Weapon
{
    public class FireGun : MonoBehaviour
    {
        [SerializeField] private Transform barrelEnd;
        [SerializeField] private float radius = 0.1f;
        [SerializeField] private float velocity = 1000f;
        [SerializeField] private float mass = .5f;
        private HandPoseController HandPose;
        public GameObject energyBall;
        private RedHollowControl energyBallController;

        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public float Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        void Start()
        {
            HandPose = GetComponent<HandPoseController>();
            HandPose.OnFiring = FireBullet;
            energyBall.SetActive(true);
            energyBallController = energyBall.GetComponent<RedHollowControl>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireBullet();
            }
        }

        IEnumerator StartBulletTrajectory(GameObject bullet, Vector3 direction)
        {
            yield return new WaitForSeconds(3f);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.drag = 0;
            rb.useGravity = true;
            rb.velocity = direction * Velocity;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Destroy(bullet, 5);
        }

        private void FireBullet()
        {
            //energyBallController.Burst_Beam();
            //StartCoroutine("StopAnimation");
            var bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 bulletOffset = barrelEnd.forward;
            bullet.tag = "Bullet";
            bullet.transform.position = barrelEnd.position;
            //bullet.transform.Translate(bulletOffset);
            bullet.transform.localScale = Vector3.one * Radius / 2;
            bullet.AddComponent<CollisionControl>();
            GameObject newEnergyBall = Instantiate(energyBall, bullet.transform);
            newEnergyBall.AddComponent<RedHollowControl>();
            newEnergyBall.SetActive(true);
            newEnergyBall.transform.position = barrelEnd.position;
            //newEnergyBall.transform.Translate(bulletOffset);
            newEnergyBall.transform.localScale = Vector3.one * Radius;
            var controller = newEnergyBall.GetComponent<RedHollowControl>();
            controller.Play_Charging();

            var mat = bullet.GetComponent<Renderer>().material;
            mat.color = Color.red;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.red);

            var rb = bullet.AddComponent<Rigidbody>();

            // initial animation
            rb.useGravity = false;
            rb.drag = 5;
            rb.velocity = barrelEnd.transform.forward * 5;
            rb.mass = mass;
            //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            StartCoroutine(StartBulletTrajectory(bullet, barrelEnd.transform.forward));

            //Destroy(bullet, 5);
        }
    }
}