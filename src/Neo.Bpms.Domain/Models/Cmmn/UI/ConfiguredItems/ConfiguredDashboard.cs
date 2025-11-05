using System.Text.Json.Serialization;

namespace Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

public class ConfiguredDashboard : ConfiguredItem
{
    public ConfiguredDashboard()
    {
    }

    public ConfiguredDashboard(Dashboard dashboard, string configId, string name)
    {
        Dashboard = dashboard;
        ConfigId = configId;
        Name = name;
    }
    [JsonIgnore]
    public string ConfigId { get; set; }
    [JsonIgnore]
    public Dashboard Dashboard { get; set; }
    public List<ConfigDiv> Divs { get; set; } = [];
    public List<ConfigWidget> Widgets { get; set; } = [];
    public bool IsMeta { get; set; }

    public class ConfigDiv
    {
        [JsonIgnore]
        public ConfigDiv Parent { get; set; }

        public string Id { get; set; }
        public long Width { get; set; }
        public bool IsRow { get; set; }
        public List<ConfigDiv> Children { get; set; }
        [JsonIgnore]
        public ConfigWidget Widget { get; set; }
        public string WidgetId { get; set; }
        public string Title { get; set; }
    }

    public class ConfigWidget
    {
        public string Id { get; set; }
        public string ReportNamespaceId { get; set; }
        public string ReportEntityId { get; set; }
        public string ReportId { get; set; }
        public string ReportConfigId { get; set; }
        public List<ConfigWidgetProperty> Properties { get; set; }

        public string GetPropertyValue(eControlPropertyId prop)
        {
            ConfigWidgetProperty pr = Properties?.FirstOrDefault(p => p.Id == prop);
            return pr != null ? pr.Value : "";
        }
        public int GetPropertyValueInt(eControlPropertyId prop)
        {
            string g = GetPropertyValue(prop);
            return string.IsNullOrEmpty(g) ? 0 : Convert.ToInt16(g);
        }
        public void AddProperty(eControlPropertyId id, string value)
        {
            Properties ??= [];
            Properties.Add(new ConfigWidgetProperty
            {
                Id = id,
                Value = value
            });
        }
        public void SetProperty(eControlPropertyId id, string value)
        {
            ConfigWidgetProperty prop = Properties?.FirstOrDefault(p => p.Id == id);
            if (prop != null)
            {
                prop.Value = value;
            }
            else
            {
                AddProperty(id, value);
            }
        }
    }

    public class ConfigWidgetProperty
    {
        public eControlPropertyId Id { get; set; }
        public string Value { get; set; }
    }
}
