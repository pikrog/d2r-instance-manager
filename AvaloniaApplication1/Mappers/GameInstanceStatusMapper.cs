using System;
using System.Linq;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Mappers;

public static class GameInstanceStatusMapper
{
    public static GameInstanceStatus Map(RuntimeSnapshot snapshot)
    {
        var status = snapshot.State switch
        {
            /*case State.Inactive:
                if (snapshot.Errors.Length > 0)
                {
                    if(snapshot.Errors[0] is UnknownError)
                        status = GameInstanceStatus.Failed;
                }
                else
                {
                    // todo:
                    // if was running but is now inactive
                    // status = GameInstanceStatus.Stopped;
                    // else
                    //     status = GameInstanceStatus.Inactive;
                    status = GameInstanceStatus.Inactive;
                }
                break;
            case State.WaitingForStart:
                status = GameInstanceStatus.QueuedForLaunch;
                break;
            case State.WaitingForUnlock:
                status = GameInstanceStatus.Unlocking;
                break;
            case State.Starting:
                status = GameInstanceStatus.Launching;
                break;
            case State.Authenticating:
                status = GameInstanceStatus.Authenticating;
                break;
            case State.Stopping:
                status = GameInstanceStatus.Stopping;
                break;
            case State.Running:
                status = GameInstanceStatus.Running;
                break;
            default:
                throw new ArgumentOutOfRangeException(); // todo: InvalidOperationException or ArgumentOutOfRangeException?*/
            State.Inactive => GameInstanceStatus.Inactive,
            State.Authenticating => GameInstanceStatus.Authenticating,
            State.WaitingForStart => GameInstanceStatus.QueuedForStart,
            State.Starting => GameInstanceStatus.Starting,
            State.WaitingForUnlock => GameInstanceStatus.Unlocking,
            State.Running => GameInstanceStatus.Running,
            State.Stopping => GameInstanceStatus.Stopping,
            _ => throw new ArgumentOutOfRangeException()
        };

        throw new NotImplementedException();
    }
}