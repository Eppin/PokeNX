namespace PokeNX.DesktopApp.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class EventAggregator
{
    private static readonly List<Delegate> Handlers = new();
    private static readonly SynchronizationContext SynchronizationContext;

    static EventAggregator()
    {
        SynchronizationContext = SynchronizationContext.Current;
    }

    public static void SendMessage<T>(T message)
    {
        if (message == null)
            return;

        if (SynchronizationContext != null)
        {
            SynchronizationContext.Send(m => Dispatch((T)m!), message);
        }
        else
        {
            Dispatch(message);
        }
    }

    public static void PostMessage<T>(T message)
    {
        if (message == null)
            return;

        if (SynchronizationContext != null)
        {
            SynchronizationContext.Post(m => Dispatch((T)m!), message);
        }
        else
        {
            Dispatch(message);
        }
    }

    public static Action<T> RegisterHandler<T>(Action<T> eventHandler)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        Handlers.Add(eventHandler);
        return eventHandler;
    }

    public static void UnregisterHandler<T>(Action<T> eventHandler)
    {
        if (eventHandler == null)
            throw new ArgumentNullException(nameof(eventHandler));

        Handlers.Remove(eventHandler);
    }

    private static void Dispatch<T>(T message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var compatibleHandlers = Handlers
            .OfType<Action<T>>()
            .ToList();

        foreach (var action in compatibleHandlers)
            action(message);
    }
}
