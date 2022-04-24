using Godot;
using System;

public class GUIStatus : NinePatchRect { 

    private GUI gui;
    private ShaderMaterial healthShader;
    private ShaderMaterial sanityShader;
    private ShaderMaterial chiShader;

    public override void _Ready() {
        gui = this.GetParent<GUI>();

        healthShader = (ShaderMaterial) GetNode<ColorRect>("HealthBar/HealthGraph").Material;
        sanityShader = (ShaderMaterial) GetNode<ColorRect>("SanityBar/SanityGraph").Material;
        chiShader = (ShaderMaterial) GetNode<ColorRect>("ChiBar/ChiGraph").Material;
    }

	public override void _Process(float delta) {
        float heartRate = 3f - ((float) gui.GetPlayerHealth() / (float) gui.GetPlayerHealthMax()) * 2f;
        SetHeartRate(heartRate);
        float sanityRate = 3f - ((float) gui.GetPlayerSanity() / (float) gui.GetPlayerSanityMax()) * 2f;
        SetSanityRate(sanityRate);
	}

    private void SetHeartRate(float rate) {
        rate = Mathf.Clamp(rate, 1f, 3f);
        healthShader.SetShaderParam("heartRate", rate);
    }

    private void SetSanityRate(float rate) {
        rate = Mathf.Clamp(rate, 1f, 3f);
        sanityShader.SetShaderParam("breathRate", rate);
    }
    
    private void SetChiRate(float rate) {
        rate = Mathf.Clamp(rate, 1f, 3f);
        chiShader.SetShaderParam("chiRate", rate);
    }
}