using UnityEngine;
using UnityEditor;

public static class CreateDustEffectPrefab
{
    [MenuItem("Tools/Create Dust Effect Prefab")]
    public static void Create()
    {
        var root = new GameObject("DustEffect");

        // --- Main Particle System (ground burst) ---
        var ps = root.AddComponent<ParticleSystem>();
        var renderer = root.GetComponent<ParticleSystemRenderer>();

        // Main
        var main = ps.main;
        main.duration = 0.6f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.65f, 0.50f, 0.30f, 1f),
            new Color(0.80f, 0.70f, 0.55f, 0.8f)
        );
        main.gravityModifier = 0.4f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // Emission: single burst
        var emission = ps.emission;
        emission.enabled = true;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 25, 35) });
        emission.rateOverTime = 0;

        // Shape: hemisphere flat on the ground
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.3f;
        shape.rotation = new Vector3(-90f, 0f, 0f); // flat against ground

        // Velocity over lifetime: spread outward
        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.Local;
        var radialCurve = new ParticleSystem.MinMaxCurve(1.5f);
        vel.radial = radialCurve;

        // Color over lifetime: fade out
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = new ParticleSystem.MinMaxGradient(gradient);

        // Size over lifetime: start big, shrink
        var sizeLt = ps.sizeOverLifetime;
        sizeLt.enabled = true;
        var sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.3f, 1.3f),
            new Keyframe(1f, 0f)
        );
        sizeLt.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Renderer: default material (Particles/Standard Unlit)
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");

        // --- Save as prefab ---
        const string path = "Assets/03 Prefabs/DustEffect.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);

        AssetDatabase.Refresh();
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);

        Debug.Log($"DustEffect prefab created at: {path}");
    }
}
