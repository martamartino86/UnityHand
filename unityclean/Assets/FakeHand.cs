using UnityEngine;
using System.Collections;
using Leap;

public class FakeHand : MonoBehaviour {
	
    Controller controller;
	//Frame frameprec = null;
	GameObject palmo1 = null, palmo2 = null;
	System.Collections.Generic.List<GameObject> dita1 = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<GameObject> dita2 = new System.Collections.Generic.List<GameObject>();
	
	// Use this for initialization
	void Start () {
	    controller = new Controller();
		palmo1 = GameObject.Find("Palmo1");
		palmo1.renderer.enabled = false;
		palmo2 = GameObject.Find("Palmo2");
		palmo2.renderer.enabled = false;
		dita1.Add(GameObject.Find("Cylinder1"));
		dita1.Add(GameObject.Find("Cylinder2"));
		dita1.Add(GameObject.Find("Cylinder3"));
		dita1.Add(GameObject.Find("Cylinder4"));
		dita1.Add(GameObject.Find("Cylinder5"));
		dita2.Add(GameObject.Find("Cylinder6"));
		dita2.Add(GameObject.Find("Cylinder7"));
		dita2.Add(GameObject.Find("Cylinder8"));
		dita2.Add(GameObject.Find("Cylinder9"));
		dita2.Add(GameObject.Find("Cylinder10"));
		foreach (GameObject d in dita1)
			d.renderer.enabled = false;
		foreach (GameObject d in dita2)
			d.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame();
		palmo1.renderer.enabled = false;
		palmo2.renderer.enabled = false;
		foreach (GameObject f in dita1)
			f.renderer.enabled = false;
		foreach (GameObject f in dita2)
			f.renderer.enabled = false;
		if (!frame.Hands.Empty)
		{
			//Debug.Log("CI SONO " + frame.Hands.Count + " MANI");
			if (frame.Hands.Count == 2)
			{
				palmo1.renderer.enabled = true;
				palmo2.renderer.enabled = true;
				Hand h1 = frame.Hands[0];
				Hand h2 = frame.Hands[1];
				// muovi entrambe le mani
				MoveHand(palmo1, h1, dita1);
				MoveHand(palmo2, h2, dita2);
			}
			else if (frame.Hands.Count == 1)
			{
				// è indifferente, i palmi sono uguali
				palmo1.renderer.enabled = true;
				palmo2.renderer.enabled = false;
				Hand h1 = frame.Hands[0];
				// muovi una sola mano
				MoveHand(palmo1, h1, dita1);
			}
		}
	}
	
	void MoveHand(GameObject palmo, Hand h, System.Collections.Generic.List<GameObject> dita)
	{
		palmo.transform.position = new Vector3(h.PalmPosition.x / 10, h.PalmPosition.y / 10 - 10, h.PalmPosition.z / 10);
		palmo.transform.rotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
			new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
		if (!h.Fingers.Empty)
		{
			//Debug.Log("CI SONO " + h.Fingers.Count + " DITA");
			int i = 0;
			foreach (Finger f in h.Fingers)
			{
				if (i < 5)
				
				{
					dita[i].renderer.enabled = true;
					// assegna un finger a un dito dandogli la posizione e rotazione.
					dita[i].transform.position = new Vector3(f.TipPosition.x / 10, f.TipPosition.y / 10 - 10, f.TipPosition.z / 10);
					dita[i].transform.rotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
						new Vector3(f.Direction.x*100, f.Direction.y*100, f.Direction.z*100));
				}
				i++;
			}
		}
	}

}
