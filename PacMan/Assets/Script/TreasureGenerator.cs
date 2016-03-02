using UnityEngine;
using System.Collections;

public class TreasureGenerator : MonoBehaviour {
	private const int GENERATOR_TIMES = 2;
	public float[] m_generateTime = new float[GENERATOR_TIMES];
	private bool[] m_generated = new bool[GENERATOR_TIMES];
	private GameObject[] m_treasureInstances = new GameObject[GENERATOR_TIMES];
	public float m_timer;
	public GameObject m_treasure;

	// Use this for initialization
	void Start () {
		m_timer = .0f;
		for (int i=0; i<GENERATOR_TIMES; i++) {
			m_generated[i] = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i=0; i<GENERATOR_TIMES; i++) {
			if(!m_generated[i] && m_generateTime[i]<=m_timer){
				m_treasureInstances[i] = Instantiate(m_treasure, transform.position, transform.rotation) as GameObject;
				m_treasureInstances[i].transform.parent = transform;
				m_generated[i] = true;
			}
		}
		m_timer += Time.deltaTime;
	}

	public void onRestart(){
		for (int i=0; i<GENERATOR_TIMES; i++) {
			if(m_treasureInstances[i] != null){
				Destroy(m_treasureInstances[i]);
				m_treasureInstances[i] = null;
			}
		}
		m_timer -= 5.0f;
	}
}







