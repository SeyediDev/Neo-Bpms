//namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes
//{
//	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
//	public class SNMP(string snmpOid, SNMPFieldFlag flag = SNMPFieldFlag.None,
//        SNMPFieldTypeId snmpFieldType = SNMPFieldTypeId.Same) : Attribute
//	{
//        public EntityFieldSNMPSetting SNMPSetting { get; set; } = new EntityFieldSNMPSetting(snmpOid, flag, snmpFieldType);
//    }
//	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
//	public class FullSNMP : Attribute
//	{
//		public FullSNMP(string snmpOid, SNMPFieldFlag flag, SNMPFieldTypeId snmpFieldType)
//		{
//			SNMPSetting = new EntityFieldSNMPSetting(snmpOid, flag, snmpFieldType) {IsFullSNMP = true};
//		}
//		public EntityFieldSNMPSetting SNMPSetting { get; set; }
//	}

//}
