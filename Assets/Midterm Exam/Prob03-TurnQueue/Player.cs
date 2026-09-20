using System.Collections.Generic;
using UnityEngine;

namespace MidtermExam.Prob03
{
    /// <summary>
    /// คลาส Player สำหรับจำลองตัวละครในเกมแนว Turn-based RPG
    /// มีความสามารถในการโจมตี (Attack) และใช้สกิลพิเศษสลับลำดับคิวการเล่น (SwapQueue)
    /// </summary>
    public class Player
    {
        public string Name;
        public int Health;

        public Player(string name, int health = 100)
        {
            Name = name;
            Health = health;
        }

        /// <summary>
        /// เมธอดสำหรับการโจมตีเป้าหมาย (Attack) เพื่อจำลองระบบการเล่นในเกม Turn-based
        /// ทำการลดค่า Health ของเป้าหมายลง 10 หน่วย
        /// </summary>
        /// <param name="target">ผู้เล่นเป้าหมายที่ถูกโจมตี</param>
        public void Attack(Player target)
        {
            if (target != null)
            {
                target.TakeDamage(10);
            }
        }

        /// <summary>
        /// ลดค่า Health ของผู้เล่นตาม damage ที่ได้รับ (ไม่ลดต่ำกว่า 0)
        /// </summary>
        public void TakeDamage(int damage)
        {
            Health = System.Math.Max(0, Health - damage);
        }

        /// <summary>
        /// ความสามารถพิเศษในการสลับลำดับคิว (Swap Queue) ของผู้เล่นใน Linked List ที่ใช้จัดการ Turn-based
        /// โดยค้นหา targetPlayer และนำออกจากตำแหน่งเดิม
        /// จากนั้นนำไปแทรกต่อท้าย afterPlayer (AddAfter)
        /// </summary>
        /// <param name="turnQueue">LinkedList ของ Player ที่ใช้เป็นคิวเทิร์น</param>
        /// <param name="targetPlayer">ผู้เล่นที่ต้องการย้ายตำแหน่งในคิว</param>
        /// <param name="afterPlayer">ผู้เล่นตำแหน่งอ้างอิง ที่จะนำ targetPlayer ไปวางต่อท้าย</param>
        /// <returns>คืนค่า true หากทำการย้ายสำเร็จ หรือ false หากไม่สามารถย้ายได้ (เช่น null, ไม่อยู่ในคิว, ผู้เล่นคนเดียวกัน)</returns>
        public bool SwapQueue(LinkedList<Player> turnQueue, Player targetPlayer, Player afterPlayer)
        {
            // TODO: ให้นักศึกษา Implement การจัดการสลับลำดับของ Node ใน LinkedList<Player>
            return false;
        }


        public override string ToString()
        {
            return $"{Name} (HP: {Health})";
        }
    }
}
