# طرح Skill و MCP برای Neo.Bpms

Skill در `.agents/skills/neo-bpms` قرار می‌گیرد و برای توسعهٔ فرآیند، فرم، وظیفهٔ انسانی، API و persistence فعال می‌شود. ابتدا csproj، نسخهٔ بسته‌ها، ماژول واقعی و تست موجود را می‌خواند.

نسخهٔ اول MCP فقط خواندنی است و پنج ابزار دارد: `bpms_inspect_project`، `bpms_search_docs`، `bpms_validate_process`، `bpms_find_registration` و `bpms_get_recipe`. خروجی‌ها evidence، شمارهٔ خط، سطح اطمینان و محدودیت دارند.

MCP عمداً workflow را اجرا نمی‌کند، به دیتابیس وصل نمی‌شود، فرم ارسال نمی‌کند و فایل را تغییر نمی‌دهد. مرحلهٔ بعد استخراج قراردادهای واقعی از ماژول‌های Process/Forms و ساخت fixtureهای BPMN سالم، ناقص و غیرقابل‌دسترسی است؛ سپس سرور stdio، smoke test و CI اضافه می‌شود.
