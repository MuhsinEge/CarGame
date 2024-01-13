using System;
using Data;
using UnityEngine;

namespace LevelEditor
{
    public static class GridEditorExtensions 
    {
        public static Texture2D GetTextureWithColor(Color color)
        {
            var texture = new Texture2D(20, 20);
            var pixels = texture.GetPixels();
            for (var index = 0; index < pixels.Length; index++)
            { 
                pixels[index] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        public static GUIStyle GetStyleByStyleAndTexture(GUIStyle style, Texture2D texture, Color textColor = default)
        {
            return new GUIStyle(style)
            {
                normal =
                {
                    background = texture,
                    textColor = textColor
                },
                fontStyle = FontStyle.Bold
            };
        }
        
        public static string ValidateSave(this GridData gridData)
        {
            foreach (var car in gridData.Cars)
            {
                if(car.Enter == (-1,-1) || car.Exit == (-1,-1))
                {
                    return "Enter and Exit must be placed for all cars";
                }
            }
            return string.Empty;
        }

        public static string ValidatePlacement(this GridEnum selectionType, int row, int col, int maxRow, int maxCol)
        {
            switch (selectionType)
            {
                case GridEnum.Empty:
                    return String.Empty;
                case GridEnum.Obstacle:
                    return String.Empty;
                default:
                    if(row == 0 || row == maxRow -1 || col == 0 || col == maxCol -1)
                    {
                        return String.Empty;
                    }
                    return "Enter and Exit can only be placed on the edges";
            }
        }
        
        public static void SelectionPostProcessor(this GridData gridData, GridEnum selectionType, int selectedCarIndex, int col, int row)
        {
            if (selectionType == GridEnum.Empty || selectionType == GridEnum.Obstacle)
            {
                var carIndex = gridData.GridElements[row][col].CarIndex;
                if (carIndex != 0)
                {
                    if (carIndex > 0)
                    {
                        gridData.Cars[Math.Abs(carIndex)-1].Enter = (-1,-1);
                    }
                    else
                    {
                        gridData.Cars[Math.Abs(carIndex)-1].Exit = (-1,-1);
                    }
                   
                }
               
                return;
            }

            if (selectionType == GridEnum.Enter)
            {
                if(gridData.Cars[Math.Abs(selectedCarIndex)-1].Enter != (-1, -1))
                {
                    gridData.GridElements[gridData.Cars[Math.Abs(selectedCarIndex)-1].Enter.Item1][gridData.Cars[Math.Abs(selectedCarIndex)-1].Enter.Item2].SetType(GridEnum.Empty);
                }

                if (gridData.Cars[Math.Abs(selectedCarIndex) - 1].Enter == (row, col))
                {
                    var carDirection = gridData.Cars[Math.Abs(selectedCarIndex) - 1].Direction;
                    var directionIndex = (int)carDirection;
                    var directionCount = Enum.GetNames(typeof(CarDirectionEnum)).Length;
                    if (directionIndex == directionCount - 1)
                    {
                        directionIndex = 0;
                    }
                    else
                    {
                        directionIndex++;
                    }
                    gridData.Cars[Math.Abs(selectedCarIndex) - 1].Direction = (CarDirectionEnum)directionIndex;
                }

                gridData.Cars[Math.Abs(selectedCarIndex)-1].Enter = (row,col);
            }else if (selectionType == GridEnum.Exit)
            {
                if(gridData.Cars[Math.Abs(selectedCarIndex)-1].Exit != (-1, -1))
                {
                    gridData.GridElements[gridData.Cars[Math.Abs(selectedCarIndex)-1].Exit.Item1][gridData.Cars[Math.Abs(selectedCarIndex)-1].Exit.Item2].SetType(GridEnum.Empty);
                }
                gridData.Cars[Math.Abs(selectedCarIndex)-1].Exit = (row,col);
            }
        }
    }
}
