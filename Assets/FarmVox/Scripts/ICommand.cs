namespace FarmVox.Scripts
{
    public interface ICommand
    {
        string CommandName { get; }
        string Run(string[] args);
    }
}