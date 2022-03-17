using _Core.Player;
using Assets.Scripts.Dialogue_System;

namespace Dialogue_System
{
    public interface IInteractable
    {
        void Interact(Player player);
    }
}