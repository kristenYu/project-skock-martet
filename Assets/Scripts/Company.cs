using System.Collections.Generic;
using UnityEngine;

public class Company : MonoBehaviour
{

    [System.Serializable]
    public struct MyDictionaryItem
    {
        public Company key;
        public int value;
    }

    public string name; 
    public float buyingCost;
    public float sellingCost;
    public int buyingPower;
    public string productName; 
    public int productPrice; 
    public int productionRate; 
    public int producedProduct;
    public List<MyDictionaryItem> inspectorList;
    public Dictionary<Company, int> desiredProductAmount = new Dictionary<Company, int>();
    public Company[] buysFrom; 
    public Company[] sellsTo;

    
    // This shows up in the Inspector as a list


    // Use this in your code for performance

    void Awake()
    {
        foreach (var item in inspectorList)
        {
            if (!desiredProductAmount.ContainsKey(item.key))
                desiredProductAmount.Add(item.key, item.value);
        }
    }

}
