# راهنمای Skill و MCP نئو BPMS

نسخهٔ 0.2 شامل Skill توسعه و پنج ابزار اجراشدنی است. Skill در
[neo-bpms](../.agents/skills/neo-bpms/SKILL.md) قرار دارد و در نبود MCP هم با خواندن منبع کار می‌کند.

## اجرا

Python 3.10 یا بالاتر لازم است؛ تست محلی با 3.12 انجام شده. سرور یا دیتابیس لازم نیست.
از ریشهٔ ریپو:

```powershell
python -B tools/Neo.Bpms.Companion/neo_bpms_mcp.py --project-root E:/SJVS/Projects/Neo-Bpms
```

این برنامه از طریق stdio به کلاینت MCP پاسخ می‌دهد و صفحهٔ وب نیست.
[نمونهٔ تنظیمات](../tools/Neo.Bpms.Companion/mcp-config.example.json) را با مسیر مطلق پروژهٔ خود تنظیم کن.

## ابزارها

| ابزار | کاربرد |
| --- | --- |
| bpms_inspect_project | خواندن اعلام پروژه و فریم‌ورک؛ بدون اجرای MSBuild |
| bpms_search_docs | جست‌وجوی چهار Recipe بررسی‌شده، نه همهٔ فایل‌های Markdown |
| bpms_validate_process | بررسی XML، شناسهٔ تکراری، ارجاع مسیر و دسترسی پایه به گره‌ها |
| bpms_find_registration | پیدا کردن نامزدهای ثبت مستقیم سرویس با محل کد |
| bpms_get_recipe | راهنمای process، form، human-task و integration همراه هش منبع |

نمونه: برای بررسی ثبت موتور، serviceName را IBpmsEngine بده.
برای integration، ابتدا Recipe را بخوان: پیاده‌سازی فعلی SendFormCommand درخواست را ارسال
نمی‌کند و false برمی‌گرداند. این موضوع در Recipe صریحاً ثبت شده است.

اعتبارسنج BPMN فقط XML را می‌پذیرد؛ پشتیبانی JSON/DMN یا تضمین اجرای فرآیند ندارد.
خروجی خالی به معنی سالم بودن اجرای برنامه نیست. شواهد C# شمارهٔ خط دارند؛ شواهد XML
شمارهٔ ترتیبی عنصر هستند. محدودیت فایل، مسیر، تعداد نتایج و موارد پوشش‌داده‌نشده گزارش می‌شوند.

## نگهداری

هر تغییر توسعه که قراردادها، مثال‌ها یا رفتار دستیار را تغییر دهد باید هم‌زمان در Skill،
MCP و README منعکس شود. تغییر هش بدون بازخوانی Recipe کافی نیست.
[قاعدهٔ نگهداری](COMPANION-MAINTENANCE.md) و
[نتایج تست](../tools/Neo.Bpms.Companion/VALIDATION.md) مرجع این کار هستند.
