using System.Collections;

namespace Neo.Bpms.Domain.Utility;

public enum eChangeType
{
    Enqueue = 0,
    Dequeue = 1,
    Clear = 2
}
public class ChangeEventArgs : EventArgs
{
    public eChangeType ChangeType;
}
/// <summary>
/// This is a subclass of basic .Net Queue class with multi threading and support of change event
/// </summary>
public class CandoQueue : Queue
{

    /// <summary>
    /// Occurs when [changed].
    /// </summary>
    public event EventHandler Changed;

    /// <summary>
    /// Called when [changed].
    /// </summary>
    /// <param name="change">The change.</param>
    protected virtual void OnChanged(eChangeType change)
    {
        if (Changed != null)
        {
            ChangeEventArgs e = new() { ChangeType = change };
            //calling the optional event handler:
            Changed(this, e);
        }
    }

    /// <summary>
    /// Enqueues the specified value.
    /// </summary>
    /// <param name="Value">The value.</param>
    public override void Enqueue(object Value)
    {
        //locking to protect from multientrance(multiple tread access to the queue:
        lock (this)
        {
            base.Enqueue(Value);
        }

        OnChanged(eChangeType.Enqueue);
    }

    /// <summary>
    /// Removes all objects from the <see cref="T:System.Collections.Queue" />.
    /// </summary>
    public override void Clear()
    {
        //locking to protect from multientrance(multiple tread access to the queue:
        lock (this)
        {
            base.Clear();
        }
        OnChanged(eChangeType.Clear);

    }

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="T:System.Collections.Queue" />.
    /// </summary>
    /// <returns>
    /// The object that is removed from the beginning of the <see cref="T:System.Collections.Queue" />.
    /// </returns>
    public override object Dequeue()
    {
        object Retval;
        //locking to protect from multientrance(multiple tread access to the queue:
        lock (this)
        {
            Retval = base.Dequeue();
        }
        OnChanged(eChangeType.Dequeue);

        return Retval;
    }
}
