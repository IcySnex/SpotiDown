using SpotiDown.Enums;

namespace SpotiDown.Interfaces
{
    public interface ILibrary
    {
        public bool IsContainer { get; }
        public string ContainerName { get; set; }
        public bool NameContains(string Input);
    }
}
