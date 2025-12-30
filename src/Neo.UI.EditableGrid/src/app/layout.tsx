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
    <html lang="fa" dir="rtl">
      <body>{children}</body>
    </html>
  );
}

