using ComputerInterface.Interfaces;
using System;

namespace RCH.CI
{
    internal class RchEntry : IComputerModEntry
    {
        public string EntryName => "Room Code Hider";
        public Type EntryViewType => typeof(RchView);
    }
}
