using System.Collections.Generic;

namespace MidtermExam.Prob03
{
    /// <summary>
    /// คลาสจัดการคิวเทิร์น (TurnManager) สำหรับเกมแนว Turn-based
    /// จัดเก็บคิวของผู้เล่นโดยใช้ Doubly Linked List (LinkedList of Player)
    /// ลำดับการเล่นเริ่มจากโหนดแรก (First) ไปจนถึงโหนดสุดท้าย (Last)
    /// </summary>
    public class TurnManager
    {
        /// <summary>
        /// คิวของตัวละครที่รอเล่นเทิร์น โดย LinkedListNode แรกคือผู้เล่นที่กำลังได้เล่นเทิร์นปัจจุบัน
        /// </summary>
        public LinkedList<Player> TurnQueue { get; set; }

        public TurnManager()
        {
            TurnQueue = new LinkedList<Player>();
        }

        public TurnManager(LinkedList<Player> initialQueue)
        {
            TurnQueue = initialQueue ?? new LinkedList<Player>();
        }

        /// <summary>
        /// ผู้เล่นที่เป็นเจ้าของเทิร์นปัจจุบัน (โหนดแรกสุดของคิว: First.Value)
        /// </summary>
        public Player CurrentPlayer => TurnQueue?.First?.Value;

        /// <summary>
        /// ดำเนินการจบเทิร์นปัจจุบัน โดยนำผู้เล่นคนแรก (First) ย้ายไปต่อท้ายคิว (Last)
        /// แล้วคืนค่าผู้เล่นที่จะได้เล่นเป็นลำดับถัดไป (First คนใหม่)
        /// </summary>
        /// <returns>Player คนถัดไปที่ถึงรอบเล่น หรือ null หากไม่มีผู้เล่นในคิว</returns>
        public Player NextTurn()
        {
            if (TurnQueue == null || TurnQueue.Count == 0) return null;

            Player current = TurnQueue.First.Value;
            TurnQueue.RemoveFirst();
            TurnQueue.AddLast(current);

            return TurnQueue.First.Value;
        }

        /// <summary>
        /// สั่งให้ผู้เล่นปัจจุบัน (CurrentPlayer) ใช้ความสามารถ SwapQueue เพื่อเปลี่ยนลำดับคิวใน TurnQueue
        /// </summary>
        /// <param name="targetPlayer">ผู้เล่นที่ต้องการสลับตำแหน่ง</param>
        /// <param name="afterPlayer">ผู้เล่นที่ต้องการนำ targetPlayer ไปวางต่อท้าย</param>
        /// <returns>true หากดำเนินการสำเร็จ, false หากล้มเหลว</returns>
        public bool ExecuteSwapQueue(Player targetPlayer, Player afterPlayer)
        {
            if (CurrentPlayer == null || TurnQueue == null) return false;
            return CurrentPlayer.SwapQueue(TurnQueue, targetPlayer, afterPlayer);
        }
    }
}
