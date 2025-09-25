using UnityEngine;
using System.Runtime.InteropServices;

public class GPUPlantsRenderer : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)]
    struct SegmentData
    {
        public Vector2 pos;
        public float rot;
        public float scale;
        public Vector4 uvRect;
        public uint plantId;
        public uint segId;
        public float z;
        public uint flags;
    }

    public Material renderMat;
    public Material idMat;
    public Mesh quadMesh;
    public Texture2D atlasTex;
    public Vector2Int atlasGrid = new Vector2Int(4, 4);

    public int plantCount = 500;
    public int maxSegments = 255;

    ComputeBuffer segmentsBuffer;
    GraphicsBuffer argsBuffer;
    SegmentData[] cpuSegments;
    Bounds worldBounds;

    RenderTexture idRT;
    const int ID_RT_SIZE = 512;

    Camera mainCam;

    void Start()
    {
        Application.targetFrameRate = 300;
        mainCam = Camera.main;

        // Dane CPU
        int total = 0;
        for (int i = 0; i < plantCount; i++)
            total += Random.Range(1, maxSegments + 1);

        cpuSegments = new SegmentData[total];
        int idx = 0;
        for (uint p = 0; p < plantCount; p++)
        {
            int segs = Random.Range(1, maxSegments + 1);
            for (uint s = 0; s < segs; s++)
            {
                cpuSegments[idx] = new SegmentData
                {
                    pos = new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f)),
                    rot = Random.Range(0f, Mathf.PI * 2f),
                    scale = Random.Range(0.5f, 1.5f),
                    uvRect = CalcUV(Random.Range(0, atlasGrid.x * atlasGrid.y)),
                    plantId = p,
                    segId = s,
                    z = 0f,
                    flags = 0
                };
                idx++;
            }
        }

        // Buffery
        int stride = Marshal.SizeOf(typeof(SegmentData));
        segmentsBuffer = new ComputeBuffer(cpuSegments.Length, stride, ComputeBufferType.Structured);
        segmentsBuffer.SetData(cpuSegments);

        uint[] args = new uint[5];
        args[0] = quadMesh.GetIndexCount(0);
        args[1] = (uint)cpuSegments.Length;
        args[2] = quadMesh.GetIndexStart(0);
        args[3] = quadMesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, sizeof(uint) * 5);
        argsBuffer.SetData(args);

        renderMat.SetBuffer("_Segments", segmentsBuffer);
        renderMat.SetTexture("_MainTex", atlasTex);
        idMat.SetBuffer("_Segments", segmentsBuffer);

        worldBounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        idRT = new RenderTexture(ID_RT_SIZE, ID_RT_SIZE, 0, RenderTextureFormat.ARGB32);
        idRT.Create();
    }

    Vector4 CalcUV(int index)
    {
        int x = index % atlasGrid.x;
        int y = index / atlasGrid.x;
        Vector2 cell = new Vector2(1f / atlasGrid.x, 1f / atlasGrid.y);
        return new Vector4(cell.x * x, cell.y * y, cell.x, cell.y);
    }

    void Update()
    {
        // Ustawiamy VP dla głównego passa
        if (mainCam != null)
        {
            Matrix4x4 VP = mainCam.projectionMatrix * mainCam.worldToCameraMatrix;
            renderMat.SetMatrix("_VP", VP);
        }

        // Rysowanie główne (ekran)
        Graphics.DrawMeshInstancedIndirect(quadMesh, 0, renderMat, worldBounds, argsBuffer);

        // Picking na klik
        if (Input.GetMouseButtonDown(0))
        {
            DoIdPickUnderMouse();
        }
    }

    void DoIdPickUnderMouse()
    {
        if (mainCam == null) return;

        // 1) Ustawiamy macierz VP dla ID passa
        Matrix4x4 VP = mainCam.projectionMatrix * mainCam.worldToCameraMatrix;
        idMat.SetMatrix("_VP", VP);

        // 2) Render do RT
        RenderTexture activeBefore = RenderTexture.active;
        RenderTexture.active = idRT;
        GL.Viewport(new Rect(0, 0, ID_RT_SIZE, ID_RT_SIZE));
        GL.Clear(true, true, Color.black);

        // Rysujemy TYLKO ID materiałem do aktywnego RT
        Graphics.DrawMeshInstancedIndirect(quadMesh, 0, idMat, worldBounds, argsBuffer);

        // 3) Odczyt piksela pod myszą
        Vector3 mouse = Input.mousePosition;
        int px = Mathf.Clamp(Mathf.RoundToInt(mouse.x * ID_RT_SIZE / Screen.width), 0, ID_RT_SIZE - 1);
        int py = Mathf.Clamp(Mathf.RoundToInt(mouse.y * ID_RT_SIZE / Screen.height), 0, ID_RT_SIZE - 1);

        Texture2D read = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        read.ReadPixels(new Rect(px, py, 1, 1), 0, 0);
        read.Apply();
        Color32 c = read.GetPixel(0, 0);
        Destroy(read);

        // Dekodowanie: R=seg+1, G=plant low+1, B=plant high
        uint segId = c.r == 0 ? uint.MaxValue : (uint)c.r - 1;

        ushort plantCode = (ushort)(c.g | (c.b << 8));
        uint plantId = plantCode == 0 ? uint.MaxValue : (uint)(plantCode - 1);

        if (segId != uint.MaxValue && plantId != uint.MaxValue)
            Debug.Log($"Clicked plant {plantId}, segment {segId}");
        else
            Debug.Log("No segment under cursor");

        // 4) Przywróć RT
        RenderTexture.active = activeBefore;
    }

    void OnDisable()
    {
        segmentsBuffer?.Dispose();
        argsBuffer?.Dispose();
        if (idRT != null) idRT.Release();
    }
}
