using UnityEngine;
using System.Collections;
using Leap;

public class FakeHand : MonoBehaviour {
	
    Controller controller;
	//Frame frameprec = null;
	GameObject palmo = null;
	System.Collections.Generic.List<GameObject> dita = new System.Collections.Generic.List<GameObject>();
	
	// Use this for initialization
	void Start () {
		//Debug.Log("CI SONO IO, STRUNZ!");
	    controller = new Controller();
		palmo = GameObject.Find("Cube");
		palmo.renderer.enabled = false;
		dita.Add(GameObject.Find("Cylinder1"));
		dita.Add(GameObject.Find("Cylinder2"));
		dita.Add(GameObject.Find("Cylinder3"));
		dita.Add(GameObject.Find("Cylinder4"));
		dita.Add(GameObject.Find("Cylinder5"));
		foreach (GameObject d in dita)
			d.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = controller.Frame();
		palmo.renderer.enabled = false;
		foreach (GameObject f in dita)
			f.renderer.enabled = false;
		
		if (!frame.Hands.Empty)
		{
			palmo.renderer.enabled = true;
			Hand h = frame.Hands[0];
			palmo.transform.position = new Vector3(h.PalmPosition.x / 10, h.PalmPosition.y / 10 - 10, h.PalmPosition.z / 10);
			palmo.transform.rotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
				new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
			if (!h.Fingers.Empty)
			{
				int i = 0;
				foreach (Finger f in h.Fingers)
				{
					dita[i].renderer.enabled = true;
					// assegna un finger a un dito dandogli la posizione e rotazione.
					dita[i].transform.position = new Vector3(f.TipPosition.x / 10, f.TipPosition.y / 10 - 10, f.TipPosition.z / 10);
					dita[i].transform.rotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
						new Vector3(f.Direction.x*100, f.Direction.y*100, f.Direction.z*100));
					i++;
				}
			}
		}
	}
}
