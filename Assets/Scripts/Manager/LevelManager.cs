using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    private bool _isStageActive = false;        // 현재 씬이 Stage 씬인가

    [Header("스테이지 설정")]
    [SerializeField] private List<GameObject> stagePrefabs;       // 스테이지 프리팹 리스트
    [SerializeField] private Transform stageRoot;                 // 스테이지가 생성될 부모 오브젝트
    public int currentStageIndex = 0;                             // 스테이지 인덱스
    public int nodeCount;                                         // 스테이지별 노드 연결 최대치
    [SerializeField] private int checkIndex;                      // 스테이지 확인용 인덱스

    [Header("배경 설정")]
    [SerializeField] private Renderer backQuad;                   // 배경
    [SerializeField] private List<Material> backMats;             // 스테이지별 배경 마테리얼

    [Header("플레이어 인풋")]
    [SerializeField] private PlayerMovement player;

    private Camera _camera;
    private GameObject _currentStageObject;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage")
        {
            _isStageActive = true;

            FindReferences();

            LoadStage(PlayerPrefs.GetInt("SelectedStage"));
        }
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        if (scene.name == "Stage")
            _isStageActive = false;
    }

    private void FindReferences()
    {
        _camera = Camera.main;
        player = FindObjectOfType<PlayerMovement>();
        stageRoot = GameObject.Find("StageRoot")?.transform;

        backQuad = GameObject.Find("Quad")?.GetComponent<Renderer>();
    }

    public void LoadStage(int index)
    {
        if (!_isStageActive) return;
        if (index > stagePrefabs.Count) return;

        StartCoroutine(StageTransition(index));
    }

    public void ClearStage()
    {
        int currentCleared = PlayerPrefs.GetInt("MaxClearedStage", 0);

        // 현재 스테이지가 기존에 클리어한 곳보다 높다면 갱신
        if (currentStageIndex >= currentCleared)
        {
            PlayerPrefs.SetInt("MaxClearedStage", currentStageIndex + 1);
            PlayerPrefs.Save(); // 데이터 저장
        }
    }

    /// <summary>
    /// 스테이지 생성 코루틴
    /// </summary>
    /// <param name="index"> 생성하려는 스테이지 인덱스 </param>
    /// <returns></returns>
    private IEnumerator StageTransition(int index)
    {
        // 참조되지 않은 경우 다시 찾기
        if (player == null) player = FindObjectOfType<PlayerMovement>();
        if (_camera == null) _camera = Camera.main;
        if (stageRoot == null) stageRoot = GameObject.Find("StageRoot")?.transform;
        if (backQuad == null) backQuad = GameObject.Find("Quad")?.GetComponent<Renderer>();

        // 생성이 덜 된 경우 한 프레임 대기
        if (player == null) yield return null;

        _camera.orthographicSize = index + 9;

        // 스테이지가 null이 아니면 파괴
        if (_currentStageObject != null)
        {
            Destroy(_currentStageObject);
            yield return null;
        }

        if (index > 0) nodeCount = 3;
        else nodeCount = 2;

        // 새 스테이지 생성
        _currentStageObject = Instantiate(stagePrefabs[index], stageRoot);
        _currentStageObject.name = "Level" + (index + 1);

        // 플레이어 정보 갱신
        Node startNode = _currentStageObject.GetComponentInChildren<Node>();
        if (startNode != null)
        {
            player.transform.position = startNode.walkTarget;
            player.currentNode = startNode;
            player.transform.SetParent(startNode.transform.parent);
        }

        // 착시 매니저 노드 리스트 갱신
        IllusionManager.Instance.Init(50.0f, nodeCount);
        IllusionManager.Instance.NodeInit();
        // 전체 노드 이웃 설정
        FindObjectOfType<StageManager>().ScanAll();
        // 착시 매니저 경로 설정
        IllusionManager.Instance.UpdateIllusionPaths();

        backQuad.material = backMats[index];

        currentStageIndex = index;

        yield return null;

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.ShowTutorial();
    }
}
