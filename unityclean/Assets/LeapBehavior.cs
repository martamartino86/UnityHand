/*
using UnityEngine;
using System.Collections;
using Leap;

public class LeapBehavior : MonoBehaviour {
    Controller controller;
	Frame frameprec = null;
	GameObject hand = null;
	GameObject sphere = null;
	
	
    void Start ()
    {
        controller = new Controller();
		hand = GameObject.Find("hand");
		sphere = GameObject.Find("Sphere");
		Debug.Log("bla bla");
    }

    void Update ()
    {
        Frame frame = controller.Frame();
		if (frameprec == null)
			frameprec = frame;
		//Debug.Log(frame.Translation(frameprec).ToString());
        // do something with the tracking data in the frame...
	    if(!frame.Hands.Empty)
		{
			Hand h = frame.Hands[0];
			
			//
			if (!h.Fingers.Empty)
			{
				Finger f = h.Fingers[0];
				Vector3 v = new Vector3(f.TipPosition.x/10, f.TipPosition.y/10 - 10, f.TipPosition.z/10);
				// traslo la mano col solo movimento di un dito
				transform.position = (v);
			}
			//
			
			// traslo la mano in base alla posizione del palmo
			Vector3 v = new Vector3(h.PalmPosition.x/10, h.PalmPosition.y/10 - 10, h.PalmPosition.z/10);
			hand.transform.position = v;
			
			//Vector3 palm = new Vector3(h.PalmNormal.x*100, h.PalmNormal.y*100, h.PalmNormal.z*100);
			//transform.localEulerAngles = palm;
			hand.transform.localRotation = Quaternion.Euler(h.PalmNormal.Yaw*10, h.PalmNormal.Pitch*10, h.PalmNormal.Roll*10); // del tutto useless?
			hand.transform.rotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
				new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
			
			if (!h.Fingers.Empty)
			{				
				Finger f = h.Fingers[0];
				Vector3 fv = new Vector3(f.TipPosition.x/10, f.TipPosition.y/10 - 10, f.TipPosition.z/10);
				sphere.transform.position = fv;
			}
		}
		frameprec = frame;
		//	transform.Rotate(new Vector3(0,1f,0), Space.Self);
    }

}
*/