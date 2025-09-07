using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class VoronoiSplitBackup : MonoBehaviour
{
    private struct RenderProperties
    {
        public readonly int Width;
        public readonly int Height;
        public readonly float AspectRatio;
        public float OrthoSize;

        public RenderProperties(int width, int height, float orthoSize = 0)
        {
            Width = width;
            Height = height;
            OrthoSize = orthoSize;
            AspectRatio = (float)Width / Height;
        }
    }

    #region Constants

    private readonly string[] SHADER_PLAYER_POSITION = new[]
    {
        "_Player1Pos",
        "_Player2Pos"
    };

    private readonly string SHADER_PLAYER = "_Player";
    private readonly string SHADER_LINE_COLOR = "_LineColor";
    private readonly string SHADER_LINE_THICKNESS = "_LineThickness";
    private readonly string SHADER_CELLS_STENCIL_OP = "_VoronoiCellsStencilOp";
    private readonly string SHADER_CELLS_STENCIL_PLAYER = "_VoronoiCellsPlayerStencil";
    private readonly string SHADER_CELLS_STENCIL_TEX = "_VornoiTex";
    private readonly string SHADER_MASKED_STENCIL_OP = "_MaskedStencilOp";
    private readonly string SHADER_BLEND_TEXTURE = "_SecondaryTex";
    
    private const int MAX_PLAYERS = 2;

    #endregion

    private float modeBlend = 0f; // 0 = WorldDistance, 1 = ScreenRelative
    public float modeSmooth = 2f; // Velocidad de transición

    #region Public Variables

    [Header("Camera Zoom")]
    public float baseOrthoSize = 1.25f;
    public float maxOrthoSize = 3f;
    public float zoomSmooth = 5f;

    public enum SplitMode { ScreenRelative, WorldDistance }
    public SplitMode splitMode = SplitMode.ScreenRelative;

    // Screen-relative
    [Tooltip("Factor >1 retrasa el split (permite más separación antes de dividir).")]
    public float splitDistanceMultiplier = 1.5f;
    [Tooltip("Ancho de transición en 'ratio' (1 = igual que el original).")]
    public float screenSmoothing = 1f;

    // World-space
    [Tooltip("Distancia en unidades del mundo para empezar a dividir pantallas.")]
    public float worldSplitDistance = 20f;
    [Tooltip("Ancho de transición adicional en unidades del mundo.")]
    public float worldSmoothing = 5f;

    [Header("References")]
    public Camera MainCamera;
    public Camera PlayerCamera;
    public Renderer MaskRenderer;
    public Transform MaskTransform;

    public Material VoronoiCellsMaterial;
    public Material SplitLineMaterial;
    public Material AlphaBlendMaterial;
    public Material FxaaMaterial;

    [Header("Graphics")]
    public Color LineColor = Color.black;
    public bool EnableFXAA = true;

    [Header("Players")]
    public int PlayerCount = 2;
    public Transform[] Players;
    public bool EnableMerging = true;
    
    #endregion

    #region Variables

    private Color lastLineColor = Color.black;
    
    private RenderProperties screen;
    private RenderTexture playerTex;
    private RenderTexture cellsTexture;

    private Vector2[] worldPositions = new Vector2[MAX_PLAYERS];
    private Vector2[] normalizedPositions = new Vector2[MAX_PLAYERS];
    private Vector2 mergedPosition = Vector2.one / 2;
    private float mergeRatio = 1f;
    private int activePlayers = 2;

    #endregion

    private void Awake()
    {
        VoronoiCellsMaterial = Instantiate(VoronoiCellsMaterial);
        SplitLineMaterial = Instantiate(SplitLineMaterial);
        AlphaBlendMaterial = Instantiate(AlphaBlendMaterial);
        FxaaMaterial = Instantiate(FxaaMaterial);

        MaskRenderer.sharedMaterial = VoronoiCellsMaterial;

        InitializeCameras();
        SetLineColor(LineColor);
    }

    private void InitializeCameras()
    {
        PlayerCamera.depthTextureMode = DepthTextureMode.Depth;
        UpdateRenderProperties();
    }

    private void UpdateRenderProperties()
    {
        OnResolutionChanged(Screen.width, Screen.height);
        OnOrthoSizeChanged(MainCamera.orthographicSize);
    }

    private void OnResolutionChanged(int width, int height)
    {
        if (screen.Width == width && screen.Height == height)
            return;

        playerTex?.Release();
        cellsTexture?.Release();

        playerTex = new RenderTexture(width, height, 32) { name = "Player Render" };
        PlayerCamera.targetTexture = playerTex;

        cellsTexture = new RenderTexture(width, height, 0, GraphicsFormat.R8_UNorm) { name = "Cells Visualization Texture" };

        SplitLineMaterial.SetTexture(SHADER_CELLS_STENCIL_TEX, cellsTexture);
        SplitLineMaterial.SetFloat(SHADER_LINE_THICKNESS, (float)height / 200);

        screen = new RenderProperties(width, height);
    }

    private void OnOrthoSizeChanged(float orthoSize)
    {
        if (Mathf.Abs(screen.OrthoSize - orthoSize) < Mathf.Epsilon)
            return;

        PlayerCamera.orthographicSize = orthoSize;
        MaskTransform.localScale = new Vector3(orthoSize * screen.AspectRatio * 2, orthoSize * 2);
        screen.OrthoSize = orthoSize;
    }

    private void SetLineColor(Color color)
    {
        SplitLineMaterial.SetColor(SHADER_LINE_COLOR, LineColor);
        lastLineColor = LineColor;
    }

    private void Update()
    {
        if (Players.Length < PlayerCount)
        {
            GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");
            Players = new Transform[foundPlayers.Length];
            for (int i = 0; i < foundPlayers.Length; i++)
                Players[i] = foundPlayers[i].transform;
        }

        if (PlayerCount > MAX_PLAYERS)
        {
            PlayerCount = MAX_PLAYERS;
            Debug.LogWarning($"Voronoi split screen soporta como máximo {MAX_PLAYERS} players.");
        }

        // posiciones
        for (int i = 0; i < Players.Length && i < MAX_PLAYERS; i++)
            worldPositions[i] = Players[i].position;

        // single player
        if (PlayerCount <= 1)
        {
            normalizedPositions[0] = Vector3.one / 2;
            mergeRatio = 0;
            activePlayers = 1;
            RenderPlayers(activePlayers);
            return;
        }

        // zoom dinámico
        if (Players.Length >= 2)
        {
            float distance = Vector2.Distance(worldPositions[0], worldPositions[1]);
            float t = Mathf.InverseLerp(0f, 20f, distance);
            float targetSize = Mathf.Lerp(baseOrthoSize, maxOrthoSize, t);
            float newSize = Mathf.Lerp(MainCamera.orthographicSize, targetSize, Time.deltaTime * zoomSmooth);

            MainCamera.orthographicSize = newSize;
            OnOrthoSizeChanged(newSize);
        }

        UpdateRenderProperties();
        if (lastLineColor != LineColor) SetLineColor(LineColor);

        // normalización
        Vector2 min = normalizedPositions[0] = worldPositions[0];
        Vector2 max = normalizedPositions[0];
        for (int i = 1; i < MAX_PLAYERS; i++)
        {
            normalizedPositions[i] = worldPositions[i];
            min = Vector2.Min(min, normalizedPositions[i]);
            max = Vector2.Max(max, normalizedPositions[i]);
        }

        max -= min;
        for (int i = 0; i < MAX_PLAYERS; i++) normalizedPositions[i] -= min;

        Vector2 diff = Vector2.zero;
        if (max.x > max.y * screen.AspectRatio)
            diff.y = ((max.x / screen.AspectRatio) - max.y) / 2;
        else if (max.y > max.x * 1 / screen.AspectRatio)
            diff.x = ((max.y * screen.AspectRatio) - max.x) / 2;

        for (int i = 0; i < MAX_PLAYERS; i++) normalizedPositions[i] += diff;
        max += diff * 2;

        for (int i = 0; i < MAX_PLAYERS; i++) normalizedPositions[i] /= max;

        // merging
        activePlayers = 2;
        mergeRatio = 0f;
        if (EnableMerging)
        {
            var diffNorm = normalizedPositions[1] - normalizedPositions[0];
            var realDiff = worldPositions[1] - worldPositions[0];
            mergedPosition = Vector2.Lerp(worldPositions[0], worldPositions[1], 0.5f);

            float realDist = realDiff.magnitude;

            // ----- Blend de modos -----
            float targetMode = (realDist > worldSplitDistance + worldSmoothing) ? 1f : 0f;
            modeBlend = Mathf.Lerp(modeBlend, targetMode, Time.deltaTime * modeSmooth);

            // mergeRatio se calcula como siempre (WorldDistance style)
            float threshold = Mathf.Max(0f, worldSplitDistance);
            float smooth = Mathf.Max(0f, worldSmoothing);

            if (realDist <= threshold + smooth)
            {
                if (realDist <= threshold)
                {
                    mergeRatio = 1f;
                    activePlayers = 1;
                }
                else
                {
                    mergeRatio = Mathf.InverseLerp(threshold + smooth, threshold, realDist);
                }
            }
        }

        RenderPlayers(activePlayers);
    }

    private void RenderPlayers(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            var pivot = normalizedPositions[i];
            VoronoiCellsMaterial.SetVector(SHADER_PLAYER_POSITION[i], new Vector4(pivot.x, pivot.y, 0.0f, 1.0f));
        }

        for (int i = playerCount; i < MAX_PLAYERS; i++)
            VoronoiCellsMaterial.SetVector(SHADER_PLAYER_POSITION[i], Vector4.zero);

        VoronoiCellsMaterial.SetInt(SHADER_CELLS_STENCIL_OP, (int)CompareFunction.Always);
        Shader.SetGlobalInt(SHADER_MASKED_STENCIL_OP, (int)CompareFunction.Equal);
        MaskRenderer.enabled = true;

        RenderTexture.active = playerTex;
        GL.Clear(true, false, Color.black);
        RenderTexture.active = null;

        for (int i = 0; i < playerCount; i++)
        {
            Shader.SetGlobalInt(SHADER_CELLS_STENCIL_PLAYER, i + 1);

            // --- WorldDistance target ---
            Vector3 targetWorld = Vector3.Lerp(worldPositions[i], mergedPosition, mergeRatio);

            // --- ScreenRelative target ---
            Vector2 center = Vector2.one / 2;
            Vector2 offset = (center - normalizedPositions[i]) * screen.OrthoSize * new Vector2(screen.AspectRatio, 1);
            Vector3 targetScreen = Vector2.Lerp(worldPositions[i] + offset, mergedPosition, mergeRatio);

            // --- Blend entre ambos ---
            Vector3 blendedTarget = Vector3.Lerp(targetWorld, targetScreen, modeBlend);

            transform.localPosition = blendedTarget;

            VoronoiCellsMaterial.SetInt(SHADER_PLAYER, i + 1);
            PlayerCamera.Render();
        }


        MaskRenderer.enabled = false;
        Shader.SetGlobalInt(SHADER_MASKED_STENCIL_OP, (int)CompareFunction.Disabled);
        VoronoiCellsMaterial.SetInt(SHADER_CELLS_STENCIL_OP, (int)CompareFunction.Disabled);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        var screenTex = RenderTexture.GetTemporary(screen.Width, screen.Height);
        var fxaaTex = EnableFXAA ? RenderTexture.GetTemporary(screen.Width, screen.Height) : null;

        Graphics.Blit(null, cellsTexture, VoronoiCellsMaterial);
        Graphics.Blit(playerTex, screenTex, SplitLineMaterial);
        if (EnableFXAA) Graphics.Blit(screenTex, fxaaTex, FxaaMaterial);
        AlphaBlendMaterial.SetTexture(SHADER_BLEND_TEXTURE, EnableFXAA ? fxaaTex : screenTex);
        Graphics.Blit(src, dst, AlphaBlendMaterial);

        if (EnableFXAA) RenderTexture.ReleaseTemporary(fxaaTex);
        RenderTexture.ReleaseTemporary(screenTex);
    }
}
