using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using MidtermExam.Prob03;

namespace MidtermExam.Tests
{
    /// <summary>
    /// ========================================================================================
    /// ชุดแบบทดสอบ Problem 03: Turn-Based Queue Manipulation with LinkedList (20 คะแนน)
    /// ========================================================================================
    /// 
    /// วัตถุประสงค์:
    /// ตรวจสอบการจัดการลำดับคิวในระบบ Turn-based ด้วย Doubly Linked List (LinkedList of Player)
    /// ผ่านเมธอด SwapQueue ของคลาส Player
    /// 
    /// โครงสร้างชุดการทดสอบ (12 Test Cases):
    /// 1. [TC01 - TC03] Validation & Edge Cases: ตรวจสอบ null, ข้อมูลนอกคิว, สลับตัวเอง, คิวขนาด 1 โหนด
    /// 2. [TC04 - TC07] Queue Swapping Scenarios: ตัวอย่างตามโจทย์, สลับหน้า-หลัง, สลับคู่ติดกัน, คิว 2 คน
    /// 3. [TC08 - TC10] Boundary & Pointer Integrity: ย้ายหัวคิว (Head), ท้ายคิว (Tail), และตรวจ Pointer สองทิศทาง
    /// 4. [TC11 - TC12] Gameplay Simulation: ตรวจสอบ Attack, NextTurn cycle, และการนำสกิล SwapQueue ไปใช้จริง
    /// ========================================================================================
    /// </summary>
    [TestFixture]
    [Category("MidtermExam")]
    [Category("Prob03")]
    public class Prob03_TurnQueue_Testcase
    {
        private Player caster;

        [SetUp]
        public void Setup()
        {
            caster = new Player("Caster", 100);
        }

        #region Helper Methods

        /// <summary>
        /// สร้าง LinkedList of Player จากรายชื่อ
        /// </summary>
        private LinkedList<Player> CreateQueue(params string[] names)
        {
            if (names == null) return null;
            var queue = new LinkedList<Player>();
            foreach (var name in names)
            {
                queue.AddLast(new Player(name));
            }
            return queue;
        }

        /// <summary>
        /// ค้นหา Player Object จากชื่อใน LinkedList
        /// </summary>
        private Player FindPlayer(LinkedList<Player> queue, string name)
        {
            if (queue == null) return null;
            foreach (var p in queue)
            {
                if (p.Name == name) return p;
            }
            return null;
        }

        /// <summary>
        /// แปลงคิวเป็นข้อความสำหรับแสดงใน Assertion Failure Message
        /// </summary>
        private string FormatQueue(LinkedList<Player> queue)
        {
            if (queue == null) return "null";
            if (queue.Count == 0) return "[] (คิวว่าง)";
            return "[" + string.Join(" -> ", queue.Select(p => p.Name)) + "]";
        }

        /// <summary>
        /// ตรวจสอบความถูกต้องของลำดับผู้เล่นในคิว และตรวจสอบความสมบูรณ์ของ Pointer (.Next และ .Previous)
        /// </summary>
        private void AssertQueueEquals(string[] expectedNames, LinkedList<Player> actualQueue, string testCaseName)
        {
            Assert.IsNotNull(actualQueue, $"[{testCaseName}] คิวต้องไม่เป็น null");
            Assert.AreEqual(expectedNames.Length, actualQueue.Count,
                $"[{testCaseName}] จำนวนผู้เล่นในคิวไม่ตรงตามที่คาดหวัง คาดหวัง: {expectedNames.Length} แต่ได้: {actualQueue.Count}\nคิวปัจจุบัน: {FormatQueue(actualQueue)}");

            // ตรวจสอบ Forward Traversal (.Next)
            var actualNames = new List<string>();
            var current = actualQueue.First;
            LinkedListNode<Player> prevNode = null;

            while (current != null)
            {
                actualNames.Add(current.Value.Name);

                // ตรวจสอบว่า Previous Pointer เชื่อมโยงกลับไปยังโหนดก่อนหน้าอย่างถูกต้อง
                Assert.AreSame(prevNode, current.Previous,
                    $"[{testCaseName}] Pointer .Previous ของโหนด '{current.Value.Name}' ชี้ผิดตำแหน่ง");

                prevNode = current;
                current = current.Next;
            }

            CollectionAssert.AreEqual(expectedNames, actualNames,
                $"[{testCaseName}] ลำดับผู้เล่นจากการเดินหน้า (.Next) ไม่ถูกต้อง!\nคาดหวัง: [{string.Join(" -> ", expectedNames)}]\nได้จริง : [{string.Join(" -> ", actualNames)}]");

            // ตรวจสอบ Backward Traversal (.Previous)
            var reverseActualNames = new List<string>();
            var reverseExpected = expectedNames.Reverse().ToArray();
            var revCurrent = actualQueue.Last;
            LinkedListNode<Player> nextNode = null;

            while (revCurrent != null)
            {
                reverseActualNames.Add(revCurrent.Value.Name);

                // ตรวจสอบว่า Next Pointer เชื่อมโยงไปยังโหนดถัดไปอย่างถูกต้อง
                Assert.AreSame(nextNode, revCurrent.Next,
                    $"[{testCaseName}] Pointer .Next ของโหนด '{revCurrent.Value.Name}' ชี้ผิดตำแหน่งเมื่อย้อนกลับ");

                nextNode = revCurrent;
                revCurrent = revCurrent.Previous;
            }

            CollectionAssert.AreEqual(reverseExpected, reverseActualNames,
                $"[{testCaseName}] ลำดับผู้เล่นจากการเดินถอยหลัง (.Previous) ไม่ตรงกับลำดับย้อนกลับ");

            // ตรวจสอบขอบเขต First และ Last
            if (expectedNames.Length > 0)
            {
                Assert.IsNull(actualQueue.First.Previous, $"[{testCaseName}] First.Previous ต้องเป็น null เสมอ");
                Assert.IsNull(actualQueue.Last.Next, $"[{testCaseName}] Last.Next ต้องเป็น null เสมอ");
                Assert.AreEqual(expectedNames[0], actualQueue.First.Value.Name, $"[{testCaseName}] First Player ต้องเป็น {expectedNames[0]}");
                Assert.AreEqual(expectedNames[expectedNames.Length - 1], actualQueue.Last.Value.Name, $"[{testCaseName}] Last Player ต้องเป็น {expectedNames[expectedNames.Length - 1]}");
            }
        }

        #endregion

        #region หมวดที่ 1: Validation & Edge Cases (TC01 - TC03)

        [Test]
        public void TC01_Validation_NullInputs_ReturnsFalse()
        {
            // ตรวจสอบกรณี Arguments เป็น null ทั้ง 3 กรณี:
            // 1. turnQueue เป็น null
            // 2. targetPlayer เป็น null
            // 3. afterPlayer เป็น null
            var p1 = new Player("Player 1");
            var p2 = new Player("Player 2");

            Assert.IsFalse(caster.SwapQueue(null, p1, p2), "[TC01] ส่ง turnQueue เป็น null ต้องคืนค่า false");

            var queue = CreateQueue("Player 1", "Player 2", "Player 3");
            Assert.IsFalse(caster.SwapQueue(queue, null, p2), "[TC01] ส่ง targetPlayer เป็น null ต้องคืนค่า false");
            Assert.IsFalse(caster.SwapQueue(queue, p1, null), "[TC01] ส่ง afterPlayer เป็น null ต้องคืนค่า false");
            AssertQueueEquals(new string[] { "Player 1", "Player 2", "Player 3" }, queue, "TC01");
        }

        [Test]
        public void TC02_Validation_PlayerNotInQueue_ReturnsFalse()
        {
            // ตรวจสอบกรณีผู้เล่นไม่อยู่ในคิว:
            // 1. targetPlayer เป็นผู้เล่นภายนอกที่ไม่อยู่ในคิว
            // 2. afterPlayer เป็นผู้เล่นภายนอกที่ไม่อยู่ในคิว
            var queue = CreateQueue("Player 1", "Player 2", "Player 3");
            var p1 = FindPlayer(queue, "Player 1");
            var outsider = new Player("Outsider");

            Assert.IsFalse(caster.SwapQueue(queue, outsider, p1), "[TC02] targetPlayer ไม่อยู่ในคิว ต้องคืนค่า false");
            Assert.IsFalse(caster.SwapQueue(queue, p1, outsider), "[TC02] afterPlayer ไม่อยู่ในคิว ต้องคืนค่า false");
            AssertQueueEquals(new string[] { "Player 1", "Player 2", "Player 3" }, queue, "TC02");
        }

        [Test]
        public void TC03_Validation_SelfTargetAndSmallQueue_ReturnsFalse()
        {
            // ตรวจสอบกรณีพยายามวางต่อท้ายตัวเอง และคิวมีผู้เล่นไม่เพียงพอ:
            // 1. targetPlayer == afterPlayer
            // 2. คิวขนาด 1 คน (Count < 2)
            var queue = CreateQueue("Player 1", "Player 2", "Player 3");
            var p2 = FindPlayer(queue, "Player 2");
            Assert.IsFalse(caster.SwapQueue(queue, p2, p2), "[TC03] target == after ต้องคืนค่า false");
            AssertQueueEquals(new string[] { "Player 1", "Player 2", "Player 3" }, queue, "TC03");

            var singleQueue = CreateQueue("SoloPlayer");
            var solo = FindPlayer(singleQueue, "SoloPlayer");
            Assert.IsFalse(caster.SwapQueue(singleQueue, solo, solo), "[TC03] คิว 1 คนต้องคืนค่า false");
        }

        #endregion

        #region หมวดที่ 2: Queue Swapping Scenarios (TC04 - TC07)

        [Test]
        public void TC04_Swap_PromptExample_MovePlayer4AfterPlayer1()
        {
            // ตัวอย่างตามโจทย์: [Player 1, Player 2, Player 3, Player 4, Player 5]
            // สั่งย้าย Player 4 ไปต่อท้าย Player 1 -> [Player 1, Player 4, Player 2, Player 3, Player 5]
            var queue = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4", "Player 5");
            var p4 = FindPlayer(queue, "Player 4");
            var p1 = FindPlayer(queue, "Player 1");

            bool result = caster.SwapQueue(queue, p4, p1);

            Assert.IsTrue(result, "[TC04] การสลับคิวต้องคืนค่า true");
            AssertQueueEquals(new string[] { "Player 1", "Player 4", "Player 2", "Player 3", "Player 5" }, queue, "TC04");
        }

        [Test]
        public void TC05_Swap_MoveForwardAndBackward()
        {
            // 1. ย้ายจากหลังมาหน้า: ย้าย Player 5 ต่อท้าย Player 2
            var queue1 = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4", "Player 5");
            Assert.IsTrue(caster.SwapQueue(queue1, FindPlayer(queue1, "Player 5"), FindPlayer(queue1, "Player 2")), "[TC05] ย้ายไปข้างหน้าสำเร็จ");
            AssertQueueEquals(new string[] { "Player 1", "Player 2", "Player 5", "Player 3", "Player 4" }, queue1, "TC05-Forward");

            // 2. ย้ายจากหน้าไปหลัง: ย้าย Player 2 ต่อท้าย Player 4
            var queue2 = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4", "Player 5");
            Assert.IsTrue(caster.SwapQueue(queue2, FindPlayer(queue2, "Player 2"), FindPlayer(queue2, "Player 4")), "[TC05] ย้ายไปข้างหลังสำเร็จ");
            AssertQueueEquals(new string[] { "Player 1", "Player 3", "Player 4", "Player 2", "Player 5" }, queue2, "TC05-Backward");
        }

        [Test]
        public void TC06_Swap_AdjacentAndAlreadyInPosition()
        {
            // 1. สลับผู้เล่นสองคนที่อยู่ติดกัน: ย้าย Player 2 ต่อท้าย Player 3
            var queue = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4");
            Assert.IsTrue(caster.SwapQueue(queue, FindPlayer(queue, "Player 2"), FindPlayer(queue, "Player 3")), "[TC06] สลับคู่ติดกันสำเร็จ");
            AssertQueueEquals(new string[] { "Player 1", "Player 3", "Player 2", "Player 4" }, queue, "TC06-Adjacent");

            // 2. กรณีผู้เล่นอยู่ต่อท้ายตำแหน่งนั้นอยู่แล้ว: ย้าย Player 2 ต่อท้าย Player 3 (ซึ่งต่อท้ายอยู่แล้ว)
            Assert.IsTrue(caster.SwapQueue(queue, FindPlayer(queue, "Player 2"), FindPlayer(queue, "Player 3")), "[TC06] อยู่ตำแหน่งเดิมแล้วต้องคืนค่า true");
            AssertQueueEquals(new string[] { "Player 1", "Player 3", "Player 2", "Player 4" }, queue, "TC06-AlreadyInPosition");
        }

        [Test]
        public void TC07_Swap_TwoPlayersList()
        {
            // คิวขนาดเล็กที่สุด 2 คน: [Player 1, Player 2] -> ย้าย Player 1 ต่อท้าย Player 2 -> [Player 2, Player 1]
            var queue = CreateQueue("Player 1", "Player 2");
            Assert.IsTrue(caster.SwapQueue(queue, FindPlayer(queue, "Player 1"), FindPlayer(queue, "Player 2")), "[TC07] คิว 2 คนสลับตำแหน่งสำเร็จ");
            AssertQueueEquals(new string[] { "Player 2", "Player 1" }, queue, "TC07");
        }

        #endregion

        #region หมวดที่ 3: Boundary & Pointer Integrity (TC08 - TC10)

        [Test]
        public void TC08_Boundary_MoveHeadAndTail()
        {
            // 1. ย้ายโหนดหัวคิว (Head/First) ไปไว้ตรงกลาง: ย้าย Player 1 ต่อท้าย Player 3
            var queue1 = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4");
            Assert.IsTrue(caster.SwapQueue(queue1, queue1.First.Value, FindPlayer(queue1, "Player 3")), "[TC08] ย้าย Head สำเร็จ");
            AssertQueueEquals(new string[] { "Player 2", "Player 3", "Player 1", "Player 4" }, queue1, "TC08-Head");
            Assert.AreEqual("Player 2", queue1.First.Value.Name, "[TC08] Head ใหม่ต้องเป็น Player 2");
            Assert.IsNull(queue1.First.Previous, "[TC08] First.Previous ต้องเป็น null");

            // 2. ย้ายโหนดท้ายคิว (Tail/Last) ไปไว้ตรงกลาง: ย้าย Player 4 ต่อท้าย Player 2
            var queue2 = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4");
            Assert.IsTrue(caster.SwapQueue(queue2, queue2.Last.Value, FindPlayer(queue2, "Player 2")), "[TC08] ย้าย Tail สำเร็จ");
            AssertQueueEquals(new string[] { "Player 1", "Player 2", "Player 4", "Player 3" }, queue2, "TC08-Tail");
            Assert.AreEqual("Player 3", queue2.Last.Value.Name, "[TC08] Tail ใหม่ต้องเป็น Player 3");
            Assert.IsNull(queue2.Last.Next, "[TC08] Last.Next ต้องเป็น null");
        }

        [Test]
        public void TC09_Boundary_MovePlayerToBecomeNewTail()
        {
            // 1. ย้ายผู้เล่นจากตรงกลางไปต่อท้าย Last เดิม -> ผู้เล่นที่ย้ายกลายเป็น New Tail
            var queue = CreateQueue("Player 1", "Player 2", "Player 3", "Player 4");
            Assert.IsTrue(caster.SwapQueue(queue, FindPlayer(queue, "Player 2"), queue.Last.Value), "[TC09] ย้ายไปเป็น New Tail สำเร็จ");
            AssertQueueEquals(new string[] { "Player 1", "Player 3", "Player 4", "Player 2" }, queue, "TC09-NewTail");
            Assert.AreEqual("Player 2", queue.Last.Value.Name, "[TC09] Tail ใหม่ต้องเป็น Player 2");
            Assert.IsNull(queue.Last.Next, "[TC09] Last.Next ต้องเป็น null");

            // 2. ย้ายจาก First ไปต่อท้าย Last โดยตรง (Head ย้ายไปเป็น Tail)
            var queue2 = CreateQueue("A", "B", "C", "D");
            Assert.IsTrue(caster.SwapQueue(queue2, queue2.First.Value, queue2.Last.Value), "[TC09] ย้าย Head ไป Tail สำเร็จ");
            AssertQueueEquals(new string[] { "B", "C", "D", "A" }, queue2, "TC09-HeadToTail");
            Assert.AreEqual("B", queue2.First.Value.Name, "[TC09] New Head ต้องเป็น B");
            Assert.AreEqual("A", queue2.Last.Value.Name, "[TC09] New Tail ต้องเป็น A");
        }

        [Test]
        public void TC10_Integrity_BidirectionalPointers()
        {
            // ตรวจสอบความสมบูรณ์ของ Pointer สองทิศทางทั้งคิวขนาด 6 โหนด
            var queue = CreateQueue("A", "B", "C", "D", "E", "F");
            var pE = FindPlayer(queue, "E");
            var pB = FindPlayer(queue, "B");

            caster.SwapQueue(queue, pE, pB);
            // ผลลัพธ์: [A, B, E, C, D, F]
            AssertQueueEquals(new string[] { "A", "B", "E", "C", "D", "F" }, queue, "TC10");

            var nodeE = queue.Find(pE);
            Assert.IsNotNull(nodeE);
            Assert.AreEqual("B", nodeE.Previous.Value.Name, "[TC10] nodeE.Previous ต้องเป็น B");
            Assert.AreEqual("C", nodeE.Next.Value.Name, "[TC10] nodeE.Next ต้องเป็น C");
            Assert.AreSame(nodeE, nodeE.Previous.Next, "[TC10] B.Next ต้องชี้กลับมาที่ E");
            Assert.AreSame(nodeE, nodeE.Next.Previous, "[TC10] C.Previous ต้องชี้กลับมาที่ E");
        }

        #endregion

        #region หมวดที่ 4: Gameplay Simulation (TC11 - TC12)

        [Test]
        public void TC11_Gameplay_AttackAndTurnCycle()
        {
            // 1. ตรวจสอบการโจมตี (Attack)
            var attacker = new Player("Hero", 100);
            var defender = new Player("Monster", 100);
            attacker.Attack(defender);
            Assert.AreEqual(90, defender.Health, "[TC11] Defender ต้องลด HP 10 หน่วย");

            // 2. ตรวจสอบการหมุนเวียนรอบเทิร์นใน TurnManager
            var queue = CreateQueue("P1", "P2", "P3");
            var turnManager = new TurnManager(queue);
            Assert.AreEqual("P1", turnManager.CurrentPlayer.Name, "[TC11] เทิร์นแรกต้องเป็น P1");

            var nextP = turnManager.NextTurn();
            Assert.AreEqual("P2", nextP.Name, "[TC11] NextTurn ครั้งที่ 1 ต้องเป็น P2");
            AssertQueueEquals(new string[] { "P2", "P3", "P1" }, turnManager.TurnQueue, "TC11-TurnCycle");
        }

        [Test]
        public void TC12_Gameplay_TurnQueue_CombatAbilitySimulation()
        {
            // จำลองการต่อสู้จริง:
            // [Hero, Boss, Mage, Warrior]
            // Hero ใช้สกิล SwapQueue ย้าย Boss ไปต่อท้าย Warrior -> [Hero, Mage, Warrior, Boss]
            // Hero โจมตี Boss -> Boss HP 90
            // จบเทิร์น Hero (NextTurn) -> Mage ได้เล่นเป็นคนถัดไปแทน Boss
            var queue = CreateQueue("Hero", "Boss", "Mage", "Warrior");
            var turnManager = new TurnManager(queue);

            var hero = turnManager.CurrentPlayer;
            var boss = FindPlayer(queue, "Boss");
            var warrior = FindPlayer(queue, "Warrior");

            bool swapped = hero.SwapQueue(turnManager.TurnQueue, boss, warrior);
            Assert.IsTrue(swapped, "[TC12] ใช้สกิลสำเร็จ");
            AssertQueueEquals(new string[] { "Hero", "Mage", "Warrior", "Boss" }, turnManager.TurnQueue, "TC12-Delayed");

            hero.Attack(boss);
            Assert.AreEqual(90, boss.Health, "[TC12] Boss ได้รับดาเมจ");

            var nextPlayer = turnManager.NextTurn();
            Assert.AreEqual("Mage", nextPlayer.Name, "[TC12] Mage ได้ออกแอ็กชันก่อน Boss");
            AssertQueueEquals(new string[] { "Mage", "Warrior", "Boss", "Hero" }, turnManager.TurnQueue, "TC12-NextTurn");

            // ทดสอบสลับคิวต่อเนื่องอีกครั้งในเกม
            var mage = turnManager.CurrentPlayer;
            mage.SwapQueue(turnManager.TurnQueue, warrior, hero);
            AssertQueueEquals(new string[] { "Mage", "Boss", "Hero", "Warrior" }, turnManager.TurnQueue, "TC12-Consecutive");
        }

        #endregion
    }
}
