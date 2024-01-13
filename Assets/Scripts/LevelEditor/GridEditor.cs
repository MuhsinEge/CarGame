#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace LevelEditor
{
    [CustomEditor(typeof(GridContainer))]
    public class GridEditor : Editor
    {
        private const int Rows = 12;
        private const int Columns = 22;
        private const int CarCount = 8;
        
        private int _selected;
        private int _selectedCarIndex;
        private Color[] _carColors;
        private GridEnum _selectedType;
        private GridData _gridData;
        private string _errorMessage = string.Empty;
        private GridContainer _gridContainer;
        
        private void OnEnable()
        {
            GenerateFreshGrid();
        }
        
        private void GenerateFreshGrid()
        {
            _gridContainer = (GridContainer)target;
            _gridContainer.OnLoad();
            var colors = serializedObject.FindProperty("carColors");
            //_levelIndex = _gridManager.GetCurrentLevelCount();

            _gridData = new GridData();
            _carColors = new Color[CarCount];
            
            for(int y = 0; y < CarCount; y++)
            {
                _carColors[y] = colors.GetArrayElementAtIndex(y).colorValue;
                _gridData.Cars.Add(new CarData());
            }
            
            for(int i = 0; i < Rows; i++)
            {
                _gridData.GridElements.Add(new List<GridElement>());
                for(int j = 0; j < Columns; j++)
                {
                    _gridData.GridElements[i].Add(new GridElement(GridEnum.Empty));
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

            //_loadedLevelIndex = EditorGUILayout.IntField("Level Index", _loadedLevelIndex);
            var currentLevelCount = _gridContainer.GetCurrentLevelCount();
            
            if (currentLevelCount >= 0)
            {
                EditorGUI.BeginChangeCheck();
                
                List<string> levels = new List<string>(currentLevelCount);

                levels.Add("New Level");
                for (int i = 0; i <= currentLevelCount; i++)
                {
                    levels.Add("Level " + i);
                }
             

                _selected = EditorGUILayout.Popup("Load Level", _selected, levels.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    LoadExistingLevel();
                }
            }
            
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Grid Preview", EditorStyles.boldLabel);
            
            DrawGridPreview();

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                GUILayout.Space(10);
            
                EditorGUILayout.HelpBox(_errorMessage, MessageType.Error);
            }

            GUILayout.Space(10);
            
            EditorGUILayout.LabelField("Decisions", EditorStyles.boldLabel);
            
            DrawDecisionPreview();
            
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("Selections", EditorStyles.boldLabel);
            
            DrawSelectionPreview();
            
            
            
            serializedObject.ApplyModifiedProperties();
        }

        private void LoadExistingLevel()
        {
            if (_selected == 0)
            {
                GenerateFreshGrid();
            }
            else
            {
                var data = _gridContainer.GetLevelIndexData(_selected -1);
                if (data != null)
                {
                    _gridData = data;
                } else
                {
                    _selected = 0;
                }
            }
        }
        
        private void DrawGridPreview()
        {
            if (_gridData.GridElements == null)
            {
                LoadExistingLevel();
            }
            GUILayout.BeginHorizontal();

            for (int col = 0; col < Columns; col++)
            {
                GUILayout.BeginVertical();

                for (int row = Rows -1; row >= 0; row--)
                {
                   DrawGridElement(row, col);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
        }
        
        private void DrawSelectionPreview()
        {
            GUILayout.Space(25);
            GUILayout.BeginHorizontal();

            GUILayout.Box(GridEditorExtensions.GetTextureWithColor(Color.black),GUILayout.Width(20), GUILayout.Height(20));
            if (GUILayout.Button("Obstacle", GUILayout.Width(100),GUILayout.Height(20)))
            {
                OnSelectionMade(true);
            }
            
            if (GUILayout.Button("Drivable", GUILayout.Width(100),GUILayout.Height(20)))
            {
                OnSelectionMade();
            }
            GUILayout.Box(GridEditorExtensions.GetTextureWithColor(Color.white),GUILayout.Width(20), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(25);
            for(int i = 1; i <= CarCount; i++)
            {
                GUILayout.BeginHorizontal();
                
                var texture = GridEditorExtensions.GetTextureWithColor(_carColors[i-1]);
                
                GUILayout.Box(texture,GUILayout.Width(20), GUILayout.Height(20));
                
                if (GUILayout.Button("Car " + i + " Enter",GUILayout.Width(100),GUILayout.Height(20)))
                {
                    OnSelectionMade(false, i);
                }
                if (GUILayout.Button("Car " + i + " Exit",GUILayout.Width(100),GUILayout.Height(20)))
                {
                    OnSelectionMade(false, -i);
                }
                GUILayout.EndHorizontal();
            }
            
           
           
        }

        private void DrawDecisionPreview()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("Clear Level", GUILayout.Width(100),GUILayout.Height(20)))
            {
                GenerateFreshGrid();
            }
            
            GUILayout.Space(50);
            if (GUILayout.Button("Save Level", GUILayout.Width(100),GUILayout.Height(20)))
            {
                _errorMessage = _gridData.ValidateSave();
                
                if (string.IsNullOrEmpty(_errorMessage))
                {
                    _gridContainer.SaveLevel(_selected -1,_gridData);
                    if (_selected == 0)
                    {
                        _selected = _gridContainer.GetCurrentLevelCount() + 1;
                    }
                }
            }
            
            GUILayout.Space(50);
            if (GUILayout.Button("Delete Level", GUILayout.Width(100),GUILayout.Height(20)))
            {
                if (_selected == 0)
                {
                    GenerateFreshGrid();
                }
                else
                {
                    _gridContainer.DeleteLevel(_selected -1);
                    _selected = 0;
                    GenerateFreshGrid();
                }
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawGridElement(int row, int col)
        {
            var element = _gridData.GridElements[row][col];
            Color color;
            string text = "";
            switch (element.Type)
            {
                case GridEnum.Empty:
                    color = Color.white;
                    break;
                case GridEnum.Obstacle:
                    color = Color.black;
                    break;
                case GridEnum.Enter:
                    var direction = _gridData.Cars[Math.Abs(element.CarIndex) - 1].Direction;
                    text = direction == CarDirectionEnum.Up ? "↑" :
                        direction == CarDirectionEnum.Down ? "↓" :
                        direction == CarDirectionEnum.Left ? "←" : "→";
                    color = _carColors[Math.Abs(element.CarIndex)- 1];
                    break;
                case GridEnum.Exit:
                    text = "X";
                    color = _carColors[Math.Abs(element.CarIndex)- 1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var texture = GridEditorExtensions.GetTextureWithColor(color);
            var style = GridEditorExtensions.GetStyleByStyleAndTexture(GUI.skin.button, texture, Color.black);
            if (GUILayout.Button(text,style,GUILayout.Width(20), GUILayout.Height(20)))
            {
                OnSelectionPlaced(row, col);
            }
        }
        
        private void OnSelectionMade(bool isObstacle = false, int carIndex = 0)
        {
            _selectedCarIndex = carIndex;
            if (isObstacle)
            {
                _selectedType = GridEnum.Obstacle;
            }
            else
            {
                if (carIndex == 0)
                {
                    _selectedType = GridEnum.Empty;
                    return;
                }
                _selectedType = carIndex > 0 ? GridEnum.Enter : GridEnum.Exit;
            }
        }
        
        private void OnSelectionPlaced(int row, int col)
        {
            _errorMessage = _selectedType.ValidatePlacement(row,col,Rows,Columns);
            if (!string.IsNullOrEmpty(_errorMessage))
                return;
            _gridData.SelectionPostProcessor(_selectedType, _selectedCarIndex, col, row);
            _gridData.GridElements[row][col].SetType(_selectedType);
            _gridData.GridElements[row][col].SetCarIndex(_selectedCarIndex);
        }
        
    }
}
#endif
