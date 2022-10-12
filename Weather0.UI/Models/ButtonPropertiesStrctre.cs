using System.Windows.Input;

public struct ButtonPropertiesStrctre
{
    public double _opacityPropertyDble;
    public int _heightPropertyInt;
    public ICommand Command { get; set; }
    public string OpacityPropertyChangeToRaise { get; set; }
    public string HeightPropertyChangeToRaise { get; set; }
    public bool EnablingAnimationFlag { get; set; }
    public bool DisablingAnimationFlag { get; set; }
}