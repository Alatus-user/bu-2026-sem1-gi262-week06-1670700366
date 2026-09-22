using UnityEngine;

namespace MidtermExam.Prob01
{
    public class Character : GameEntity
    {
        public string characterName;
        protected float moveSpeed;
        private int level;

        public virtual void Attack(GameEntity target)
        {

        }
        protected virtual void LevelUp()
        {

        }

        // TODO: Implement inheritance, fields and methods according to Class Diagram
    }
}
