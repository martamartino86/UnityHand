using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;


public class Coordinates {
	public Vector3 position;
	public Quaternion rotation;
	
	public Coordinates(Vector3 p, Quaternion r) {
		this.position = p;
		this.rotation = r;
	}
}

public class MyInstance {
	public enum Type {
		Palm,
		Finger,
		ToolFinger
	}
	
	public GameObject gameObj;
	public long lastTimeVisible;
	public Coordinates PosAndRot;
	public Type typeOfObj;
	
	public MyInstance(GameObject go, long ltv, Coordinates pr, Type t)
	{
		this.gameObj = go;
		this.lastTimeVisible = ltv;
		this.PosAndRot = pr;
		this.typeOfObj = t;
	}
}

public class FakeHand_3 : MonoBehaviour {
	// Strutture dati.
	static Dictionary<long, MyInstance> listActive = new System.Collections.Generic.Dictionary<long, MyInstance>();
	static Dictionary<long, MyInstance> listZombie = new System.Collections.Generic.Dictionary<long, MyInstance>();
	static Dictionary<long, MyInstance> listaSfere = new System.Collections.Generic.Dictionary<long, MyInstance>();
	static long myId = 0;
	Controller controller;
	GameObject palmo = null, dito = null, sfera = null;
	
FakeHand_3() : base() { Debug.Log("constructor"); }
	
	// Use this for initialization
	void Start () {
		
		controller = new Controller();
		palmo = GameObject.Find("Palmo");
		palmo.renderer.enabled = false;
		dito = GameObject.Find("Dito");
		dito.renderer.enabled = false;
		sfera = GameObject.Find("Sfera");
		sfera.renderer.enabled = false;
		Debug.Log("awake!");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 palmPosition, fingerPosition, sphereCenter;
		Quaternion palmRotation, fingerRotation;
		MyInstance instance, temp;
		GameObject g;
		List<long> buffer = new List<long>();
		var t = System.DateTime.Now.Ticks;
				
		Frame frame = controller.Frame();
		if (!frame.Hands.Empty)
		{
			foreach (Hand h in frame.Hands)
			{
				// Se non ce l'ho gia' in lista, lo aggiungo agli attivi.
				if (true || !listActive.ContainsKey(h.Id))
				{
					palmPosition = new Vector3(h.PalmPosition.x, h.PalmPosition.y, h.PalmPosition.z);
					palmRotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
						new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
					g = (GameObject)(Instantiate(palmo, palmPosition, palmRotation));
					g.renderer.enabled = true;
					instance = new MyInstance(g, t, new Coordinates(palmPosition, palmRotation), MyInstance.Type.Palm);
					listActive.Add(myId++, instance);
				}
				// Altrimenti ne aggiorno il timestamp
				else
				{
					g = listActive[h.Id].gameObj;
					g.transform.position = new Vector3(h.PalmPosition.x, h.PalmPosition.y, h.PalmPosition.z);
					g.transform.rotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
						new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
					listActive[h.Id].lastTimeVisible = t;
				}
				// Creo anche la sua sfera (da sistemare).
				foreach (var s in listaSfere.Values) {
					Destroy(s.gameObj);
				}
				listaSfere.Clear();
				sphereCenter = new Vector3(h.SphereCenter.x, h.SphereCenter.y, h.SphereCenter.z);
				g = (GameObject)(Instantiate(sfera, sphereCenter, Quaternion.identity));
				g.transform.localScale = Vector3.one * (h.SphereRadius);
				g.renderer.material.color = new Color(1, 0, 0, 0.5F);
				g.renderer.enabled = true;
				g.renderer.material.shader = Shader.Find("Transparent/Diffuse");
				instance = new MyInstance(g, t, new Coordinates(sphereCenter, new Quaternion(0,0,0,h.SphereRadius)), MyInstance.Type.Palm);
				listaSfere.Add(myId++, instance);
			}
			// Idem con patatine fritte per le dita.
			if (!frame.Pointables.Empty)
			{
				if (!frame.Fingers.Empty)
				{
					foreach (Finger f in frame.Fingers)
					{
						if (true || !listActive.ContainsKey(f.Id))
						{
							fingerPosition = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
							//Debug.Log(f.Id + " " + fingerPosition.ToString());
							fingerRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
								new Vector3(f.Direction.x, f.Direction.y, f.Direction.z));
							g = (GameObject)(Instantiate(dito, fingerPosition, fingerRotation));
							g.renderer.enabled = true;
							instance = new MyInstance(g, t, new Coordinates(fingerPosition,
								fingerRotation), MyInstance.Type.Finger);
							listActive.Add(myId++, instance);
						}
						else
						{
							g = listActive[f.Id].gameObj;
							g.transform.position = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
							g.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
								new Vector3(f.Direction.x, f.Direction.y, f.Direction.z));
							listActive[f.Id].lastTimeVisible = t;
						}
					}
				}
				// Fallo anche per i Tools!!! (nel giorno del mai nel mese del poi)
			}
		}
		
		// Tolgo dalla lista attivi gli oggetti non piu' rilevati.
		foreach (long k in listActive.Keys)
		{
			instance = listActive[k];
			//Debug.Log("differenza = " + (t - instance.lastTimeVisible));
			if (instance.lastTimeVisible != t)
			{
				listZombie.Add(k, instance);
			}
		}

		foreach (long k in listZombie.Keys)
			listActive.Remove(k);

		foreach (long k in listZombie.Keys)
		{
			instance = listZombie[k];

			if (t - instance.lastTimeVisible < 5000000)
			{
				foreach (var o in listActive)
				{
					temp = listActive[o.Key];
					float d = Vector3.Distance(instance.PosAndRot.position, temp.PosAndRot.position);
					if (d <= 10)
					{
						buffer.Add(k);
					}
				}
			}
			else
			{
				Debug.Log("differenza = " + (t - instance.lastTimeVisible));
				// Il tempo e' scaduto: rimuovi l'elemento dalla lista zombie.
				buffer.Add(k);
			}
		}
		
		foreach(long k in buffer)
		{
			Destroy(listZombie[k].gameObj);
			listZombie.Remove(k);
		}
		
		//Debug.Log("Active: " + listActive.Count + " Zombie: " + listZombie.Count);
	}
}
