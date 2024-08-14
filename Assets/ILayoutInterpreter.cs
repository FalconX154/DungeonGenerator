public partial class DungeonGenerator
{
    public interface ILayoutInterpreter
    {
        public void InterpretLayout(ERoomType[,] _layout);
    }
}