using System.Collections.Generic;
using LevelEditor;

namespace Data
{
    [System.Serializable]
    public class GridData
    {
        public List<List<GridElement>> GridElements;
        public List<CarData> Cars;

        public GridData()
        {
            GridElements = new List<List<GridElement>>();
            Cars = new List<CarData>();
        }
    }
}