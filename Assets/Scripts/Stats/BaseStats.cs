using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using System;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 30)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;
        int currentLevel = 0;
        public event Action onLevelUp;

        Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();
        }

        void Start()
        {
            currentLevel = CalculateLevel();
        }

        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExpereinceGained += UpdateLevel; //+= -= onenable disable yap opimizasyon
            }
        }
        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExpereinceGained -= UpdateLevel; //+= -= onenable disable yap opimizasyon
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                print("levelled up!!!!");
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            GameObject peffetct = Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat) //base stats scripti biliyor zaten level ile karakteri 
        // bu script ana script progression scriptable script...
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }



        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifiers(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXp = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExpereinceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExpereinceToLevelUp, characterClass, level);
                if (XPToLevelUp >= currentXp)
                {
                    return level;
                }
            }
            return penultimateLevel;
        }

    }
}
