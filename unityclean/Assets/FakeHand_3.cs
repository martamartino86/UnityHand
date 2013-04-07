using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;


public class Coordinates {
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 velocity;
	
	public Coordinates(Vector3 p, Quaternion r, Vector3 v) {
		this.position = p;
		this.rotation = r;
		this.velocity = v;
	}
}

public class MyInstance {
	public enum Type {
		Sphere,
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
	static List<MyInstance> listActive = new System.Collections.Generic.List<MyInstance>();
	Controller controller;
	Leap.Vector oldPos = new Vector();
	long oldts = 0;
	GameObject palmo = null, dito = null, sfera = null, tool = null;
	
	Vector3 precpalmPosition = Vector3.zero;
	//FakeHand_3() : base() { Debug.Log("constructor"); }
	
	// Use this for initialization
	void Start () {
		
		controller = new Controller();
		palmo = GameObject.Find("Palmo");
		palmo.renderer.enabled = false;
		palmo.renderer.material.shader = Shader.Find("Transparent/Diffuse");

		dito = GameObject.Find("Dito");
		dito.renderer.enabled = false;
		dito.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		
		sfera = GameObject.Find("Sfera");
		sfera.renderer.enabled = false;
		sfera.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		
		tool = GameObject.Find("Tool");
		tool.renderer.enabled = false;
		sfera.renderer.material.shader = Shader.Find("Transparent/Diffuse");
		//Debug.Log("awake!");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 palmPosition, fingerPosition, sphereCenter, palmVelocity, fingerVelocity;
		Quaternion palmRotation, fingerRotation;
		MyInstance instance;
		GameObject g;
		List<long> buffer = new List<long>();
		var t = System.DateTime.Now.Ticks;
		
		Frame frame = controller.Frame();
		//Debug.Log("mani: " + frame.Hands.Count);
		foreach (Hand h in frame.Hands)
		{
			Debug.Log("ID PALM: " + h.Id);
			palmPosition = new Vector3(h.PalmPosition.x, h.PalmPosition.y, h.PalmPosition.z);
			//
			if (precpalmPosition == Vector3.zero)
				precpalmPosition = new Vector3(palmPosition.x, palmPosition.y, palmPosition.z);
			float diff = (palmPosition.y - precpalmPosition.y);
			//Debug.Log ("DIFF: " + diff);
			precpalmPosition = palmPosition;
			//
			palmRotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
				new UnityEngine.Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z).normalized);
			palmVelocity = new Vector3(h.PalmVelocity.x, h.PalmVelocity.y, h.PalmVelocity.z);
			g = (GameObject)(Instantiate(palmo, palmPosition, palmRotation));
			g.renderer.enabled = true;
			instance = new MyInstance(g, t, new Coordinates(palmPosition, palmRotation, palmVelocity), MyInstance.Type.Palm);
			listActive.Add(instance);
			//Debug.Log("Palm " + h.Id + "position: " + h.PalmPosition + "; velocity: " + h.PalmVelocity.Magnitude);
			// Creo anche la sua sfera (da sistemare).
			//Debug.Log("radius: "+h.SphereRadius);
			sphereCenter = new Vector3(h.SphereCenter.x, h.SphereCenter.y, h.SphereCenter.z);
			g = (GameObject)(Instantiate(sfera, sphereCenter, Quaternion.identity));
			g.transform.localScale = Vector3.one * (h.SphereRadius);
			g.renderer.enabled = true;
			instance = new MyInstance(g, t, new Coordinates(sphereCenter, new Quaternion(0,0,0,h.SphereRadius), palmVelocity),
				MyInstance.Type.Sphere);
			listActive.Add(instance);
		}
		// Idem con patatine fritte per le dita e per i tools.
		foreach (var f in frame.Pointables)
		{
			Debug.Log("ID POINTABLE: " + f.Id + "; LENGTH: " + f.Length);
			fingerPosition = new Vector3(f.TipPosition.x, f.TipPosition.y, f.TipPosition.z);
			//Debug.Log(f.Id + " " + fingerPosition.ToString());
			fingerRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
				new Vector3(f.Direction.x, f.Direction.y, f.Direction.z));
			fingerVelocity = new Vector3(f.TipVelocity.x, f.TipVelocity.y, f.TipVelocity.z);
			g = (GameObject)(Instantiate(dito, fingerPosition, fingerRotation));
			g.transform.localScale = new Vector3(f.Width, f.Length, f.Width) / 2;
			g.renderer.enabled = true;
			MyInstance.Type type;
			if (f.IsFinger)
				type = MyInstance.Type.Finger;
			else
				type = MyInstance.Type.ToolFinger;
			instance = new MyInstance(g, t, new Coordinates(fingerPosition,
				fingerRotation, fingerVelocity), type);
			var delta = f.TipPosition - oldPos;
			/*delta = new Vector(f.TipVelocity.x / delta.x,
				f.TipVelocity.y / delta.y, 
				f.TipVelocity.z / delta.z);
				*/
			delta /= (frame.Timestamp - oldts) * 1.0e-6f;
//			Debug.Log("Finger " + f.Id + " velocity: " + f.TipVelocity + " ratio: " + delta);
			oldPos = f.TipPosition;
			oldts = frame.Timestamp;
			listActive.Add(instance);
		}
		
		while ((listActive.Count != 0) && (t - listActive[0].lastTimeVisible > 0.5 / 100.0e-9))
		{
			Destroy(listActive[0].gameObj);
			listActive.RemoveAt(0);
		}

		foreach (var o in listActive)
		{
			Color c;
			if (o.lastTimeVisible == t)
			{
				//Debug.Log("velocity: "+fingerVelocity.magnitude);
				switch (o.typeOfObj)
				{
				case MyInstance.Type.Finger:
				case MyInstance.Type.Palm:
					c = new Color(1.8f, 0.2f + (o.PosAndRot.velocity.magnitude / 1500), 0.0f, 1.0f);
					break;
				case MyInstance.Type.Sphere:
					c = new Color(0.8f, 0.2f + (o.PosAndRot.velocity.magnitude / 1500), 0.0f, 0.5f);
					break;
				case MyInstance.Type.ToolFinger:
					c = new Color(0.2f, 0.5f, 0.2f + (o.PosAndRot.velocity.magnitude / 1500), 1.0f);
					break;
				default:
					c = new Color(1.0f, 0.0f, 1.0f, 1.0f);
					break;
				}
			}
			else
			{
				c = new Color(0.5f, 0.5f, 0.5f, 0.1f * (1.0f - (t - o.lastTimeVisible) / (0.5f / 100.0e-9f)));
				o.gameObj.transform.localScale *= 0.9f;
			}
			o.gameObj.renderer.material.color = c;
		}

		/*
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
		*/
	}
}
