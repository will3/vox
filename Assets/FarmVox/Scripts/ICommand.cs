namespace FarmVox.Scripts
{
    public interface ICommand
    {
        string Name { get; }
        string Run(string[] args);
    }
}