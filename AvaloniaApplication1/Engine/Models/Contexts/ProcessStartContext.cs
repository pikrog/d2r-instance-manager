using System;
using System.Collections.Generic;
using System.Text;
using AvaloniaApplication1.Engine.CommandLine;

namespace AvaloniaApplication1.Engine.Models.Contexts;

public sealed record ProcessStartContext(
    string ExecutablePath,
    IReadOnlyList<Argument> Arguments,
    IntPtr? DisplayHandle)
{
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append(
            $"{nameof(ExecutablePath)} = {ExecutablePath}, " +
            $"{nameof(Arguments)} = [{string.Join(", ", Arguments)}], " +
            $"{nameof(DisplayHandle)} = {DisplayHandle}");
        
        return true;
    }
}