using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Stats;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Resources
{
    public class Health : MonoBehaviour
    {
        GameObject player;
        Health playerHealth;
        Health enemyHealth;
        [SerializeField] float healthPoints = 100f;
        [SerializeField] float regenateHealtPercentage = 70;
        [SerializeField] UnityEvent takeDamage;

        void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerHealth = player.GetComponent<Health>();
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenateHealth;
        }

        void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenateHealth;
        }

        bool isDead = false;
        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage, GameObject instigator)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0); //0 ın altına düşerse 0da kalsın.
            print("player's health" + playerHealth.healthPoints);
            // print(healthPoints);
            if (healthPoints == 0)
            {
                Die();

                Experience experience = instigator.GetComponent<Experience>();
                if (experience == null) return;
                experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExpereinceReward));

            }

            else
            {
                takeDamage.Invoke(); //for event
            }
        }

        public void RegenateHealth()
        {
            float regenateHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenateHealtPercentage / 100);
            healthPoints = Mathf.Max(healthPoints, regenateHealthPoints);
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public float GetPercentage()
        {
            return 100 * (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public float GetHealthPoints()
        {
            return healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
    }



}

