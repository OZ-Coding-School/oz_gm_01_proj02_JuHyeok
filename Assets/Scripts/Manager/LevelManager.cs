using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
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

    private GameObject _currentStageObject;

    private void Start()
    {
        LoadStage(checkIndex);
    }

    public void LoadStage(int index)
    {
        if (index > stagePrefabs.Count) return;

        StartCoroutine(StageTransition(index));
    }

    private IEnumerator StageTransition(int index)
    {
        player.transform.SetParent(null);

        // 스테이지가 null이 아니면 파괴
        if (_currentStageObject != null)
        {
            Destroy(_currentStageObject);
            yield return null;
        }

        if (index > 0)  nodeCount = 3;
        else            nodeCount = 2;

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
        FindObjectOfType<IllusionManager>().UpdateIllusionPaths();

        backQuad.material = backMats[index];

        currentStageIndex = index;
        Debug.Log($"스테이지{index + 1} 생성");

        yield return null;
    }
}
