using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BaseCommand
{
    //Safeguard against infinite loops
    private const int MAX_ITERATIONS = 100000;
    
    //Used to allow type selection in the editor
    [HideInInspector] [SerializeField] private int m_concreteTypeIndex = -1;

    public int TypeIndex
    {
        get => m_concreteTypeIndex;
        set => m_concreteTypeIndex = value;
    }
    
    [SerializeField] private float m_duration = 1f;
    [SerializeField] protected AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public static List<Type> GetAllCommandTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BaseCommand))).ToList();
    }
    
    public virtual void Validate(GameObject gameObject) {}

    protected virtual void StartCommand() {}
    
    //Note: all the commands used here only need sum time but the delta is useful in many cases
    protected virtual void ProcessCommand(float t, float delta) {}

    private float m_currentTime = 0f;
    
#if UNITY_EDITOR
    private float EditorTimeDelta()
    {
        float delta = 0f;
        float newTime = (float) EditorApplication.timeSinceStartup;
        delta = newTime - m_currentTime;
        m_currentTime = newTime;
        return delta;
    }
#endif
    
#if UNITY_EDITOR
    private void InitializeEditorTimer()
    {
        m_currentTime = (float) EditorApplication.timeSinceStartup;
    }
#endif
    
    public async Task ActivateCommand()
    {
        m_cancel = false;
        float activeTime = 0f;
        Func<float> GetTimeDelta;
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            GetTimeDelta = () => Time.deltaTime;
        }
        else
        {
            InitializeEditorTimer();
            GetTimeDelta = EditorTimeDelta;
        }
#elif !UNITY_EDITOR
        GetTimeDelta = () => Time.deltaTime;
#endif

        StartCommand();
        for (int i = 0; i < MAX_ITERATIONS && activeTime < m_duration; i++)
        {
            float deltaTime = GetTimeDelta();
            activeTime += deltaTime;
            ProcessCommand(activeTime / m_duration, deltaTime);
            await Task.Yield();
            
            if (m_cancel)
            {
                m_cancel = false;
                break;
            }
        }
    }
    
    public void Cancel()
    {
        m_cancel = true;
    }

    private bool m_cancel;
}
