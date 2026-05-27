namespace Laba1.Interfaces;

public interface IPowerable
{
    bool HasPower { get; }
    bool TurnOn();
    bool TurnOff();
}