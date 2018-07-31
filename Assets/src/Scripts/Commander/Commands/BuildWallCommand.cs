namespace FarmVox
{
    public class BuildWallCommand : Command
    {
        public override bool Update()
        {
            DragLine();
            return false;
        }
    }
}