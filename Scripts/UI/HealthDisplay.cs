using Godot;
using System;

public partial class HealthDisplay : ColorRect
{
	[Export]private ShaderMaterial _ekgMaterial;

    public override void _Ready()
    {
        // Cache the shader material at startup
        if (Material is ShaderMaterial shaderMat)
        {
            _ekgMaterial = shaderMat;
        }
        else
        {
            GD.PrintErr("EkgMonitor requires a ShaderMaterial to be assigned to its Material property.");
        }
    }

    /// <summary>
    /// Call this method whenever the player's health changes.
    /// </summary>
   public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (_ekgMaterial == null) return;

        float healthPercent = Mathf.Clamp(currentHealth / maxHealth, 0.0f, 1.0f);

        Color targetColor;
        float targetSpeed;
        float targetFrequency;
        float targetAmplitude;

        if (healthPercent > 0.65f)
        {
            // FINE: Strong, healthy, normal speed
            targetColor = new Color(0.0f, 1.0f, 0.2f, 1.0f);
            targetSpeed = 1.5f; 
            targetFrequency = 1.5f;
            targetAmplitude = 0.6f; // Tall spikes
        }
        else if (healthPercent > 0.25f)
        {
            // CAUTION: Losing blood. Heart is slowing down and spikes are shrinking.
            targetColor = new Color(1.0f, 0.8f, 0.0f, 1.0f);
            targetSpeed = 1.0f;
            targetFrequency = 1.0f;
            targetAmplitude = 0.3f; // Half the height
        }
        else if (healthPercent > 0.0f)
        {
            // DANGER: Barely alive. Very slow, very weak pulse.
            targetColor = new Color(1.0f, 0.1f, 0.1f, 1.0f);
            targetSpeed = 0.5f;
            targetFrequency = 0.5f;
            targetAmplitude = 0.15f; // Almost flat
        }
        else
        {
            // DEAD: Flatline
            targetColor = new Color(0.5f, 0.0f, 0.0f, 0.6f);
            targetSpeed = 0.1f;
            targetFrequency = 0.0f;
            targetAmplitude = 0.0f; // Completely flat
        }

        _ekgMaterial.SetShaderParameter("line_color", targetColor);
        _ekgMaterial.SetShaderParameter("speed", targetSpeed);
        _ekgMaterial.SetShaderParameter("pulse_frequency", targetFrequency);
        _ekgMaterial.SetShaderParameter("amplitude", targetAmplitude);
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
