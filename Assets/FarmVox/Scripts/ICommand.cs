namespace FarmVox.Scripts
{
    public interface ICommand
    {
        string Name { get; }
        void Run(string[] args);
    }
}