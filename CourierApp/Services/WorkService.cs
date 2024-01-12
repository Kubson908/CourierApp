using CourierAPI.Models.Dto;

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
                    ChangeStatus(workTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    public void ChangeStatus(WorkTime workTime)
    {
        workTime.Status = workTime.Status == WorkStatus.Active ? 
            WorkStatus.RecentlyActive : WorkStatus.Inactive;

        if (workTime.Status == WorkStatus.RecentlyActive)
        {
            WorkStatusEventArgs args = new(workTime);
            DeleteWorkTime(workTime.CourierId);
            Inactive?.Invoke(this, args);
        }
        else if (workTime.Status == WorkStatus.Active)
        {
            WorkStatusEventArgs args = new(workTime);
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
