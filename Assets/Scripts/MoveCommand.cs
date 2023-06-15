using UnityEngine;

public class MoveCommand : BaseCommand
{
    [SerializeField] private Transform m_transform;
    [SerializeField] private Vector3 m_targetVector;
    [SerializeField] private bool m_relative;

    private Vector3 m_target;
    private Vector3 m_initialPosition;
    
    public override void Validate(GameObject gameObject)
    {
        if (m_transform == null)
        {
            m_transform = gameObject.transform;
        }
    }

    protected override void StartCommand()
    {
        m_initialPosition = m_transform.position;
        m_target = m_relative ? m_initialPosition + m_targetVector : m_targetVector;
    }

    protected override void ProcessCommand(float t, float delta)
    {
        m_transform.position = Vector3.Lerp(m_initialPosition, m_target, m_curve.Evaluate(t));
    }
}
