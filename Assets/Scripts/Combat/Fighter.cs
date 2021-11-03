using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;
using RPG.Combat;
using System;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, IModifierProvider
    {


        [SerializeField] float timeBetweenAttacks = 1f;


        [SerializeField] Transform rightHandTransfrom = null;
        [SerializeField] Transform leftHandTransfrom = null;

        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        Weapon currentWeapon = null;


        Health target; //health scriptine shaip olan herseyi erisebilmek için degiştim transform to health.
        float timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(defaultWeaponName);
            //EquipWeapon(defaultWeapon);
            EquipWeapon(weapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransfrom, leftHandTransfrom, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null)
            {
                return;
            }

            if (target.IsDead()) return;

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //hit eventini triggerlayacak
                GetComponent<Animator>().ResetTrigger("stopAttacking");
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeapon.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttacking");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetPercantageBonus();
            }
        }

        //Animation Event
        void Hit()
        {
            if (target == null) { return; }
            float baseDamage = GetComponent<BaseStats>().GetStat(Stat.Damage); //base dmg'im additive dmg ile basedmg+silahın dmg'i oldu..
            print("base damageim: " + baseDamage);
            // float calculatedDamage = baseDamage + currentWeapon.GetDamage(); // base dmg'im + silahımın dmg'i
            // calculated dmg yerine basedamage gönderiyorum additive modifierdan ekstra dmg alıcak.

            // Health healtcompenent = target.GetComponent<Health>(); artık gerek yok Healt target; yaptım.
            // healtcompenent yerine direkt target. kullanabilirm
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransfrom, leftHandTransfrom, target, gameObject, baseDamage);
            }
            else
            {
                target.TakeDamage(baseDamage, gameObject);
            }
            print("weapon dmg: " + baseDamage);
        }

        void Shoot()
        {
            Hit();
        }

    }
}