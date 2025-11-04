// using System;
// 
// namespace Neo.Bpms.Design.CodeFirst
// {
// 	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
// 	public class FAttr_Constraint : Attribute
// 	{
// 		public FAttr_Constraint(string constraint)
// 		{
// 			Constraint = constraint;
// 		}
// 		public string Constraint { get; set; }
// 	}
//
// 	public class Constraint : FAttr_Constraint
// 	{
// 		public Constraint(string constraint):base(constraint)
// 		{
// 		}
// 	}
// }