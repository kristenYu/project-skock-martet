using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CompanyManager companyManager; 
    public GameObject[] companyUIObject;
    public int companyIndex;

    public TextMeshProUGUI playerMoney;
    public TMP_Dropdown buyDropDown; 
    public TMP_Dropdown sellDropDown; 
    public TMP_InputField  buyAmountInput; 
    public TMP_InputField  sellAmountInput;
    public TextMeshProUGUI goodSeedsIncAmount; 
    public TextMeshProUGUI sonodaFarmsAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        companyIndex = 0;
        foreach(Company company in companyManager.companies)
        {
            companyUIObject[companyIndex].GetComponent<TextMeshProUGUI>().text = company.name;
            foreach (Transform child in companyUIObject[companyIndex].transform){
                if(child.name == "Company Icon")
                {
                    
                }
                else if (child.name == "stock price")
                {
                    child.GetComponent<TextMeshProUGUI>().text = company.buyingCost.ToString();
                }
            }
            companyIndex ++;
        }

        goodSeedsIncAmount.text = 0.ToString();
        sonodaFarmsAmount.text = 0.ToString();
    }

    public void updateStockPrice(Company company)
    {
        for(int i = 0; i < companyManager.companies.Length; i++)
        {
            if(companyManager.companies[i].name == company.name)
            {
                companyIndex = i;
                break;
            }
        }

        foreach (Transform child in companyUIObject[companyIndex].transform){
            if (child.name == "stock price")
            {
                child.GetComponent<TextMeshProUGUI>().text = company.buyingCost.ToString();
            }
        }
    }

    public void setCurrentPlayerMoney(float value)
    {
        playerMoney.text = value.ToString();
    }

    public void setCurrentAmountOfStock(string companyName, int value)
    {
        if(companyName == "Good Seeds Inc")
        {
            goodSeedsIncAmount.text = value.ToString();
        }
        else if(companyName == "Sonoda Farms")
        {
            sonodaFarmsAmount.text = value.ToString();
        }
    }
}
