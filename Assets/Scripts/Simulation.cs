using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Timeline;

public class Simulation : MonoBehaviour
{
    public CompanyManager companyManager; 
    public int desiredPurchaseAmount;
    public int desiredPurchasePricing;
    public Event currentEvent; 
    public TextMeshProUGUI eventText; 
    public EventManager eventManager;
    public UIManager uiManager;
    public float playerMoney;
    public Company currentBuyingCompany;
    public float attemptedBuyingAmount;

    public Company currentSellingCompany; 
    public float attemtedSellingAmount;
    public Dictionary<string, int> playerStocks;

    public Transform currentParentofClickedButton;
    public TMP_InputField currentBuyStockAmount;

    public TMP_InputField currentSellStockAmount;

    public TextMeshProUGUI currentStockOfCompany;
    void Start()
    {
        eventText.text = currentEvent.flavorText;
        playerMoney = 10000;

        playerStocks = new Dictionary<string, int>();
        foreach(Company company in companyManager.companies)
        {
            playerStocks[company.name] = 0;
        }

        foreach(KeyValuePair<string, int> item in playerStocks)
        {
            uiManager.setCurrentAmountOfStock(item.Key, item.Value);
        }
    }
    public void SimulateOneTick()
    {
        //produce product
        foreach(Company company in companyManager.companies)
        {
            company.producedProduct += company.productionRate;
        }

        //Calculate sinks 
        for(int i = 0; i < companyManager.sinks.Length; i ++)
        {
            foreach (Company company in companyManager.sinks[i].buysFrom)
            {
                company.producedProduct -= companyManager.sinks[i].consumeRate;
                if(company.producedProduct < 0)
                {
                    company.producedProduct = 0;
                }

                company.buyingPower += companyManager.sinks[i].consumeRate * company.productPrice;
            }
        }
        //calculate companies buying and selling to each other 
        foreach(Company buyingCompany in companyManager.companies)
        {
            foreach(Company sellingCompany in buyingCompany.buysFrom)
            {
                desiredPurchaseAmount = sellingCompany.producedProduct - buyingCompany.desiredProductAmount[sellingCompany];       
                Debug.Log(desiredPurchaseAmount);     
                if(desiredPurchaseAmount < 0)
                {
                    desiredPurchaseAmount = sellingCompany.producedProduct;
                }
                desiredPurchasePricing = buyingCompany.buyingPower - (desiredPurchaseAmount * sellingCompany.productPrice);
                if(desiredPurchasePricing < 0)
                {
                    desiredPurchasePricing = buyingCompany.buyingPower;
                }

                buyingCompany.buyingPower = desiredPurchasePricing;
                sellingCompany.producedProduct = desiredPurchaseAmount;
            }
        }

        //recalculate cost of stock based on an equation I am making up
        foreach(Company company in companyManager.companies)
        {
            int demand = 0;
            foreach(Company sellingToCompany in company.buysFrom)
            {
                demand += company.desiredProductAmount[sellingToCompany];
            }
            company.buyingCost = ((demand*1.5f) + company.buyingPower + (company.producedProduct * company.productPrice))/12;
            if(demand <= 0)
            {
                company.sellingCost = company.buyingCost * 1.2f;
            }
            else
            {
                company.sellingCost = company.buyingCost * 0.8f;
            }
        }
        
        //Select the next event
        //currentEvent = eventManager.events[Random.Range(0, eventManager.events.Length)];
        eventText.text = currentEvent.flavorText;

        //apply event happenings
        if(currentEvent.name == "Zombies")
        {
            currentEvent.affectedSink[0].consumeRate = currentEvent.affectedSink[0].consumeRate/2;
        }
        else if (currentEvent.name == "Cthulu")
        {
            currentEvent.affectedCompany[0].productionRate =  currentEvent.affectedCompany[0].productionRate * 2;
            currentEvent.affectedCompany[0].producedProduct = currentEvent.affectedCompany[0].producedProduct/2;
            currentEvent.affectedSink[0].consumeRate = currentEvent.affectedSink[0].consumeRate/2;
        }
        else if(currentEvent.name == "Khnum")
        {
            currentEvent.affectedCompany[0].producedProduct = 0;
            currentEvent.affectedCompany[1].producedProduct = 0;
        }

        //update UI with new stock price
        foreach(Company company in companyManager.companies)
        {
            uiManager.updateStockPrice(company);
        }
    }

    public void buyStock(GameObject clickedButton)
    {
        currentParentofClickedButton = clickedButton.transform.parent;
        foreach(Transform child in currentParentofClickedButton)
        {
            if(child.gameObject.name == "buyInput")
            {
                currentBuyStockAmount = child.GetComponent<TMP_InputField>();
                break;
            }
        }

        if (int.TryParse(currentBuyStockAmount.text, out int result))
        {
            if(result > 0)
            {
                currentBuyingCompany = getCompanyFromName(currentParentofClickedButton.GetComponent<TextMeshProUGUI>().text);
                attemptedBuyingAmount = currentBuyingCompany.buyingCost * result; 
                if(playerMoney >= attemptedBuyingAmount)
                {
                    playerStocks[currentBuyingCompany.name] += result;
                    playerMoney -= attemptedBuyingAmount;
                    uiManager.setCurrentPlayerMoney(playerMoney);
                    uiManager.setCurrentAmountOfStock(currentBuyingCompany.name, playerStocks[currentBuyingCompany.name]);
                }
                else
                {
                    Debug.Log("Player does not have enough money to buy that amount");
                }
            }
            else
            {
                Debug.Log("buy amount is not a number greater than 0");
            }
        }
        else
        {
            Debug.Log("Buy Amount is not a valid number");
        }

    }

     public void sellStock(GameObject clickedButton)
    {

        currentParentofClickedButton = clickedButton.transform.parent;
        foreach(Transform child in currentParentofClickedButton)
        {
            if(child.gameObject.name == "sellInput")
            {
                currentSellStockAmount = child.GetComponent<TMP_InputField>();
                break;
            }
        }

        if (int.TryParse(currentSellStockAmount.text, out int result))
        {
            if(result > 0)
            {
                currentSellingCompany = getCompanyFromName(currentParentofClickedButton.GetComponent<TextMeshProUGUI>().text);
                attemtedSellingAmount = currentSellingCompany.buyingCost * result; 
                if(playerStocks[currentSellingCompany.name] >= result)
                {
                    playerStocks[currentSellingCompany.name] -= result;
                    playerMoney += attemtedSellingAmount;
                    uiManager.setCurrentPlayerMoney(playerMoney);
                    uiManager.setCurrentAmountOfStock(currentSellingCompany.name, playerStocks[currentSellingCompany.name]);
                }
                else
                {
                    Debug.Log("Player does not have enough stock to sell that amount");
                }
              
            }
            else
            {
                Debug.Log("buy amount is not a number greater than 0");
            }
        }
        else
        {
            Debug.Log("Buy Amount is not a valid number");
        }

    }

    public void buyMaxStock(GameObject clickedButton)
    {
        currentParentofClickedButton = clickedButton.transform.parent;
        foreach(Transform child in currentParentofClickedButton)
        {
            if(child.gameObject.name == "currentStock")
            {
                currentStockOfCompany = child.GetComponent<TextMeshProUGUI>();
                break;
            }
        }
        currentSellingCompany = getCompanyFromName(currentParentofClickedButton.GetComponent<TextMeshProUGUI>().text);
        int maxNumberOfStocks = Mathf.FloorToInt(playerMoney / currentSellingCompany.buyingCost);
        playerStocks[currentSellingCompany.name] += maxNumberOfStocks;
        currentStockOfCompany.text =  playerStocks[currentSellingCompany.name].ToString();
        playerMoney -= maxNumberOfStocks * currentSellingCompany.buyingCost;
        uiManager.setCurrentPlayerMoney(playerMoney);
        uiManager.setCurrentAmountOfStock(currentSellingCompany.name, playerStocks[currentSellingCompany.name]);


    }

    public void SellMaxStock(GameObject clickedButton)
    {
        currentParentofClickedButton = clickedButton.transform.parent;
        foreach(Transform child in currentParentofClickedButton)
        {
            if(child.gameObject.name == "currentStock")
            {
                currentStockOfCompany = child.GetComponent<TextMeshProUGUI>();
                break;
            }
        }
        currentSellingCompany = getCompanyFromName(currentParentofClickedButton.GetComponent<TextMeshProUGUI>().text);
        if (int.TryParse(currentStockOfCompany.text, out int result))
        {
            attemtedSellingAmount = currentSellingCompany.buyingCost * result; 
            playerStocks[currentSellingCompany.name] -= result;
            playerMoney += attemtedSellingAmount;
            uiManager.setCurrentPlayerMoney(playerMoney);
            uiManager.setCurrentAmountOfStock(currentSellingCompany.name, playerStocks[currentSellingCompany.name]);
        }
        else
        {
            Debug.Log("There was an error parsing the current stock of company to int");
        }
        
    }


    private Company getCompanyFromName(string name)
    {
        Debug.Log(companyManager);
        foreach(Company company in companyManager.companies)
        {
            if(company.name == name)
            {
                return company;
            }
        }
        return null;
    }
}
