using UnityEngine;
using System.Collections;
using Leap;

public class FakeHand_2 : MonoBehaviour {
	Controller controller;
	Frame precFrame = null;
	GameObject palmo = null, dito = null, sfera = null;
	System.Collections.Generic.List<GameObject> listaPalmi = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<GameObject> listaDita = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<GameObject> listaSfere = new System.Collections.Generic.List<GameObject>();
	
	
	// Use this for initialization
	void Start () {
		controller = new Controller();
		palmo = GameObject.Find("Palmo");
		palmo.renderer.enabled = false;
		dito = GameObject.Find("Dito");
		dito.renderer.enabled = false;
		sfera = GameObject.Find("Sfera");
		sfera.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 palmPosition, fingerPosition, sphereCenter;
		Quaternion palmRotation, fingerRotation;
		GameObject g;
		
		Frame frame = controller.Frame();
		if (precFrame == null)
			precFrame = frame;
		//Debug.Log("F.FINGERS: " + frame.Fingers.Count + "F.HANDS: " + frame.Hands.Count);
		
		// Se al frame precedente ne avevo, ora li cancello per disegnare quelli del frame corrente.
		foreach (GameObject go in listaPalmi)
			Destroy(go);
		foreach (GameObject go in listaDita)
			Destroy(go);
		foreach (GameObject go in listaSfere)
			Destroy(go);
		
		// Se vede dita, esiste ALMENO una mano.
		if (!frame.Hands.Empty)
		{
			foreach (Hand h in frame.Hands)
			{
				palmPosition = new Vector3(h.PalmPosition.x, h.PalmPosition.y, h.PalmPosition.z);
				palmRotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
					new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
				g = (GameObject)(Instantiate(palmo, palmPosition, palmRotation));
				g.renderer.enabled = true;
				listaPalmi.Add(g);

				// Creo anche la sua sfera.
				sphereCenter = new Vector3(h.SphereCenter.x, h.SphereCenter.y, h.SphereCenter.z);
				g = (GameObject)(Instantiate(sfera, sphereCenter, Quaternion.identity));
				g.transform.localScale = Vector3.one * (h.SphereRadius);
				g.renderer.material.color = new Color(1, 0, 0, 0.5F);
				g.renderer.enabled = true;
				g.renderer.material.shader = Shader.Find("Transparent/Diffuse");
				listaSfere.Add(g);
			}
			if (!frame.Pointables.Empty)
			{
				foreach (Pointable f in frame.Pointables)
				{
					fingerPosition = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
					fingerRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
						new Vector3(f.Direction.x, f.Direction.y, f.Direction.z));
					g = (GameObject)(Instantiate(dito, fingerPosition, fingerRotation));
					g.renderer.enabled = true;
					listaDita.Add(g);
				}
			}
		}
		precFrame = frame;
	}
}
