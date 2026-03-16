using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public int healAmount = 1; // จำนวนเลือดที่จะเพิ่ม
    public static event Action<int> OnHealthCollect; // สร้าง Event สำหรับส่งค่าการเพิ่มเลือด

    public void Collect()
    {
        OnHealthCollect?.Invoke(healAmount); // ส่งสัญญาณพร้อมค่า healAmount
        Destroy(gameObject); // ทำลายไอเทมทิ้ง
    }
}