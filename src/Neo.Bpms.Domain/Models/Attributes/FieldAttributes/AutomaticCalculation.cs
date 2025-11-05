namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes
{
    public class AutomaticCalculation : FAttr_AutomaticCalculation
    {
        /// <summary>
        /// مشخص کردن مقدار فیلد بصورت فرمول
        /// </summary>
        /// <param name="formula">فرمول</param>
        public AutomaticCalculation(string formula) : base(formula)
        {
        }

        /// <summary>
        /// مقدار دهی فیلد از نوع عددی بصورت خودکار افزایش یابد
        /// </summary>
        public AutomaticCalculation() : base("AutoIncrement()")
        {
            GenerationType = eGenerationType.DBInsert;
        }
    }
}
public enum eGenerationType
{
    //			Never,
    Always = 0,
    Insert = 1,
    IfNull = 2,
    DBInsert = 3
}
