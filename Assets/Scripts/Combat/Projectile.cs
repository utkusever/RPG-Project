using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 1f;
        Health target = null;
        GameObject instigator = null;
        float damage = 0;


        // Start is called before the first frame update
        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) return;

            if (isHoming)
            {
                if (target.IsDead() == false)
                {
                    transform.LookAt(GetAimLocation()); // to loook directly the target:()
                }
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime); //to move it to forward where its lookin at
        }

        public void SetTarget(Health target,GameObject instigator ,float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
            print("target's name: " + target);
        }

        private Vector3 GetAimLocation() //asagı dogru aim alıyor cünkü transformumuz ayaklarımızın altı bu yüzden cheste gitmesi için
                                         //capsule collidera göre aim aldırıyoruz yüksekligimizin yarısına falan. 
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 1.8f;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Projectile>())
            {
                Destroy(gameObject);
                return;
            }

            if (other.GetComponent<Health>() == null) return;
            if (target.IsDead()) return;
            speed = 0;
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            target.TakeDamage(damage,instigator);
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
