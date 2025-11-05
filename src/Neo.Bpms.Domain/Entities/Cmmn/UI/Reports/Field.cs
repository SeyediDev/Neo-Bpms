using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public partial class Report
{
    /// <summary>
    /// Field
    /// </summary>
    public class Field : IBaseClass
    {
        public Field()
        {

        }
        public string fieldId { get; set; }
        public bool asColumn { get; set; }
        public bool asAggregation { get; set; }
        public bool asGroupBy { get; set; }
        public List<FormProperty> properties { get; set; }
        public string formula { get; set; }
        public string alias { get; set; }

        public void addProperty(FormProperty property)
        {
            properties ??= [];
            properties.Add(property);
        }

        public string Id
        {
            get => fieldId;
            set => fieldId = value;
        }
        public string sId => Id;
        public string Name
        {
            get => alias;
            set => alias = value;
        }
    }
    public class ReportFields : Dictionary<string, Field>
    {
        public void TryRemove(string id, out Field field)
        {
            throw new NotImplementedException();
        }
    }
}
