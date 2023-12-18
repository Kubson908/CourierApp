using CourierAPI.Models.Dto;

namespace CourierAPI.Services;

public class WorkService
{
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
            DeleteWorkTime(id);
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
