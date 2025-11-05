namespace Neo.Bpms.Domain.Models.Cmmn.Fields;

public enum TVariableTypes
{
    invalid = -1,
    None = 0,
    Char = 1,
    Short = 2,
    Long = 3,
    UChar = 4,
    UShort = 5,
    ULong = 6,
    Double = 7,
    String = 8,
    Table = 9,//جدول داخلی 
    List = 10,
    DateTime = 11,
    Date = 12,
    StringListItem = 13,//اشاره به انتخاب از فهرست متون 
    Decimal=14,
    HourMinute = 15,
    StringListBitMask = 16, //انتخاب ترکیبی از فهرست متون 
    File=17,
    DayHourMinute = 18,
    DoubleMinuteSecond = 19,
    Int = 20,
    BOOL = 21,
    DurHourMinute = 22,
    ByteArray=23,

    Link = 34,
    DateStr = 35,

    BaseEntity = 36,
    Association = 37,
    Composition = 38,
    ParentEntity = 39,

    WeakEntityAssociation = 40,
    BitMaskAssociation = 41
}
