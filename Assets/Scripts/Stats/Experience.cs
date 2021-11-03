using UnityEngine;
using System;
namespace RPG.Stats
{
    public class Experience : MonoBehaviour
    {
        [SerializeField] float experiencePoints = 0f;
        public event Action onExpereinceGained;
        
        public void GainExperience(float exp)
        {
            experiencePoints+=exp;
            onExpereinceGained();
        }

        public float GetPoints(){
            return experiencePoints;
        }
    }
}