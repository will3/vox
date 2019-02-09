namespace FarmVox.Commands
{
    public abstract class CommandBase
    {
        public CommandType CommandType { get; protected set; }
    }
}