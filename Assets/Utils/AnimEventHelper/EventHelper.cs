using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using System;

namespace CreativityZero.Utils {

	public class EventHelper : MonoBehaviour {

		public UnityEngine.Object[] objs = new UnityEngine.Object[0];
		public string[] args = new string[0];
		public string[] myTypes = new string[0];

		void Start () {
		}

		void Update () {
		
		}

		public void RemoveEvent (int index) {
			List<UnityEngine.Object> lObjs = new List<UnityEngine.Object> (this.objs);
			List<string> lArgs = new List<string> (this.args);
			List<string> lMyTypes = new List<string> (this.myTypes);
			
			lObjs.RemoveAt (index);
			lArgs.RemoveAt (index);
			lMyTypes.RemoveAt (index);
			
			this.objs = lObjs.ToArray ();
			this.args = lArgs.ToArray ();
			this.myTypes = lMyTypes.ToArray ();
		}

		private void AddEvent (UnityEngine.Object obj, string arg, string myType) {
			UnityEngine.Object[] newObjs = new UnityEngine.Object[this.objs.Length + 1];
			string[] newArgs = new string[this.args.Length + 1];
			string[] newMyTypes = new string[this.myTypes.Length + 1];

			for (int i = 0; i < this.objs.Length; i++) {
				newObjs[i] = this.objs[i];
			}
			for (int i = 0; i < this.args.Length; i++) {
				newArgs[i] = this.args[i];
			}
			for (int i = 0; i < this.myTypes.Length; i++) {
				newMyTypes[i] = this.myTypes[i];
			}

			newObjs [this.objs.Length] = obj;
			newArgs [this.args.Length] = arg;
			newMyTypes [this.myTypes.Length] = myType;

			this.objs = newObjs;
			this.args = newArgs;
			this.myTypes = newMyTypes;
		}

		public void AddEvent (UnityEngine.Object obj, string param) {

			if (obj != null) {
				if (obj.GetType () == typeof(ParticleSystem)) {
					Debug.Log ("ParticleSystem added");
					this.AddEvent (obj, param, "ParticleSystem");
				}

				else if (obj.GetType () == typeof(Animator)) {
					Debug.Log ("Animator added");
					this.AddEvent (obj, param, "Animator");
				}

				else if (obj.GetType () == typeof(GameObject)) {
					GameObject gObj = (GameObject) obj;
					if (gObj.GetComponent<ParticleSystem> () != null) {
						Debug.Log ("GameObject with ParticleSystem added");
						this.AddEvent (obj, param, "GameObjectWithParticleSystem");
					}
					else if (gObj.GetComponent<Animator> () != null) {
						Debug.Log ("GameObject with Animator added");
						this.AddEvent (obj, param, "GameObjectWithAnimator");
					}
				}
			}
		}

		public void CZeroEvent (int index) {
			if (index >= 0) {
				if (index < this.objs.Length) {
					if (index < this.myTypes.Length) {
						if (this.myTypes[index] == "GameObjectWithParticleSystem") {
							GameObject gObj = (GameObject) this.objs[index];
							ParticleSystem pSys = gObj.GetComponent<ParticleSystem> ();

							pSys.Play ();
						}
						else if (this.myTypes[index] == "GameObjectWithAnimator") {
							GameObject gObj = (GameObject) this.objs[index];
							Animator anim = gObj.GetComponent<Animator> ();
							
							anim.SetTrigger(this.args[index]);
						}
					}
				}
			}
		}
	}
}
