using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using cfg;
using System;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;

public class ECSMapAuthoringWindow : EditorWindow
{
    private interface IMapEditorCommand
    {
        public bool Do();
        public bool UnDo();
    }

    private class EditorAStarMapCommand : IMapEditorCommand
    {
        public int2 Pos;
        public MapData CurrentMapData;

        public static EditorAStarMapCommand Create(int2 pos, MapData mapData)
        {
            var data = new EditorAStarMapCommand();
            data.Pos = pos;
            data.CurrentMapData = mapData;
            return data;
        }

        public bool Do()
        {
            return CurrentMapData.Grids.TryAdd(Pos, cfg.GridType.AStar.ToInt());
        }

        public bool UnDo()
        {
            return CurrentMapData.Grids.Remove(Pos);
        }
    }

    private class EditorPlaceableMapCommand : IMapEditorCommand
    {
        public int2 Pos;
        public MapData CurrentMapData;

        public static EditorPlaceableMapCommand Create(int2 pos, MapData mapData)
        {
            var data = new EditorPlaceableMapCommand();
            data.Pos = pos;
            data.CurrentMapData = mapData;
            return data;
        }

        public bool Do()
        {
            if (!CurrentMapData.Grids.ContainsKey(Pos)) return false;
            var gridValue = CurrentMapData.Grids[Pos];
            var calcValue = (gridValue & cfg.GridType.Placeable.ToInt());
            if (calcValue > 0) return false;
            CurrentMapData.Grids[Pos] |= cfg.GridType.Placeable.ToInt();
            return true;
        }

        public bool UnDo()
        {
            CurrentMapData.Grids[Pos] -= cfg.GridType.Placeable.ToInt();
            return true;
        }
    }

    private class EditorRemoveMapCommand : IMapEditorCommand
    {
        public int2 Pos;
        public MapData CurrentMapData;

        private int _gridValue;

        public static EditorRemoveMapCommand Create(int2 pos, MapData mapData)
        {
            var data = new EditorRemoveMapCommand();
            data.Pos = pos;
            data.CurrentMapData = mapData;
            return data;
        }

        public bool Do()
        {
            if (!CurrentMapData.Grids.ContainsKey(Pos)) return false;
            _gridValue = CurrentMapData.Grids[Pos];
            return CurrentMapData.Grids.Remove(Pos);
        }

        public bool UnDo()
        {
            return CurrentMapData.Grids.TryAdd(Pos, _gridValue);            
        }
    }

    private class EditorObstacleAddMapCommand : IMapEditorCommand
    {
        public Vector3 Pos;
        public List<Vector3> CurrentMapData;

        public static EditorObstacleAddMapCommand Create(Vector3 pos, List<Vector3> currentMapData)
        {
            var data = new EditorObstacleAddMapCommand();
            data.Pos = pos;
            data.CurrentMapData = currentMapData;
            return data;
        }

        public bool Do()
        {
            if (CurrentMapData.Contains(Pos)) return false;

            CurrentMapData.Add(Pos);
            return true;
        }

        public bool UnDo()
        {            
            return CurrentMapData.Remove(Pos);
        }
    }

    private class EditorObstacleRemoveMapCommand : IMapEditorCommand
    {
        public Vector3 Pos;
        public List<Vector3> CurrentMapData;

        private int _index;
        private Vector3 _pos;

        public static EditorObstacleRemoveMapCommand Create(Vector3 pos, List<Vector3> currentMapData)
        {
            var data = new EditorObstacleRemoveMapCommand();
            data.Pos = pos;
            data.CurrentMapData = currentMapData;
            return data;
        }

        public bool Do()
        {
            if (CurrentMapData.Count == 0) return false;

            var nearestIndex = 0;
            _pos = CurrentMapData[0];
            var nearestPos = Vector3.Distance(CurrentMapData[0], Pos);
            for (int i = 1; i < CurrentMapData.Count; i++)
            {
                var tempDis = Vector3.Distance(CurrentMapData[i], Pos);
                if (tempDis < nearestPos)
                {
                    nearestPos = tempDis;
                    nearestIndex = i;
                    _pos = CurrentMapData[i];
                }
            }

            CurrentMapData.RemoveAt(nearestIndex);
            _index = nearestIndex;
            return true;
        }

        public bool UnDo()
        {
            CurrentMapData.Insert(_index, Pos);
            return true;
        }
    }

    [MenuItem("LCFramework/AStar")]
    public static void CreateAStarWindow()
    {
        var window = UnityEditor.EditorWindow.GetWindow<ECSMapAuthoringWindow>();
        window.minSize = new Vector2(100, 100);
    }

    public enum DrawMode
    {
        AStarEditor,
        PlaceableEditor,
        RemoveEditor,
        Obstacle,
        DelObstacle,
    }

    private MapData _currentMapData;
    private DrawMode _currentMode;
    private bool _editorMode;
    private Stack<IMapEditorCommand> _commandDo;
    private Stack<IMapEditorCommand> _commandUnDo;
    private MapData_Obstacle _currentObstacle;
    private Vector2 _scorllPos;

    private void OnGUI()
    {
        _currentMapData = (MapData)EditorGUILayout.ObjectField(_currentMapData, typeof(MapData), true);
        if (_currentMapData == null) return;
        
        GUILayout.Label(_currentMapData.name);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Size:");
        var temp = GUILayout.TextField(_currentMapData.GridSize.ToString());
        if (int.TryParse(temp, out var value) && value != _currentMapData.GridSize)
        {
            _currentMapData.GridSize = value;
            if (_currentMapData.Grids != null)
            {
                _currentMapData.Grids.Clear();
            }
            _commandDo.Clear();
            _commandUnDo.Clear();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer:");
        temp = GUILayout.TextField(_currentMapData.Layer.ToString());
        if (int.TryParse(temp, out value))
        {
            _currentMapData.Layer = value;
        }
        GUILayout.EndHorizontal();

        _scorllPos = GUILayout.BeginScrollView(_scorllPos, GUILayout.Width(300));
        
        GUILayout.EndScrollView();
        if (_currentMapData.Obstacles == null) _currentMapData.Obstacles = new List<MapData_Obstacle>();
        for (int i = 0; i < _currentMapData.Obstacles.Count; i++)
        {
            var obstacleData = _currentMapData.Obstacles[i];
            GUILayout.BeginHorizontal();
            obstacleData.Name = GUILayout.TextField(obstacleData.Name);
            if (GUILayout.Button("Load")) _currentObstacle = obstacleData;
            if (GUILayout.Button("Delete"))
            {
                _currentMapData.Obstacles.RemoveAt(i--);
                _currentObstacle = null;
            }
            GUILayout.EndHorizontal();
        }
        if(GUILayout.Button("Add Obstacle"))
        {
            _currentMapData.Obstacles.Add(new MapData_Obstacle()
            {
                Name = "New Obstacle",
                Vertexs = new List<Vector3>()
            });
        }
        if(GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(_currentMapData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();            
        }
    }

    public void OnEnable()
    {
        _currentMode = DrawMode.AStarEditor;
        _commandDo = new Stack<IMapEditorCommand>();
        _commandUnDo = new Stack<IMapEditorCommand>();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView view)
    {
        if (!_currentMapData) return;

        GridDraw();
        ObstacleDraw();

        Handles.BeginGUI();
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        GUILayout.Label(_editorMode ? "编辑模式" : "非编辑模式", style);
        Handles.EndGUI();
        if (Event.current.type == EventType.KeyDown
            && Event.current.keyCode == KeyCode.M)
        {
            _editorMode = !_editorMode;
            Event.current.Use();    
        }
        if (!_editorMode) return;

        Handles.BeginGUI();
        GUILayout.Label($"Mode:{_currentMode}", style);
        if(_currentObstacle != null)
        {
            GUILayout.Label($"Obstacle:{_currentObstacle.Name}", style);
        }

        var keyCode = Event.current.keyCode;
        if(keyCode == KeyCode.Alpha1)
        {
            _currentMode = DrawMode.AStarEditor;
            Event.current.Use();
        }
        else if(keyCode == KeyCode.Alpha2)
        {
            _currentMode = DrawMode.PlaceableEditor;
            Event.current.Use();
        }
        else if (keyCode == KeyCode.Alpha3)
        {
            _currentMode = DrawMode.RemoveEditor;
            Event.current.Use();
        }
        else if (keyCode == KeyCode.Alpha4)
        {
            _currentMode = DrawMode.Obstacle;
            Event.current.Use();
        }
        else if (keyCode == KeyCode.Alpha5)
        {
            _currentMode = DrawMode.DelObstacle;
            Event.current.Use();
        }
        Handles.EndGUI();

        if (_currentMapData == null) return;
        if (_currentMapData.Grids == null) _currentMapData.Grids = new MapData_Grid();
        if (Selection.activeGameObject != null) return;

        if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
            && !Event.current.alt
            && Event.current.button == 0)
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var oy = ray.origin.y;
            var ry = (ray.direction + ray.origin).y;
            var ratio = oy / (oy - ry);
            var pos = ray.origin + ray.direction * ratio;
            pos.y = 0;
            var key = pos.ToInt2(_currentMapData.GridSize);
            Command(key, pos);
            Event.current.Use();
        }        
        if((Event.current.type == EventType.KeyDown)
            && Event.current.keyCode == KeyCode.Z
            && _commandDo.Count > 0)
        {
            var command = _commandDo.Peek();

            if (command == null) return;

            if (!command.UnDo()) return;

            _commandDo.Pop();
            _commandUnDo.Push(command);
            Event.current.Use();
        }

        if (Event.current.type == EventType.KeyDown
            && Event.current.keyCode == KeyCode.Y
            && _commandUnDo.Count > 0)
        {            
            var command = _commandUnDo.Peek();

            if (command == null) return;

            if (!command.Do()) return;

            _commandUnDo.Pop();
            _commandDo.Push(command);
            Event.current.Use();
        }
    }

    private void ObstacleDraw()
    {        
        if (_currentObstacle == null || _currentObstacle.Vertexs == null) return;
        var count = _currentObstacle.Vertexs.Count;
        if (count == 0) return;
        var lastPoint = _currentObstacle.Vertexs[0];
        //Handles.color = Color.black;
        for (int i = 1; i < count; i++)
        {
            Debug.DrawLine(lastPoint, _currentObstacle.Vertexs[i]);
            lastPoint = _currentObstacle.Vertexs[i];
            //Debug.LogError(_currentObstacle.Vertexs[i]);
        }
        Debug.DrawLine(_currentObstacle.Vertexs[0], _currentObstacle.Vertexs[count-1]);
    }

    private void Command(int2 key, Vector3 pos)
    {
        IMapEditorCommand command = null;
        if (_currentMode == DrawMode.AStarEditor)
        {
            command = EditorAStarMapCommand.Create(key, _currentMapData);
        }
        else if (_currentMode == DrawMode.PlaceableEditor)
        {
            command = EditorPlaceableMapCommand.Create(key, _currentMapData);
        }
        else if (_currentMode == DrawMode.RemoveEditor)
        {
            command = EditorRemoveMapCommand.Create(key, _currentMapData);
        }
        else if(_currentMode == DrawMode.Obstacle && _currentObstacle != null)
        {
            command = EditorObstacleAddMapCommand.Create(pos, _currentObstacle.Vertexs);
        }
        else if(_currentMode == DrawMode.DelObstacle && _currentObstacle != null)
        {
            command = EditorObstacleRemoveMapCommand.Create(pos, _currentObstacle.Vertexs);
        }

        if(command != null && command.Do())
        {
            _commandDo.Push(command);
        }
    }

    private void GridDraw()
    {
        for (int i = 0; i < _currentMapData.Grids.Pos.Count; i++)
        {
            var pos = _currentMapData.Grids.Pos[i];

            var value = _currentMapData.Grids.Value[i];
            if ((value & (int)GridType.Placeable) > 0) continue;

            Handles.color = Color.green;
            Handles.DrawWireCube(pos.ToVecotr3() * _currentMapData.GridSize, Vector3.one * _currentMapData.GridSize);
        }

        for (int i = 0; i < _currentMapData.Grids.Pos.Count; i++)
        {
            var pos = _currentMapData.Grids.Pos[i];
            var value = _currentMapData.Grids.Value[i];

            if((value & (int)GridType.Placeable) > 0)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(pos.ToVecotr3() * _currentMapData.GridSize, Vector3.one * _currentMapData.GridSize);
            }
        }
    }    
}