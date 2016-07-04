using UnityEngine;
using System.Collections;

public class Demo_ActionBars : MonoBehaviour {

	public GameObject actionBar1;
	public GameObject actionBar2;
	
	public void OnChange1(bool state)
	{
		if (this.actionBar1 != null)
			this.actionBar1.SetActive(state);
	}
	
	public void OnChange2(bool state)
	{
		if (this.actionBar2 != null)
			this.actionBar2.SetActive(state);
	}
}
