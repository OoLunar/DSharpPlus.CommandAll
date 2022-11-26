using System;
using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public interface IBuilder
    {
        void Verify();
        bool TryVerify();
        bool TryVerify([NotNullWhen(false)] out Exception? error);
    }
}
