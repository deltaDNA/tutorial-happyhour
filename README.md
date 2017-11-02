# tutorial-happyhour
This Unity tutorial shows how to use the Engage In-Game campaign system to schedule Happy Hour sales or similar timed offers in the player's local timezone. 

![screenshot](/Assets/Images/HappyHour-Screenshot.png)

A request is made to the Engage In-Game Messagaing API, to check for any timed offers, when the app is launched or when the player presses the "Check Offers" button. If there are any active offers, Engage will respond with two game parameters indicating the start and end hours for the offer. The game client can then perform logic to decide whether there is a pending or active offer and take the appropriate action. 

## 1) Create the game parameters to send to device
Use the SETUP > Game Parameters tool to create two new game parameters that will be used to send the start hour and end hour to the app.
![parameters](/Assets/Images/saleStartHour.png)

## 2) Create the Action (content) to be sent to the device
Use the ENGAGE > In Game > Actions tool to create a new "Game Parameters" action, add your new start and end parameters and set their values. If you want the parameters to be communicated to the device on every campaign hit ensure you check the every response checkbox.
![game parameters action](/Assets/Images/HappyHourActionParameters.png)

## 3) Create the Happy Hour Campaign 
Use the ENGAGE > In Game > Campaigns tool to create a new campaign containing the Action you just created. You will probably want to toggle the campaign repeat option to ensure your campaign re-sends the start and end parameters with each request, rather than just the first time the player enters the campaign step. We created an "offerCheck" decision point in the SETUP > Decision Point tool and selected it for this campaign.  
![campaign configuration](/Assets/Images/HappyHour-Campaign.png)

That takes care of the campaign configuration. 

In the code we will make an Engage request to the to the "offerCheck" decision point then check the response to see if it contains a "parameters" block. If it does we will then check to see if it contains our start and end parameters and use them to control our Happy Hour if it does. 
```C#
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
                    saleStartTime = s.Date + new System.TimeSpan( System.Convert.ToInt32(p["saleStartHour"]),0, 0); 
                    
                if (p.ContainsKey("saleEndHour"))
                    saleEndTime = s.Date + new System.TimeSpan( System.Convert.ToInt32(p["saleEndHour"]), 0,0);
                .
                .
                .                   
            }
        }, exception =>
        {
            Debug.Log("Engage encountered an error: " + exception.Message);
        });
    }
```

Then we can simply use the values in `saleStartTime` and `saleEndTime` to control the Happy Hour sale displays. 

```C#
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
```

![screenshot](/Assets/Images/HappyHour-Screenshot2.png)

If you download and build this tutorial, you will need to set your local device time to between 17:00 and 18:00 to trigger the Happy Hour. The "Simulate Offer" button will show a preview what the Happy Hour display looks like for two minutes, without making a request to Engage. 
