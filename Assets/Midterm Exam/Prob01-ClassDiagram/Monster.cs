using UnityEngine;

namespace MidtermExam.Prob01
{
    public class Monster : Character
    {
        public int baseDamage;
        protected int aggroRange;

        public override void Attack(GameEntity target)
        {
            base.Attack(target);
        }

        protected virtual void Roar()
        {

        }
        // TODO: Implement inheritance, fields and methods according to Class Diagram
    }
}
