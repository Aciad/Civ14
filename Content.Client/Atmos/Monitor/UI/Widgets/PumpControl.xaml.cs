using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Atmos.Monitor.UI.Widgets;

[GenerateTypedNameReferences]
public sealed partial class PumpControl : BoxContainer
{
    private GasVentPumpData _data;
    private string _address;

    public event Action<string, IAtmosDeviceData>? PumpDataChanged;
	public event Action<IAtmosDeviceData>? PumpDataCopied;

    private CheckBox _enabled => CEnableDevice;
    private CollapsibleHeading _addressLabel => CAddress;
    private OptionButton _pumpDirection => CPumpDirection;
    private OptionButton _pressureCheck => CPressureCheck;
    private FloatSpinBox _externalBound => CExternalBound;
    private FloatSpinBox _internalBound => CInternalBound;
    private Button _copySettings => CCopySettings;

    public PumpControl(GasVentPumpData data, string address)
    {
        RobustXamlLoader.Load(this);

        Name = address;

        _data = data;
        _address = address;

        _addressLabel.Title = Loc.GetString("air-alarm-ui-atmos-net-device-label", ("address", $"{address}"));

        _enabled.Pressed = data.Enabled;
        _enabled.OnToggled += _ =>
        {
            _data.Enabled = _enabled.Pressed;
            PumpDataChanged?.Invoke(_address, _data);
        };

        _internalBound.Value = _data.InternalPressureBound;
        _internalBound.OnValueChanged += _ =>
        {
            _data.InternalPressureBound = _internalBound.Value;
            PumpDataChanged?.Invoke(_address, _data);
        };
        _internalBound.IsValid += value => value >= 0;

        _externalBound.Value = _data.ExternalPressureBound;
        _externalBound.OnValueChanged += _ =>
        {
            _data.ExternalPressureBound = _externalBound.Value;
            PumpDataChanged?.Invoke(_address, _data);
        };
        _externalBound.IsValid += value => value >= 0;

        foreach (var value in Enum.GetValues<VentPumpDirection>())
        {
            _pumpDirection.AddItem(Loc.GetString($"{value}"), (int) value);
        }

        _pumpDirection.SelectId((int) _data.PumpDirection);
        _pumpDirection.OnItemSelected += args =>
        {
            _pumpDirection.SelectId(args.Id);
            _data.PumpDirection = (VentPumpDirection) args.Id;
            PumpDataChanged?.Invoke(_address, _data);
        };

        foreach (var value in Enum.GetValues<VentPressureBound>())
        {
            _pressureCheck.AddItem(Loc.GetString($"{value}"), (int) value);
        }

        _pressureCheck.SelectId((int) _data.PressureChecks);
        _pressureCheck.OnItemSelected += args =>
        {
            _pressureCheck.SelectId(args.Id);
            _data.PressureChecks = (VentPressureBound) args.Id;
            PumpDataChanged?.Invoke(_address, _data);
        };

        _copySettings.OnPressed += _ =>
        {
            PumpDataCopied?.Invoke(_data);
        };
    }

    public void ChangeData(GasVentPumpData data)
    {
        _data.Enabled = data.Enabled;
        _enabled.Pressed = _data.Enabled;

        _data.PumpDirection = data.PumpDirection;
        _pumpDirection.SelectId((int) _data.PumpDirection);

        _data.PressureChecks = data.PressureChecks;
        _pressureCheck.SelectId((int) _data.PressureChecks);

        _data.ExternalPressureBound = data.ExternalPressureBound;
        _externalBound.Value = _data.ExternalPressureBound;

        _data.InternalPressureBound = data.InternalPressureBound;
        _internalBound.Value = _data.InternalPressureBound;
    }
}
