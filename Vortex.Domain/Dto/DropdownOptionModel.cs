namespace Vortex.Domain.Dto;

public class DropdownOptionModel<TValue>
{
    public required TValue Value { get; set; }
    public required string Label { get; set; }
}

public class DropdownOptionModel<TValue, TExtra> : DropdownOptionModel<TValue>
{
    // It's optional (?) so you can leave it null if needed, 
    // but the type TExtra is still defined.
    public TExtra? ExtraData { get; set; }
}