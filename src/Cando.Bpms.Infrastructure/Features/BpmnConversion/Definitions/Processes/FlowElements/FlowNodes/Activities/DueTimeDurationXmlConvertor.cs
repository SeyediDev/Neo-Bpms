using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal class DueTimeDurationXmlConvertor
{
    public static void Export(dynamic containerElement, IDueTimeDurationContainer dueTimeDurationContainer)
    {
        if (dueTimeDurationContainer?.dueDuration == null) return;
        var dueDuration = dueTimeDurationContainer.dueDuration;
        var element = containerElement;
        element.timeOptions = "TimeDuration";
        element.calculationType = (int)dueDuration.calculationType;
        //element.formula = dueDuration.formula;

        element.timeReference = (int)dueDuration.timeReference;
        element.referenceProperty = dueDuration.referenceProperty;
        element.resolution = (int)dueDuration.resolution;
    }

    public static void Import(ElasticObject containerElement, IDueTimeDurationContainer dueTimeDurationContainer, string formula)
    {
        var element = containerElement;
        var dueDuration = dueTimeDurationContainer.dueDuration = new DueTimeDuration();
        dueDuration.calculationType = element.GetEnumText("calculationType", DueTimeDuration.eCalculationType.Iso8601);
        dueDuration.formula = formula;

        dueDuration.timeReference = element.GetEnumText("timeReference", DueTimeDuration.eTimeReference.Property);
        dueDuration.referenceProperty = element.GetString("referenceProperty");
        dueDuration.resolution = element.GetEnumText("resolution", DueTimeDuration.eResolution.Hour);
    }
}