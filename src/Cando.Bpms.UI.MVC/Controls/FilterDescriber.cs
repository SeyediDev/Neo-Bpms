namespace Neo.Bpms.UI.MVC.Controls;

public class FilterDescriber
{
    public static string DescribeFilter(CommonFormStructure structure, ElasticObject values)
    {
        CandoStringBuilder result = new();
        foreach (InputFieldDefinition field in structure.Fields)
        {
            if (values.GetField(field.FieldName, out object value) && value != null)
            {
                result += $"{field.Alias}: {TableHelper.CreateCellElem(value, field)} ";
            }
        }
        return result.ToString();
    }
    public static string DescribeWorkItemsFilter(WorkItemsFilter filter)
    {
        CandoStringBuilder result = new();
        if (!string.IsNullOrEmpty(filter.ProcessId))
        {
            Domain.Model.BPMN.Processes.Process process = ProjectDefinition.Project.GetBpmnDefinition(filter.ProcessId, filter.ProcessVersionId)
                ?.Process;
            if (process != null)
            {
                result += $"فرآیند: {process.Name} ";
                Domain.Entities.Bpmn.Processes.Activities.Activity activity = process.GetActivity(filter.Activity);
                if (activity != null)
                    result += $"فعالیت: {activity.Name} ";
            }
        }

        if (!string.IsNullOrEmpty(filter.FromCreationDateText))
        {
            result += "از تاریخ: ";
            result += filter.FromCreationDateText?.ToDateTimeFromMiladi().ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar);
        }
        if (!string.IsNullOrEmpty(filter.ToCreationDateText))
        {
            result += "تا تاریخ: ";
            result += filter.ToCreationDateText?.ToDateTimeFromMiladi().ToHtmlInputValue(ProjectDefinition.Project.DefaultCalendar);
        }
        if (!string.IsNullOrEmpty(filter.Description))
        {
            result += $"شرح: {filter.Description} ";
        }

        return result.ToString();
    }
    //filter fields and elastic?
}