using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JSONObject = System.Collections.Generic.Dictionary<string, object>;

using DeltaDNA;
public class HappyHour : MonoBehaviour {
    
    // These will be populated by the Engage campaign system 
    // and used to display / hide Happy Hour information.
    private System.DateTime saleStartTime;
    private System.DateTime saleEndTime;

    // Some UI display items
    private float deltaTime;
    public UnityEngine.UI.Text lblCurrentTime;
    public UnityEngine.UI.Text lblCurrentTimeUTC;    
    public UnityEngine.UI.Text lblHappyHour;
    public UnityEngine.UI.Text lblHappyHourExpiry;
    public UnityEngine.UI.Text lblHappyHourNext; 
    public UnityEngine.UI.Image imgSaleClock;


    // Use this for initialization
    void Start()
    {
        UpdateTimeDisplays();

        // Enter additional configuration here
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.ClientVersion = "1.0";

        // Launch the SDK
        DDNA.Instance.StartSDK(
            "16236542504940240977685435115096",
            "https://collect12360ttrlh.deltadna.net/collect/api",
            "https://engage12360ttrlh.deltadna.net"
        );

        CheckOffers();
    }

    // Update is called once per frame
    void Update() {
        deltaTime += Time.deltaTime;
        if (deltaTime >= 1.0f)
        {
            deltaTime = 0;
            UpdateTimeDisplays();
        }
    }

    public void CheckOffers()
    {
        // Check with deltaDNA Engage to see if there are any current offers. 
        var offerCheck = new Engagement("offerCheck");
        
        DDNA.Instance.RequestEngagement(offerCheck, response =>
        {            
            if (response.JSON.ContainsKey("parameters"))
            {      
                // Engage has responsed, and the response contains some paramters for the game client. 
                object parameters;
                response.JSON.TryGetValue("parameters", out parameters);

                JSONObject p = parameters as JSONObject;

                System.DateTime s = System.DateTime.Now;

                // Check if the parameters are for controlling the Start and End time of an offer. 
                if (p.ContainsKey("saleStartHour"))
                    saleStartTime = s.Date + new System.TimeSpan( System.Convert.ToInt32(p["saleStartHour"]),0, 0); ;
                if (p.ContainsKey("saleEndHour"))
                    saleEndTime = s.Date + new System.TimeSpan( System.Convert.ToInt32(p["saleEndHour"]), 0,0);

                // If the start and end time look valid, then use them to set the Happy Hour.
                if (saleStartTime != System.DateTime.MinValue && saleEndTime != System.DateTime.MinValue)
                {
                    Debug.Log(string.Format("Happy Hour Sale Active {0} to {1} "
                        , saleStartTime.ToString()
                        , saleEndTime.ToString()));
                }
            }
        }, exception =>
        {
            Debug.Log("Engage encountered an error: " + exception.Message);
        });
    }


    public void LocalOffer()
    {
        // Simulates an offer locally without using Engage, to check that offer displays and timing works correctly.
        saleStartTime = System.DateTime.Now;
        saleEndTime = System.DateTime.Now.AddMinutes(2);

    }


    void UpdateTimeDisplays()
    {
        lblCurrentTime.text = System.DateTime.Now.ToString("HH:mm:ss");
        lblCurrentTimeUTC.text = System.DateTime.Now.ToUniversalTime().ToString("HH:mm:ss");

        UpdateSaleDisplay();
    }


    void UpdateSaleDisplay()
    {
        // Update Happy Hour Displays
        if (saleStartTime != null && saleEndTime !=null)
        {            
            if(saleStartTime < System.DateTime.Now && saleEndTime > System.DateTime.Now)
            {
                // Sale is active 
                // Update Expiry Duration                
                System.TimeSpan expiresIn = saleEndTime - System.DateTime.Now;
                lblHappyHourExpiry.text = string.Format("{0}:{1}:{2}", expiresIn.Hours, expiresIn.Minutes, expiresIn.Seconds);
            
                ShowSaleDisplays(true);                               
            }
            else
            {
                // Sale not active
                if (saleStartTime > System.DateTime.Now)
                    lblHappyHourNext.text = string.Format("Next Happy Hour \n{0}", saleStartTime.ToString());
                else if (saleEndTime < System.DateTime.Now)
                    lblHappyHourNext.text = string.Format("Happy Hour Expired at\n{0}", saleEndTime.ToString());

                ShowSaleDisplays(false);
            }
        }
    }


    void ShowSaleDisplays(bool IsActive)
    {
        // Show or Hide the Happy Hour sale image and countdown timer.
        imgSaleClock.gameObject.SetActive(IsActive);
        lblHappyHourExpiry.gameObject.SetActive(IsActive);
        lblHappyHour.gameObject.SetActive(IsActive);
        lblHappyHourNext.gameObject.SetActive(!IsActive && saleStartTime > System.DateTime.Now);

    }

}