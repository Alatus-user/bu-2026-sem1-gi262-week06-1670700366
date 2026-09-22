using UnityEngine;

namespace MidtermExam.Prob01
{
    public class Weapon : Equipment
    {
        public int extraDamage;
        private float criticalChance;

        public override void Equip(Hero hero)
        {
            base.Equip(hero);
        }

        public void Polish()
        {

        }

        // TODO: Implement inheritance, fields and methods according to Class Diagram
    }
}
