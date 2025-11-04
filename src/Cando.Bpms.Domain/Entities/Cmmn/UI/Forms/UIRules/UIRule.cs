using System.Diagnostics.CodeAnalysis;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;

/// <summary>
/// An instance of rule defined to control logic of forms and reports
/// </summary>
public class UIRule : BaseModelClass
{
    //public string Id; //TableItemIndex;
    //public string name;
    public List<UIRuleEvent> Events = [];
    public List<UIRuleTask> Operations = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="UIRule"/> class.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="pagePart">Indicates what part of the page this rule will.</param>
    public UIRule(Form parent, string id, string name, PagePart pagePart = PagePart.Form) :
        base(parent, id, name)
    {
        //this.Id = id;
        //this.name = name;
        PagePart = pagePart;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UIRule"/> class.
    /// </summary>
    /// <param name="c">The c.</param>
    public UIRule(UIRule c)
    {
        Id = 10000 + c.Id;
        Name = c.Name;
        Parent = c.Parent;
        //convert to foreach to clone:
        Events = c.Events;
        //convert to foreach to clone:
        Operations = c.Operations;
        PagePart = c.PagePart;
    }

    /// <summary>
    /// for serialization
    /// </summary>
    public UIRule()
    {
        PagePart = PagePart.Form;
    }

    public PagePart PagePart { get; set; }
}

public enum PagePart
{
    Form,
    IndexList
}

/// <summary>
/// enumerations used to define rule local parameters
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum eLocalParameterIds
{
    None = 0,
    lc_1 = 1,
    lc_2 = 2,
    lc_3 = 3,
    lc_4 = 4,
    lc_5 = 5,
    lc_6 = 6,
    lc_7 = 7,
    lc_8 = 8,
    lc_9 = 9,
    lc_10 = 10,
    lc_11 = 11,
    lc_12 = 12,
    lc_13 = 13,
    lc_14 = 14,
    lc_15 = 15,
    lc_16 = 16,
    lc_17 = 17,
    lc_18 = 18,
    lc_19 = 19,
    lc_20 = 20,
    lc_21 = 21,
    lc_22 = 22,
    lc_23 = 23,
    lc_24 = 24,
    lc_25 = 25,
    lc_26 = 26,
    lc_27 = 27,
    lc_28 = 28,
    lc_29 = 29,
    lc_30 = 30,
    lc_31 = 31,
    lc_32 = 32,
    lc_33 = 33,
    lc_34 = 34,
    lc_35 = 35,
    lc_36 = 36,
    lc_37 = 37,
    lc_38 = 38,
    lc_39 = 39,
    lc_40 = 40,
    lc_41 = 41,
    lc_42 = 42,
    lc_43 = 43,
    lc_44 = 44,
    lc_45 = 45,
    lc_46 = 46,
    lc_47 = 47,
    lc_48 = 48,
    lc_49 = 49,
    lc_50 = 50,
    lc_51 = 51,
    lc_52 = 52,
    lc_53 = 53,
    lc_54 = 54,
    lc_55 = 55,
    lc_56 = 56,
    lc_57 = 57,
    lc_58 = 58,
    lc_59 = 59,
    lc_60 = 60,
    lc_61 = 61,
    lc_62 = 62,
    lc_63 = 63,
    lc_64 = 64,
    lc_65 = 65,
    lc_66 = 66,
    lc_67 = 67,
    lc_68 = 68,
    lc_69 = 69,
    lc_70 = 70,
    lc_71 = 71,
    lc_72 = 72,
    lc_73 = 73,
    lc_74 = 74,
    lc_75 = 75,
    lc_76 = 76,
    lc_77 = 77,
    lc_78 = 78,
    lc_79 = 79,
    lc_80 = 80,
    lc_81 = 81,
    lc_82 = 82,
    lc_83 = 83,
    lc_84 = 84,
    lc_85 = 85,
    lc_86 = 86,
    lc_87 = 87,
    lc_88 = 88,
    lc_89 = 89,
    lc_90 = 90,
    lc_91 = 91,
    lc_92 = 92,
    lc_93 = 93,
    lc_94 = 94,
    lc_95 = 95,
    lc_96 = 96,
    lc_97 = 97,
    lc_98 = 98,
    lc_99 = 99,
}
