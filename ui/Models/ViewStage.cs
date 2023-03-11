using shared.Models.Job;

namespace ui.Models;

public class ViewStage
{
    public string Name { get; set; }
    public List<JobStep> Steps { get; set; }
}