using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

public interface IItemAwareContainer
{
    IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems);
}
public static class ItemAwareContainer
{
    public static IItemAwareElement GetItemAwareElement(IItemAwareContainer itemAwareContainer,
        string itemId, string itemName, bool fromInputItems)
    {
        if (itemAwareContainer is Activity
            {
                loopCharacteristics: MultiInstanceLoopCharacteristics multiInstanceLoopCharacteristics
            })
        {
            if (fromInputItems)
            {
                if (multiInstanceLoopCharacteristics.inputDataItem != null)
                {
                    if (multiInstanceLoopCharacteristics.inputDataItem.CheckItem(itemId, itemName))
                        return multiInstanceLoopCharacteristics.inputDataItem;
                }
            }
            else
            {
                if (multiInstanceLoopCharacteristics.outputDataItem != null)
                {
                    if (multiInstanceLoopCharacteristics.outputDataItem.CheckItem(itemId, itemName))
                        return multiInstanceLoopCharacteristics.outputDataItem;
                }
            }
        }
        Property property =
            (itemAwareContainer as IPropertyContainer)?.properties?.FirstOrDefault(p => p.CheckItem(itemId, itemName));
        if (property != null)
            return property;
        InputOutputSpecification ioSpecification = (itemAwareContainer as IIoSpecificationContainer)?.ioSpecification;
        if (ioSpecification != null)
        {
            if (fromInputItems)
            {
                DataInput input = ioSpecification.dataInputs?.FirstOrDefault(p => p.CheckItem(itemId, itemName));
                if (input != null)
                    return input;
            }
            else
            {
                DataOutput output = ioSpecification.dataOutputs?.FirstOrDefault(p => p.CheckItem(itemId, itemName));
                if (output != null)
                    return output;
            }
        }
        if (fromInputItems)
        {
            DataInput input = (itemAwareContainer as IDataInputContainer)?.dataInputs?.FirstOrDefault(p =>
                p.CheckItem(itemId, itemName));
            if (input != null)
                return input;
        }
        else
        {
            DataOutput output =
                (itemAwareContainer as IDataOutputContainer)?.dataOutputs?.FirstOrDefault(p => p.CheckItem(itemId, itemName));
            if (output != null)
                return output;
        }
        IFlowElementsContainer flowElementsContainer = itemAwareContainer as IFlowElementsContainer;
        if (flowElementsContainer != null)
        {
            FlowElement flowElement = null;
            if (!string.IsNullOrEmpty(itemId))
                flowElementsContainer.flowElements?.TryGetValue(itemId, out flowElement);
            if (flowElement == null && !string.IsNullOrEmpty(itemName))
                flowElementsContainer.flowElements?.TryGetValue(itemName, out flowElement);
            if (flowElement == null && !string.IsNullOrEmpty(itemName))
                flowElement = flowElementsContainer.flowElements?.Values.OfType<DataFlowElement>().FirstOrDefault(p => p.CheckItem(itemId, itemName));
            if (flowElement != null)
                return flowElement as IItemAwareElement;
        }
        IItemAwareContainer parentContainer = (itemAwareContainer as BaseElement)?.Parent as IItemAwareContainer;
        return parentContainer?.GetItemAwareElement(itemId, itemName, fromInputItems);
    }
}
