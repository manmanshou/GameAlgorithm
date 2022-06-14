using System;
using System.Collections.Generic;
using System.Diagnostics;
using conf;
using UnityEngine;
using UnityEngine.Rendering;
using Wod;
using Wod.ThirdParty.Util;
using Wod.Util;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class TerritoryBlock
{
    public ulong Id;            //领土所属
    public uint Color;        //领土颜色
    public bool IsBuildFinish; //是否还在建造中
    public bool Valid;     //是否有效
}

internal class LodInfo
{
    public GameObject Go; //游戏物体
    public Material Mat; //材质
    public readonly Mesh Mesh; //Mesh
    public float FadeOutStart; //相机大于这个高度时开始消失
    public float FadeOutEnd; //相机大于这个高度时将显示这一层完全消失（隐藏）
    public float FadeInStart; //相机大于这个高度时开始淡入
    public float FadeInEnd;   //相机大于等于这个高度时淡入结束

    public LodInfo()
    {
        Go = null;
        Mesh = new Mesh {indexFormat = IndexFormat.UInt32};
    }
}
//纹理坐标类型，9宫格，其中4个角包含内接、外接
internal enum TextureType
{
    LeftSide = 0,
    RightSide,
    UpSide,
    DownSide,
    LeftUpOutCorner,
    RightUpOutCorner,
    LeftDownOutCorner,
    RightDownOutCorner,
    LeftUpInCorner,
    RightUpInCorner,
    LeftDownInCorner,
    RightDownInCorner,
    Center,
}
//领土矩形类型，一共9宫格
internal enum RectType
{
    LeftSide = 0,
    RightSide,
    UpSide,
    DownSide,
    LeftUpCorner,
    RightUpCorner,
    LeftDownCorner,
    RightDownCorner,
    Center,
}

public class TerritoryManager
{
    public static TerritoryManager Instance;
    
    private TerritoryBlock[] _blocks; //块数据
    private readonly LodInfo[] _lodArray = new[] { new LodInfo(), new LodInfo()};
    private readonly int _propIdAlpha;
    private readonly int _propIdColor;
    //配置参数
    private readonly int _maxBlockX; //地图横向块数量
    private readonly int _maxBlockZ; //地图纵向块数量
    private readonly float _blockSize;        //领土最小区域边长
    private readonly float _blockMesh0EdgeSize;  //精度0情况下每块边缘显示范围宽度
    private readonly float _meshYPos; //整个网格的Y坐标
    
    //优化
    private bool _dirty; //脏标记 
    private float _lastCameraPosY; //上一次相机的高度
    private bool _runningRefresh; //是否正在刷新网格
    private bool _waitRefresh; //是否等待刷新
    private Rect _visRect; //当前相机能看到的地面范围
    private readonly Stopwatch _sw = new Stopwatch(); //上一次刷新网格结束时开启的计时器

    private const string TerritoryHolderName = "TerritoryHolder";
    
    //根据纹理类型设置的纹理坐标
    private readonly List<List<Vector2>> _textureTypeCoordinate = new List<List<Vector2>>
    {
        //LeftSide
        new List<Vector2>{ new Vector2(0.25f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 1f), new Vector2(0.25f, 1f) },
        //RightSide
        new List<Vector2>{ new Vector2(0f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 1f), new Vector2(0f, 1f) },
        //UpSide
        new List<Vector2>{ new Vector2(0f, 1f), new Vector2(0f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 1f) },
        //DownSide
        new List<Vector2>{ new Vector2(0.25f, 0.5f), new Vector2(0.25f, 1f), new Vector2(0f, 1f), new Vector2(0f, 0.5f) },        
        //LeftUpOutCorner
        new List<Vector2>{ new Vector2(0f, 0f), new Vector2(0f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 0f) },        
        //RightUpOutCorner
        new List<Vector2>{ new Vector2(0f, 0.5f), new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0.5f) },
        //LeftDownOutCorner
        new List<Vector2>{ new Vector2(0.25f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0.5f), new Vector2(0.25f, 0.5f) },
        //RightDownOutCorner
        new List<Vector2>{ new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(0.25f, 0.5f), new Vector2(0f, 0.5f) },
        //LeftUpInCorner
        new List<Vector2>{ new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 1f), new Vector2(0.5f, 1f) },
        //RightUpInCorner
        new List<Vector2>{ new Vector2(0.25f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1f), new Vector2(0.25f, 1f) },
        //LeftDownInCorner
        new List<Vector2>{ new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f), new Vector2(0.25f, 1f) },
        //RightDownInCorner
        new List<Vector2>{ new Vector2(0.25f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f) },
        //Center
        new List<Vector2>{ new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0.25f, 0.5f) },
    }; 
    //建设建筑时预览静态范围
    private readonly Dictionary<int, GameObject> _staticPreviewRange = new Dictionary<int, GameObject>();
    private const int RectVertCount = 4; //一个矩形4个顶点
    private static readonly Vector2 Offset = new Vector2(0.5f, 0);

    public TerritoryManager(int maxBlockX, int maxBlockY, 
        float blockSize, float mesh0EdgeSize, float meshY, float lod0Dis)
    {
        Instance = this;
        
        _maxBlockX = maxBlockX;
        _maxBlockZ = maxBlockY;
        _blockSize = blockSize;
        _blockMesh0EdgeSize = mesh0EdgeSize;
        _meshYPos = meshY;
        
        _propIdAlpha = Shader.PropertyToID("_Alpha");
        _propIdColor = Shader.PropertyToID("_Color");
        
        LoggerHelper.DebugFormat("max block x: {0}, max block y: {1} \n block size: {2} BlockMesh0EdgeSize: {3} MeshYPos: {4}", 
            _maxBlockX, _maxBlockZ, _blockSize, _blockMesh0EdgeSize, _meshYPos);
        
        _lodArray[0].FadeInStart = 0;
        _lodArray[0].FadeInEnd = 0;
        _lodArray[0].FadeOutStart = lod0Dis;
        _lodArray[0].FadeOutEnd = lod0Dis + 10;
        
        _lodArray[1].FadeInStart = lod0Dis;
        _lodArray[1].FadeInEnd = lod0Dis + 10;
        _lodArray[1].FadeOutStart = 2000;
        _lodArray[1].FadeOutEnd = 2000;
        
        _sw.Start();
        _blocks = new TerritoryBlock[_maxBlockX * _maxBlockZ];
        for (var i=0; i < _blocks.Length; i++)
        {
            _blocks[i] = new TerritoryBlock();
        }
    }
    private void CreateLodSceneObject()
    {
        var loadCount = 0;
        for (var i = 0; i < _lodArray.Length; i++)
        {
            var path = "TerritoryLOD_" + i;
            var lod = _lodArray[i];
            ResourceManager.Instance.Instantiate(path, (obj, isSuccess) =>
            {
                if (!isSuccess)
                {
                    LoggerHelper.Error("can not load territory object from " + path);
                    return;
                }

                lod.Go = (GameObject) obj;
                lod.Go.SetParent(KingdomManager.Instance.GetHolderWithName(TerritoryHolderName).gameObject, false);
                var filter = lod.Go.GetComponent<MeshFilter>();
                filter.mesh = lod.Mesh;
                //lod.Mat = lod.Go.GetComponent<MeshRenderer>().material;
                
                loadCount++;
                if (loadCount >= _lodArray.Length)
                {
                    if (Camera.main != null)
                    {
                        _lastCameraPosY = Camera.main.transform.position.y;
                        CheckLodByCamera(true);
                    }
                }
            });
        }
    }

    private void CleanLodSceneObject()
    {
        foreach (var t in _lodArray)
        {
            Object.Destroy(t.Go);
            t.Go = null;
        }
    }
    
    private Mesh CreatePreviewMesh(int width, int height)
    {
        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var colors = new List<Color32>();
        var defaultColor = new Color32();
        var indices = new List<int>();
        var vertIdx = 0;
        
        //左下角
        var startX = 0f;
        var startZ = 0f;
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(startX, _meshYPos, startZ)); 
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);   
        FillTexUvAndColor(vertIdx, uvs, TextureType.LeftDownOutCorner, colors, defaultColor);
        vertIdx += RectVertCount;
        
        //右下角
        startX = (width - 1) * _blockSize;
        startZ = 0;
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.RightDownOutCorner, colors, defaultColor);
        vertIdx += RectVertCount;
        
        //左上角
        startX = 0f;
        startZ = (height - 1) * _blockSize;
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.LeftUpOutCorner, colors, defaultColor);
        vertIdx += RectVertCount;
        
        //右上角
        startX = (width - 1) * _blockSize;
        startZ = (height - 1) * _blockSize;
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.RightUpOutCorner, colors, defaultColor);
        vertIdx += RectVertCount;
        
        //下边
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, 0));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, 0));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.DownSide, colors, defaultColor);
        vertIdx += RectVertCount;
        //上边
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, height*_blockSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, height*_blockSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.UpSide, colors, defaultColor);
        vertIdx += RectVertCount;
        //左边
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(0, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(0, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.LeftSide, colors, defaultColor);
        vertIdx += RectVertCount;        
        //右边
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.RightSide, colors, defaultColor);
        vertIdx += RectVertCount;
        //中间
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, _blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,0)); colors.Add(defaultColor);
        vertices.Add(new Vector3(width*_blockSize-_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(1,1)); colors.Add(defaultColor);
        vertices.Add(new Vector3(_blockMesh0EdgeSize, _meshYPos, height*_blockSize-_blockMesh0EdgeSize));
        uvs.Add(new Vector2(0,1)); colors.Add(defaultColor);
        FillTexUvAndColor(vertIdx, uvs, TextureType.Center, colors, defaultColor);
        
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        return mesh;
    }
    
    //初始化范围预览
    private void CreateStaticRangeSceneObject()
    {
        _staticPreviewRange.Clear();
        var path = "TerritoryPreview";
        ResourceManager.Instance.LoadAsset<GameObject>(path, (obj, isSuccess) =>
        {
            if (!isSuccess)
            {
                LoggerHelper.Error("can not load " + path);
                return;
            }
            
            //遍历建筑配置文件，为每类建筑创建一个游戏对象，以便于显示
            foreach (var id in Tables.AllianceBuildingConf.Ids)
            {
                var range = Tables.AllianceBuildingConf[id].TerritorySide;
                if (_staticPreviewRange.ContainsKey(range))
                    continue;
                var go = Object.Instantiate(obj);
                go.SetParent(KingdomManager.Instance.GetHolderWithName(TerritoryHolderName).gameObject, false);
                go.name = "TerritoryPreview_" + range;
                var filter = go.GetComponent<MeshFilter>();
                filter.mesh = CreatePreviewMesh(range, range);
                _staticPreviewRange.Add(range, go);
                UGUIUtil.SetActiveSafe(go,  false);
            }
        });
    }
    
    //清理范围预览
    private void CleanPreviewRangeSceneObject()
    {
        foreach (var v in _staticPreviewRange)
        {
            var go = v.Value;
            Object.Destroy(go);
        }
        _staticPreviewRange.Clear();
    }
    
    private void FillTexUvAndColor(int vertIdx, List<Vector2> uvs, TextureType type, List<Color32> colors, Color32 color)
    {
        for (var i = 0; i < 4; i++)
        {
            uvs[vertIdx + i] = _textureTypeCoordinate[(int) type][i] + Offset;
            colors[vertIdx + i] = color;
        }
    }
    //建立一个矩形的网格
    private void InitRectMesh(
        RectType rectType,
        TextureType texType,
        float startX,
        float startZ,
        List<Vector3> vertices,
        List<Vector2> uvs, 
        List<Color32> colors, 
        Color32 color,
        List<int> indices, 
        bool isBuildFinish)
    {
        var vertIdx = vertices.Count;
        switch (rectType)
        {
            case RectType.LeftSide:
                vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                break;
            case RectType.RightSide:
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                break;
            case RectType.UpSide:
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
                break;
            case RectType.DownSide:
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                break;
            case RectType.LeftUpCorner:
                vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
                vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockSize));
                break;
            case RectType.RightUpCorner:
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize));
                break;
            case RectType.LeftDownCorner:
                vertices.Add(new Vector3(startX, _meshYPos, startZ)); 
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX, _meshYPos, startZ+_blockMesh0EdgeSize));
                break;
            case RectType.RightDownCorner:
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ));
                vertices.Add(new Vector3(startX+_blockSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                break;
            case RectType.Center:
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockSize-_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));
                vertices.Add(new Vector3(startX+_blockMesh0EdgeSize, _meshYPos, startZ+_blockSize-_blockMesh0EdgeSize));                
                break;
            default:
                return;
        }        
        
        indices.Add(vertIdx + 0);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 1);
        indices.Add(vertIdx + 3);
        indices.Add(vertIdx + 2);
        indices.Add(vertIdx + 1);
        for (var i = 0; i < 4; i++)
        {
            if (isBuildFinish)
                uvs.Add(_textureTypeCoordinate[(int) texType][i]); //实线
            else
                uvs.Add(_textureTypeCoordinate[(int) texType][i] + Offset); //虚线
            colors.Add(color);
        }
    }
    //构建领土网格
    private void RefreshMeshEdged(int lodIdx, bool isSolid)
    {
        bool HaveEdge(TerritoryBlock side, TerritoryBlock block)
        {
            return (side == null || side.Id != block.Id || side.IsBuildFinish != block.IsBuildFinish ||
                    side.Valid != block.Valid);
        }
        
        var lod = _lodArray[lodIdx];
        var mesh = lod.Mesh;
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();
        var colors = new List<Color32>();
        var startBlockX = (int)(_visRect.xMin / _blockSize) - 1;
        var startBlockY = (int)(_visRect.yMin / _blockSize) - 1;
        var endBlockX = (int)(_visRect.xMax / _blockSize) + 1;
        var endBlockY = (int)(_visRect.yMax / _blockSize) + 1;
        for (var y = startBlockY; y < endBlockY; y++)
        {
            if (y < 0 || y >= _maxBlockZ)
                continue;
            
            for (var x = startBlockX; x < endBlockX; x++)
            {
                if (x < 0 || x >= _maxBlockX)
                    continue;
                
                var idx = y * _maxBlockX + x;
                var block = _blocks[idx];
                if (block.Id == 0)
                    continue; //未占领的区域不用处理
                
                var red = (byte)(block.Color & 0xFF);
                var green = (byte)((block.Color & 0xFF00) >> 8);
                var blue = (byte)((block.Color & 0xFF0000) >> 16);
                var color = new Color32(red, green, blue, 255);
                var left = GetBlock(x - 1, y);
                var right = GetBlock(x + 1, y);
                var up = GetBlock(x, y + 1);
                var down = GetBlock(x, y - 1);
                var hasLeftEdge = HaveEdge(left, block);
                var hasRightEdge = HaveEdge(right, block);
                var hasUpEdge = HaveEdge(up, block);
                var hasDownEdge = HaveEdge(down, block);
                var startX = x * _blockSize;
                var startZ = y * _blockSize;
                if (hasLeftEdge) //左边
                    InitRectMesh(RectType.LeftSide, TextureType.LeftSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                else if (isSolid)
                    InitRectMesh(RectType.LeftSide, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                
                if (hasRightEdge) //右边
                    InitRectMesh(RectType.RightSide, TextureType.RightSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                else if (isSolid)
                    InitRectMesh(RectType.RightSide, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);

                if (hasUpEdge) //上边
                    InitRectMesh(RectType.UpSide, TextureType.UpSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                else if (isSolid)
                    InitRectMesh(RectType.UpSide, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                
                if (hasDownEdge) //下边
                    InitRectMesh(RectType.DownSide, TextureType.DownSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                else if (isSolid)
                    InitRectMesh(RectType.DownSide, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);

                if (hasLeftEdge || hasUpEdge) //左上角
                {
                    if (hasLeftEdge && !hasUpEdge)
                        InitRectMesh(RectType.LeftUpCorner, TextureType.LeftSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (!hasLeftEdge)
                        InitRectMesh(RectType.LeftUpCorner, TextureType.UpSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else
                        InitRectMesh(RectType.LeftUpCorner, TextureType.LeftUpOutCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                else
                {
                    var leftUp = GetBlock(x - 1, y + 1);
                    if (HaveEdge(leftUp, block))
                        InitRectMesh(RectType.LeftUpCorner, TextureType.LeftUpInCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (isSolid)
                        InitRectMesh(RectType.LeftUpCorner, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                
                if (hasRightEdge || hasUpEdge) //右上角
                {
                    if (hasRightEdge && !hasUpEdge)
                        InitRectMesh(RectType.RightUpCorner, TextureType.RightSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (!hasRightEdge)
                        InitRectMesh(RectType.RightUpCorner, TextureType.UpSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else
                        InitRectMesh(RectType.RightUpCorner, TextureType.RightUpOutCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                else
                {
                    var rightUp = GetBlock(x + 1, y + 1);
                    if (HaveEdge(rightUp, block))                    
                        InitRectMesh(RectType.RightUpCorner, TextureType.RightUpInCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (isSolid)
                        InitRectMesh(RectType.RightUpCorner, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                
                if (hasLeftEdge || hasDownEdge) //左下角
                {
                    if (hasLeftEdge && !hasDownEdge)
                        InitRectMesh(RectType.LeftDownCorner, TextureType.LeftSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (!hasLeftEdge)
                        InitRectMesh(RectType.LeftDownCorner, TextureType.DownSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else
                        InitRectMesh(RectType.LeftDownCorner, TextureType.LeftDownOutCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);                    
                }
                else
                {
                    var leftDown = GetBlock(x - 1, y - 1);
                    if (HaveEdge(leftDown, block))             
                        InitRectMesh(RectType.LeftDownCorner, TextureType.LeftDownInCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);                    
                    else if (isSolid)
                        InitRectMesh(RectType.LeftDownCorner, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);                    
                }
                
                if (hasRightEdge || hasDownEdge) //右下角
                {
                    if (hasRightEdge && !hasDownEdge)
                        InitRectMesh(RectType.RightDownCorner, TextureType.RightSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (!hasRightEdge)
                        InitRectMesh(RectType.RightDownCorner, TextureType.DownSide, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else
                        InitRectMesh(RectType.RightDownCorner, TextureType.RightDownOutCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                else
                {
                    var rightDown = GetBlock(x + 1, y - 1);
                    if (HaveEdge(rightDown, block))               
                        InitRectMesh(RectType.RightDownCorner, TextureType.RightDownInCorner, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                    else if (isSolid)
                        InitRectMesh(RectType.RightDownCorner, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
                //中间
                if (isSolid)
                {
                    InitRectMesh(RectType.Center, TextureType.Center, startX, startZ, vertices, uvs, colors, color, indices, block.IsBuildFinish);
                }
            }
        }
        //放到主线程中
        Loom.QueueOnMainThread((param) =>
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            //Debug.LogFormat("refresh mesh lod {0} done, vert count {1}, color {2}", lodIdx, vertices.Count, colors.Count);
        }, null);
    }
    
    private TerritoryBlock GetBlock(int x, int y)
    {
        if (x < 0 || x >= _maxBlockX)
            return null;
        if (y < 0 || y >= _maxBlockZ)
            return null;
        return _blocks[y * _maxBlockX + x];
    }

    public void ResetBlocks()
    {
        foreach (var t in _blocks)
        {
            t.Id = 0;
            t.Color = 0;
            t.IsBuildFinish = false;
            t.Valid = false;
        }
    }

    public void ChangeBlock(uint idx, TerritoryBlock block)
    {
        _blocks[idx - 1] = block;
        _dirty = true;
    }
    //处理刷新网格请求
    public void RefreshView()
    {
        if (!_dirty) //没有脏，无需刷新
            return;

        //还没冷却好则先缓存一下刷新，等下次刷新
        bool isCooling = _sw.IsRunning && _sw.ElapsedMilliseconds < 500;

        //如果正在异步刷新网格过程中（繁忙），先把刷新请求暂存起来，等空闲了再刷新，避免短时间内发起太多异步网格刷新
        if (_runningRefresh || isCooling) 
        {
            //LoggerHelper.Debug("wait for last mesh refresh...");
            _waitRefresh = true;
            return;
        }
        
        _runningRefresh = true; //置繁忙标记
        Loom.RunAsync(() =>
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            RefreshMeshEdged(0, false);
            RefreshMeshEdged(1, true);
            //sw.Stop();
            //Debug.LogFormat("RefreshMeshLod total: {0} ms", sw.ElapsedMilliseconds);
            _runningRefresh = false; //进入空闲
            
            _sw.Restart(); //重启计时器
        });

        _dirty = false;
    }

    public void OnEnterMainScene()
    {
        CreateLodSceneObject();
        
        CreateStaticRangeSceneObject();
    }

    public void OnExitMainScene()
    {
        CleanLodSceneObject();

        CleanPreviewRangeSceneObject();
    }

    private MaterialPropertyBlock _materialLodPropertyBlock;
    private void CheckTerritoryLodByCamera()
    {
        if (Camera.main == null)
            return;
        
        var y = Camera.main.transform.position.y;
        //所有的obj必须先加载好
        foreach (var lod in _lodArray)
        {
            if (lod.Go == null)
            {
                return;
            }
        }

        if (_materialLodPropertyBlock == null)
            _materialLodPropertyBlock = new MaterialPropertyBlock();
        
        _materialLodPropertyBlock.Clear();
        
        //根据设置来激活并计算设置淡入淡出透明度
        foreach (var lod in _lodArray)
        {
            if (y > lod.FadeOutEnd)
            {
                UGUIUtil.SetActiveSafe(lod.Go,  false);
            }
            else
            {
                float alpha;
                if (y > lod.FadeOutStart)
                    alpha = 1 - (y - lod.FadeOutStart) / (lod.FadeOutEnd - lod.FadeOutStart);
                else if (y > lod.FadeInEnd)
                    alpha = 1;
                else if (y > lod.FadeInStart)
                    alpha = (y - lod.FadeInStart) / (lod.FadeInEnd - lod.FadeInStart);
                else
                    alpha = 0;
                
                _materialLodPropertyBlock.SetFloat(_propIdAlpha, alpha);
                lod.Go.GetComponent<MeshRenderer>().SetPropertyBlock(_materialLodPropertyBlock);
                
                //lod.Mat.SetFloat(_propIdAlpha, alpha);
                UGUIUtil.SetActiveSafe(lod.Go,  alpha > 0);
            }
        }
    }
    //计算当前相机看到的格子范围
    private void CheckCameraVisRect()
    {
        var upRight = FunctionUtil.ScreenToWordPos(new Vector3(Screen.width, Screen.height, 0), Camera.main);
        var downLeft = FunctionUtil.ScreenToWordPos(new Vector3(0, 0, 0), Camera.main);
        var upLeft = FunctionUtil.ScreenToWordPos(new Vector3(0, Screen.height, 0), Camera.main);
        var min = new Vector2(upLeft.x, downLeft.z);
        var max = new Vector2(upRight.x, upRight.z);
        Debug.DrawLine(upRight, downLeft, Color.red);
        if (min != _visRect.min || max != _visRect.max)
        {
            _visRect.min = min;
            _visRect.max = max;
            _dirty = true;            
            RefreshView();
        }
    }
    //每帧调用
    public void CheckLodByCamera(bool force)
    {
        if (Camera.main == null)
            return;
        
        CheckCameraVisRect();
        
        if (_waitRefresh && !_runningRefresh) //如果空闲且需要刷新，则进行刷新
        {
            _waitRefresh = false;
            RefreshView();
        }
        
        //如果相机高度变化太小则无需切换精度
        var y = Camera.main.transform.position.y;
        const float minCalcDis = 5;
        if (!force && Math.Abs(y - _lastCameraPosY) < minCalcDis)
            return;

        _lastCameraPosY = y;
        
        CheckTerritoryLodByCamera();
    }

    private MaterialPropertyBlock _materialPropertyBlock;
    public void ShowStaticPreviewRange(int range, int x, int y, Color c)
    {
        if (!_staticPreviewRange.ContainsKey(range))
        {
            LoggerHelper.DebugFormat("can not found range object: {0}", range);         
            //LoggerHelper.ErrorFormat("can not found range object: {0}", range);            
            return;
        }

        foreach (var v in _staticPreviewRange)
        {
            UGUIUtil.SetActiveSafe(v.Value,  v.Key == range);
            if (v.Key == range)
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();
                
                _materialPropertyBlock.Clear();
                
                _materialPropertyBlock.SetColor(_propIdColor, c);  
                v.Value.GetComponent<MeshRenderer>().SetPropertyBlock(_materialPropertyBlock);
                
//                var mat = v.Value.GetComponent<MeshRenderer>().material;
//                mat.SetColor(_propIdColor, c);
                v.Value.transform.position = new Vector3(x * _blockSize, _meshYPos, y * _blockSize);
            }
        }
    }

    public void HideStaticPreviewRange()
    {
        foreach (var v in _staticPreviewRange)
        {
            UGUIUtil.SetActiveSafe(v.Value,  false);
        }
    }
}
