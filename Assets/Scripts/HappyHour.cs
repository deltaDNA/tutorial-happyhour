using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
public class HappyHour : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        // Enter additional configuration here
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.ClientVersion = "1.0";

        // Launch the SDK
        DDNA.Instance.StartSDK(
            "16236542504940240977685435115096",
            "https://collect12360ttrlh.deltadna.net/collect/api",
            "https://engage12360ttrlh.deltadna.net"
        );
    }

    // Update is called once per frame
    void Update () {
		
	}
}


