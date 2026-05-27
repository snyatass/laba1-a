namespace Laba1.Interfaces;

public interface IHasStorage
{
    int StorageGb { get; }
    int FreeStorageGb { get; }
    bool InstallSoftware(string name, int sizeGb);
}