using CourierAPI.Models.Dto;
using CourierAPI.Websocket;
using System.Reflection;

namespace CourierAPI.Services;

public class WorkService
{
    public event EventHandler<WorkStatusEventArgs>? RecentlyActive;
    public event EventHandler<WorkStatusEventArgs>? Inactive;
    public List<WorkTime> workTimes { get; set; }

    public WorkService()
    {
        workTimes = new List<WorkTime>();
    }

    public void CheckTime()
    {
        foreach (var workTime in workTimes)
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
        workTimes.First(t => t.CourierId == id).Status = 
            status == WorkStatus.Active ? WorkStatus.RecentlyActive : WorkStatus.Inactive;

        if (status == WorkStatus.RecentlyActive)
        {
            WorkStatusEventArgs args = new WorkStatusEventArgs(workTimes.First(t => t.CourierId == id));
            DeleteWorkTime(id);
            Inactive.Invoke(this, args);
        }
        else if (status == WorkStatus.Active)
        {
            WorkStatusEventArgs args = new WorkStatusEventArgs(workTimes.First(t => t.CourierId == id));
            RecentlyActive.Invoke(this, args);
        }
    }

    public void DeleteWorkTime(string id)
    {
        workTimes.Remove(workTimes.First(t => t.CourierId == id));
    }

    public void AddWorkTime(string id)
    {
        workTimes.Add(new WorkTime { CourierId = id, RequestTime = DateTime.Now, Status = WorkStatus.Active });
    }

    public void CheckIn(string id)
    {
        var workTime = workTimes.First(x => x.CourierId == id);
        workTime.RequestTime = DateTime.Now;
        workTime.Status = WorkStatus.Active;
    }

    public bool CheckWorkStatus(string id)
    {
        return workTimes.Any(x => x.CourierId == id);
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
