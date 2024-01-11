using CourierAPI.Models.Dto;
using CourierAPI.Websocket;
using System.Reflection;

namespace CourierAPI.Services;

public class WorkService
{
    public event EventHandler<WorkStatusEventArgs>? RecentlyActive;
    public event EventHandler<WorkStatusEventArgs>? Inactive;
    public List<WorkTime> WorkTimes { get; set; }

    public WorkService()
    {
        WorkTimes = new List<WorkTime>();
    }

    public void CheckTime()
    {
        foreach (var workTime in WorkTimes)
        {
            DateTime now = DateTime.Now;
            if (now.Subtract(workTime.RequestTime).TotalSeconds > 6 * 60)
            {
                try
                {
                    ChangeStatus(workTime.CourierId, workTime.Status);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public void ChangeStatus(string id, WorkStatus status)
    {
        WorkTimes.First(t => t.CourierId == id).Status = 
            status == WorkStatus.Active ? WorkStatus.RecentlyActive : WorkStatus.Inactive;

        if (status == WorkStatus.RecentlyActive)
        {
            WorkStatusEventArgs args = new(WorkTimes.First(t => t.CourierId == id));
            DeleteWorkTime(id);
            Inactive?.Invoke(this, args);
        }
        else if (status == WorkStatus.Active)
        {
            WorkStatusEventArgs args = new(WorkTimes.First(t => t.CourierId == id));
            RecentlyActive?.Invoke(this, args);
        }
    }

    public void DeleteWorkTime(string id)
    {
        WorkTimes.Remove(WorkTimes.First(t => t.CourierId == id));
    }

    public void AddWorkTime(string id)
    {
        WorkTimes.Add(new WorkTime { CourierId = id, RequestTime = DateTime.Now, Status = WorkStatus.Active });
    }

    public void CheckIn(string id)
    {
        var workTime = WorkTimes.First(x => x.CourierId == id);
        workTime.RequestTime = DateTime.Now;
        workTime.Status = WorkStatus.Active;
    }

    public bool CheckWorkStatus(string id)
    {
        return WorkTimes.Any(x => x.CourierId == id);
    }
}

public class WorkStatusEventArgs : EventArgs
{
    private readonly WorkTime _workTime;

    public WorkStatusEventArgs(WorkTime workTime)
    {
        _workTime = workTime;
    }
    public WorkTime WorkTime { get { return _workTime; } }
}
