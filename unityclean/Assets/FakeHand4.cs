using UnityEngine;
using System.Collections.Generic;
using Leap;
using GestIT;
using FakeDriver;
using MyLeapFrame;

public class FakeHand4 : MonoBehaviour {
	static LeapSensor s = new LeapSensor();
	MyFrame f = null;
	GameObject palmo = null, dito = null, sfera = null, tool = null;
	static List<GameObject> oldObjs = new List<GameObject>();
	
	static Dictionary<FakeId,Color> id2Color = new Dictionary<FakeId, Color>();
	
	static Color[] colors = {
		Color.black,
		Color.blue,
		Color.cyan,
		Color.gray,
		Color.green,
		Color.magenta,
		Color.red,
		Color.white,
		Color.yellow		
	};
	
	static Vector3 LeapToUnity(Vector v) {
		return new Vector3(v.x, v.y, v.z);
	}
	
	// Use this for initialization
	void Start () {
		s = new LeapSensor();
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

		((ISensor<LeapFeatureTypes,LeapEventArgs>) s).SensorEvents += HandleEvent;
	}
	
	void HandleEvent(object sender, SensorEventArgs<LeapFeatureTypes,LeapEventArgs> e) {
		f = e.Event.Frame.Clone();
	}

	// Update is called once per frame
	void Update () {
		foreach (var x in oldObjs)
			Destroy (x);

		oldObjs.Clear();
		
		if (f == null)
			return;
		
		GameObject g;
		Vector3 palmPosition, fingerPosition, sphereCenter, fingerVelocity;
		Quaternion palmRotation, fingerRotation;
		
		Color c;

		foreach (var h in f.HandList.Values) {
			palmPosition = LeapToUnity(h.Position);
			palmRotation = Quaternion.FromToRotation(new UnityEngine.Vector3(0,-1,0),
				LeapToUnity(h.Normal).normalized);
			g = (GameObject)(Instantiate(palmo, palmPosition, palmRotation));
			g.renderer.enabled = true;
			oldObjs.Add(g);

			sphereCenter = new Vector3(h.SphereCenter.x, h.SphereCenter.y, h.SphereCenter.z);
			g = (GameObject)(Instantiate(sfera, sphereCenter, Quaternion.identity));
			g.transform.localScale = Vector3.one * (h.SphereRadius);
			g.renderer.enabled = true;
			oldObjs.Add(g);
		}

		foreach (var p in f.PointableList.Values) {
			fingerPosition = LeapToUnity(p.Position);
			fingerRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0),
				LeapToUnity(p.Direction));
			fingerVelocity = new Vector3(p.Velocity.x, p.Velocity.y, p.Velocity.z);
			g = (GameObject)(Instantiate(dito, fingerPosition, fingerRotation));
			g.transform.localScale = new Vector3(p.Width, p.Length, p.Width) / 2;
			g.renderer.enabled = true;
			oldObjs.Add(g);
		}
	}	
}
