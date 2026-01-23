using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Managers
{
    // 매니저들의 부모 오브젝트
    private static GameObject _root;

    // 셀렉트 씬
    private static GameManager _game;
    private static StageManager _stage;

    // 스테이지 씬
    private static PoolManager _pool;           // 풀매니저
    private static IllusionManager _illusion;   // 착시매니저
    private static TutorialManager _tutorial;   // 튜토리얼 매니저

    private static void Init()
    {
        if (_root == null)
        {
            //빈 게임 오브젝트 생성(@Managers으로)
            _root = new GameObject("@Managers");
            Object.DontDestroyOnLoad(_root);
        }
    }
    private static void CreateManager<T>(ref T manager, string name) where T : Component
    {
        if (manager == null)
        {
            Init();

            GameObject obj = new GameObject(name);

            //T 타입 매니저 컴포넌트 추가
            manager = obj.AddComponent<T>();

            Object.DontDestroyOnLoad(obj);

            //@Managers 밑으로 붙여서 계층 정리
            obj.transform.SetParent(_root.transform);
        }
    }

    // 게임 매니저 접근자
    public static GameManager game
    {
        get
        {
            CreateManager(ref _game, "GameManager");
            return _game;
        }
    }
    // 스테이지 매니저 접근자
    public static StageManager stage
    {
        get
        {
            CreateManager(ref _stage, "StageManager");
            return _stage;
        }
    }    
    // 풀 매니저 접근자
    public static PoolManager Pool
    {
        get
        {
            CreateManager(ref _pool, "PoolManager");
            return _pool;
        }
    }
    // 착시 매니저 접근자
    public static IllusionManager Illusion
    {
        get
        {
            CreateManager(ref _illusion, "IllusionManager");
            return _illusion;
        }
    }
    // 튜토리얼 매니저 접근자
    public static TutorialManager Tutorial
    {
        get
        {
            CreateManager(ref _tutorial, "TutorialManager");
            return _tutorial;
        }
    }
}
