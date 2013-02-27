using UnityEngine;
using System.Collections;
using Leap;

public class FakeHand_2 : MonoBehaviour {
	Controller controller;
	GameObject palmo = null, dito = null;
	System.Collections.Generic.List<GameObject> listaPalmi = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<GameObject> listaDita = new System.Collections.Generic.List<GameObject>();
	
	
	// Use this for initialization
	void Start () {
		controller = new Controller();
		palmo = GameObject.Find("Palmo");
		palmo.renderer.enabled = false;
		dito = GameObject.Find("Dito");
		dito.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 palmPosition, fingerPosition;
		Quaternion palmRotation, fingerRotation;
		GameObject g;
		
		Frame frame = controller.Frame();
		Debug.Log("F.FINGERS: " + frame.Fingers.Count + "F.HANDS: " + frame.Hands.Count);
		
		foreach (GameObject go in listaPalmi)
			Destroy(go);
		foreach (GameObject go in listaDita)
			Destroy(go);
		
		/* Se vede dita, esiste ALMENO una mano. */
		if (!frame.Hands.Empty)
		{
			foreach (Hand h in frame.Hands)
			{
				Debug.Log("Ma quante volte ci entri???");
				palmPosition = new Vector3(h.PalmPosition.x / 10, h.PalmPosition.y / 10 - 10, h.PalmPosition.z / 10);
				palmRotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
					new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
				g = (GameObject)(Instantiate(palmo, palmPosition, palmRotation));
				g.renderer.enabled = true;
				listaPalmi.Add(g);
			}
			if (!frame.Pointables.Empty)
			{
				foreach (Pointable f in frame.Pointables)
				{
					fingerPosition = new Vector3(f.TipPosition.x / 10, f.TipPosition.y / 10 - 10, f.TipPosition.z / 10);
					fingerRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
						new Vector3(f.Direction.x*100, f.Direction.y*100, f.Direction.z*100));
					g = (GameObject)(Instantiate(dito, fingerPosition, fingerRotation));
					g.renderer.enabled = true;
					listaDita.Add(g);
				}
			}
		}
		
		
	}
}
