using System.ComponentModel.DataAnnotations.Schema;

namespace Neo.Bpms.Domain.Models.Cmmn.Common;

[DisplayName("موجودیت صفحه‌ی اول")]
[DontSync]
[NotMapped]
[DataProvider(nameof(DomainProvider.Domain))]
public class HomePageEntity
{
}
