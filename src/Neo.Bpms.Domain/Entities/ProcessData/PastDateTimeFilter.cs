namespace Neo.Bpms.Domain.Entities.ProcessData;

[DisplayNameAndEnName("فیلتر تاریخی گذشته")]
public class PastDateTimeFilter : BaseStringListProcessModelEntity
{
}
[DisplayNameAndEnName("فیلتر تاریخی گذشته")]
public enum PastDateTimeFilterId
{
    [DisplayNameAndEnName("روز گذشته")]
    Yesterday = 1,
    [DisplayNameAndEnName("هفته گذشته")]
    LastWeek = 2,
    [DisplayNameAndEnName("ماه گذشته")]
    LastMonth = 3,
    [DisplayNameAndEnName("سال گذشته")]
    LastYear = 4,
    [DisplayNameAndEnName("از تاریخ/تا تاریخ")]
    SpecificDate = 5
}
