using System;

public interface IInteractableTrigger
{
    event Action OnActivate;
    event Action OnDeactivate;
}