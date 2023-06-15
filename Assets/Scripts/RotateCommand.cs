using UnityEngine;

public class RotateCommand : BaseCommand
{    
    [SerializeField] private Transform m_transform;
    [SerializeField] private Vector3 m_targetEular;
    [SerializeField] private bool m_relative;

    private Quaternion m_target;
    private Quaternion m_initialRotation;
    
    public override void Validate(GameObject gameObject)
    {
        if (m_transform == null)
        {
            m_transform = gameObject.transform;
        }
    }

    protected override void StartCommand()
    {
        m_initialRotation = m_transform.rotation;
        m_target = m_relative ? m_initialRotation * Quaternion.Euler(m_targetEular) : Quaternion.Euler(m_targetEular);
    }

    protected override void ProcessCommand(float t, float delta)
    {
        m_transform.rotation = Quaternion.Slerp(m_initialRotation, m_target, m_curve.Evaluate(t));
    }
}
