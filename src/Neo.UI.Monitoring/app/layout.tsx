import { themeScript } from '../../Neo.Bpms.UI.MVC/wwwroot/js/neo-theme';
import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'مانیتورینگ سیستم | Neo BPMS',
  description: 'داشبورد مانیتورینگ سیستم Neo BPMS',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="fa" dir="rtl" suppressHydrationWarning>
      <head><script dangerouslySetInnerHTML={{ __html: themeScript }} /></head>
      <body className="neo-theme-scope min-h-screen bg-canvas text-text antialiased">
        {children}
      </body>
    </html>
  );
}

