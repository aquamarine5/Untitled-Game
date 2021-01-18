using UnityEngine;
using System.Collections;
using Mirror;

[AddComponentMenu("EasyTouch Controls/Set Direct Action Transform ")]
public class ETCSetDirectActionTransform : NetworkBehaviour {

	public string axisName1;
	public string axisName2;

	void Start(){
		if (!string.IsNullOrEmpty(axisName1)){
			ETCInput.SetAxisDirecTransform(axisName1, transform);
		}

		if (!string.IsNullOrEmpty(axisName2)){
			ETCInput.SetAxisDirecTransform(axisName2, transform);
		}
	}
}
