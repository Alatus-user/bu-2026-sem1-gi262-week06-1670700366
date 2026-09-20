using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using MidtermExam.Prob02;

namespace MidtermExam.Tests
{
    /// <summary>
    /// ========================================================================================
    /// ชุดแบบทดสอบ Problem 02: LinkedList Sorting Algorithm (40 คะแนน)
    /// ========================================================================================
    /// 
    /// วัตถุประสงค์:
    /// ตรวจสอบการทำงานของ LinkedListSorter ในการจัดเรียงลำดับข้อมูลจำนวนเต็ม (int) ใน Doubly Linked List
    /// - SortAscending (20 คะแนน): จัดเรียงจากน้อยไปมาก
    /// - SortDescending (20 คะแนน): จัดเรียงจากมากไปน้อย
    /// 
    /// โครงสร้างชุดการทดสอบ (24 Test Cases):
    /// 1. [TC01 - TC06] Edge Cases (กรณีพิเศษ): null, รายการว่าง, โหนดเดียว, 2 โหนด, ตัวเลขซ้ำกันทั้งหมด
    /// 2. [TC07 - TC13] SortAscending Scenarios (เรียงจากน้อยไปมาก): ข้อมูลทั่วไป, สลับหน้าหลัง, ตัวเลขซ้ำ, จำนวนลบ
    /// 3. [TC14 - TC20] SortDescending Scenarios (เรียงจากมากไปน้อย): ข้อมูลทั่วไป, สลับหน้าหลัง, ตัวเลขซ้ำ, จำนวนลบ
    /// 4. [TC21 - TC24] Extreme & Stress Cases: ขอบเขต Min/Max, ฟันปลา Zigzag, รายการใหญ่ 500 โหนด, สุ่ม 300 จำนวน
    /// 
    /// ทุก Test Case จะตรวจสอบทั้ง Forward Traversal (.Next) และ Backward Traversal (.Previous)
    /// เพื่อยืนยันว่าการเรียงลำดับรักษาความสมบูรณ์ของ Pointer สองทิศทางของ Doubly Linked List
    /// ========================================================================================
    /// </summary>
    [TestFixture]
    [Category("MidtermExam")]
    [Category("Prob02")]
    public class Prob02_Sorting_Testcase
    {
        private LinkedListSorter sorter;

        [SetUp]
        public void Setup()
        {
            sorter = new LinkedListSorter();
        }

        #region Helper Methods สำหรับสร้างและตรวจสอบ LinkedList

        /// <summary>
        /// สร้าง LinkedList<int> จากอาเรย์ของตัวเลข
        /// </summary>
        private LinkedList<int> CreateLinkedList(params int[] items)
        {
            if (items == null) return null;
            var list = new LinkedList<int>();
            for (int i = 0; i < items.Length; i++)
            {
                list.AddLast(items[i]);
            }
            return list;
        }

        /// <summary>
        /// แปลง LinkedList เป็นข้อความเพื่อแสดงในข้อความแจ้งเตือน Error
        /// </summary>
        private string FormatList(LinkedList<int> list)
        {
            if (list == null) return "null";
            if (list.Count == 0) return "[] (รายการว่าง / Empty)";
            if (list.Count > 15)
            {
                var firstFew = list.Take(5);
                var lastFew = list.Skip(list.Count - 5);
                return $"[{string.Join(", ", firstFew)}, ... ({list.Count} items) ..., {string.Join(", ", lastFew)}]";
            }
            return "[" + string.Join(", ", list) + "]";
        }

        /// <summary>
        /// แปลง Array เป็นข้อความเพื่อแสดงในข้อความแจ้งเตือน Error
        /// </summary>
        private string FormatArray(int[] arr)
        {
            if (arr == null) return "null";
            if (arr.Length == 0) return "[] (รายการว่าง / Empty)";
            if (arr.Length > 15)
            {
                var firstFew = arr.Take(5);
                var lastFew = arr.Skip(arr.Length - 5);
                return $"[{string.Join(", ", firstFew)}, ... ({arr.Length} items) ..., {string.Join(", ", lastFew)}]";
            }
            return "[" + string.Join(", ", arr) + "]";
        }

        /// <summary>
        /// ตรวจสอบความถูกต้องของ LinkedList เทียบกับลำดับตัวเลขที่คาดหวัง
        /// ตรวจสอบทั้ง Forward Traversal (.Next) และ Backward Traversal (.Previous)
        /// </summary>
        private void AssertLinkedListEquals(int[] expected, LinkedList<int> actual, string testCaseName, string operationName)
        {
            if (expected == null)
            {
                Assert.IsNull(actual, $"[{testCaseName}] {operationName}: คาดหวังผลลัพธ์ null แต่ได้รับ Object");
                return;
            }

            Assert.IsNotNull(actual, 
                $"[{testCaseName}] {operationName}: ผลลัพธ์ที่ได้เป็น null\n" +
                $"-> Expected: {FormatArray(expected)}\n" +
                $"-> Actual: null (โปรดตรวจสอบว่าเมธอด return list ไม่ใช่ return null)");

            Assert.AreEqual(expected.Length, actual.Count, 
                $"[{testCaseName}] {operationName}: จำนวนโหนด (Count) ไม่ถูกต้อง\n" +
                $"-> Expected Count: {expected.Length} {FormatArray(expected)}\n" +
                $"-> Actual Count:   {actual.Count} {FormatList(actual)}\n" +
                $"-> ข้อผิดพลาดนี้อาจเกิดจากโหนดหลุดหายระหว่างเรียงลำดับ หรือนับจำนวนคลาดเคลื่อน");

            // 1. ตรวจสอบการวนลูปไปข้างหน้า (Forward Traversal: First -> Next -> null)
            int index = 0;
            var current = actual.First;
            while (current != null)
            {
                Assert.AreEqual(expected[index], current.Value, 
                    $"[{testCaseName}] {operationName}: ข้อมูลในโหนดไม่ตรงกันที่ตำแหน่ง index [{index}] (Forward traversal)\n" +
                    $"-> ค่าที่คาดหวัง (Expected): {expected[index]}\n" +
                    $"-> ค่าที่ได้รับจริง (Actual): {current.Value}\n" +
                    $"-> รายการผลลัพธ์ทั้งหมดที่ได้: {FormatList(actual)}\n" +
                    $"-> รายการผลลัพธ์ที่ถูกต้อง:    {FormatArray(expected)}");
                index++;
                current = current.Next;
            }
            Assert.AreEqual(expected.Length, index, 
                $"[{testCaseName}] {operationName}: การวนลูปโหนดไปข้างหน้าสิ้นสุดก่อนครบจำนวน (Forward pointer chain broken)");

            // 2. ตรวจสอบการวนลูปย้อนกลับจากท้ายมาหน้า (Backward Traversal: Last -> Previous -> null)
            index = expected.Length - 1;
            var backward = actual.Last;
            while (backward != null)
            {
                Assert.AreEqual(expected[index], backward.Value, 
                    $"[{testCaseName}] {operationName}: ข้อมูลผิดพลาดขณะวนลูปย้อนกลับจากท้ายมาหน้า (Backward traversal) ที่ index [{index}]\n" +
                    $"-> ค่าที่คาดหวัง (Expected): {expected[index]}\n" +
                    $"-> ค่าที่ได้รับจริง (Actual): {backward.Value}\n" +
                    $"-> Previous pointer ของโหนดเสียหาย หรือไม่ได้เชื่อมโยงสองทิศทางอย่างสมบูรณ์");
                index--;
                backward = backward.Previous;
            }
            Assert.AreEqual(-1, index, 
                $"[{testCaseName}] {operationName}: การวนลูปโหนดจากท้ายมาหน้าสิ้นสุดก่อนครบจำนวน (Previous pointer chain broken)");
        }

        #endregion

        #region หมวดที่ 1: Edge Cases (TC01 - TC06: กรณีพิเศษและข้อมูลขอบเขต)

        /// <summary>
        /// [TC01] Edge Case: Null List
        /// 
        /// [คำสั่ง / Instruction]:
        /// กรณีที่ส่ง list เป็น null เข้ามา ทั้ง SortAscending และ SortDescending ต้องรองรับได้อย่างปลอดภัย
        /// ไม่เกิด NullReferenceException และคืนค่ากลับเป็น null
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// null
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// null
        /// </summary>
        [Test(Description = "TC01: กรณีพิเศษ Null List -> ต้องไม่ Throw Exception และ Return null")]
        public void TC01_Edge_NullList()
        {
            Assert.DoesNotThrow(() =>
            {
                var resultAsc = sorter.SortAscending(null);
                Assert.IsNull(resultAsc, "[TC01] SortAscending(null) ควรรองรับอย่างปลอดภัยและ return null");

                var resultDesc = sorter.SortDescending(null);
                Assert.IsNull(resultDesc, "[TC01] SortDescending(null) ควรรองรับอย่างปลอดภัยและ return null");
            }, "[TC01] เกิด Exception เมื่อเรียกฟังก์ชันด้วยค่า null");
        }

        /// <summary>
        /// [TC02] Edge Case: Empty List
        /// 
        /// [คำสั่ง / Instruction]:
        /// กรณีที่ LinkedList ไม่มีข้อมูล (Count == 0) ต้อง return LinkedList ว่างกลับมาได้ถูกต้อง
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [] (Empty List)
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [] (Empty List)
        /// </summary>
        [Test(Description = "TC02: กรณีพิเศษ Empty List [] -> คืนค่า []")]
        public void TC02_Edge_EmptyList()
        {
            int[] empty = new int[0];

            var listAsc = CreateLinkedList(empty);
            var sortedAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(empty, sortedAsc, "TC02_Edge_EmptyList", "SortAscending");

            var listDesc = CreateLinkedList(empty);
            var sortedDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(empty, sortedDesc, "TC02_Edge_EmptyList", "SortDescending");
        }

        /// <summary>
        /// [TC03] Edge Case: Single Element List
        /// 
        /// [คำสั่ง / Instruction]:
        /// กรณีที่ LinkedList มีสมาชิกเพียงตัวเดียว (Count == 1) ไม่ต้องทำการสลับใดๆ และคืนค่าเดิม
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [42]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [42]
        /// </summary>
        [Test(Description = "TC03: กรณีพิเศษ โหนดเดียว [42] -> คืนค่า [42]")]
        public void TC03_Edge_SingleElement()
        {
            int[] input = new int[] { 42 };

            var listAsc = CreateLinkedList(input);
            var sortedAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(input, sortedAsc, "TC03_Edge_SingleElement", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var sortedDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(input, sortedDesc, "TC03_Edge_SingleElement", "SortDescending");
        }

        /// <summary>
        /// [TC04] Edge Case: Two Elements Already In Order
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูล 2 ตัวที่เรียงตามลำดับอยู่แล้ว
        /// - SortAscending: [10, 20] -> [10, 20]
        /// - SortDescending: [20, 10] -> [20, 10]
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// Asc: [10, 20] | Desc: [20, 10]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// Asc: [10, 20] | Desc: [20, 10]
        /// </summary>
        [Test(Description = "TC04: กรณีพิเศษ สองโหนดที่เรียงอยู่แล้ว: Asc [10, 20] -> [10, 20] / Desc [20, 10] -> [20, 10]")]
        public void TC04_Edge_TwoElements_AlreadySorted()
        {
            var listAsc = CreateLinkedList(10, 20);
            var sortedAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(new int[] { 10, 20 }, sortedAsc, "TC04_Edge_TwoElements_AlreadySorted", "SortAscending");

            var listDesc = CreateLinkedList(20, 10);
            var sortedDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(new int[] { 20, 10 }, sortedDesc, "TC04_Edge_TwoElements_AlreadySorted", "SortDescending");
        }

        /// <summary>
        /// [TC05] Edge Case: Two Elements Inverted
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูล 2 ตัวที่สลับทิศทางอยู่ ต้องสลับค่าโหนดให้ถูกต้อง
        /// - SortAscending: [20, 10] -> [10, 20]
        /// - SortDescending: [10, 20] -> [20, 10]
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// Asc: [20, 10] | Desc: [10, 20]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// Asc: [10, 20] | Desc: [20, 10]
        /// </summary>
        [Test(Description = "TC05: กรณีพิเศษ สองโหนดสลับตำแหน่ง: Asc [20, 10] -> [10, 20] / Desc [10, 20] -> [20, 10]")]
        public void TC05_Edge_TwoElements_Inverted()
        {
            var listAsc = CreateLinkedList(20, 10);
            var sortedAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(new int[] { 10, 20 }, sortedAsc, "TC05_Edge_TwoElements_Inverted", "SortAscending");

            var listDesc = CreateLinkedList(10, 20);
            var sortedDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(new int[] { 20, 10 }, sortedDesc, "TC05_Edge_TwoElements_Inverted", "SortDescending");
        }

        /// <summary>
        /// [TC06] Edge Case: All Identical Elements
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูลทุกโหนดมีค่าเท่ากันทั้งหมด ต้องไม่เกิด Infinite Loop หรือหลุดการเชื่อมต่อ
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [7, 7, 7, 7, 7]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [7, 7, 7, 7, 7]
        /// </summary>
        [Test(Description = "TC06: กรณีพิเศษ ตัวเลขซ้ำกันทุกตัว [7, 7, 7, 7, 7] -> [7, 7, 7, 7, 7]")]
        public void TC06_Edge_AllIdenticalElements()
        {
            int[] input = new int[] { 7, 7, 7, 7, 7 };

            var listAsc = CreateLinkedList(input);
            var sortedAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(input, sortedAsc, "TC06_Edge_AllIdenticalElements", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var sortedDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(input, sortedDesc, "TC06_Edge_AllIdenticalElements", "SortDescending");
        }

        #endregion

        #region หมวดที่ 2: SortAscending Scenarios (TC07 - TC13: เรียงจากน้อยไปมาก)

        /// <summary>
        /// [TC07] SortAscending - รายการทั่วไป (General Unsorted List)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลทั่วไป 5 จำนวนจากน้อยไปมาก
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [5, 2, 8, 1, 9]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [1, 2, 5, 8, 9]
        /// </summary>
        [Test(Description = "TC07: SortAscending รายการทั่วไป [5, 2, 8, 1, 9] -> [1, 2, 5, 8, 9]")]
        public void TC07_SortAsc_BasicUnsorted()
        {
            int[] input = new int[] { 5, 2, 8, 1, 9 };
            int[] expected = new int[] { 1, 2, 5, 8, 9 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC07_SortAsc_BasicUnsorted", "SortAscending");
        }

        /// <summary>
        /// [TC08] SortAscending - ข้อมูลขนาดกลางแบบไม่เรียงลำดับ (Medium Unsorted List)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูล 7 จำนวนจากน้อยไปมาก
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [64, 34, 25, 12, 22, 11, 90]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [11, 12, 22, 25, 34, 64, 90]
        /// </summary>
        [Test(Description = "TC08: SortAscending ขนาดกลาง [64, 34, 25, 12, 22, 11, 90] -> [11, 12, 22, 25, 34, 64, 90]")]
        public void TC08_SortAsc_MediumUnsorted()
        {
            int[] input = new int[] { 64, 34, 25, 12, 22, 11, 90 };
            int[] expected = new int[] { 11, 12, 22, 25, 34, 64, 90 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC08_SortAsc_MediumUnsorted", "SortAscending");
        }

        /// <summary>
        /// [TC09] SortAscending - ข้อมูลเรียงจากน้อยไปมากอยู่แล้ว (Already Sorted)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่เรียงจากน้อยไปมากอยู่แล้ว ต้องคงลำดับเดิมไว้
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [1, 2, 3, 4, 5]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [1, 2, 3, 4, 5]
        /// </summary>
        [Test(Description = "TC09: SortAscending เรียงอยู่แล้ว [1, 2, 3, 4, 5] -> [1, 2, 3, 4, 5]")]
        public void TC09_SortAsc_AlreadySorted()
        {
            int[] input = new int[] { 1, 2, 3, 4, 5 };
            int[] expected = new int[] { 1, 2, 3, 4, 5 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC09_SortAsc_AlreadySorted", "SortAscending");
        }

        /// <summary>
        /// [TC10] SortAscending - ข้อมูลเรียงกลับหลัง (Reverse Sorted / Worst Case)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่เรียงจากมากไปน้อย ให้กลับมาเป็นน้อยไปมาก
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [5, 4, 3, 2, 1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [1, 2, 3, 4, 5]
        /// </summary>
        [Test(Description = "TC10: SortAscending เรียงกลับหลังจากมากไปน้อย [5, 4, 3, 2, 1] -> [1, 2, 3, 4, 5]")]
        public void TC10_SortAsc_ReverseSorted()
        {
            int[] input = new int[] { 5, 4, 3, 2, 1 };
            int[] expected = new int[] { 1, 2, 3, 4, 5 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC10_SortAsc_ReverseSorted", "SortAscending");
        }

        /// <summary>
        /// [TC11] SortAscending - ข้อมูลที่มีค่าซ้ำกันหลายตัว (With Duplicates)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่มีตัวเลขซ้ำกัน เช่น มีเลข 2 สามตัว และเลข 4, 7 สองตัว
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [4, 2, 7, 2, 4, 1, 7, 2]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [1, 2, 2, 2, 4, 4, 7, 7]
        /// </summary>
        [Test(Description = "TC11: SortAscending มีตัวเลขซ้ำ [4, 2, 7, 2, 4, 1, 7, 2] -> [1, 2, 2, 2, 4, 4, 7, 7]")]
        public void TC11_SortAsc_WithDuplicates()
        {
            int[] input = new int[] { 4, 2, 7, 2, 4, 1, 7, 2 };
            int[] expected = new int[] { 1, 2, 2, 2, 4, 4, 7, 7 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC11_SortAsc_WithDuplicates", "SortAscending");
        }

        /// <summary>
        /// [TC12] SortAscending - ข้อมูลมีทั้งจำนวนเต็มลบและศูนย์ (With Negatives and Zero)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่มีจำนวนติดลบและศูนย์ ค่าติดลบมากสุดต้องอยู่หน้าสุด
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [3, -1, 0, -5, 2, -1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [-5, -1, -1, 0, 2, 3]
        /// </summary>
        [Test(Description = "TC12: SortAscending จำนวนลบและศูนย์ [3, -1, 0, -5, 2, -1] -> [-5, -1, -1, 0, 2, 3]")]
        public void TC12_SortAsc_WithNegativesAndZero()
        {
            int[] input = new int[] { 3, -1, 0, -5, 2, -1 };
            int[] expected = new int[] { -5, -1, -1, 0, 2, 3 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC12_SortAsc_WithNegativesAndZero", "SortAscending");
        }

        /// <summary>
        /// [TC13] SortAscending - ข้อมูลเป็นจำนวนเต็มลบทั้งหมด (All Negative Numbers)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่ทุกตัวเป็นจำนวนติดลบ เรียงจากค่าน้อยสุด (-50) ไปหาค่ามากสุด (-1)
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [-10, -50, -3, -20, -1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [-50, -20, -10, -3, -1]
        /// </summary>
        [Test(Description = "TC13: SortAscending จำนวนลบทั้งหมด [-10, -50, -3, -20, -1] -> [-50, -20, -10, -3, -1]")]
        public void TC13_SortAsc_AllNegative()
        {
            int[] input = new int[] { -10, -50, -3, -20, -1 };
            int[] expected = new int[] { -50, -20, -10, -3, -1 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortAscending(list);
            AssertLinkedListEquals(expected, actual, "TC13_SortAsc_AllNegative", "SortAscending");
        }

        #endregion

        #region หมวดที่ 3: SortDescending Scenarios (TC14 - TC20: เรียงจากมากไปน้อย)

        /// <summary>
        /// [TC14] SortDescending - รายการทั่วไป (General Unsorted List)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลทั่วไป 5 จำนวนจากมากไปน้อย
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [5, 2, 8, 1, 9]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [9, 8, 5, 2, 1]
        /// </summary>
        [Test(Description = "TC14: SortDescending รายการทั่วไป [5, 2, 8, 1, 9] -> [9, 8, 5, 2, 1]")]
        public void TC14_SortDesc_BasicUnsorted()
        {
            int[] input = new int[] { 5, 2, 8, 1, 9 };
            int[] expected = new int[] { 9, 8, 5, 2, 1 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC14_SortDesc_BasicUnsorted", "SortDescending");
        }

        /// <summary>
        /// [TC15] SortDescending - ข้อมูลขนาดกลางแบบไม่เรียงลำดับ (Medium Unsorted List)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูล 7 จำนวนจากมากไปน้อย
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [64, 34, 25, 12, 22, 11, 90]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [90, 64, 34, 25, 22, 12, 11]
        /// </summary>
        [Test(Description = "TC15: SortDescending ขนาดกลาง [64, 34, 25, 12, 22, 11, 90] -> [90, 64, 34, 25, 22, 12, 11]")]
        public void TC15_SortDesc_MediumUnsorted()
        {
            int[] input = new int[] { 64, 34, 25, 12, 22, 11, 90 };
            int[] expected = new int[] { 90, 64, 34, 25, 22, 12, 11 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC15_SortDesc_MediumUnsorted", "SortDescending");
        }

        /// <summary>
        /// [TC16] SortDescending - ข้อมูลเรียงจากมากไปน้อยอยู่แล้ว (Already Descending)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลที่เรียงจากมากไปน้อยอยู่แล้ว ต้องคงลำดับเดิมไว้
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [9, 7, 5, 3, 1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [9, 7, 5, 3, 1]
        /// </summary>
        [Test(Description = "TC16: SortDescending เรียงจากมากไปน้อยอยู่แล้ว [9, 7, 5, 3, 1] -> [9, 7, 5, 3, 1]")]
        public void TC16_SortDesc_AlreadyDescending()
        {
            int[] input = new int[] { 9, 7, 5, 3, 1 };
            int[] expected = new int[] { 9, 7, 5, 3, 1 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC16_SortDesc_AlreadyDescending", "SortDescending");
        }

        /// <summary>
        /// [TC17] SortDescending - ข้อมูลเรียงจากน้อยไปมาก (Ascending Input / Reverse Needed)
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูลเรียงจากน้อยไปมาก ต้องจัดเรียงใหม่ให้กลายเป็นมากไปน้อย
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [1, 3, 5, 7, 9]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [9, 7, 5, 3, 1]
        /// </summary>
        [Test(Description = "TC17: SortDescending ข้อมูลเรียงจากน้อยไปมาก [1, 3, 5, 7, 9] -> [9, 7, 5, 3, 1]")]
        public void TC17_SortDesc_AscendingInput()
        {
            int[] input = new int[] { 1, 3, 5, 7, 9 };
            int[] expected = new int[] { 9, 7, 5, 3, 1 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC17_SortDesc_AscendingInput", "SortDescending");
        }

        /// <summary>
        /// [TC18] SortDescending - ข้อมูลที่มีค่าซ้ำกันหลายตัว (With Duplicates)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลจากมากไปน้อย โดยมีตัวเลขซ้ำกัน
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [4, 2, 7, 2, 4, 1, 7, 2]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [7, 7, 4, 4, 2, 2, 2, 1]
        /// </summary>
        [Test(Description = "TC18: SortDescending มีตัวเลขซ้ำ [4, 2, 7, 2, 4, 1, 7, 2] -> [7, 7, 4, 4, 2, 2, 2, 1]")]
        public void TC18_SortDesc_WithDuplicates()
        {
            int[] input = new int[] { 4, 2, 7, 2, 4, 1, 7, 2 };
            int[] expected = new int[] { 7, 7, 4, 4, 2, 2, 2, 1 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC18_SortDesc_WithDuplicates", "SortDescending");
        }

        /// <summary>
        /// [TC19] SortDescending - ข้อมูลมีทั้งจำนวนเต็มลบและศูนย์ (With Negatives and Zero)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลจากมากไปน้อย โดยมีจำนวนติดลบและศูนย์ ค่าบวกมากสุดต้องอยู่หน้าสุด
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [3, -1, 0, -5, 2, -1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [3, 2, 0, -1, -1, -5]
        /// </summary>
        [Test(Description = "TC19: SortDescending จำนวนลบและศูนย์ [3, -1, 0, -5, 2, -1] -> [3, 2, 0, -1, -1, -5]")]
        public void TC19_SortDesc_WithNegativesAndZero()
        {
            int[] input = new int[] { 3, -1, 0, -5, 2, -1 };
            int[] expected = new int[] { 3, 2, 0, -1, -1, -5 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC19_SortDesc_WithNegativesAndZero", "SortDescending");
        }

        /// <summary>
        /// [TC20] SortDescending - ข้อมูลเป็นจำนวนเต็มลบทั้งหมด (All Negative Numbers)
        /// 
        /// [คำสั่ง / Instruction]:
        /// จัดเรียงข้อมูลจากมากไปน้อย โดยทุกตัวเป็นค่าติดลบ ค่าติดลบน้อยสุด (-1) ต้องอยู่หน้าสุด
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [-10, -50, -3, -20, -1]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// [-1, -3, -10, -20, -50]
        /// </summary>
        [Test(Description = "TC20: SortDescending จำนวนลบทั้งหมด [-10, -50, -3, -20, -1] -> [-1, -3, -10, -20, -50]")]
        public void TC20_SortDesc_AllNegative()
        {
            int[] input = new int[] { -10, -50, -3, -20, -1 };
            int[] expected = new int[] { -1, -3, -10, -20, -50 };

            var list = CreateLinkedList(input);
            var actual = sorter.SortDescending(list);
            AssertLinkedListEquals(expected, actual, "TC20_SortDesc_AllNegative", "SortDescending");
        }

        #endregion

        #region หมวดที่ 4: Extreme & Stress Cases (TC21 - TC24: กรณีพิเศษและข้อมูลขนาดใหญ่)

        /// <summary>
        /// [TC21] Extreme Case: Boundary Values (int.MinValue & int.MaxValue)
        /// 
        /// [คำสั่ง / Instruction]:
        /// ตรวจสอบการเปรียบเทียบค่าขอบเขตสูงสุดและต่ำสุดของชนิดข้อมูล integer (ป้องกัน Integer Overflow Bug)
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [int.MaxValue, 0, int.MinValue, -1, 1, 100, -100]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// Asc:  [int.MinValue, -100, -1, 0, 1, 100, int.MaxValue]
        /// Desc: [int.MaxValue, 100, 1, 0, -1, -100, int.MinValue]
        /// </summary>
        [Test(Description = "TC21: ค่าขอบเขต int.MinValue และ int.MaxValue -> ตรวจสอบ Integer Overflow")]
        public void TC21_Extreme_IntegerMinMaxBounds()
        {
            int[] input = new int[] { int.MaxValue, 0, int.MinValue, -1, 1, 100, -100 };
            int[] expectedAsc = new int[] { int.MinValue, -100, -1, 0, 1, 100, int.MaxValue };
            int[] expectedDesc = new int[] { int.MaxValue, 100, 1, 0, -1, -100, int.MinValue };

            var listAsc = CreateLinkedList(input);
            var actualAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(expectedAsc, actualAsc, "TC21_Extreme_IntegerMinMaxBounds", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var actualDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(expectedDesc, actualDesc, "TC21_Extreme_IntegerMinMaxBounds", "SortDescending");
        }

        /// <summary>
        /// [TC22] Extreme Case: Zigzag / Alternating High & Low Pattern
        /// 
        /// [คำสั่ง / Instruction]:
        /// รูปแบบข้อมูลสลับฟันปลา สูง-ต่ำ สลับกันทุกโหนด
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [1, 1000, 2, 999, 3, 998, 4, 997, 5, 996]
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// Asc:  [1, 2, 3, 4, 5, 996, 997, 998, 999, 1000]
        /// Desc: [1000, 999, 998, 997, 996, 5, 4, 3, 2, 1]
        /// </summary>
        [Test(Description = "TC22: รูปแบบฟันปลา Zigzag สูง-ต่ำสลับกัน [1, 1000, 2, 999, ...]")]
        public void TC22_Extreme_ZigzagAlternatingPattern()
        {
            int[] input = new int[] { 1, 1000, 2, 999, 3, 998, 4, 997, 5, 996 };
            int[] expectedAsc = new int[] { 1, 2, 3, 4, 5, 996, 997, 998, 999, 1000 };
            int[] expectedDesc = new int[] { 1000, 999, 998, 997, 996, 5, 4, 3, 2, 1 };

            var listAsc = CreateLinkedList(input);
            var actualAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(expectedAsc, actualAsc, "TC22_Extreme_ZigzagAlternatingPattern", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var actualDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(expectedDesc, actualDesc, "TC22_Extreme_ZigzagAlternatingPattern", "SortDescending");
        }

        /// <summary>
        /// [TC23] Extreme Case: Large List (500 Elements Stress Test)
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูลขนาดใหญ่ 500 โหนด เรียงกลับหลัง (500 ถึง 1) เพื่อทดสอบประสิทธิภาพและความเสถียรของอัลกอริทึม
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// [500, 499, 498, ..., 3, 2, 1] (ขนาด 500 โหนด)
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// Asc:  [1, 2, 3, ..., 498, 499, 500]
        /// Desc: [500, 499, 498, ..., 3, 2, 1]
        /// </summary>
        [Test(Description = "TC23: ข้อมูลขนาดใหญ่ 500 โหนด (Stress Test 500 Reverse Elements)")]
        public void TC23_Extreme_LargeList_500Elements()
        {
            const int size = 500;
            int[] input = new int[size];
            int[] expectedAsc = new int[size];
            int[] expectedDesc = new int[size];

            for (int i = 0; i < size; i++)
            {
                input[i] = size - i;       // 500 ลงไปหา 1
                expectedAsc[i] = i + 1;     // 1 ขึ้นไปหา 500
                expectedDesc[i] = size - i; // 500 ลงไปหา 1
            }

            var listAsc = CreateLinkedList(input);
            var actualAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(expectedAsc, actualAsc, "TC23_Extreme_LargeList_500Elements", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var actualDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(expectedDesc, actualDesc, "TC23_Extreme_LargeList_500Elements", "SortDescending");
        }

        /// <summary>
        /// [TC24] Extreme Case: Random Generated 300 Elements
        /// 
        /// [คำสั่ง / Instruction]:
        /// ข้อมูลสุ่มจำนวน 300 จำนวน (ช่วงค่า -5000 ถึง 5000) มีตัวเลขซ้ำและสลับกระจัดกระจาย
        /// 
        /// [ข้อมูลนำเข้า / Input]:
        /// ตัวเลขสุ่ม 300 จำนวน ด้วย Seed ที่กำหนด
        /// 
        /// [ผลลัพธ์ที่คาดหวัง / Expected Output]:
        /// ผลลัพธ์จัดเรียงลำดับถูกต้องตาม OrderBy และ OrderByDescending
        /// </summary>
        [Test(Description = "TC24: ข้อมูลสุ่ม 300 จำนวน (Random 300 Elements ช่วง -5000 ถึง 5000)")]
        public void TC24_Extreme_RandomElements_300Elements()
        {
            var rand = new System.Random(2026);
            const int count = 300;
            int[] input = new int[count];
            for (int i = 0; i < count; i++)
            {
                input[i] = rand.Next(-5000, 5000);
            }

            int[] expectedAsc = input.OrderBy(x => x).ToArray();
            int[] expectedDesc = input.OrderByDescending(x => x).ToArray();

            var listAsc = CreateLinkedList(input);
            var actualAsc = sorter.SortAscending(listAsc);
            AssertLinkedListEquals(expectedAsc, actualAsc, "TC24_Extreme_RandomElements_300Elements", "SortAscending");

            var listDesc = CreateLinkedList(input);
            var actualDesc = sorter.SortDescending(listDesc);
            AssertLinkedListEquals(expectedDesc, actualDesc, "TC24_Extreme_RandomElements_300Elements", "SortDescending");
        }

        #endregion
    }
}
