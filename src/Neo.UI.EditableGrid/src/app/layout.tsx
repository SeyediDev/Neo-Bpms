import { themeScript } from '../../../Neo.Bpms.UI.MVC/wwwroot/js/neo-theme';
import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Editable Grid',
  description: 'Google Sheets-like editable grid component',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="fa" dir="rtl" suppressHydrationWarning>
      <head><script dangerouslySetInnerHTML={{ __html: themeScript }} /></head>
      <body className="neo-theme-scope bg-canvas text-text">{children}</body>
    </html>
  );
}

