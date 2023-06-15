using UnityEngine;

public class ColorCommand : BaseCommand
{    
    [SerializeField] private Renderer m_renderer;
    [SerializeField] private Gradient m_gradient;
    
    public override void Validate(GameObject gameObject)
    {
        if (m_renderer == null)
        {
            m_renderer = gameObject.GetComponent<Renderer>();
        }
    }

    protected override void StartCommand() { }

    protected override void ProcessCommand(float t, float delta)
    {
        //Note: if used in editor time this will instantiate a material instance
        m_renderer.sharedMaterial.color = m_gradient.Evaluate(m_curve.Evaluate(t));
    }
}