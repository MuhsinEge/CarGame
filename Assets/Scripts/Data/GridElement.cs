using LevelEditor;

namespace Data
{
    public class GridElement
    { 
        public GridEnum Type;
        public int CarIndex;

        public GridElement()
        {
            Type = GridEnum.Empty;
            CarIndex = -1;
        }
        public GridElement(GridEnum type)
        {
            Type = type;
        }
        
        public GridElement(GridEnum type, int carIndex)
        {
            Type = type;
            CarIndex = carIndex;
        }
        
        public void SetCarIndex(int index)
        {
            CarIndex = index;
        }
        
        public void SetType(GridEnum type)
        {
            Type = type;
        }
        
    }
}