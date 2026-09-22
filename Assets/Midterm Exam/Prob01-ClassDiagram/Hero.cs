using UnityEngine;

namespace MidtermExam.Prob01
{
    public class Hero : Character
    {
        public int currentExp;
        private int gold;

        public override void Attack(GameEntity target)
        {
            base.Attack(target);
        }

        public void CollectGold(int amount)
        {

        }

        protected override void LevelUp()
        {
            base.LevelUp();
        }


        // TODO: Implement inheritance, fields and methods according to Class Diagram
    }
}
