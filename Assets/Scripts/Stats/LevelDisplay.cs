using UnityEngine;
using System;
using UnityEngine.UI;

namespace RPG.Stats
{
    class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}",baseStats.GetLevel());
        }
    }
}