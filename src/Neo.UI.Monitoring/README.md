# Neo.UI.Monitoring

داشبورد مانیتورینگ حرفه‌ای برای Neo BPMS با استفاده از React و Next.js

## ✨ ویژگی‌ها

- **داشبورد Real-time**: نمایش لحظه‌ای متریک‌های سیستم و اپلیکیشن
- **Logs Panel**: نمایش و فیلتر لاگ‌ها با سطوح مختلف
- **Traces Panel**: نمایش تریس‌ها و جزئیات span‌ها
- **SignalR Integration**: بروزرسانی خودکار با WebSocket
- **RTL Support**: پشتیبانی کامل از راست به چپ
- **Dark Theme**: تم تاریک حرفه‌ای

## 🚀 شروع سریع

### نصب وابستگی‌ها

```bash
npm install
```

### اجرای در محیط توسعه

```bash
npm run dev
```

داشبورد در `http://localhost:3001` قابل دسترسی است.

### Build برای Production

```bash
npm run build
```

### Export استاتیک

```bash
npm run export
```

فایل‌های استاتیک در پوشه `dist` قرار می‌گیرند.

## 🔧 پیکربندی

### متغیرهای محیطی

```env
# API Base URL (default: same origin)
NEXT_PUBLIC_API_URL=http://localhost:5000

# Base Path (for embedding in MVC)
BASE_PATH=/monitoring

# Asset Prefix (for CDN)
ASSET_PREFIX=
```

## 📁 ساختار پروژه

```
Neo.UI.Monitoring/
├── app/                    # Next.js App Router
│   ├── layout.tsx         # Root layout
│   ├── page.tsx           # Main dashboard page
│   └── globals.css        # Global styles
├── src/
│   ├── components/        # React components
│   │   ├── MetricCard.tsx
│   │   ├── GaugeChart.tsx
│   │   ├── SystemMetrics.tsx
│   │   ├── LogsPanel.tsx
│   │   └── TracesPanel.tsx
│   └── hooks/             # Custom React hooks
│       ├── useMonitoringData.ts
│       └── useSignalR.ts
├── public/                # Static assets
├── package.json
├── tailwind.config.js
├── tsconfig.json
└── next.config.js
```

## 🎨 کامپوننت‌ها

### MetricCard
نمایش یک متریک با استاتوس و روند

```tsx
<MetricCard
  title="CPU Usage"
  value={85}
  unit="%"
  status="warning"
  trend={{ direction: 'up', percentage: 5 }}
/>
```

### GaugeChart
نمایش متریک به صورت گراف دایره‌ای

```tsx
<GaugeChart
  label="Memory"
  value={60}
  max={100}
  unit="%"
  status="healthy"
/>
```

## 🔌 API Endpoints

| Endpoint | توضیحات |
|----------|---------|
| `/api/monitoring/dashboard` | داده‌های کلی داشبورد |
| `/api/monitoring/metrics/system` | متریک‌های سیستم |
| `/api/monitoring/logs/recent` | لاگ‌های اخیر |
| `/api/monitoring/traces/recent` | تریس‌های اخیر |
| `/hubs/monitoring` | SignalR Hub |

## 🏗️ Micro-Frontend Integration

این پروژه می‌تواند به عنوان micro-frontend در پنل ادمین MVC موجود embed شود.

### روش 1: Static Export
1. Build با `npm run export`
2. کپی فایل‌های `dist` به `wwwroot/monitoring`
3. استفاده از iframe یا dynamic import

### روش 2: Module Federation (آینده)
پیکربندی Module Federation در `next.config.js` برای dynamic loading

## 📝 License

MIT License - Neo BPMS Team

