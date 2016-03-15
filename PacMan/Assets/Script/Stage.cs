using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour
{
	public GameControl m_gameControl;

	private const int STAGE_ORIGIN_X = 0;
	private const int STAGE_ORIGIN_Z = 0;

	private const char GEM = 'c';
	private const char WALL = '*';
	private const char SWORD = 's';
	private const char PLAYER_SPAWN_POINT = 'p';
	private const char TREASURE_SPAWN_POINT = 't';

	private const float WALL_Y = .0F;
	private const float GEM_Y = .5f;
	public float GEM_SIZE = 1.0f;
	public int GEM_SCORE = 10;

	public enum SPAWN_POINT_TYPE{
		PLAYER = 0,
		ENEMY_1,
		ENEMY_2,
		ENEMY_3,
		ENEMY_4,
		TREASURE,
		NUM
	}

	struct StageData{
		public int width;
		public int length;
		public int offset_x;
		public int offset_z;
		public char[,] data;
		public float[,] height;

		public int[,] gameParticleIndex;
	}

	private StageData m_stageData;
	private Vector3[] m_spawnPositions;

	private GameObject m_itemsFolder;
	private GameObject m_stageObjects;
	private GameObject m_stageCollision;

	public GameObject[] m_wallObjects;
	public GameObject[] m_itemObjects;
	public GameObject m_wallForCollision;

	public TextAsset m_defaultAsset;
	public TextAsset[] m_stageAssets;

	private int m_gemTotalNum;
	private int m_gemCurrentNum;
	private ParticleEmitter m_gemEmitter;

	public AudioChannels m_audio;
	public AudioClip m_gemPickupSe;

	public void onStageStart(){
		destroyStage ();
		setStageData ();
		createStage (true, "StageCollosion");
		createStage (false, "StageBlocks");
		m_gemCurrentNum = m_gemTotalNum;
	}

	private void setStageData(){
		int stageNo = m_gameControl.getStageNo ();
		if (stageNo > m_stageAssets.Length) {
			stageNo = (stageNo-3)%3+3;
		}
		loadFromTXT (m_stageAssets [stageNo-1]);
	}

	private void loadFromTXT(TextAsset txt){
		m_stageData.offset_x = STAGE_ORIGIN_X;
		m_stageData.offset_z = STAGE_ORIGIN_Z;

		if(txt != null){
			string txtData = txt.text;
			System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
			string[] lines = txtData.Split(new char[]{'\r', '\n'}, option);
			string[] sizewh = lines[0].Split(new char[]{','}, option);
			m_stageData.width = int.Parse(sizewh[0]);
			m_stageData.length = int.Parse(sizewh[1]);

			char[,] stageData = new char[m_stageData.length,m_stageData.width];
			for(int lineCount = 0; lineCount < m_stageData.length; lineCount++){
				if(lines.Length <= lineCount+1){
					break;
				}
				string[] data = lines[lineCount+1].Split(new char[]{','}, option);
				for(int col = 0; col < m_stageData.width; col++){
					if(data.Length <= col){
						break;
					}
					stageData[lineCount, col] = data[col][0];
				}
			}
			m_stageData.data = stageData;
		}
		else{
			Debug.LogWarning("stage data is null !");
		}
	}

	public void createModel(){
		loadFromTXT(m_defaultAsset);
		createStage(false, "Model", true);
	}

	private void createStage(bool collisionMode, string stageName, bool modelOnly = false){
		m_stageObjects = new GameObject (stageName);
		m_spawnPositions = new Vector3[(int)SPAWN_POINT_TYPE.NUM];
		if (m_itemsFolder != null) {
			Destroy(m_itemsFolder);
		}
		m_itemsFolder = new GameObject("itemFolder");

		for (int x = 0; x < m_stageData.width; x++) {
			for(int z = 0; z < m_stageData.length; z++){
				switch(m_stageData.data[z, x]){
				case WALL:
					if(collisionMode){
						GameObject obj = Instantiate(m_wallForCollision,
						                             new Vector3(x+m_stageData.offset_x, 0.5f, z+m_stageData.offset_z),
						                             Quaternion.identity) as GameObject;
						obj.transform.parent = m_stageObjects.transform;
					}
					else{
						GameObject obj = Instantiate(m_wallObjects[0],
						                             new Vector3(x+m_stageData.offset_x, WALL_Y, z+m_stageData.offset_z),
						                             Quaternion.identity) as GameObject;
						obj.transform.parent = m_stageObjects.transform;
					}
					break;
				case PLAYER_SPAWN_POINT:
					m_spawnPositions[(int)SPAWN_POINT_TYPE.PLAYER] = new Vector3(x+m_stageData.offset_x, 0.0f, z+m_stageData.offset_z);
					break;
				case TREASURE_SPAWN_POINT:
					m_spawnPositions[(int)SPAWN_POINT_TYPE.TREASURE] = new Vector3(x+m_stageData.offset_x, 0.0f, z+m_stageData.offset_z);
					break;
				case '1':
				case '2':
				case '3':
				case '4':
					int enemyType = int.Parse(m_stageData.data[z, x].ToString());
					m_spawnPositions[enemyType] = new Vector3(x+m_stageData.offset_x,0.0f,z+m_stageData.offset_z);
					break;
				case '5':
					m_spawnPositions[1]
					= new Vector3(x+m_stageData.offset_x,0.0f,z+m_stageData.offset_z);
					m_spawnPositions[2] = new Vector3(x+m_stageData.offset_x,0.0f,z+m_stageData.offset_z);					
					break;
				}
			}
		}

		if(modelOnly)return;
		
		Transform[] combineChildren = m_stageObjects.GetComponentsInChildren<Transform>();
		m_stageObjects.AddComponent<CombineChildren_Custom>();
		m_stageObjects.GetComponent<CombineChildren_Custom>().Combine();
		Destroy(m_stageObjects.GetComponent<CombineChildren_Custom>());
		
		for(int i=0; i<combineChildren.Length; i++){
			Destroy(combineChildren[i].gameObject);
		}
		
		if(collisionMode){
			m_stageObjects.AddComponent<MeshCollider>();
			m_stageObjects.GetComponent<MeshCollider>().sharedMesh = m_stageObjects.GetComponent<MeshFilter>().mesh;
			Destroy(m_stageObjects.GetComponent<MeshRenderer>());
			m_stageCollision = m_stageObjects;
		}
		else{
			setupGemsAndItems();
		}
	}

	void destroyStage(){
		if(m_stageObjects != null){
			Destroy(m_stageObjects);
		}
		m_stageObjects = null;
		if(m_stageCollision != null){
			Destroy(m_stageCollision);
		}
		m_stageCollision = null;
		if(m_gemEmitter != null){
			m_gemEmitter.ClearParticles();
		}
		m_gemEmitter = null;
		if(m_itemsFolder != null){
			Destroy(m_itemsFolder);
		}
		m_itemsFolder = null;
	}

	void setupGemsAndItems(){
		m_stageData.gameParticleIndex = new int[m_stageData.length,m_stageData.width];
		m_gemTotalNum = 0;

		for(int x = 0; x < m_stageData.width; x++){
			for(int z = 0; z < m_stageData.length; z++){
				if(isGem(m_stageData.data[z,x])){
					m_gemTotalNum++;
				}
			}
		}

		m_gemEmitter = this.GetComponent<ParticleEmitter>() as ParticleEmitter;
		m_gemEmitter.Emit(m_gemTotalNum);

		Particle[] gemParticle = m_gemEmitter.particles;
		int gemCount = 0;
		for(int x = 0; x < m_stageData.width; x++){
			for(int z = 0; z < m_stageData.length; z++){
				m_stageData.gameParticleIndex[z, x] = -1;
				if(isGem(m_stageData.data[z,x])){
					gemParticle[gemCount].position = new Vector3((float)x+m_stageData.offset_x,
					                                         							   	 GEM_Y,
					                                       								     (float)z+m_stageData.offset_z);
					gemParticle[gemCount].size = GEM_SIZE;
					m_stageData.gameParticleIndex[z, x] = gemCount;
					gemCount++;
				}

				if(m_stageData.data[z, x] == SWORD){
					Vector3 pos = new Vector3((float)x+m_stageData.offset_x,
					                          GEM_Y,
					                          (float)z+m_stageData.offset_z);
					GameObject obj = (GameObject)Instantiate(m_itemObjects[0], pos, Quaternion.identity);
					obj.transform.parent = m_itemsFolder.transform;
				}
			}
		}
		m_gemEmitter.particles = gemParticle;
	}



	bool isGem(char c){
		bool ret = false;
		if(c == GEM || c == '1' || c == '2' || c == '3' || c == '4'){
			ret = true;
		}
		return ret;
	}

	public int getGemRemain(){
		return m_gemCurrentNum;
	}

	public Vector3 GetSpawnPoint(SPAWN_POINT_TYPE type)
	{
		int t = (int)type;
		if (t >= m_spawnPositions.Length ) {
			Debug.LogWarning("Spawn Point is not found");
			return new Vector3((float)STAGE_ORIGIN_X,0,(float)STAGE_ORIGIN_Z);
		}

		return m_spawnPositions[t];
	}

	public Vector3 GetSpawnPoint(int type) 
	{
		return m_spawnPositions[type];
	}

	public void PickUpItem(Vector3 p)
	{
		int gx,gz;
		bool ret = PositionToIndex(p,out gx,out gz);

		if (ret) {
			int idx = m_stageData.gameParticleIndex[gz,gx];
			if (idx >= 0) {
				Particle[] gemParticle = m_gemEmitter.particles;
				gemParticle[idx].size = 0;
				m_gemEmitter.particles = gemParticle;
				m_stageData.gameParticleIndex[gz,gx] = -1;
				m_audio.playOneShot(m_gemPickupSe,1.0f,0);
				Score.AddScore(GEM_SCORE);
				m_gemCurrentNum--;
				if (m_gemCurrentNum <= 0)
					m_gameControl.OnEatAll();
			}
		}
	}

	public bool PositionToIndex(Vector3 pos,out int x,out int z)
	{
		x = Mathf.RoundToInt(pos.x);
		z = Mathf.RoundToInt(pos.z);

		// マップの位置になおす.
		x -= m_stageData.offset_x;
		z -= m_stageData.offset_z;
		// 範囲チェック.
		if (x < 0 || z < 0 || x >= m_stageData.width || z >= m_stageData.length)
			return false;
		return true;
	}
}



















